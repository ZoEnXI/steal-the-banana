using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Assign in Inspector (or leave empty to auto-find 'TP1'/'TP2')")]
    [SerializeField] private Transform tp1;
    [SerializeField] private Transform tp2;

    [Header("Settings")]
    [SerializeField] private float teleportDelay = 0.2f;
    [SerializeField] private float teleportCooldown = 1.5f;

    private float lastTeleportTime = -100f;

    private void Start()
    {
        // Auto-find children if not assigned
        if (tp1 == null) tp1 = transform.Find("TP1");
        if (tp2 == null) tp2 = transform.Find("TP2");

        if (tp1 != null && tp2 != null)
        {
            SetupPad(tp1, tp2);
            SetupPad(tp2, tp1);
        }
        else
        {
            Debug.LogWarning("[Teleporter] TP1 ou TP2 introuvable ! Vérifie les noms ou assigne-les dans l'Inspector.");
        }
    }

    private void SetupPad(Transform padTransform, Transform destination)
    {
        // On s'assure qu'il y a bien un collider en mode trigger
        Collider col = padTransform.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"[Teleporter] Ajoute un BoxCollider (IsTrigger) sur {padTransform.name}");
        }

        // On ajoute le sous-script qui va détecter la collision
        TeleportPad pad = padTransform.gameObject.AddComponent<TeleportPad>();
        pad.Init(this, destination);
    }

    public void TriggerTeleport(Collider other, Transform destination)
    {
        // Le mouvement étant géré par le Serveur, seul le Serveur TP le joueur
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        // Cooldown pour éviter le ping-pong infini
        if (Time.time < lastTeleportTime + teleportCooldown)
        {
            return;
        }

        // On vérifie si c'est bien le joueur (le PlayerController et le Rigidbody sont dessus)
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            // On bloque les autres TP pendant le cooldown
            lastTeleportTime = Time.time + teleportCooldown;
            StartCoroutine(DoTeleport(player.GetComponent<Rigidbody>(), destination.position));
        }
    }

    private IEnumerator DoTeleport(Rigidbody playerRb, Vector3 destPos)
    {
        // Attente du délai
        yield return new WaitForSeconds(teleportDelay);
        
        if (playerRb != null)
        {
            // On TP le joueur via son Rigidbody (NetworkTransform va synchroniser automatiquement)
            playerRb.position = destPos;
        }
    }
}

// Mini-script ajouté automatiquement aux enfants TP1 et TP2 pour capter OnTriggerEnter
public class TeleportPad : MonoBehaviour
{
    private Teleporter parentTeleporter;
    private Transform destination;

    public void Init(Teleporter parent, Transform dest)
    {
        parentTeleporter = parent;
        destination = dest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentTeleporter != null && destination != null)
        {
            parentTeleporter.TriggerTeleport(other, destination);
        }
    }
}
