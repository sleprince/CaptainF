using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneLoader : MonoBehaviour
{

    private bool loadSceneInProgress;

    //load a new scene
    public void LoadScene(string sceneName)
    {
        if (!loadSceneInProgress) StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        loadSceneInProgress = true;

        //Fade out screen
        UIFader fader = GameObject.FindObjectOfType<UIFader>();
        if (fader != null) fader.Fade(UIFader.FADE.FadeOut, 0.4f, 0.4f);
        yield return new WaitForSeconds(1f);

        // Check if we need to reconnect to Photon
        if (PhotonNetwork.IsConnected)
        {
            // If connected to Photon network, load scene for all players synchronously
            PhotonNetwork.LoadLevel(sceneName);
        }
        else if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Disconnected)
        {
            // Try reconnecting to Photon if disconnected
            Debug.Log("Attempting to reconnect to Photon...");
            PhotonNetwork.ConnectUsingSettings();

            // Wait until connected to Photon
            while (!PhotonNetwork.IsConnectedAndReady)
            {
                yield return null;
            }

            // Load scene for all players synchronously after reconnection
            Debug.Log("Reconnected to Photon. Loading scene for all players...");
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            // For local games, load the scene directly
            Debug.Log("Reloading Scene Locally");
            SceneManager.LoadScene(sceneName);
        }

        loadSceneInProgress = false;
    }
}
