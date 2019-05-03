using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class EasyAI : MonoBehaviour {

	ArrayList emptyPositions=new ArrayList();
	public int count;
	int noOfPlayers;
	int noOfEnemies;
	int playerCount;
	int enemyCount;

	bool computerTurn=false;
	bool isPlacing=true;
	bool isMoving=false;
	bool isRemoving=false;
	bool canMovePlayer=false;

	public TextMesh enemyRemain;
	public TextMesh playerRemain;
	public TextMesh playerScore;
	public TextMesh enemyScore;

	public TextMesh statusText;

	public GameObject playerPawn;
	public GameObject enemyPawn;
	public GameObject removePS;
	public GameObject stars;

	public GameObject turnCube;
	private GameObject movableCube;

	public Texture2D backTexture;
	public Texture2D replayTexture;
	public Texture2D passTexture;

	public GameObject playerWon;
	public GameObject enemyWon;

	private bool wait = false;

	ArrayList daDDyCubes=new ArrayList();
	ArrayList goingDaddyEnemies = new ArrayList();
	Hashtable table=new Hashtable();

	void Start () {
		turnCube.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
		statusText.text = "";
		noOfPlayers=count;
		noOfEnemies=count;

		playerCount = 0;
		enemyCount = 0;
		GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
		foreach(GameObject cube in cubes)
			emptyPositions.Add(cube);
	}

	void Update () {

		if(computerTurn)
			turnCube.transform.GetComponent<Renderer>().material.color = Color.red;
		else
			turnCube.transform.GetComponent<Renderer>().material.color = Color.green;
		
		playerScore.text = "Kills : "+playerCount;
		enemyScore.text = "Kills : "+enemyCount;
		
		if(noOfPlayers == 0 && noOfEnemies == 0){
			isPlacing = false;
			isMoving = true;
			playerRemain.text = "Remaining : "+(count-enemyCount);
			enemyRemain.text =  "Remaining : "+(count-playerCount);
		}else{
			playerRemain.text = "Placed : "+(count-noOfPlayers);
			enemyRemain.text =  "Placed : "+(count-noOfEnemies);
		}
		
		if(playerCount > 8){
			statusText.text = "Player won! Play Again..";
			Object win = Instantiate(playerWon, new Vector3(0, -40, 0), Quaternion.Euler(0,0,0));
			gameOver();
		}
		if(enemyCount > 8){
			statusText.text = "Enemy won! Play Again..";
			Object win = Instantiate(enemyWon, new Vector3(0, -40, 0), Quaternion.Euler(0,0,0));
			gameOver();
		}
		
		if(wait){
			Quaternion newpos = Quaternion.Euler(new Vector3(90, 0, -179));
			turnCube.transform.rotation = Quaternion.Lerp(turnCube.transform.rotation,newpos,(float)5.0*Time.deltaTime);
			if(turnCube.transform.rotation == newpos){
				GameObject[] smokes = GameObject.FindGameObjectsWithTag("Smoke");
				foreach(GameObject sm in smokes){
					DestroyObject(sm);
				}
				GameObject[] overStars = GameObject.FindGameObjectsWithTag("Star");
				foreach(GameObject st in overStars){
					DestroyObject(st);
				}
				wait = false;
			}
		}else{
			turnCube.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

			if(isPlacing)
			{
				if(computerTurn)
				    placeComputer();
				else
					placePlayer();
			}

			if(isMoving){
				if(computerTurn)
					moveComputer();
				else{
					if(canMovePlayer)
					{
						if (Input.GetMouseButtonDown(0)){
							Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
							RaycastHit rayCastHit;
							if(Physics.Raycast(ray, out rayCastHit)){
								if(rayCastHit.collider.gameObject.tag == "Cube"){
									GameObject selectedCube = rayCastHit.collider.gameObject;
									if(selectedCube.transform.GetComponent<Renderer>().material.color==Color.magenta)
									{
										if(table.ContainsKey(movableCube.transform.parent.gameObject.name))
										{
											string strDaddyCubes=table[movableCube.transform.parent.gameObject.name] as string;
											table.Remove(movableCube.transform.parent.gameObject.name);
											ArrayList xPositions = new ArrayList();
											xPositions.AddRange(strDaddyCubes.Split(','));
											foreach(string s in xPositions)
											{
												if(daDDyCubes.Contains(s))
												{
													daDDyCubes.Remove(s);
												}
											}
										}
										emptyPositions.Add(movableCube.transform.parent.gameObject);
										emptyPositions.Remove(selectedCube);
										movableCube.transform.parent=null;
										movableCube.transform.position = new Vector3(selectedCube.transform.position.x,selectedCube.transform.position.y,-1.0f);
										movableCube.transform.parent=selectedCube.transform;
										makeWhite();
										canMovePlayer=false;
										selectedCube.transform.GetComponent<Renderer>().material.color=Color.white;
										
										if(checkDaddy(selectedCube, playerPawn)){
											isRemoving=true;
											isMoving=false;
											statusText.text = "Daadi! Remove an enemy";
										}else{
											computerTurn=!computerTurn;
											wait = true;
										}
									}
								}
							}
						}
					}
					movePlayer();
				}
			}

			if(isRemoving)
			{
				if(computerTurn)
					removePlayerPawn("player");
				else{
					removePlayerPawn("enemy");
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

	public void makeWhite(){
		GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
		foreach(GameObject cube in cubes){
			cube.transform.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void removePlayerPawn(string st)
	{
		if(st == "enemy"){
			if (Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit rayCastHit;
				if(Physics.Raycast(ray, out rayCastHit)){
					GameObject enemy =rayCastHit.collider.gameObject;
					if(enemy.tag == "Enemy" && !daDDyCubes.Contains(enemy.transform.parent.name)){
						emptyPositions.Add(enemy.transform.parent.gameObject);
						GameObject removea = Instantiate(removePS, new Vector3(enemy.transform.position.x, enemy.transform.position.y, -2.0f), Quaternion.Euler(0, 0, 0)) as GameObject;

						DestroyObject(enemy);
						playerCount++;

						isRemoving=false;
						if(noOfPlayers == 0 && noOfEnemies == 0){
							isPlacing = false;
							isMoving = true;
						}else{
							isPlacing = true;
							isMoving = false;
						}
						statusText.text = "";
						computerTurn=!computerTurn;
						wait = true;
					}
				}
			}
		}else if(st == "player"){
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject obj in players){
				if(!daDDyCubes.Contains(obj.transform.parent.name)){
					emptyPositions.Add(obj.transform.parent.gameObject);
					GameObject removea = Instantiate(removePS, new Vector3(obj.transform.position.x, obj.transform.position.y, -2.0f), Quaternion.Euler(0, 0, 0)) as GameObject;

					DestroyObject(obj);

					enemyCount++;

					isRemoving=false;
					if(noOfPlayers == 0 && noOfEnemies == 0){
						isPlacing = false;
						isMoving = true;
					}else{
						isPlacing = true;
						isMoving = false;
					}
					statusText.text = "";
					computerTurn=!computerTurn;
					return;
				}
			}
			isRemoving=false;
			if(noOfPlayers == 0 && noOfEnemies == 0){
				isPlacing = false;
				isMoving = true;
			}else{
				isPlacing = true;
				isMoving = false;
			}
			statusText.text = "";
			computerTurn=!computerTurn;
		}
	}

	public void placeComputer()
	{
		if(noOfEnemies > 0)
		{
			ArrayList tempList=new ArrayList();
			GameObject[] enemies=GameObject.FindGameObjectsWithTag("Enemy");
			GameObject[] players=GameObject.FindGameObjectsWithTag("Player");
			tempList=new ArrayList(getTwoPlayerPositions(enemies));
			int index=0;
			if(tempList.Count!=0)
				index=Random.Range(0, tempList.Count-1);
			else
			{
				tempList.Clear();
				tempList=new ArrayList(getTwoPlayerPositions(players));
				if(tempList.Count != 0)
					index=Random.Range(0, tempList.Count-1);
				else{
					tempList.Clear();
					tempList = new ArrayList(getLshapePositions(enemyPawn));
					if(tempList.Count != 0)
						index=Random.Range(0, tempList.Count-1);
					else{
						tempList.Clear();
						tempList = new ArrayList(getLshapePositions(playerPawn));
						if(tempList.Count != 0)
							index=Random.Range(0, tempList.Count-1);
						else{
							tempList.Clear();
							tempList=new ArrayList(emptyPositions);
							index=Random.Range(0, tempList.Count-1);
						}
					}
				}
			}
			GameObject placingCube=tempList[index] as GameObject;
			if(checkDaddy(placingCube,enemyPawn)){
				isRemoving=true;
				isPlacing=false;
				statusText.text = "Daadi! Remove an enemy";
				wait = true;
			}else{
				computerTurn=!computerTurn;
			}
			GameObject player = Instantiate(enemyPawn, new Vector3(placingCube.transform.position.x, placingCube.transform.position.y, -1.0f), Quaternion.Euler(90, 0, 0)) as GameObject;
			
			player.transform.parent = placingCube.transform;
			emptyPositions.Remove(placingCube);
			noOfEnemies--;
		}
		if(noOfPlayers == 0 && noOfEnemies == 0)
			statusText.text = "Placing is done. Now Move..";
	}

	public void placePlayer()
	{
		if (Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayCastHit;
			if(noOfPlayers>0)
			{
				if(Physics.Raycast(ray, out rayCastHit)){
					if(emptyPositions.Contains(rayCastHit.collider.gameObject))
					{
						GameObject placingCube=rayCastHit.collider.gameObject;
						if(checkDaddy(placingCube, playerPawn)){
							isRemoving=true;
							isPlacing=false;
							statusText.text = "Daadi! Remove an enemy";
						}else{
							computerTurn=!computerTurn;
							wait = true;
						}
						GameObject player = Instantiate(playerPawn, new Vector3(placingCube.transform.position.x, placingCube.transform.position.y, -1.0f), Quaternion.Euler(90, 0, 0)) as GameObject;
						player.transform.parent = placingCube.transform;
						emptyPositions.Remove(placingCube);
						noOfPlayers--;
					}
				}
			}
		}
	}

	private ArrayList shuffleList(ArrayList inputList)
	{
		ArrayList randomList = new ArrayList();

		int randomIndex = 0;
		while (inputList.Count > 0)
		{
			randomIndex = Random.Range(0, inputList.Count); //Choose a random object in the list
			randomList.Add(inputList[randomIndex]); //add it to the new, random list
			inputList.RemoveAt(randomIndex); //remove to avoid duplicates
		}
		
		return randomList; //return the new random list
	}

	public void moveComputer(){
		Hashtable movablePos = new Hashtable();
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject obj in enemies){
			Tile script = obj.transform.parent.gameObject.GetComponent<Tile>();
			string adjNames = "";
			foreach(GameObject adjCube in script.adjusents){
				if(emptyPositions.Contains(adjCube)){
					if(!movablePos.Contains(obj)){
						if(adjNames == "")
							adjNames = adjCube.name;
						else
							adjNames = adjNames+","+adjCube.name;
					}
				}
			}
			if(adjNames.Contains("Cube"))
				movablePos[obj] = adjNames;
		}
		ArrayList keys = new ArrayList(movablePos.Keys);
		keys = shuffleList(keys);
		////////////////////////
		ArrayList tempList = new ArrayList();
		GameObject cubeDest = null;
		GameObject movPawn = null;
		bool leav = false;
		foreach(GameObject ob in keys){
			ArrayList destCubes = new ArrayList();
			destCubes.AddRange((movablePos[ob] as string).Split(','));
			foreach(string str in destCubes){
				GameObject destC = GameObject.Find(str as string);
				if(destC){
					GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
					tempList=new ArrayList(getTwoPlayerPositions(enemies));
					if(tempList.Contains(destC) && !goingDaddyEnemies.Contains(ob)){
						cubeDest = destC;
						movPawn = ob;
						goto asdf;
					}
					else
					{
						tempList.Clear();
						tempList=new ArrayList(getTwoPlayerPositions(players));
						if(tempList.Contains(destC)){
							cubeDest = destC;
							movPawn = ob;
						}
					}
				}
			}
		}
		///////////////////////
//		GameObject movPawn = keys[Random.Range(0,keys.Count)] as GameObject;
//		GameObject cubeDest = GameObject.Find(movablePos[movPawn] as string);
		if(movPawn == null || cubeDest == null){
			foreach(GameObject key in keys){
				movPawn = key;
				ArrayList destCubes = new ArrayList();
				destCubes.AddRange((movablePos[movPawn] as string).Split(','));
				foreach(string st in destCubes){
					cubeDest = GameObject.Find(st);
					if(cubeDest)
						goto asdf;
				}
			}
		}

	asdf:

		if(table.ContainsKey(movPawn.transform.parent.gameObject.name)){
			string strDaddyCubes=table[movPawn.transform.parent.gameObject.name] as string;
			table.Remove(movPawn.transform.parent.gameObject.name);
			ArrayList xPositions = new ArrayList();
			xPositions.AddRange(strDaddyCubes.Split(','));
			foreach(string s in xPositions)
			{
				if(daDDyCubes.Contains(s))
				{
					daDDyCubes.Remove(s);
				}
			}
		}
		emptyPositions.Add(movPawn.transform.parent.gameObject);
		emptyPositions.Remove(cubeDest);
		movPawn.transform.parent = null;
		movPawn.transform.position = new Vector3(cubeDest.transform.position.x,cubeDest.transform.position.y,-1.0f);
		movPawn.transform.parent = cubeDest.transform;

		if(checkDaddy(cubeDest, enemyPawn)){
			isRemoving=true;
			isMoving = false;
			statusText.text = "Daadi! Remove an enemy";
			wait = true;
		}else{
			computerTurn=!computerTurn;
		}
	}

	public void movePlayer(){
		if (Input.GetMouseButtonDown(0)){

			foreach(GameObject cube in emptyPositions){
				cube.transform.GetComponent<Renderer>().material.color = Color.white;
			}
			GameObject selectedPlayer = null;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayCastHit;
			if(Physics.Raycast(ray, out rayCastHit)){
				if(rayCastHit.collider.gameObject.tag == "Player"){
					selectedPlayer = rayCastHit.collider.gameObject;
					Tile tileScript=selectedPlayer.transform.parent.GetComponent<Tile>();
					foreach(GameObject adjecentCube in tileScript.adjusents)
					{
						if(emptyPositions.Contains(adjecentCube))
						{
							movableCube=selectedPlayer;
							adjecentCube.transform.GetComponent<Renderer>().material.color = Color.magenta;
							canMovePlayer=true;
						}
					}
				}
			}
		}
	}

	public bool checkDaddy(GameObject placingCube,GameObject type)
	{
		Tile tileScript=placingCube.GetComponent<Tile>();
		Vector3[] daadiGameObjects=tileScript.dadiPositions;
		string positionY="Cube"+daadiGameObjects[0].x+","+"Cube"+daadiGameObjects[0].y+","+"Cube"+daadiGameObjects[0].z;
		string positionX="Cube"+daadiGameObjects[1].x+","+"Cube"+daadiGameObjects[1].y+","+"Cube"+daadiGameObjects[1].z;

		string playerName= placingCube.name;
		ArrayList xPositions = new ArrayList();
		xPositions.AddRange(positionX.Split(','));
		ArrayList yPositions = new ArrayList();
		yPositions.AddRange(positionY.Split(','));
		if(xPositions.Contains(playerName))
		{
			xPositions.Remove(playerName);
		}
		if(yPositions.Contains(playerName))
		{
			yPositions.Remove(playerName);
		}
	
		if(checkIfPlayerExist(xPositions[0] as string, type) && checkIfPlayerExist(xPositions[1] as string, type) ){
			xPositions.Add(playerName);
			foreach(string s in xPositions)
			{
				if(!daDDyCubes.Contains(s))
				{
					daDDyCubes.Add(s);
				}

				if(table.ContainsKey(s))
					table[s]=positionX+","+table[s];
				else
					table[s]=positionX;
			}
			//produce stars
			produceStars(xPositions[0] as string, xPositions[1] as string, xPositions[2] as string);
			return true;
		}
		if(checkIfPlayerExist(yPositions[0] as string, type) && checkIfPlayerExist(yPositions[1] as string, type) ){
			yPositions.Add(playerName);
			foreach(string s in yPositions)
			{
				if(!daDDyCubes.Contains(s))
				{
					daDDyCubes.Add(s);
				}
				if(table.ContainsKey(s))
					table[s]=positionY+","+table[s];
				else
					table[s]=positionY;
			}
			//produce stars
			produceStars(yPositions[0] as string, yPositions[1] as string, yPositions[2] as string);
			return true;
		}
		return false;
	}

	public AudioClip daadiEffect;

	public void produceStars(string cube1, string cube2, string cube3){
		Vector3 c1 = GameObject.Find(cube1).transform.position;
		Vector3 c2 = GameObject.Find(cube2).transform.position;
		Vector3 c3 = GameObject.Find(cube3).transform.position;

		GetComponent<AudioSource>().clip = daadiEffect;
		GetComponent<AudioSource>().Play();

		GameObject st1 = Instantiate(stars, new Vector3(c1.x, c1.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;
		GameObject st2 = Instantiate(stars, new Vector3(c2.x, c2.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;
		GameObject st3 = Instantiate(stars, new Vector3(c3.x, c3.y, -3.0f), Quaternion.Euler(270, 0, 0)) as GameObject;
	}

	public bool checkIfPlayerExist(string position,GameObject player)
	{
		GameObject obj=GameObject.Find(position);
		if(obj.transform.childCount==1)
		{
			if(obj.transform.GetChild(0).tag==player.tag)
			{
				return true;
			}
		}
		return false;
	}

	public ArrayList getLshapePositions(GameObject type){
		ArrayList positions = new ArrayList();
		foreach(GameObject cube in emptyPositions){
			Tile scrip = cube.GetComponent<Tile>();
			int i=0;
			foreach(GameObject adjcube in scrip.adjusents){
				if(adjcube && checkIfPlayerExist(adjcube.name, type))
					i++;
			}
			if(i>1)
				positions.Add(cube);
		}
		return positions;
	}

	public ArrayList getTwoPlayerPositions(GameObject[] objects)
	{
		goingDaddyEnemies.Clear();
		ArrayList positions=new ArrayList();
		foreach(GameObject player in objects){
			Tile tileScript=player.transform.parent.gameObject.GetComponent<Tile>();
			Vector3[] daadiGameObjects=tileScript.dadiPositions;
			string positionY="Cube"+daadiGameObjects[0].x+","+"Cube"+daadiGameObjects[0].y+","+"Cube"+daadiGameObjects[0].z;
			string positionX="Cube"+daadiGameObjects[1].x+","+"Cube"+daadiGameObjects[1].y+","+"Cube"+daadiGameObjects[1].z;
			string playerName= player.transform.parent.name;
			ArrayList xPositions = new ArrayList();
			xPositions.AddRange(positionX.Split(','));

			ArrayList yPositions = new ArrayList();
			yPositions.AddRange(positionY.Split(','));

			if(xPositions.Contains(playerName))
			{
				xPositions.Remove(playerName);
				if(checkIfPlayerExist(xPositions[0] as string,player))
				{
					GameObject obj=	GameObject.Find(xPositions[1] as string);
					if(emptyPositions.Contains(obj)){
						positions.Add(obj);
						if(player.tag == "Enemy"){
						if(!goingDaddyEnemies.Contains(player))
							goingDaddyEnemies.Add(player);
						GameObject secEnemy = GameObject.Find(xPositions[0] as string).transform.GetChild(0).gameObject as GameObject;
						if(!goingDaddyEnemies.Contains(secEnemy))
							goingDaddyEnemies.Add(secEnemy);
						}
					}
				}
				if(checkIfPlayerExist(xPositions[1] as string,player))
				{
					GameObject obj=	GameObject.Find(xPositions[0] as string);
					if(emptyPositions.Contains(obj)) {
						positions.Add(obj);
						if(player.tag == "Enemy"){
							if(!goingDaddyEnemies.Contains(player))
								goingDaddyEnemies.Add(player);
							GameObject secEnemy = GameObject.Find(xPositions[1] as string).transform.GetChild(0).gameObject as GameObject;
							if(!goingDaddyEnemies.Contains(secEnemy))
								goingDaddyEnemies.Add(secEnemy);
						}
					}
				}
			}

			if(yPositions.Contains(playerName))
			{
				yPositions.Remove(playerName);
				if(checkIfPlayerExist(yPositions[0] as string,player))
				{
					GameObject obj=	GameObject.Find(yPositions[1] as string);
					if(emptyPositions.Contains(obj)){ 
						positions.Add(obj);
						if(player.tag == "Enemy"){
							if(!goingDaddyEnemies.Contains(player))
								goingDaddyEnemies.Add(player);
							GameObject secEnemy = GameObject.Find(yPositions[0] as string).transform.GetChild(0).gameObject as GameObject;
							if(!goingDaddyEnemies.Contains(secEnemy))
								goingDaddyEnemies.Add(secEnemy);
						}
					}

				}
				if(checkIfPlayerExist(yPositions[1] as string,player))
				{
					GameObject obj=	GameObject.Find(yPositions[0] as string);
					if(emptyPositions.Contains(obj)) {
						positions.Add(obj);
						if(player.tag == "Enemy"){
							if(!goingDaddyEnemies.Contains(player))
								goingDaddyEnemies.Add(player);
							GameObject secEnemy = GameObject.Find(yPositions[1] as string).transform.GetChild(0).gameObject as GameObject;
							if(!goingDaddyEnemies.Contains(secEnemy))
								goingDaddyEnemies.Add(secEnemy);
						}
					}
				}
			}
		}
		return positions;
	}

	public GUIStyle myStyle;

	void OnGUI() {
		
		if (GUI.Button(new Rect(Screen.width/2-Screen.height/24, Screen.height-Screen.height*3/15-3,  Screen.height/12, Screen.height/12), passTexture, myStyle)){
			statusText.text = "";
			if(!isPlacing){
				if(computerTurn){
					computerTurn = false;
				}else{
					computerTurn = true;
					wait=true;
				}
				if(isRemoving){
					isRemoving = false;
					if(noOfPlayers == 0 && noOfEnemies == 0){
						isPlacing = false;
						isMoving = true;
					}else{
						isPlacing = true;
						isMoving = false;
					}
				}
			}
		}
		
		if (GUI.Button(new Rect(Screen.width-Screen.height/8-Screen.height/12, Screen.height-Screen.height*3/15-3, Screen.height/12, Screen.height/12), replayTexture, myStyle)){
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(new Rect(Screen.height/8, Screen.height-Screen.height*3/15-3, Screen.height/12, Screen.height/12), backTexture, myStyle)){
			Application.LoadLevel(0);
		}
	}
}
