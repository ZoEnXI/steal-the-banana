using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRoleData", menuName = "StealTheBanana/PlayerRoleData")]
public class PlayerRoleData : ScriptableObject
{
    public PlayerRole role;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float knockPower = 10f;       // force appliquée sur la cible lors d'une frappe
    public float knockResistance = 5f;   // seuil de force reçue avant d'être assommé

    [Header("Dash (Lemurien uniquement)")]
    public bool hasDash;
    public float dashForce = 15f;
    public float dashCooldown = 3f;

    [Header("Pouvoir spécial (Alt)")]
    public float abilityCooldown = 10f;
}
