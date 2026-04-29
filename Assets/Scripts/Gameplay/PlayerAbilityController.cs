using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerRoleHandler))]
public class PlayerAbilityController : NetworkBehaviour
{
    private AbilityBase currentAbility;
    private PlayerRoleHandler roleHandler;
    private InputAction abilityAction;

    private void Awake()
    {
        roleHandler = GetComponent<PlayerRoleHandler>();

        abilityAction = new InputAction("Ability", InputActionType.Button);
        abilityAction.AddBinding("<Keyboard>/leftAlt");
        abilityAction.AddBinding("<Keyboard>/rightAlt");
        abilityAction.AddBinding("<Gamepad>/leftTrigger");
    }

    private void OnEnable() => abilityAction?.Enable();
    private void OnDisable() => abilityAction?.Disable();
    private void OnDestroy() => abilityAction?.Dispose();

    // Appelé par PlayerRoleHandler.SetRole (server-side)
    public void InitializeAbility(PlayerRole role, PlayerRoleData data)
    {
        currentAbility = role switch
        {
            PlayerRole.Orangutan or PlayerRole.Lemurien => new GrappleAbility(data.abilityCooldown),
            PlayerRole.Banane => new SkinBoostAbility(),
            _ => null
        };
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner) return;

        if (abilityAction.WasPressedThisFrame())
            UseAbilityServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void UseAbilityServerRpc(RpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId != OwnerClientId) return;
        currentAbility?.TryExecute(roleHandler);
    }

    // Appelé par l'UI pour afficher le cooldown restant
    public float GetCooldownRemaining() => currentAbility?.GetCooldownRemaining() ?? 0f;
}
