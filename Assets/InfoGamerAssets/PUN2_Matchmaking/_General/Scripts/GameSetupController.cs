using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetupController : MonoBehaviour
{
    [SerializeField]
    private GameObject player1SpawnPoint;
    [SerializeField]
    private GameObject player2SpawnPoint;

    public int ID;

    void Start()
    {
        DestroyOldInputManager(); // Destroy the old InputManager if it exists
        AssignPlayerID(); // Assign correct player ID based on whether the player is Master Client or not
        CreatePlayer(ID);  // Create a networked player object for each player that loads into the multiplayer scene
        EnsureSingleEventSystem();
    }

    private void EnsureSingleEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);  // Destroy additional event systems
            }
        }
    }

    private void DestroyOldInputManager()
    {
        // Look for any existing InputManager and destroy it, but only if it's not part of a Photon player prefab
        InputManager oldInputManager = FindObjectOfType<InputManager>();

        if (oldInputManager != null && oldInputManager.transform.root.gameObject.tag != "Player")
        {
            Debug.Log("Destroying old InputManager");
            Destroy(oldInputManager.gameObject);
        }
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
        GameObject spawnPoint = ID == 1 ? player1SpawnPoint : player2SpawnPoint;
        Debug.Log("Creating Player with ID: " + ID);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer" + ID), spawnPoint.transform.position, Quaternion.identity);
    }
}
