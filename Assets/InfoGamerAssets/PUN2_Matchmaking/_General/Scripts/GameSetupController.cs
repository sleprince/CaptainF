using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    [SerializeField]
    private GameObject SpawnPoint;
    public int ID;

    // This script will be added to any multiplayer scene
    void Start()
    {
        AssignPlayerID(); // Assign correct player ID based on whether the player is Master Client or not
        CreatePlayer(ID);  // Create a networked player object for each player that loads into the multiplayer scene
    }

    private void AssignPlayerID()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Assign ID = 1 to the Master Client (Player 1)
            ID = 1;
        }
        else
        {
            // Assign ID = 2 to the non-master client (Player 2)
            ID = 2;
        }

        Debug.Log("Player assigned ID: " + ID);
    }

    private void CreatePlayer(int ID)
    {
        Debug.Log("Creating Player with ID: " + ID);
        // Load the appropriate player prefab based on the assigned ID
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer" + ID),
                                  SpawnPoint.transform.position,
                                  Quaternion.identity);
    }
}
