using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float inputSendRate = 30f;

    private Rigidbody playerRigidbody;
    private InputAction moveAction;
    private Vector2 currentMoveInput;
    private Vector2 lastSentInput;
    private float nextInputSendTime;

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
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }

    private void OnDestroy()
    {
        moveAction?.Dispose();
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
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner)
        {
            return;
        }

        HandleMovementInput();
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

        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector3 nextPosition = playerRigidbody.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(nextPosition);
    }

    private void HandleMovementInput()
    {
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

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

    [Rpc(SendTo.Server)]
    private void SendMoveInputRpc(Vector2 input, RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId)
        {
            return;
        }

        currentMoveInput = input;
    }
}
