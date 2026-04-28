using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float inputSendRate = 30f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody playerRigidbody;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 currentMoveInput;
    private Vector2 localMoveInput;
    private Vector2 lastSentInput;
    private float nextInputSendTime;
    private Quaternion targetRotation;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        moveAction = new InputAction("Move", InputActionType.Value);

        var qwertyBinding = moveAction.AddCompositeBinding("2DVector");
        qwertyBinding.With("Up", "<Keyboard>/w");
        qwertyBinding.With("Down", "<Keyboard>/s");
        qwertyBinding.With("Left", "<Keyboard>/a");
        qwertyBinding.With("Right", "<Keyboard>/d");

        var azertyBinding = moveAction.AddCompositeBinding("2DVector");
        azertyBinding.With("Up", "<Keyboard>/z");
        azertyBinding.With("Down", "<Keyboard>/s");
        azertyBinding.With("Left", "<Keyboard>/q");
        azertyBinding.With("Right", "<Keyboard>/d");

        var arrowsBinding = moveAction.AddCompositeBinding("2DVector");
        arrowsBinding.With("Up", "<Keyboard>/upArrow");
        arrowsBinding.With("Down", "<Keyboard>/downArrow");
        arrowsBinding.With("Left", "<Keyboard>/leftArrow");
        arrowsBinding.With("Right", "<Keyboard>/rightArrow");

        moveAction.AddBinding("<Gamepad>/leftStick");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
        jumpAction?.Dispose();
    }

    public override void OnNetworkSpawn()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // Only the server simulates player physics to keep a single source of truth.
        playerRigidbody.isKinematic = !IsServer;
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner)
        {
            return;
        }

        HandleMovementInput();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        if (!IsSpawned || !IsServer)
        {
            return;
        }

        Vector3 moveDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 nextPosition = playerRigidbody.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(nextPosition);
        }

        if (Quaternion.Angle(playerRigidbody.rotation, targetRotation) > 0.1f)
        {
            Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    private void HandleMovementInput()
    {
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        localMoveInput = input;

        if (IsServer)
        {
            currentMoveInput = input;
            return;
        }

        bool inputChanged = (input - lastSentInput).sqrMagnitude > 0.0001f;
        bool sendRateReached = Time.time >= nextInputSendTime;

        if (!inputChanged && !sendRateReached)
        {
            return;
        }

        SendMoveInputRpc(input);
        lastSentInput = input;
        nextInputSendTime = Time.time + 1f / Mathf.Max(1f, inputSendRate);
    }

    private void HandleJumpInput()
    {
        if (jumpAction != null && jumpAction.triggered)
        {
            if (IsServer)
            {
                TryPerformJump(localMoveInput);
            }
            else
            {
                RequestJumpRpc(localMoveInput);
            }
        }
    }

    private void TryPerformJump(Vector2 direction)
    {
        // Simple check: if we are not moving up/down much, we assume we are on the ground
        if (Mathf.Abs(playerRigidbody.linearVelocity.y) > 0.1f)
        {
            return; 
        }

        // Apply immediate vertical velocity (more responsive than AddForce sometimes)
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, jumpForce, playerRigidbody.linearVelocity.z);

        // Apply Smooth Rotation Target
        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector3 eulerDelta = Vector3.zero;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                eulerDelta.z = direction.x > 0f ? -90f : 90f;
            else
                eulerDelta.x = direction.y > 0f ? 90f : -90f;

            targetRotation *= Quaternion.Euler(eulerDelta);
        }
    }

    [Rpc(SendTo.Server)]
    private void SendMoveInputRpc(Vector2 input, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId)
        {
            return;
        }

        currentMoveInput = input;
    }

    [Rpc(SendTo.Server)]
    private void RequestJumpRpc(Vector2 direction, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId)
        {
            return;
        }

        TryPerformJump(direction);
    }
}
