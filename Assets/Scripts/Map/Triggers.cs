using Unity.Netcode;
using UnityEngine;

public class Triggers : MonoBehaviour
{
    public enum TriggerAction
    {
        SizeBoost,
        SizeDown,
        Explode
    }

    private void Start()
    {
        // Cherche automatiquement les enfants par leur nom et assigne leurs rôles
        SetupTrigger("SizeBoost", TriggerAction.SizeBoost);
        SetupTrigger("SizeDown", TriggerAction.SizeDown);
        SetupTrigger("Explode", TriggerAction.Explode);
    }

    private void SetupTrigger(string childName, TriggerAction action)
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            Collider col = child.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogWarning($"[Triggers] Ajoute un Collider (IsTrigger) sur l'enfant {childName}");
            }
            
            // Ajoute le mini-script de détection de collision
            var pad = child.gameObject.AddComponent<TriggerPad>();
            pad.Init(this, action);
        }
        else
        {
            Debug.LogWarning($"[Triggers] Enfant '{childName}' introuvable. Vérifie le nom dans la hiérarchie.");
        }
    }

    public void ActivateTrigger(Collider other, TriggerAction action, GameObject padObject)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) return;

        // L'effet physique/réseau ne s'applique que depuis le Serveur
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            switch (action)
            {
                case TriggerAction.SizeBoost:
                    player.transform.localScale *= 3.5f;
                    break;
                case TriggerAction.SizeDown:
                    player.transform.localScale *= 0.3f;
                    break;
                case TriggerAction.Explode:
                    // Désintègre le joueur proprement en multijoueur
                    NetworkObject netObj = player.GetComponent<NetworkObject>();
                    if (netObj != null && netObj.IsSpawned)
                    {
                        netObj.Despawn();
                    }
                    else
                    {
                        Destroy(player.gameObject);
                    }
                    break;
            }
        }

        // On détruit l'objet bonus/piège. 
        // Comme les triggers sont des objets normaux de la scène (souvent non-NetworkObject), 
        // chaque joueur qui le touche "physiquement" (trigger) va le détruire chez lui aussi localement.
        if (padObject != null)
        {
            Destroy(padObject);
        }
    }
}

// Mini-script caché ajouté automatiquement aux enfants SizeBoost / SizeDown / Explode
public class TriggerPad : MonoBehaviour
{
    private Triggers parentTriggers;
    private Triggers.TriggerAction action;
    private bool isTriggered = false;

    public void Init(Triggers parent, Triggers.TriggerAction act)
    {
        parentTriggers = parent;
        action = act;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered || parentTriggers == null) return;

        // Si ce qui collisionne possède bien un PlayerController (et pas le sol)
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            isTriggered = true;
            parentTriggers.ActivateTrigger(other, action, gameObject);
        }
    }
}
