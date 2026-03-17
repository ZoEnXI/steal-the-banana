using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host (Server + Client)"))
        {
            NetworkManager.Singleton.StartHost();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Client (Join only)"))
        {
            NetworkManager.Singleton.StartClient();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Server (Dedicated)"))
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    private void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
