using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {
	
	public Texture2D backTexture;
	public GUIStyle myStyle;

	void Start(){
		myStyle.fontSize = Screen.height/15;
	}
	void OnGUI(){
		if(GUI.Button(new Rect(Screen.width/2-Screen.height/5, Screen.height/2, Screen.height/6, Screen.height/12), "Easy", myStyle)) {
			Application.LoadLevel(2);
		}
		
		if(GUI.Button(new Rect(Screen.width/2+5, Screen.height/2, Screen.height/6, Screen.height/12), "Hard", myStyle)) {
			Application.LoadLevel(3);
		}
		
		if(GUI.Button(new Rect(5, 5,Screen.height/12,Screen.height/12), backTexture, myStyle)) {
			Application.LoadLevel(0);
		}
	}
}
