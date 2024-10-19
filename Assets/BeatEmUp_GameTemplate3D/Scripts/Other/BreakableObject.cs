using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableObject : MonoBehaviour, IDamagable<DamageObject> {

	public string hitSFX = "";

	[Header ("Gameobject Destroyed")]
	public GameObject destroyedGO;
	public bool orientToImpactDir;

	[Header ("Spawn an item")]
	public GameObject spawnItem;
	public float spawnChance = 100;

	[Space(10)]
	public bool destroyOnHit;

    private PhotonView photonView;

    void Start(){

        photonView = GetComponent<PhotonView>();

        gameObject.layer = LayerMask.NameToLayer("DestroyableObject");
	}

	//this object was Hit
	public void Hit(DamageObject DO)
	{

		if (PhotonNetwork.IsConnected && photonView != null && photonView.IsMine)
		{
			photonView.RPC("RPC_HandleHit", RpcTarget.All, DO.inflictor != null ? DO.inflictor.transform.position.x : 0);
		}
		else
		{

			//play hit sfx
			if (hitSFX != "")
			{
				GlobalAudioPlayer.PlaySFXAtPosition(hitSFX, transform.position);
			}

			//spawn destroyed gameobject version
			if (destroyedGO != null)
			{
				GameObject BrokenGO = GameObject.Instantiate(destroyedGO);
				BrokenGO.transform.position = transform.position;

				//chance direction based on the impact direction
				if (orientToImpactDir && DO.inflictor != null)
				{
					float dir = Mathf.Sign(DO.inflictor.transform.position.x - transform.position.x);
					BrokenGO.transform.rotation = Quaternion.LookRotation(Vector3.forward * dir);
				}
			}

			//spawn an item
			if (spawnItem != null)
			{
				if (Random.Range(0, 100) < spawnChance)
				{
					GameObject item = GameObject.Instantiate(spawnItem);
					item.transform.position = transform.position;

					//add up force to object
					item.GetComponent<Rigidbody>().velocity = Vector3.up * 8f;
				}
			}

			//destroy 
			if (destroyOnHit)
			{
				Destroy(gameObject);
			}
		}
	}

    [PunRPC]
    void RPC_HandleHit(float inflictorXPos)
    {
        // Play hit SFX
        if (hitSFX != "")
        {
            GlobalAudioPlayer.PlaySFXAtPosition(hitSFX, transform.position);
        }

        // Spawn destroyed gameobject version
        if (destroyedGO != null)
        {
            GameObject BrokenGO = GameObject.Instantiate(destroyedGO);
            BrokenGO.transform.position = transform.position;

            // Orient to impact direction
            if (orientToImpactDir)
            {
                float dir = Mathf.Sign(inflictorXPos - transform.position.x);
                BrokenGO.transform.rotation = Quaternion.LookRotation(Vector3.forward * dir);
            }
        }

        // Spawn an item
        if (spawnItem != null)
        {
            if (Random.Range(0, 100) < spawnChance)
            {
                GameObject item = GameObject.Instantiate(spawnItem);
                item.transform.position = transform.position;

                // Add upward force to object
                item.GetComponent<Rigidbody>().velocity = Vector3.up * 8f;
            }
        }

        // Destroy the object
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

}