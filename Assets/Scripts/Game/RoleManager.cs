using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoleManager : NetworkBehaviour
{
    [SerializeField] private PlayerRoleData[] roleDataAssets;

    // clientId → poids cumulatif de chance d'être Banane (server-only)
    private readonly Dictionary<ulong, float> bananaProbability = new();

    private Dictionary<PlayerRole, PlayerRoleData> roleDataLookup;

    public static RoleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        roleDataLookup = new Dictionary<PlayerRole, PlayerRoleData>();
        foreach (var data in roleDataAssets)
        {
            if (data != null)
                roleDataLookup[data.role] = data;
        }
    }

    public override void OnDestroy()
    {
        if (Instance == this) Instance = null;
        base.OnDestroy();
    }

    public PlayerRoleData GetRoleData(PlayerRole role) =>
        roleDataLookup.TryGetValue(role, out var data) ? data : null;

    // Server-only : à appeler au démarrage de chaque manche
    public void AssignRolesForNewMatch()
    {
        if (!IsServer) return;

        var connectedIds = NetworkManager.Singleton.ConnectedClientsIds;
        if (connectedIds.Count == 0) return;

        ulong bananaClientId = DrawBananaPlayer(connectedIds);

        int monkeyIndex = 0;
        foreach (ulong clientId in connectedIds)
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
                continue;

            var roleHandler = client.PlayerObject?.GetComponent<PlayerRoleHandler>();
            if (roleHandler == null) continue;

            PlayerRole assignedRole;
            if (clientId == bananaClientId)
            {
                assignedRole = PlayerRole.Banane;
            }
            else
            {
                // Alterne Orangutan / Lemurien pour équilibrer les équipes
                assignedRole = (monkeyIndex % 2 == 0) ? PlayerRole.Orangutan : PlayerRole.Lemurien;
                monkeyIndex++;
            }

            if (roleDataLookup.TryGetValue(assignedRole, out var data))
                roleHandler.SetRole(assignedRole, data);
        }

        UpdateBananaProbabilities(bananaClientId, connectedIds);
    }

    // Server-only : transformation à 5:00
    public void TransformToChadBanana(ulong bananaClientId)
    {
        if (!IsServer) return;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(bananaClientId, out var client))
            return;

        var roleHandler = client.PlayerObject?.GetComponent<PlayerRoleHandler>();
        if (roleHandler == null) return;

        if (roleDataLookup.TryGetValue(PlayerRole.ChadBanana, out var data))
            roleHandler.SetRole(PlayerRole.ChadBanana, data);
    }

    private ulong DrawBananaPlayer(IReadOnlyList<ulong> clients)
    {
        float totalWeight = 0f;
        var weights = new float[clients.Count];

        for (int i = 0; i < clients.Count; i++)
        {
            ulong id = clients[i];
            if (!bananaProbability.ContainsKey(id))
                bananaProbability[id] = 1f;

            weights[i] = bananaProbability[id];
            totalWeight += weights[i];
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < clients.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return clients[i];
        }

        return clients[clients.Count - 1];
    }

    private void UpdateBananaProbabilities(ulong bananaClientId, IReadOnlyList<ulong> clients)
    {
        foreach (ulong id in clients)
        {
            if (id == bananaClientId)
                bananaProbability[id] = 1f; // reset après avoir été Banane
            else
                bananaProbability[id] = (bananaProbability.TryGetValue(id, out float p) ? p : 1f)
                                        + GameConstants.BananaChanceIncrement;
        }
    }
}
