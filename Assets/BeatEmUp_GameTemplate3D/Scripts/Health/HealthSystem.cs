using Photon.Pun;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour {

	[Header("Health Settings")]
	public int MaxHp = 20;
	public int CurrentHp = 20;
	public bool invulnerable;

	#if UNITY_EDITOR
	[HelpAttribute("Change these settings if you want to change the player portrait or player name in the healthbar located in the upperleft corner of the screen.", UnityEditor.MessageType.Info)]
	#endif

	[Header("Healthbar Settings")]
	public Sprite HUDPortrait;
	public string PlayerName;

	public delegate void OnHealthChange(float percentage, GameObject GO);
	public static event OnHealthChange onHealthChange;

    private PhotonView photonView;

    void Start(){

        photonView = GetComponent<PhotonView>();

        SendHealthUpdateEvent();
	}

    private void Update()
    {
        // Toggle invulnerability on and off with the "I" key, for debugging
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (CompareTag("Player"))
            {
                invulnerable = !invulnerable;
            }

        }

        // Suicide button, for debugging
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (CompareTag("Player"))
            {
                CurrentHp = 1;
            }

        }


        if (PhotonNetwork.IsConnected && photonView.IsMine && CompareTag("Player"))
        {
            // Call RPC to toggle invulnerability for this player
            photonView.RPC("RPC_ToggleInvulnerability", RpcTarget.AllBuffered, !invulnerable);
        }
    }
    







    // RPC to toggle invulnerability across the network
    [PunRPC]
    void RPC_ToggleInvulnerability(bool newInvulnerableState)
    {
        invulnerable = newInvulnerableState;
    }


    //substract health
    public void SubstractHealth(int damage){

		if (PhotonNetwork.IsConnected)
		{
            int attackerID = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("RPC_ApplyHealthReduction", RpcTarget.All, damage, attackerID);
		}
		else
		{
			if (!invulnerable)
			{

				//reduce hp
				CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

				//Health reaches 0
				if (isDead()) gameObject.SendMessage("Death", SendMessageOptions.DontRequireReceiver);
			}

			SendHealthUpdateEvent();
		}
	}

    [PunRPC]
    private void RPC_ApplyHealthReduction(int damage, int attackerID)
    {
        if (!invulnerable)
        {

            //reduce hp
            CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

            //Health reaches 0
            if (isDead())
            {
                gameObject.SendMessage("Death", SendMessageOptions.DontRequireReceiver);

                // Health reaches 0 networked
                if (PhotonNetwork.IsConnected && CompareTag("Player") && CurrentHp == 0)
                {

                    Debug.Log("Game Over!");

                    StartCoroutine(ReStartLevel());





                }


            }

        
        }

        // Calculate health percentage
        float healthPercentage = (float)CurrentHp / MaxHp;

        // Determine the UI for the local player (attacker)
        GameObject playerUI = GameObject.Find("UI_Player" + attackerID);


        if (playerUI != null)
        {
            UIHUDHealthBar healthBar = null;

            // Check if it's a player or enemy, then select the correct health bar
            if (this.CompareTag("Player"))
            {
                healthBar = playerUI.GetComponentInChildren<UIHUDHealthBar>();

                // Update health bar if found
                if (healthBar != null)
                {
                    healthBar.UpdateHealth(healthPercentage, this.gameObject);
                }
            }


            else
            {

                // Ensure this block only runs for the local attacker
                if (PhotonNetwork.LocalPlayer.ActorNumber == attackerID)
                {
                    // Locate EnemyHealthBar under the attacker's UI
                    Transform healthBarTransform = playerUI.transform.Find("Canvas/HUD/EnemyHealthBar");
                    healthBar = healthBarTransform?.GetComponent<UIHUDHealthBar>();

                    // Update local UI for this player only
                    if (healthBar != null)
                    {
                        healthBar.UpdateHealth(healthPercentage, this.gameObject);
                        healthBar.nameField.text = this.GetComponent<EnemyActions>().enemyName;
                        healthBar.HpSlider.gameObject.SetActive(healthPercentage > 0); // Show until the enemy is dead
                        healthBar.nameField.gameObject.SetActive(healthPercentage > 0);

                        // Stop the current coroutine if already running
                        if (hideHealthBarCoroutine != null)
                        {
                            StopCoroutine(hideHealthBarCoroutine);
                        }

                        // Start a new coroutine to hide after delay
                        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(healthBar, 3.0f));
                    }

                    // Hide the health bar for all other players
                    photonView.RPC("RPC_UpdateHealthVisibility", RpcTarget.AllBuffered, attackerID, healthPercentage > 0);

                }

            }

        }



    }

    IEnumerator ReStartLevel()
    {
        yield return new WaitForSeconds(2);

        // Determine the UI for the local player (attacker)
        GameObject playerUI = GameObject.Find("UI_Player" + PhotonNetwork.LocalPlayer.ActorNumber);
        UIManagerNetwork UI = playerUI.GetComponent<UIManagerNetwork>();

        if (UI != null)
        {
            float fadeoutTime = 1.3f;
            //fade out
            UI.UI_fader.Fade(UIFader.FADE.FadeOut, fadeoutTime, 0);
            yield return new WaitForSeconds(fadeoutTime);

            // Show game over screen
            UI.DisableAllScreens();
            UI.ShowMenu("GameOver");
            //UI.GetComponentInChildren<UIControlSwitcher>().touchControlsOverlay.SetActive(false);
        }
        else
        {
            Debug.Log("UI is null");
        }

        // Notify all players to show the game over screen
        //photonView.RPC("RPC_GameOver", RpcTarget.All, UI);

   

            

           
       
    }



    [PunRPC]
    private void RPC_GameOver(UIManagerNetwork UI)
    {
        // Show game over screen
        UI.DisableAllScreens();
        UI.ShowMenu("GameOver");
    }



    [PunRPC]
    private void RPC_UpdateHealthVisibility(int attackerID, bool isVisible)
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject otherPlayerUI = GameObject.Find("UI_Player" + player.ActorNumber);

            if (otherPlayerUI != null)
            {
                Transform otherHealthBarTransform = otherPlayerUI.transform.Find("Canvas/HUD/EnemyHealthBar");

                if (otherHealthBarTransform != null)
                {
                    UIHUDHealthBar otherHealthBar = otherHealthBarTransform.GetComponent<UIHUDHealthBar>();

                    if (otherHealthBar != null)
                    {
                        // Show for the attacker, hide for others
                        bool shouldDisplay = player.ActorNumber == attackerID && isVisible;
                        otherHealthBar.gameObject.SetActive(shouldDisplay);
                    }
                }
            }
        }
    }

    private Coroutine hideHealthBarCoroutine;

    // Coroutine to hide health bar after delay
    private IEnumerator HideHealthBarAfterDelay(UIHUDHealthBar healthBar, float delay)
    {
        yield return new WaitForSeconds(delay);
        healthBar.gameObject.SetActive(false);
    }

    //add health
    public void AddHealth(int amount){
		CurrentHp = Mathf.Clamp(CurrentHp += amount, 0, MaxHp);
		SendHealthUpdateEvent();
	}

	//health update event
	void SendHealthUpdateEvent(){

        float CurrentHealthPercentage = 1f/MaxHp * CurrentHp;
		if(onHealthChange != null) onHealthChange(CurrentHealthPercentage, gameObject);
	}

	//death
	bool isDead(){
		return CurrentHp == 0;
	}
}
