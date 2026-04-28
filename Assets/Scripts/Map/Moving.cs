using UnityEngine;

public class Moving : MonoBehaviour
{
    [Header("Paramètres du mouvement")]
    [SerializeField] private float speed = 2f;     // Vitesse de déplacement
    [SerializeField] private float distance = 5f;  // Distance parcourue de chaque côté (en X)
    [SerializeField] private bool oppositeDirections = false; // Cocher pour que la 2ème aille dans le sens inverse

    private Transform[] platforms;
    private Vector3[] startPositions;

    void Start()
    {
        // On récupère automatiquement tous les enfants (tes plateformes "1", "2", etc.)
        int childCount = transform.childCount;
        platforms = new Transform[childCount];
        startPositions = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            platforms[i] = transform.GetChild(i);
            startPositions[i] = platforms[i].position;
        }
    }

    void Update()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
            {
                // Mathf.Sin crée un cycle infini et fluide entre -1 et 1
                float offset = Mathf.Sin(Time.time * speed) * distance;
                
                // Si on a activé oppositeDirections, on inverse le mouvement une plateforme sur deux
                if (oppositeDirections && i % 2 != 0)
                {
                    offset = -offset;
                }

                // On applique la nouvelle position sur l'axe X
                platforms[i].position = startPositions[i] + new Vector3(offset, 0f, 0f);
            }
        }
    }
}
