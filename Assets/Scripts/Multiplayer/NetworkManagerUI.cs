using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private string serverAddress = "127.0.0.1";
    [SerializeField] private string portText = "7777";

    private string statusMessage;

    private void OnGUI()
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        GUILayout.BeginArea(new Rect(10, 10, 800, 800));

        if (networkManager == null)
        {
            GUILayout.Label("NetworkManager introuvable dans la scene.");
            GUILayout.EndArea();
            return;
        }

        if (!networkManager.IsClient && !networkManager.IsServer)
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
        GUILayout.Label("Adresse IP du serveur");
        serverAddress = GUILayout.TextField(serverAddress);

        GUILayout.Space(5);

        GUILayout.Label("Port");
        portText = GUILayout.TextField(portText);

        GUILayout.Space(10);

        if (GUILayout.Button("Host (Server + Client)"))
        {
            if (ValidateNetworkConfig() && TryApplyTransportConfig(isHostOrServer: true))
            {
                NetworkManager.Singleton.StartHost();
                statusMessage = "Host demarre.";
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Client (Join only)"))
        {
            if (ValidateNetworkConfig() && TryApplyTransportConfig(isHostOrServer: false))
            {
                NetworkManager.Singleton.StartClient();
                statusMessage = "Client demarre.";
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Server (Dedicated)"))
        {
            if (ValidateNetworkConfig() && TryApplyTransportConfig(isHostOrServer: true))
            {
                NetworkManager.Singleton.StartServer();
                statusMessage = "Serveur dedie demarre.";
            }
        }

        if (!string.IsNullOrWhiteSpace(statusMessage))
        {
            GUILayout.Space(10);
            GUILayout.Label(statusMessage);
        }
    }

    private void StatusLabels()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        var mode = networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " + networkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);

        if (networkManager.IsServer)
        {
            GUILayout.Label("Clients connectes: " + networkManager.ConnectedClientsList.Count);
        }
        
        if (GUILayout.Button("Disconnect"))
        {
            networkManager.Shutdown();
            statusMessage = "Connexion arretee.";
        }
    }

    private bool TryApplyTransportConfig(bool isHostOrServer)
    {
        if (!ushort.TryParse(portText, out ushort port))
        {
            statusMessage = "Port invalide (0-65535).";
            return false;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null)
        {
            statusMessage = "UnityTransport manquant sur le NetworkManager.";
            return false;
        }

        string address = string.IsNullOrWhiteSpace(serverAddress) ? "127.0.0.1" : serverAddress.Trim();

        if (isHostOrServer)
        {
            transport.SetConnectionData(address, port, "0.0.0.0");
        }
        else
        {
            transport.SetConnectionData(address, port);
        }

        statusMessage = $"Transport configure vers {address}:{port}.";
        return true;
    }

    private bool ValidateNetworkConfig()
    {
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager.NetworkConfig.PlayerPrefab == null)
        {
            statusMessage = "Player Prefab non assigne dans NetworkManager.";
            return false;
        }

        if (!networkManager.NetworkConfig.PlayerPrefab.TryGetComponent<NetworkObject>(out _))
        {
            statusMessage = "Le Player Prefab doit contenir un composant NetworkObject.";
            return false;
        }

        if (!networkManager.NetworkConfig.PlayerPrefab.TryGetComponent<PlayerController>(out _))
        {
            statusMessage = "Le Player Prefab doit contenir PlayerController.";
            return false;
        }

        return true;
    }
}
