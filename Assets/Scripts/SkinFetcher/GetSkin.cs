using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Tantan;

public class GetSkin : MonoBehaviour
{
	public GameObject player64x32;
	public GameObject player64x64;
	public GameObject playerSlim;
	public Picker m_picker;
	public PlayerPhysics m_playerPhysics;
	private Texture2D skin;
    public string username = "BarkBoat";
	private string lastVerifiedUsername;

    private void Awake()
    {
    }

	public void Refresh() {
		StartCoroutine(GetTexture(username));
	}

	public void Set(string a_username)
	{
		username = a_username;
		Refresh();
	}

	IEnumerator GetTexture(string username) {
		UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://minotar.net/skin/" + username);
		yield return www.SendWebRequest();

		GameObject currentGO = null;
		if(www.isNetworkError || www.isHttpError) {
			Debug.Log("Username invalid or not found!");
		}
		else {
			lastVerifiedUsername = username;
			skin = ((DownloadHandlerTexture)www.downloadHandler).texture;
			skin.filterMode = FilterMode.Point;
			if (skin.height == 64) {
				if (skin.GetPixel (50, 44).a == 0) {
					for (int i = 0; i < 6; i++) {
						playerSlim.transform.GetChild (i).gameObject.GetComponent<Renderer> ().material.mainTexture = skin;
					}
					playerSlim.SetActive (true);
					player64x64.SetActive (false); 
					player64x32.SetActive (false);
					currentGO = playerSlim;
				} else {
					for (int i = 0; i < 6; i++) {
						player64x64.transform.GetChild (i).gameObject.GetComponent<Renderer> ().material.mainTexture = skin;
					}
					player64x64.SetActive (true); 
					player64x32.SetActive (false);
					playerSlim.SetActive (false);
					currentGO = player64x64;
				}
			} else {
				for (int i = 0; i < 6; i++) {
					player64x32.transform.GetChild (i).gameObject.GetComponent<Renderer> ().material.mainTexture = skin;
				}
				player64x32.SetActive (true); 
				player64x64.SetActive (false); 
				playerSlim.SetActive (false); 
				currentGO = player64x32;
			}
		}
		Animator currentAnimator = currentGO.GetComponent<Animator>();
		m_picker.SetAnimator(currentAnimator);
		m_playerPhysics.SetAnimator(currentAnimator);
	}

	public void Download() {
		Application.OpenURL("https://minotar.net/download/" + lastVerifiedUsername);
	}
}