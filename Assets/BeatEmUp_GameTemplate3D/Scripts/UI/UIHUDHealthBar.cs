using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  // Required for networked multiplayer

[RequireComponent(typeof(Slider))]
public class UIHUDHealthBar : MonoBehaviour
{

    public Text nameField;
    public Image playerPortrait;
    public Slider HpSlider;
    public bool isPlayer;

    private HealthSystem playerHealthSystem;

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
        if (!isPlayer) Invoke("HideOnDestroy", Time.deltaTime); // Hide enemy health bar at the start
        if (isPlayer) SetPlayerForHUD();  // Assign the local player for this HUD
    }

    // Updates health for both player and enemy health bars
    void UpdateHealth(float percentage, GameObject go)
    {
        if (isPlayer && go.CompareTag("Player"))
        {
            HpSlider.value = percentage;
        }

        if (!isPlayer && go.CompareTag("Enemy"))
        {
            HpSlider.gameObject.SetActive(true);
            HpSlider.value = percentage;
            nameField.text = go.GetComponent<EnemyActions>().enemyName;
            if (percentage == 0) Invoke("HideOnDestroy", 2);
        }
    }

    void HideOnDestroy()
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
