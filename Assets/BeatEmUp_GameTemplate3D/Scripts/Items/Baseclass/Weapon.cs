﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon {

	public string weaponName;
	public GameObject playerHandPrefab;
	public GameObject WeaponEndState; // a gameobject that visually represent the end state of this weapon (e.g. a broken club, or empty gun)
	public int timesToUse = 1;
	public DEGENERATETYPE degenerateType; //select if this weapon degenerates on use or on hit (e.g. a gun degenerates on use, a club degenerates on hit)

	public DamageObject damageObject;
	[Header("Sound Effects")]
	public string useSound = "";
	public string breakSound = "";

    private PhotonView photonView;

    public void useWeapon(){

			timesToUse = Mathf.Clamp(timesToUse - 1, 0, 1000);

	}

    // This RPC is called to update the weapon usage count for all clients
    [PunRPC]

    public void onHitSomething(){
		if(degenerateType == DEGENERATETYPE.DEGENERATEONHIT) useWeapon();

		//play break sfx on last hit
		if(timesToUse == 1) damageObject.hitSFX = breakSound;
	}

	public void BreakWeapon(){

        if (PhotonNetwork.IsConnected && WeaponEndState != null)
        {

            GameObject g = g = PhotonNetwork.InstantiateRoomObject(WeaponEndState.name, playerHandPrefab.transform.position, Quaternion.identity);
        }
		else
		{
            if (WeaponEndState)
            {
                GameObject g = GameObject.Instantiate(WeaponEndState) as GameObject;
                g.transform.position = playerHandPrefab.transform.position;
            }

        }

  
	}
}

public enum DEGENERATETYPE {
	DEGENERATEONUSE,
	DEGENERATEONHIT,
}
