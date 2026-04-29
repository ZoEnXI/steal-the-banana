using Unity.Netcode;
using UnityEngine;

public class PlayerRoleHandler : NetworkBehaviour
{
    public NetworkVariable<PlayerRole> CurrentRole = new NetworkVariable<PlayerRole>(
        PlayerRole.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    // Lue par PlayerController dans FixedUpdate (server-side)
    public float MoveSpeed { get; private set; } = 5f;

    private PlayerRoleData roleData;
    private float speedMultiplier = 1f;

    private PlayerAbilityController abilityController;

    private void Awake()
    {
        abilityController = GetComponent<PlayerAbilityController>();
    }

    // Server-only : assigné par RoleManager
    public void SetRole(PlayerRole role, PlayerRoleData data)
    {
        if (!IsServer) return;

        CurrentRole.Value = role;
        roleData = data;
        speedMultiplier = 1f;
        MoveSpeed = data.moveSpeed;

        abilityController?.InitializeAbility(role, data);
    }

    // Appelé par SkinBoostAbility (server-side, via coroutine sur ce MonoBehaviour)
    public void ApplySpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        MoveSpeed = (roleData != null ? roleData.moveSpeed : 5f) * speedMultiplier;
    }

    public void RemoveSpeedMultiplier()
    {
        speedMultiplier = 1f;
        MoveSpeed = roleData != null ? roleData.moveSpeed : 5f;
    }
}
