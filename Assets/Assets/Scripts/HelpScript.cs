using UnityEngine;
using System.Collections;

public class HelpScript : MonoBehaviour {

	public GUIStyle mystyle;
	public Texture2D backTexture;

	public Vector2 scrollPosition;
	private string longString = "ELEVEN MEN'S MORRIS\n\n";

	void Start(){
		mystyle.fontSize = Screen.height/30;
		longString += " 1. The board consists of 24 spots.\n\n 2. The two players, alternatively place piece of their color on the board by turns.\n\n 3. Pieces can be placed on the vacant positions(Cubes).\n\n 4. Pieces can only be moved after placing all the pieces.\n\n 5. A piece can be moved to its adjacent empty position.\n\n 6. Players form a MILL(daadi) (three pieces in a single row or column of same player) while placing and moving the pieces.\n\n 7. A player removes opponent’s piece when he forms a MILL.\n\n 8. A piece which is not in the part of MILL can only be removed by opponent.\n\n 9. A player has to pass the turn to the opponent when he forms MILL and all the opponent pieces are in the part of MILL.\n\n 10. Player has to pass the turn when there is no chance of movement and the opponent should make way for the player.\n\n 11. A player who removes nine pieces of the opponent first is the winner.\n\n   ******************************";
	}

	void Update () {
		
		foreach(Touch touch in Input.touches) 
		{
			if(touch.phase == TouchPhase.Moved)
			{
				scrollPosition.y += touch.deltaPosition.y;        // dragging
			}
		}
	}

	void OnGUI() {

		if (GUI.Button(new Rect(5, 5, Screen.height/12, Screen.height/12), backTexture, mystyle)){
			Application.LoadLevel(0);
		}
		GUILayout.BeginArea(new Rect(Screen.width/10,Screen.height*6/17,Screen.width*9/10,Screen.height/2));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width*9/10), GUILayout.Height(Screen.height/2));
		GUILayout.Label(longString, mystyle);
		
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}