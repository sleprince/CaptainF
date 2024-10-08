using Photon.Pun;
using UnityEngine;

public class MultiplayerSceneInitializer : MonoBehaviourPunCallbacks
{
    private const string gameSetupControllerPrefab = "GameSetupController";

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && GameObject.FindObjectOfType<GameSetupController>() == null)
        {
            PhotonNetwork.InstantiateRoomObject(gameSetupControllerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("GameSetupController instantiated by Master Client in multiplayer scene.");
        }
    }
}