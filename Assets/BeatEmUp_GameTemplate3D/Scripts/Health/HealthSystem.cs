using Photon.Pun;
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



	//substract health
	public void SubstractHealth(int damage){

		if (PhotonNetwork.IsConnected && this.CompareTag("Player"))
		{
			photonView.RPC("RPC_ApplyHealthReduction", RpcTarget.All, damage);
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
    private void RPC_ApplyHealthReduction(int damage)
    {
        if (!invulnerable)
        {

            //reduce hp
            CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

            //Health reaches 0
            if (isDead()) gameObject.SendMessage("Death", SendMessageOptions.DontRequireReceiver);
        }

        // Calculate health percentage
        float healthPercentage = (float)CurrentHp / MaxHp;

        // Find the correct UI GameObject based on player ID
        int playerID = GetComponent<PhotonView>().Owner.ActorNumber; // Using PhotonView's ActorNumber as the player ID
        GameObject playerUI = GameObject.Find("UI_Player" + playerID);

        if (playerUI != null)
        {
            // Update the health bar on the correct UI instance
            UIHUDHealthBar healthBar = playerUI.GetComponentInChildren<UIHUDHealthBar>();
            if (healthBar != null)
            {
                healthBar.UpdateHealth(healthPercentage, this.gameObject);
            }
        }

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
