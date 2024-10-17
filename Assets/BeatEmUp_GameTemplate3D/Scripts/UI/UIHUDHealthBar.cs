using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // Required for networked multiplayer
using ExitGames.Client.Photon;  

[RequireComponent(typeof(Slider))]
public class UIHUDHealthBar : MonoBehaviour
{

    public Text nameField;
    public Image playerPortrait;
    public Slider HpSlider;
    public bool isPlayer;

    private HealthSystem playerHealthSystem;


    private PhotonView photonView;

    private const string HealthBarInstantiatedKey = "HealthBarInstantiated";

    void OnEnable()
    {
        HealthSystem.onHealthChange += UpdateHealth;
    }

    void OnDisable()
    {
        HealthSystem.onHealthChange -= UpdateHealth;
    }

   

    void Start()
    {
        photonView = GetComponent<PhotonView>();



        if (!isPlayer) Invoke("HideOnDestroy", Time.deltaTime); // Hide enemy health bar at the start
        if (isPlayer) SetPlayerForHUD();  // Assign the local player for this HUD
    }


    // Updates health for both player and enemy health bars
    public void UpdateHealth(float percentage, GameObject go)
    {
        PhotonView targetPhotonView = go.GetComponent<PhotonView>();

        // Ensure only the local player's health bar updates when they are hit
        if (isPlayer && go.CompareTag("Player"))
        {
            if (PhotonNetwork.IsConnected)
            {
                // Only update if the player is the owner
                if (targetPhotonView != null && targetPhotonView.IsMine)
                {
                    HpSlider.value = percentage;
                }
            }
            else
            {
                // In offline/local play, update the health directly
                HpSlider.value = percentage;
            }
        }

        if (!isPlayer && go.CompareTag("Enemy"))
        {
            HpSlider.value = percentage;
            nameField.text = go.GetComponent<EnemyAI>().enemyName;
            HpSlider.gameObject.SetActive(true);
            if (percentage == 0) HideOnDestroy();
        }

    }

    public void HideOnDestroy()
    {
        HpSlider.gameObject.SetActive(false);
        nameField.text = "";
    }

    // Set the local player for this HUD (handles both single and networked play)
    void SetPlayerForHUD()
    {
        if (playerPortrait != null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();

                // If the player is networked and owned by the local player
                if (pv != null && pv.IsMine)
                {
                    playerHealthSystem = player.GetComponent<HealthSystem>();

                    // Set the player's portrait and name for the HUD
                    if (playerHealthSystem != null)
                    {
                        Sprite HUDPortrait = playerHealthSystem.HUDPortrait;
                        playerPortrait.overrideSprite = HUDPortrait;
                        nameField.text = playerHealthSystem.PlayerName;
                    }
                    break;
                }
                // For non-networked (local) play
                if (pv == null)
                {
                    playerHealthSystem = player.GetComponent<HealthSystem>();

                    if (playerHealthSystem != null)
                    {
                        Sprite HUDPortrait = playerHealthSystem.HUDPortrait;
                        playerPortrait.overrideSprite = HUDPortrait;
                        nameField.text = playerHealthSystem.PlayerName;
                    }
                    break;
                }
            }
        }
    }
}
