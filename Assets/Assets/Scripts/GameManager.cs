using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	private Ray ray;
	private RaycastHit rayCastHit;
	private int playerCount;
	private int enemyCount;

	private bool checkPlayerMove;

	public int numberOfPlayers = 11;
	public GameObject playerPawn;
	public GameObject enemyPawn;
	public GameObject removePS;
	public GameObject stars;

	private GameObject movableObject;

	private GameObject[] enemies;
	private GameObject[] players;
	private GameObject[] cubes;


	public TextMesh statusText;
	private int noOfPlayers;
	private int noOfEnemies;


	private string removeObject = "None";

	private bool placePlayers;
	private bool movePlayers;

	private bool canMovePlayer;
	private bool canMoveEnemy;

	private Hashtable daadiHash = new Hashtable();
		
	private bool isNewDaadi;

	public GameObject turnCube;

	public TextMesh playerScore;
	public TextMesh enemyScore;

	public Texture2D backTexture;
	public Texture2D replayTexture;
	public Texture2D passTexture;

	private Hashtable availCubes;

	private bool removePlayer;

	public TextMesh pCount;
	public TextMesh eCount;

	private Hashtable playerMoves;
	private Hashtable enemyMoves;

	public GameObject player1Win;
	public GameObject player2Win;


	void Start () {
		noOfEnemies = numberOfPlayers;
		noOfPlayers = numberOfPlayers;
		checkPlayerMove = false;

		pCount.text = "";
		eCount.text = "";
		statusText.text = "";

		playerCount = 0;
		enemyCount = 0;
		placePlayers = true;
		movePlayers = false;

		canMoveEnemy = false;
		canMovePlayer = false;

		isNewDaadi = false;
		removePlayer = false;
		cubes = GameObject.FindGameObjectsWithTag("Cube");
	}


	//CHeck if the position is empty on click position
	public bool checkIfPosEmpty(Vector3 targetPos)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			if(player.gameObject.transform.position == targetPos)
				return false;
		}

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject enemy in enemies)
		{
			if(enemy.gameObject.transform.position == targetPos)
				return false;
		}

		return true;
	}

	//update func
	void Update () {

		playerScore.text = "Kills : "+(numberOfPlayers-noOfEnemies);
		enemyScore.text =  "Kills : "+(numberOfPlayers-noOfPlayers);

		pCount.text = "Placed : "+playerCount;
		eCount.text = "Placed : "+enemyCount;

		if(removeObject == "player")
		{
			turnCube.transform.GetComponent<Renderer>().material.color = Color.red;
		}else if(removeObject == "enemy")
		{
			turnCube.transform.GetComponent<Renderer>().material.color = Color.green;
		}else{
			if(checkPlayerMove == false)
			{
				turnCube.transform.GetComponent<Renderer>().material.color = Color.green;
			}else{
				turnCube.transform.GetComponent<Renderer>().material.color = Color.red;
			}
		}

		//conditions for win
		if(noOfPlayers < 3 && noOfEnemies >= 3){
			statusText.text = "Player 2 won by "+(noOfEnemies-noOfPlayers)+" points";
			Object win = Instantiate(player2Win, new Vector3(0, -40, 0), Quaternion.Euler(0,0,0));
			gameOver();
		}
		if(noOfEnemies < 3 && noOfPlayers >= 3){
			statusText.text = "Player 1 won by "+(noOfPlayers-noOfEnemies)+" points";
			Object win = Instantiate(player1Win, new Vector3(0, -40, 0), Quaternion.Euler(0,0,0));
			gameOver();
		}


		if (Input.GetMouseButtonDown(0)){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			GameObject[] smokes = GameObject.FindGameObjectsWithTag("Smoke");
			foreach(GameObject sm in smokes){
				DestroyObject(sm);
			}

			GameObject[] st = GameObject.FindGameObjectsWithTag("Star");
			foreach(GameObject s in st){
				DestroyObject(s);
			}

			//Condition to place objects
			if(placePlayers && !movePlayers && !removePlayer){
				if(playerCount < numberOfPlayers){
				if(checkPlayerMove == false){
					if(Physics.Raycast(ray, out rayCastHit)){
						if(checkIfPosEmpty(new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f))){
							Object player = Instantiate(playerPawn, new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f), Quaternion.Euler(90, 0, 0));
							playerCount++;
							checkPlayerMove = true;
							(player as GameObject).tag = "Player";
							checkDaadi();
							if(playerCount == numberOfPlayers && enemyCount == numberOfPlayers)
							{
								statusText.text = "All are placed. Start Moving!";
								movePlayers = true;
								placePlayers = false;
							}
						}
					}
				}
				}
				if(enemyCount < numberOfPlayers){
				if(checkPlayerMove == true){
					if(Physics.Raycast(ray, out rayCastHit)){
						if(checkIfPosEmpty(new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f))){
							Object enemy = Instantiate(enemyPawn, new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f), Quaternion.Euler(90, 0, 0));
							enemyCount++;
							checkPlayerMove = false;
							(enemy as GameObject).tag = "Enemy";
							checkDaadi();
							if(playerCount == numberOfPlayers && enemyCount == numberOfPlayers)
							{
								statusText.text = "All are placed. Start Moving!";
								movePlayers = true;
								placePlayers = false;
							}
						}
					}
				}
				}
			}

			//condition to move the players
			if(movePlayers && !placePlayers &&!removePlayer)
			{
				//highlighting the available positions to move
				if(checkPlayerMove == false)
				{
					if(Physics.Raycast(ray, out rayCastHit))
					{
						if(rayCastHit.collider.gameObject.tag == "Player")
						{
							GameObject playerPositionCube=null;

							foreach(GameObject cube in cubes)
							{
								cube.transform.GetComponent<Renderer>().material.color = Color.white;
								if(cube.transform.position.x == rayCastHit.collider.gameObject.transform.position.x && cube.transform.position.y == rayCastHit.collider.gameObject.transform.position.y){
									playerPositionCube=cube;
								}
							}
							if(playerPositionCube!=null)
							{
								availCubes = new Hashtable();
								Tile tileScript=playerPositionCube.GetComponent<Tile>();
								foreach(GameObject adjecentCube in tileScript.adjusents )
								{

									if(checkIfPosEmpty(new Vector3(adjecentCube.transform.position.x, adjecentCube.transform.position.y, -1.0f)))
									{
										adjecentCube.transform.GetComponent<Renderer>().material.color = Color.magenta;
										movableObject = rayCastHit.collider.gameObject;
										availCubes.Add(adjecentCube,"true");
										canMoveEnemy = false;
										canMovePlayer = true;
									}
								}
								if(!availCubes.ContainsValue("true")){
									statusText.text = "Select another player or Pass the turn";
								}else{
									statusText.text = "Move the Player";
								}
							}
						}
					}
				}else{
					if(Physics.Raycast(ray, out rayCastHit))
					{
						if(rayCastHit.collider.gameObject.tag == "Enemy")
						{
							GameObject playerPositionCube=null;
							
							foreach(GameObject cube in cubes)
							{
								cube.transform.GetComponent<Renderer>().material.color = Color.white;
								if(cube.transform.position.x == rayCastHit.collider.gameObject.transform.position.x && cube.transform.position.y == rayCastHit.collider.gameObject.transform.position.y){
									playerPositionCube=cube;
								}
							}
							if(playerPositionCube!=null)
							{
								availCubes = new Hashtable();
								Tile tileScript=playerPositionCube.GetComponent<Tile>();
								foreach(GameObject adjecentCube in tileScript.adjusents )
								{
									if(checkIfPosEmpty(new Vector3(adjecentCube.transform.position.x, adjecentCube.transform.position.y, -1.0f)))
									{
										adjecentCube.transform.GetComponent<Renderer>().material.color = Color.magenta;
										movableObject = rayCastHit.collider.gameObject;
										availCubes.Add(adjecentCube,"true");
										canMoveEnemy = true;
										canMovePlayer = false;
									}
								}
								if(!availCubes.ContainsValue("true")){
									statusText.text = "Select another player or Pass the turn";
								}else{
									statusText.text = "Move the player";
								}
							}
						}
					}
				}

				//moving to one of the avaible positions
				if(canMovePlayer || canMoveEnemy)
				{
					if(Physics.Raycast(ray, out rayCastHit))
					{
						if(availCubes.Contains(rayCastHit.collider.gameObject))
						{
							string valuex = "x : "+movableObject.transform.position.x;
							string valuey = "y : "+movableObject.transform.position.y;

							if(daadiHash.Contains(valuex) && daadiHash[valuex] == "true"){
								daadiHash[valuex] = "false";
							}

							if(daadiHash.Contains(valuey) && daadiHash[valuey] == "true"){
								daadiHash[valuey] = "false";
							}

							Vector3 newPos = new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f);
							movableObject.transform.position = newPos;

							foreach(GameObject cube in cubes){
								cube.transform.GetComponent<Renderer>().material.color = Color.white;
							}
							
							if(canMovePlayer && !canMoveEnemy){
								checkPlayerMove = true;
							}else if(canMoveEnemy && !canMovePlayer){
								checkPlayerMove = false;
							}
							checkDaadi();
						}
					}
				}
			}
			//condition to remove objects
			if(removePlayer){
				if(Physics.Raycast(ray, out rayCastHit))
				{
					GameObject obj = rayCastHit.collider.gameObject;
					string xValue = "x : "+obj.transform.position.x;
					string yValue = "y : "+obj.transform.position.y;
					

					if(checkIfPosEmpty(new Vector3(rayCastHit.collider.gameObject.transform.position.x, rayCastHit.collider.gameObject.transform.position.y, -1.0f)))
					{
						statusText.text = "Click on your enemy to remove";
					}else{
						if((daadiHash.Contains(xValue) && daadiHash[xValue] == "true") || (daadiHash.Contains(yValue) && daadiHash[yValue] == "true"))
						{
//							statusText.text = "Cannot remove from Daadi";
						}else{
							if(removeObject == "player"){
								ArrayList lst = new ArrayList();
								lst.AddRange(players);
								if(lst.Contains(rayCastHit.collider.gameObject))
								{
									GameObject removea = Instantiate(removePS, new Vector3(obj.transform.position.x, obj.transform.position.y, -2.0f), Quaternion.Euler(0, 0, 0)) as GameObject;
									DestroyObject(rayCastHit.collider.gameObject);
									if(playerCount == numberOfPlayers && enemyCount == numberOfPlayers){
										placePlayers = false;
										movePlayers = true;
									}else{
										placePlayers = true;
										movePlayers = false;
									}
									removeObject = "None";
									checkPlayerMove = false;
									statusText.text = "";
									removePlayer = false;
									noOfPlayers--;
								}
							}else if(removeObject == "enemy"){
								ArrayList lst = new ArrayList();
								lst.AddRange(enemies);
								if(lst.Contains(rayCastHit.collider.gameObject))
								{
									GameObject removea = Instantiate(removePS, new Vector3(obj.transform.position.x, obj.transform.position.y, -2.0f), Quaternion.Euler(0, 0, 0)) as GameObject;
									DestroyObject(rayCastHit.collider.gameObject);
									if(playerCount == numberOfPlayers && enemyCount == numberOfPlayers){
										placePlayers = false;
										movePlayers = true;
									}else{
										placePlayers = true;
										movePlayers = false;
									}
									removeObject = "None";
									checkPlayerMove = true;
									statusText.text = "";
									removePlayer = false;
									noOfEnemies--;
								}
							}
						}
					}
				}
			}
		}
	}

	public void gameOver(){
		GameObject[] cubesq = GameObject.FindGameObjectsWithTag("Cube");
		foreach(GameObject cube in cubesq){
			if(cube){
				cube.transform.position = Vector3.MoveTowards(cube.transform.position, new Vector3(0, 0, 0), (float)10.0f*Time.deltaTime);
				if(cube.transform.position == new Vector3(0, 0, 0)){
					DestroyObject(cube);
				}
			}
		}
	}

	//function to check which daadi is formed (player/enemy)
	public void checkDaadi()
	{
		enemies=GameObject.FindGameObjectsWithTag("Enemy");
		players=GameObject.FindGameObjectsWithTag("Player");
		if(isDaadi(enemies))
		{
			if(isNewDaadi){
				statusText.text = "Remove an enemy";
				placePlayers = false;
				removePlayer = true;
				removeObject = "player";
			}
		}
		
		if(isDaadi(players))
		{
			if(isNewDaadi){
				statusText.text = "Remove an enemy";
				placePlayers = false;
				removePlayer = true;
				removeObject = "enemy";
			}
		}
	}

	public AudioClip daadiEffect;

	//function to check weather any daadi is formed
	public bool isDaadi(GameObject[] type)
	{
		foreach(GameObject player in type)
		{
			foreach(GameObject otherPlayer in type)
			{
				if(otherPlayer != player)
				{
					foreach(GameObject thirdPlayer in type)
					{
						if(thirdPlayer!=player && thirdPlayer!=otherPlayer)
						{
							Vector3 p1 = player.transform.position;
							Vector3 p2 = otherPlayer.transform.position;
							Vector3 p3 = thirdPlayer.transform.position;

							if((p1.x == p2.x && p2.x == p3.x)|| (p1.y == p2.y && p2.y == p3.y))
							{
								string valueC;
								if(p1.x == p2.x && p2.x == p3.x)
								{
									valueC = "x : "+p1.x;
								}else{
									valueC = "y : "+p1.y;
								}
								if(daadiHash.Contains(valueC) && daadiHash[valueC] == "true"){
									isNewDaadi = false;
								}else{
									if(daadiHash[valueC] == "false"){
										daadiHash[valueC] = "true";
									}else if(!daadiHash.Contains(valueC)){
										daadiHash.Add(valueC, "true");
									}

									GetComponent<AudioSource>().clip = daadiEffect;
									GetComponent<AudioSource>().Play();

									GameObject st1 = Instantiate(stars, new Vector3(p1.x, p1.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;
									GameObject st2 = Instantiate(stars, new Vector3(p2.x, p2.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;
									GameObject st3 = Instantiate(stars, new Vector3(p3.x, p3.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;

									isNewDaadi = true;
									return true;
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	public GUIStyle mystyle;

	void OnGUI() {

		if (GUI.Button(new Rect(Screen.width/2-Screen.height/24, Screen.height-Screen.height*3/15-3,  Screen.height/12, Screen.height/12), passTexture, mystyle)){
			statusText.text = "";
			if(removePlayer){
				removePlayer = false;
				if(playerCount == numberOfPlayers && enemyCount == numberOfPlayers){
					placePlayers = false;
					movePlayers = true;
				}else{
					placePlayers = true;
					movePlayers = false;
				}
				if(removeObject == "player"){
					checkPlayerMove = false;
				}else if(removeObject == "enemy"){
					checkPlayerMove = true;
				}
				removeObject = "None";
			}
			else{
				if(movePlayers){
					if(checkPlayerMove){
						checkPlayerMove = false;
					}else{
						checkPlayerMove = true;
					}
				}
			}
		}
		
		if (GUI.Button(new Rect(Screen.width-Screen.height/8-Screen.height/12,  Screen.height-Screen.height*3/15-3,  Screen.height/12, Screen.height/12), replayTexture, mystyle)){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect(Screen.height/8, Screen.height-Screen.height*3/15-3, Screen.height/12, Screen.height/12), backTexture, mystyle)){
			Application.LoadLevel(0);
		}
	}
}
