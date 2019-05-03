using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	public Texture2D quitTexture;
	public Texture2D singleTexture;
	public Texture2D twoTexture;
	public Texture2D helpTexture;

	public GUIStyle myStyle;

	void Awake(){
		Application.targetFrameRate = 30;
	}

	void OnGUI () {
		if(GUI.Button(new Rect(Screen.width/2-Screen.height/5, Screen.height/2, Screen.height/6, Screen.height/6), singleTexture, myStyle)) {
			Application.LoadLevel(1);
		}

		if(GUI.Button(new Rect(Screen.width/2+5, Screen.height/2, Screen.height/6, Screen.height/6), twoTexture, myStyle)) {
			Application.LoadLevel(4);
		}

		if(GUI.Button(new Rect(Screen.width*5/6, 5,Screen.height/12,Screen.height/12), quitTexture, myStyle)) {
			Application.Quit();
		}

		if(GUI.Button(new Rect(Screen.width*5/6, Screen.height-Screen.height*3/15-3,Screen.height/12,Screen.height/12), helpTexture, myStyle)) {
			Application.LoadLevel(5);
		}
	}
}
