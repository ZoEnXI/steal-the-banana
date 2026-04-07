using Unity.Netcode;
using UnityEngine;

public class LocalPlayerCameraFollow : NetworkBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);
    [SerializeField] private float followSpeed = 10f;

    private Camera mainCamera;

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!IsSpawned || !IsOwner || mainCamera == null)
        {
            return;
        }

        Vector3 desiredPosition = transform.position + offset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, followSpeed * Time.deltaTime);
        mainCamera.transform.LookAt(transform.position);
    }
}
