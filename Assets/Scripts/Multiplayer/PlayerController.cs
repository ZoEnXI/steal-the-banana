using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        // Seul le joueur qui possède cet objet peut le contrôler
        if (!IsOwner) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Récupérer les inputs (fonctionne avec les touches directionnelles ou ZQSD par défaut)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            // Déplacer le joueur
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
