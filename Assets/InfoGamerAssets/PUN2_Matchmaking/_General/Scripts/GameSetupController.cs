using Photon.Pun;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    [SerializeField]
    private GameObject player1SpawnPoint;  // Assign this in the inspector for Player 1's spawn point
    [SerializeField]
    private GameObject player2SpawnPoint;  // Assign this in the inspector for Player 2's spawn point
    public int ID;

    void Start()
    {
        AssignPlayerID();  // Assign correct player ID based on whether the player is the Master Client or not
        CreatePlayer(ID);   // Create a networked player object for each player that loads into the multiplayer scene
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
        // Choose spawn point based on the player ID
        Vector3 spawnPosition;
        if (ID == 1)
        {
            spawnPosition = player1SpawnPoint.transform.position;  // Use Player 1's spawn point
        }
        else
        {
            spawnPosition = player2SpawnPoint.transform.position;  // Use Player 2's spawn point
        }

        // Load the appropriate player prefab based on the assigned ID
        string prefabName = Path.Combine("PhotonPrefabs", "PhotonPlayer" + ID);
        Debug.Log("Creating Player with ID: " + ID + " at spawn position: " + spawnPosition);

        PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);
    }
}
