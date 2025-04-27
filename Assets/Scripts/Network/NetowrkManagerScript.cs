using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetowrkManagerScript : MonoBehaviour
{
    public void StartHost()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Started as HOST");
        }
    }

    public void StartClient()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Started as CLIENT");
        }
    }

    public void CloseConnection()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Host shutdown");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            ulong localClientId = NetworkManager.Singleton.LocalClientId;
            NetworkManager.Singleton.DisconnectClient(localClientId);
            Debug.Log("Client " + localClientId + " Disconnected");
        }
    }
}
