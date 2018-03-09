using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour {

	//TileMap mapscript;

	public bool preparationPhase;

	public GameObject resourceUI;
	//public Text digText;
	public GameObject tileTextUI;
	public List<Text> tileText;
	public Text darkpowerText;
	public Button commenceButton;
	public static Game_Manager instance = null;
	public bool demonLordFound;
	public bool reachedDestination;

	//resources
	public int digLeft;
	public float darkPower;
	public float darkPower_base;
	public int darkPower_base_growth;

	public static int level = 0;
	public static int final_level =10;

	public Dictionary<string,GameObject> monsterDic;
	public MonsterType[] monsterTypes;

	public GameObject gameOverUI;
	public GameObject victoryUI;
	public List<int[,]> mapLayout;
	public List<int> tileNumbers;

	private bool game_over;

	public Vector2 return_location;

	public List<GameObject> healthUIList;
	public GameObject timerUI;
	public List<int> preparation_time_per_level;
	public int time_left;

	public GameObject current_dragger;
	public List<GameObject> poisson_monster_list_light;
	public List<GameObject> poisson_monster_list_shadow;

	public int adventurer_layer;
	public int monster_layer;
	public int tile_layer;
	public int monster_tile_layer;
	public int adventurer_tile_layer;

	public static int semaphore;


	void Awake () {
//		print ("Awake!");
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else if (instance != this) {
			Destroy (gameObject);
			return;
		}
		//resources
		darkPower_base = 10;
		//digLeft = 100;
		darkPower = darkPower_base;

		semaphore = 1;

		monster_layer = LayerMask.NameToLayer ("Monster");
		adventurer_layer = LayerMask.NameToLayer ("Adventurer");
		tile_layer = LayerMask.NameToLayer ("Tiles");

		int adventurer_layer_temp = 1 << LayerMask.NameToLayer ("Adventurer");
		int monster_layer_temp = 1 << LayerMask.NameToLayer ("Monster");
		int tile_layer_temp = 1 << LayerMask.NameToLayer ("Tiles");
		monster_tile_layer = tile_layer_temp | monster_layer_temp;
		adventurer_tile_layer = tile_layer_temp | adventurer_layer_temp;

		//commenceButton = GameObject.Find ("Button").GetComponent<Button> ();
		commenceButton.onClick.AddListener(commenceBattle);

//		digText = GameObject.Find ("digText").GetComponent<Text>();
//		digText.text = "Digs Left: " + digLeft;

		tileTextUI = GameObject.Find ("tileText");
		foreach (Transform child in tileTextUI.transform){
			tileText.Add (child.gameObject.GetComponent<Text>());
		}


		for (int i = 0; i < 3; i++) {
			tileNumbers.Add (0);
		}

		tileText[0].text = "Tiles: " + tileNumbers[0];
		tileText[1].text = "Rare: " + tileNumbers[1];
		tileText[2].text = "Rarest: " + tileNumbers[2];


		//darkpowerText = GameObject.Find ("darkpowerText").GetComponent<Text> ();
		darkpowerText.text = "Dark Power: " + darkPower;

		demonLordFound = false;
		reachedDestination = false;

		setupMonsterDic ();
	}
		
	void Start(){
//		print ("start");
		mapLayout = new List<int[,]> ();
		GenerateAllLevels ();
		//InvokeRepeating("DecrementTimer", 0f,1f);
		prepare ();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}


	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		CancelInvoke ();
		InvokeRepeating("DecrementTimer", 0f,1f);
		demonLordFound = false;
		reachedDestination = false;
		Reset ();
		if (level == 1) {
			tileNumbers [0] = 30;
		}
//		print ("OnSceneLoaded");
		time_left = preparation_time_per_level [level];
		timerUI.GetComponent<Text> ().text = "Time Till Battle: " + time_left.ToString ();

		SetUpReturnLocation ();

		StopAllCoroutines ();
		StartCoroutine(WaitToStartLevel ());
	}


	// Update is called once per frame
	void Update () {
		//SceneManager.sceneLoaded += OnSceneLoaded;
		game_over = checkIfGameOver ();
		updateText ();
	}

	void prepare(){
		preparationPhase = true;

		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
			TileMap.instance.Adventurers[i].SetActive (false);
		}

	}

	void SetUpReturnLocation(){
		if (level == 1) {
			return_location = new Vector2 (11, 28);
		} else {
			return_location = new Vector2 (11, 30);
		}
	}

	public void commenceBattle(){
		preparationPhase = false;
		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
			TileMap.instance.Adventurers[i].SetActive (true);
			TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().healthUI.SetActive (true);
		}

	}

	public void updateText(){
		//digText.text = "Digs Left: " + digLeft;
		tileText[0].text = "Tiles: " + tileNumbers[0];
		tileText[1].text = "Rare: " + tileNumbers[1];
		tileText[2].text = "Rarest: " + tileNumbers[2];

		darkpowerText.text = "Dark Power: " + darkPower;
	}

//	void updateTimer(){
//		if (time_left > 0) {
//			print ("here");
//			time_left -= Time.deltaTime;
//			timerUI.GetComponent<Text> ().text = "Time Till Battle: " + time_left.ToString ("F1");
//		}
//	}

	void DecrementTimer(){
		if (time_left > 0) {
			time_left -= 1;
			timerUI.GetComponent<Text> ().text = "Time Till Battle: " + time_left.ToString ();
		}
	}


	void Reset(){
		gameOverUI.SetActive (false);
		victoryUI.SetActive (false);
		foreach (GameObject UI in healthUIList) {
			UI.SetActive (false);
		}
		demonLordFound = false;
		reachedDestination = false;
		darkPower = darkPower_base;
		for (int i=0;i<tileNumbers.Count;i++){
			tileNumbers[i] = 0;	
		}
		//updateText ();
		preparationPhase = true;
	}

	public IEnumerator WaitToStartLevel(){
//		if (level == 0) {
//			yield return new WaitForSeconds (0);
//		} else if (level == 1) {
//			yield return new WaitForSeconds (20);
//		} else if (level == 2) {
//			yield return new WaitForSeconds (75);
//		} else if (level == 3) {
//			yield return new WaitForSeconds (85);
//		}
//		else if (level == 4) {
//			yield return new WaitForSeconds (150);
//		}
//		else if (level == 5) {
//			yield return new WaitForSeconds (200);
//		}
//		else {
//			yield return new WaitForSeconds (120);
//		}
		while (time_left >0){
			yield return null;
		}
		commenceBattle ();
	}
		
	public void loadNextLevel(){
		int c = SceneManager.GetActiveScene().buildIndex;
		if (c < SceneManager.sceneCountInBuildSettings) {
			SceneManager.LoadScene (c + 1);
			level++;
		}
	}

	public void ReloadLevel(){
		Reset ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	//this method is for performance.
	public bool returnGameOver(){
		return game_over;
	}

	bool checkIfGameOver(){
		if (demonLordFound && reachedDestination){
			//print ("Game Over, you lose!");

			gameOverUI.SetActive (true);  //prompt the game over screen
			return true;
		}

		if (TileMap.instance.Adventurers.Count == 0 && level != 0) { //if there are no more adventurers left, you win.
			//prompt victory screen, allow the player to go to next level.
			victoryUI.SetActive (true);  
			return true;
		}

		return false;
	}

	void setupMonsterDic(){
		//a manual process
		//print (monsterTypes[0].name);
		monsterDic = new Dictionary<string,GameObject>();
		for (int i=0;i<monsterTypes.Length;i++){
			monsterDic.Add(monsterTypes[i].name,monsterTypes[i].monsterPrefab);
		}
	}

	public void increaseDarkPower(float amount){
		darkPower += amount;
		//updateText ();
	}

	public void incrementDarkPower(){
		darkPower ++;
		updateText ();
	}

	public void Lock(){
		while (semaphore <= 0) {
			
		}
		semaphore--;
	}

	public void Unlock(){
		semaphore++;
	}
		


	public void GenerateAllLevels(){
		int randGateCount = 0;
		int bedrockCount = 0;
//		TileMap.instance.mapSizeX = 23;
//		TileMap.instance.mapSizeY = 11;

		int[,] tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		int x, y;
		// Initialize our map tiles to be grass

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 11; y++) {
				tiles [x, y] = 1;
			}
		}
			
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}

		for (x = 0;x < 4;x++){
			tiles [x, 10] = 0;
		}

		mapLayout.Add (tiles); //level zero


		TileMap.instance.mapSizeX = (int)TileMap.instance.defaultMapSize.x;
		TileMap.instance.mapSizeY = (int)TileMap.instance.defaultMapSize.y;

		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}


		//This is ground level
//		for (x = 0; x < 30; x++) {
//			tiles [x, 20] = (x % 2 == 0) ? 3 : 4;
//			tiles [x, 21] = 5;
//		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			tiles [x, 29] = (x % 2 == 0) ? 3 : 4;
			tiles [x, 30] = 5;
		}

		//This is the entry.
		//Transparent has index 5

//		int index = 6;
//		for (y = 20; y <= 21; y++) {
//			for (x = 13; x <= 15; x++) {
//				tiles [x, y] = index;
//				index++;
//			}
//		}

		int index = 6;
		for (y = 29; y <= 30; y++) {
			for (x = 10; x <= 12; x++) {
				tiles [x, y] = index;
				index++;
			}
		}

//		tiles [14, 19] = 0;
//		tiles [14, 18] = 0;
//		tiles [14, 17] = 0;
//		tiles [14, 16] = 0;
//		tiles [14, 15] = 0;
//		tiles [14, 14] = 0;

		tiles [11, 28] = 0;
		tiles [11, 27] = 0;
		tiles [11, 26] = 0;
		tiles [11, 25] = 0;
		tiles [11, 24] = 0;
		tiles [11, 23] = 0;

		//This is the special loop.
//		for (x = 13; x <= 15; x++) {
//			for (y = 11; y <=13 ; y++) {
//				tiles [x, y] = 0;
//			}
//		}

		for (x = 10; x <= 12; x++) {
			for (y = 20; y <=22 ; y++) {
				tiles [x, y] = 0;
			}
		}

		//The special tile - Gate
		//tiles[14,12] = 12;
		tiles[11,21] = 12;

		//keep going deeper
//		tiles [14, 10] = 0;
//		tiles [14, 9] = 0;
//		tiles [14, 8] = 0;
//		tiles [14, 7] = 0;
//		tiles [14, 6] = 0;
//		tiles [14, 5] = 0;
		tiles [11, 19] = 0;
		tiles [11, 18] = 0;
		tiles [11, 17] = 0;
		tiles [11, 16] = 0;
		tiles [11, 15] = 0;
		tiles [11, 14] = 0;

		//Where the demon lord lives...
//		for(int i=14;i<17;i++){
//			for (int j=4;j<=5;j++){
//				tiles[i,j] = 0;
//			}
//		}

		for(int i=11;i<14;i++){
			for (int j=13;j<=14;j++){
				tiles[i,j] = 0;
			}
		}

		mapLayout.Add (tiles); //this is level 1




		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

		tiles [13, 20] = 12;

	
		mapLayout.Add (tiles); //this is level 2




		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

		tiles [11, 20] = 12;


		mapLayout.Add (tiles); //this is level 3



		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}
			
		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

		tiles [10, 13] = 12;
		tiles [12, 11] = 12;


		mapLayout.Add (tiles); //this is level 4




		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 15;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;
		tiles [11, 28] = 0;
		tiles [11, 27] = 0;
		tiles [11, 26] = 0;
		tiles [11, 25] = 0;
		tiles [11, 24] = 0;
		tiles [11, 23] = 0;

		for (x = 9; x <= 13; x++) {
			for (y = 18; y <= 22; y++) {
				tiles [x,y] = 0;
			}
		}

		tiles [9, 18] = 12;
		tiles [13, 18] = 12;
		tiles [9, 22] = 12;
		tiles [13, 22] = 12;

		tiles [10, 19] = 15;
		tiles [12, 19] = 15;
		tiles [10, 21] = 15;
		tiles [12, 21] = 15;

		for (y=14;y<=17;y++){
			tiles [11, y] = 14;
		}



		mapLayout.Add (tiles); //this is level 5




		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		//This part is for smaller maps...
//		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
//			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
//				tiles [x, y] = 15;
//			}
//		}
//
//		for (x = 7; x < 16; x++) {
//			for (y = 2; y < TileMap.instance.mapSizeY; y++) {
//				tiles [x, y] = 1;
//			}
//		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

//		int gate_count = 0;
//		while(gate_count != 3){
//			int randx = Random.Range ();
//			int randy = Random.Range ();
//			if (tiles[randx,randy] !=12){
//				tiles [randx,randy] = 12;
//				gate_count++;
//			}
//		}
		tiles [5, 12] = 12;
		tiles [11, 18] = 12;
		tiles [17, 24] = 12;


		mapLayout.Add (tiles); //this is level 6


		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		//This part is for smaller maps...
		//		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
		//			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
		//				tiles [x, y] = 15;
		//			}
		//		}
		//
		//		for (x = 7; x < 16; x++) {
		//			for (y = 2; y < TileMap.instance.mapSizeY; y++) {
		//				tiles [x, y] = 1;
		//			}
		//		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

		//		int gate_count = 0;
		//		while(gate_count != 3){
		//			int randx = Random.Range ();
		//			int randy = Random.Range ();
		//			if (tiles[randx,randy] !=12){
		//				tiles [randx,randy] = 12;
		//				gate_count++;
		//			}
		//		}
		tiles [11, 12] = 12;
		tiles [5, 18] = 12;
		tiles [17, 18] = 12;
		tiles [11, 24] = 12;


		mapLayout.Add (tiles); //this is level 7


		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
//		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
//			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
//				tiles [x, y] = 15;
//			}
//		}
//
//		for (x = 7; x < 16; x++) {
//			for (y = 2; y < TileMap.instance.mapSizeY; y++) {
//				tiles [x, y] = 1;
//			}
//		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		bedrockCount = 75;
		int count = 0;
		while (count < bedrockCount) {
			int x_coord = Random.Range (0, TileMap.instance.mapSizeX);
			int y_coord = Random.Range (0, TileMap.instance.mapSizeY);
			if (tiles [x_coord, y_coord] != 15) {
				tiles [x_coord, y_coord] = 15;
				count++;
			}
		}

		tiles [11, 30] = 0;
		tiles [11, 29] = 0;
		tiles [11, 28] = 0;

		tiles [11, 18] = 13;
		tiles [14, 14] = 13;
		tiles [8, 14] = 13;

		tiles = ClearAdjacentTiles (tiles, 11, 18, 1);
		tiles = ClearAdjacentTiles (tiles, 14, 14, 1);
		tiles = ClearAdjacentTiles (tiles, 8, 14, 1);

//		while (count < randGateCount) {
//			int x_coord = Random.Range (0, TileMap.instance.mapSizeX);
//			int y_coord = Random.Range (0, TileMap.instance.mapSizeY);
//			if (tiles [x_coord, y_coord] != 13) {
//				tiles [x_coord, y_coord] = 13;
//				tiles = ClearAdjacentTiles (tiles, x_coord, y_coord, 1); //make sure the random gate is not blocked.
//				count++;
//			}
//		}

		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < 2; y++) {
				tiles [x, y] = 15;
			}
		}

		mapLayout.Add (tiles);//this is level 8
	
	}


	int[,] ClearAdjacentTiles(int[,] tiles, int x, int y, int tileNumber){ //make all adjacent tiles the one indicated by tile number
		for (int i=x-1;i<=x+1;i++){
			for (int j = y - 1; j <= y + 1; j++) {
				if (!(i==x && j==y) && i >=0 && i < TileMap.instance.mapSizeX && j>=0 && j < TileMap.instance.mapSizeY){
					tiles[i,j] = tileNumber;
				}
			}
		}
		return tiles;
	}

}
