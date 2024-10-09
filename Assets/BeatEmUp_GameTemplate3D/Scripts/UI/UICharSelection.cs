using System.Collections;
using UnityEngine;

public class UICharSelection : UISceneLoader {

	private UICharSelectionPortrait[] portraits;
    private InputManager inputManager;

    void OnEnable()
    {
        // Check if the game is connected to the network
        if (Photon.Pun.PhotonNetwork.IsConnected)
        {
            // Networked setup: look for InputManager as a child of the player
            inputManager = GetComponentInChildren<InputManager>();
        }
        else
        {
            // Local setup: find InputManager anywhere in the scene
            inputManager = FindObjectOfType<InputManager>();
        }

        if (inputManager != null)
        {
            inputManager.onInputEvent += OnInputEvent;
        }
    }

    void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.onInputEvent -= OnInputEvent;
        }
    }

    void Start(){
		
		//find all character portraits
		portraits = GetComponentsInChildren<UICharSelectionPortrait>();

		//select first portrait by default
		GetComponentInChildren<UICharSelectionPortrait>().OnClick();
	}

	void OnInputEvent(string action, BUTTONSTATE buttonState){

		//move left
		if(action == "Left" && buttonState == BUTTONSTATE.PRESS) OnLeftButtonDown();

		//move right
		if(action == "Right" && buttonState == BUTTONSTATE.PRESS) OnRightButtonDown();

	}

	//select portrait on the left
	void OnLeftButtonDown(){
		int selectedPortrait = getSelectedPortrait();
		portraits[selectedPortrait].Selected = false; //disable the current selection
		if(selectedPortrait-1 >= 0) portraits[selectedPortrait-1].OnClick(); //select previous portrait
	}

	//select portrait on the right
	void OnRightButtonDown(){
		int selectedPortrait = getSelectedPortrait();
		portraits[selectedPortrait].Selected = false; //disable the current selection
		if(selectedPortrait+1 < portraits.Length) portraits[selectedPortrait+1].OnClick(); //select next portrait
	}

	//returns the index of the current selected portrait
	int getSelectedPortrait(){
		for(int i = 0; i < portraits.Length; i++) {
			if(portraits[i].Selected) return i;
		}
		return 0;
	}

	//select a player
	public void SelectPlayer(GameObject playerPrefab){
		GlobalGameSettings.Player1Prefab = playerPrefab;
	}
}
