using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HealthPickup : MonoBehaviour {

	public int RestoreHP = 1;
	public string pickupSFX = "";
	public GameObject pickupEffect;
	public float pickUpRange = 1;
	private GameObject[] Players;

    private PhotonView photonView; // Photon view component

    void Start(){
		Players = GameObject.FindGameObjectsWithTag("Player");

        photonView = GetComponent<PhotonView>();
    }

	void LateUpdate(){
		

        if (PhotonNetwork.IsConnected)
        {
            foreach (GameObject player in Players)
            {
                if (player)
                {
                    float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

                    // Player is in pickup range
                    if (distanceToPlayer < pickUpRange)
                    {

                        // Only trigger the pickup event once
                        photonView.RPC("RPC_AddHealthToPlayer", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
                    }
                }
            }
        }
		else
		{
            foreach (GameObject player in Players)
            {
                if (player)
                {
                    float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

                    //player is in pickup range
                    if (distanceToPlayer < pickUpRange)
                        AddHealthToPlayer(player);
                }
            }

        }


    }

	//add health to player
	void AddHealthToPlayer(GameObject player){
		HealthSystem healthSystem = player.GetComponent<HealthSystem> ();

		if (healthSystem != null) {

			//restore hp to unit
			healthSystem.AddHealth(RestoreHP);

		} else {
			Debug.Log("no health system found on GameObject '" + player.gameObject.name + "'.");
		}

		//show pickup effect
		if (pickupEffect != null) {
			GameObject effect = GameObject.Instantiate (pickupEffect);
			effect.transform.position = transform.position;
		}

		//play sfx
		if (pickupSFX != "") {
			GlobalAudioPlayer.PlaySFXAtPosition (pickupSFX, transform.position);
		}

		Destroy(gameObject);
	}

    // RPC to add health and sync pickup destruction
    [PunRPC]
    void RPC_AddHealthToPlayer(int playerViewID)
    {
        GameObject player = PhotonView.Find(playerViewID).gameObject;
        HealthSystem healthSystem = player.GetComponent<HealthSystem>();

        if (healthSystem != null)
        {
            // Restore HP to the player
            healthSystem.AddHealth(RestoreHP);
        }
        else
        {
            Debug.Log("No health system found on GameObject '" + player.gameObject.name + "'.");
        }

        // Show pickup effect
        if (pickupEffect != null)
        {
            GameObject effect = GameObject.Instantiate(pickupEffect);
            effect.transform.position = transform.position;
        }

        // Play SFX
        if (pickupSFX != "")
        {
            GlobalAudioPlayer.PlaySFXAtPosition(pickupSFX, transform.position);
        }

        // Destroy the pickup across the network

            PhotonNetwork.Destroy(gameObject);
        
    }

#if UNITY_EDITOR
    void OnDrawGizmos(){

		//Show pickup range
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, pickUpRange); 

	}
	#endif
}
