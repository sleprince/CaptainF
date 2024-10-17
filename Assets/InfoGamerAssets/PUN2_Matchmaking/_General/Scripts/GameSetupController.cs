using UnityEngine;
using Photon.Pun;
using System.IO;

public class GameSetupController : MonoBehaviour
{
    [SerializeField]
    private GameObject player1SpawnPoint;
    [SerializeField]
    private GameObject player2SpawnPoint;
    [SerializeField]
    private GameObject[] additionalSpawnPoints;  // Additional spawn points for more than 2 players

    public int ID;

    // The camera prefab to be instantiated for each player
    public GameObject cameraPrefab;

    // InputManager prefab (added to instantiate it for each player)
    public GameObject inputManagerPrefab;

    public GameObject uiPrefab;

    public GameObject EnemyHealthBar;

    // Prefab names in Resources folder
    private const string enemyWaveSystemPrefab = "EnemyWaveSystem";
    private const string itemsPrefab = "Items";


    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        DontDestroyOnLoad(this.gameObject);

        // Only the Master Client should instantiate dynamic game elements
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateDynamicObjects();
        }

        AssignPlayerID();         // Assign correct player ID based on whether the player is Master Client or not
        CreatePlayer(ID);         // Create a networked player object for each player that loads into the multiplayer scene

    }

    private void InstantiateDynamicObjects()
    {
        // Instantiate EnemyWaveSystem at Vector3.zero and Quaternion.identity
        GameObject enemyWaveSystem = PhotonNetwork.InstantiateRoomObject(enemyWaveSystemPrefab, Vector3.zero, Quaternion.identity);
        enemyWaveSystem.name = "EnemyWaveSystem";

        // Instantiate Items at Vector3.zero and Quaternion.identity
        GameObject items = PhotonNetwork.InstantiateRoomObject(itemsPrefab, Vector3.zero, Quaternion.identity);
        items.name = "Items";

        // Set objects to their prefab positions
        enemyWaveSystem.transform.localPosition = enemyWaveSystem.GetComponent<Transform>().position;
        enemyWaveSystem.transform.localRotation = enemyWaveSystem.GetComponent<Transform>().rotation;

        items.transform.localPosition = items.GetComponent<Transform>().position;
        items.transform.localRotation = items.GetComponent<Transform>().rotation;
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
            // Assign ID = the next available player number for other clients
            ID = PhotonNetwork.PlayerList.Length;
        }

        Debug.Log("Player assigned ID: " + ID);
    }

    private void CreatePlayer(int ID)
    {
        // Determine the spawn point based on player ID
        GameObject spawnPoint = GetSpawnPoint(ID);
        Debug.Log("Creating Player with ID: " + ID);

        // Instantiate the player prefab with the correct ID
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer" + ID), spawnPoint.transform.position, Quaternion.identity);

        // Instantiate and assign a camera to each player
        if (cameraPrefab != null)
        {
            CreateAndAssignCamera(player, ID);
        }

        // Instantiate and assign InputManager for each player
        if (inputManagerPrefab != null)
        {
            CreateAndAssignInputManager(player, ID);
        }

        CreateAndAssignUI(player, ID);

    }

    private GameObject GetSpawnPoint(int ID)
    {
        // Return the appropriate spawn point for the player ID
        if (ID == 1)
        {
            return player1SpawnPoint;
        }
        else if (ID == 2)
        {
            return player2SpawnPoint;
        }
        else if (ID - 3 < additionalSpawnPoints.Length)
        {
            return additionalSpawnPoints[ID - 3]; // Spawn points for players 3, 4, etc.
        }
        else
        {
            Debug.LogWarning("No spawn point found for Player ID: " + ID);
            return player1SpawnPoint;  // Fallback to Player 1's spawn point if no others are available
        }
    }

    private void CreateAndAssignCamera(GameObject player, int playerID)
    {
        // Instantiate the camera
        GameObject cameraInstance = Instantiate(cameraPrefab);

        // Optionally set a name for better organization in the hierarchy
        cameraInstance.name = "PlayerCamera" + playerID;

        // Find the CameraFollow component and assign the target to the instantiated player
        CameraFollow cameraFollow = cameraInstance.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.targets = new GameObject[] { player };
        }

        Debug.Log("Camera instantiated for Player " + playerID);
    }

    private void CreateAndAssignInputManager(GameObject player, int playerID)
    {
        // Instantiate InputManager prefab (assuming you have an InputManager prefab in the project)
        GameObject inputManagerInstance = Instantiate(inputManagerPrefab);

        // Set the InputManager instance as a child of the player GameObject
        inputManagerInstance.transform.SetParent(player.transform);

        // Set a unique name to better organize in the hierarchy
        inputManagerInstance.name = "InputManager_Player" + playerID;

        // Find the InputManager script component and set the PlayerID
        InputManager inputManager = inputManagerInstance.GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.PlayerID = playerID; // Assign the player ID or PhotonView ID
            inputManager.playerPhotonView = player.GetComponent<PhotonView>(); // Assign the player's PhotonView
            Debug.Log("Assigned PlayerID " + playerID + " to InputManager and set as child of player.");
        }
    }

    private void CreateAndAssignUI(GameObject player, int playerID)
    {
        // Find the existing UI object and destroy it if necessary
        GameObject oldUI = GameObject.FindWithTag("UI");
        if (oldUI != null)
        {
            Destroy(oldUI);
        }

        // Instantiate UI from Resources folder using Photon for networked play
        GameObject uiInstance = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);

        // Set a unique name to better organize it in the hierarchy
        uiInstance.name = "UI_Player" + playerID;

        // Ensure UI stays intact across scenes if needed
        DontDestroyOnLoad(uiInstance);

        Debug.Log("UI instantiated for Player " + playerID);


        // Find the HUD transform under the UI instance where the health bar should be instantiated
        Transform hudTransform = uiInstance.transform.Find("Canvas/HUD");

        if (hudTransform == null)
        {
            Debug.LogError("HUD Transform not found. Ensure that Canvas and HUD objects are correctly named and nested in the UI prefab.");
            return;
        }

        // Instantiate EnemyHealthBar at the world position of the HUD and then set it as a child
        GameObject healthBarInstance = Instantiate(EnemyHealthBar, hudTransform.position, Quaternion.identity);
        healthBarInstance.name = "EnemyHealthBar";

        // Set the HUD as the parent and reset local position and scale to align correctly within HUD
        healthBarInstance.transform.SetParent(hudTransform, false);

        // Use RectTransform for positioning within the Canvas UI system
        RectTransform healthBarRect = healthBarInstance.GetComponent<RectTransform>();

        // Set anchor to top-right
        healthBarRect.anchorMin = new Vector2(1, 1);
        healthBarRect.anchorMax = new Vector2(1, 1);

        // Lock the anchored position to zero to ensure it stays in the top-right
        healthBarRect.anchoredPosition = Vector2.zero;
        healthBarRect.localScale = Vector3.one;        // Ensure it has a scale of 1
        healthBarRect.localRotation = Quaternion.identity; // Reset rotation to default

        // Ensure anchored position is not affected again by subsequent updates
        healthBarRect.pivot = new Vector2(1, 1); // Keep pivot at top-right if needed
        healthBarRect.anchoredPosition3D = Vector3.zero; // Locks 3D position to zero as well, in case of other scripts or influences


        if (uiInstance.name == "UINetwork(Clone)")
        {
            Destroy(uiInstance);
        }
    }


}
