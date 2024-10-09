using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIInputList : MonoBehaviour {

	private int maxIconCount = 20; //the max number of icons in the list

    private InputManager inputManager;

    void OnEnable()
    {
        // Find the InputManager instance
        inputManager = FindObjectsOfType<InputManager>()
            .FirstOrDefault(im => !im.isNetworkedGame ||
                                  (im.isNetworkedGame && im.playerPhotonView != null && im.playerPhotonView.IsMine));

        if (inputManager != null)
        {
            inputManager.onInputEvent += OnInputEvent;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from the events
        if (inputManager != null)
        {
            inputManager.onInputEvent -= OnInputEvent;
        }
    }

    void OnInputEvent(string action, BUTTONSTATE buttonState){
		if(buttonState != BUTTONSTATE.PRESS) return; //only respond to button press states

		Sprite icon = Resources.Load<Sprite>("Icons/Icon" + action);
		if(icon != null) AddIcon(action.ToString(), icon);
	}
		
	//adds a new icon to the input list
	void AddIcon(string iconName, Sprite iconSprite){
		GameObject icon = new GameObject();
		Image img = icon.AddComponent<Image>();
		img.sprite = iconSprite;
		icon.GetComponent<RectTransform>().SetParent(transform);
		icon.GetComponent<RectTransform>().transform.localScale = Vector3.one;
		icon.transform.SetAsFirstSibling();
		icon.name = iconName;
		icon.gameObject.AddComponent<UISpriteFade>();
		if(transform.childCount > maxIconCount) Destroy(transform.GetChild(maxIconCount).gameObject);
	}
}