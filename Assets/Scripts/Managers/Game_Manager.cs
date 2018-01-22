using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour {

	//TileMap mapscript;

	public Vector3 original_camera_pos;

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
	public List<int[,]> mapLayout;
	public List<int> tileNumbers;

	private bool game_over;

	int scrollDistance = 5; 
	float scrollSpeed = 30;

	void Awake () {
		//print ("Awake!");
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);	
		}
			
		DontDestroyOnLoad(gameObject);

		original_camera_pos = transform.position;

//		mapLayout = new List<int[,]> ();
//		GenerateAllLevels ();

		//TileMap.instance = GameObject.Find ("Map").GetComponent<TileMap> ();

//		gameOverUI.SetActive (true);
//		gameOverUI = GameObject.Find ("GameOver");
//		gameOverUI.SetActive (false);
		prepare ();

		//resources
		darkPower_base = 10;
		//digLeft = 100;
		darkPower = darkPower_base;

		commenceButton = GameObject.Find ("Button").GetComponent<Button> ();
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

		tileText[0].text = "Common: " + tileNumbers[0];
		tileText[1].text = "Rare: " + tileNumbers[1];
		tileText[2].text = "Rarest: " + tileNumbers[2];


		darkpowerText = GameObject.Find ("darkpowerText").GetComponent<Text> ();
		darkpowerText.text = "Dark Power: " + darkPower;

		demonLordFound = false;
		reachedDestination = false;

		setupMonsterDic ();
		prepare ();
	}
		
	void Start(){
		mapLayout = new List<int[,]> ();
		GenerateAllLevels ();
		InvokeRepeating("incrementDarkPower", 0f,5f);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		demonLordFound = false;
		reachedDestination = false;
		Reset ();
//		print ("OnSceneLoaded!");
//		//setup button
//		commenceButton = GameObject.Find ("Button").GetComponent<Button> ();
//		commenceButton.onClick.AddListener(commenceBattle);
//		//setup map
//		TileMap.instance = GameObject.Find ("Map").GetComponent<TileMap> ();
//		gameOverUI = GameObject.Find ("GameOver");
//		gameOverUI.SetActive (false);
//		//setup text
//		digText = GameObject.Find ("digText").GetComponent<Text>();
//		darkpowerText = GameObject.Find ("darkpowerText").GetComponent<Text>();
//		Reset ();
//		//digText.text = "Digs Left: " + digLeft;
//		prepare ();
	}

	// Update is called once per frame
	void Update () {
		
		//adjustCamera ();
		SceneManager.sceneLoaded += OnSceneLoaded;
		game_over = checkIfGameOver ();
		updateText ();
	}

	void prepare(){
		preparationPhase = true;

		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
			TileMap.instance.Adventurers[i].SetActive (false);
		}

	}

	public void commenceBattle(){
		preparationPhase = false;
		TileMap.instance.getDemonLordPos ();
		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
			TileMap.instance.Adventurers[i].SetActive (true);
		}

	}

	public void updateText(){
		//digText.text = "Digs Left: " + digLeft;
		tileText[0].text = "Common: " + tileNumbers[0];
		tileText[1].text = "Rare: " + tileNumbers[1];
		tileText[2].text = "Rarest: " + tileNumbers[2];

		darkpowerText.text = "Dark Power: " + darkPower;
	}

	void Reset(){
		gameOverUI.SetActive (false);
		demonLordFound = false;
		reachedDestination = false;
		darkPower = darkPower_base;
		for (int i=0;i<tileNumbers.Count;i++){
			tileNumbers[i] = 0;	
		}
		//updateText ();
		preparationPhase = true;
	}
		
	public void loadNextLevel(){
		level++;
		int c = SceneManager.GetActiveScene().buildIndex;
		if (c < SceneManager.sceneCountInBuildSettings)
			SceneManager.LoadScene(c+1);
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

		if (TileMap.instance.Adventurers.Count == 0) { //if there are no more adventurers left, you win.
			//prompt victory screen, allow the player to go to next level.

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

	void adjustCamera(){

		//press H to restore default position
		if (Input.GetKeyDown("h")){
			transform.position = original_camera_pos;
		}

		float mousePosX = Input.mousePosition.x; 
		float mousePosY = Input.mousePosition.y;
		const int orthographicSizeMin = 8;
		const int orthographicSizeMax = 20;

		if (mousePosX < scrollDistance) 
		{ 
			transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime); 
		} 

		if (mousePosX >= Screen.width - scrollDistance) 
		{ 
			transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime); 
		}

		if (mousePosY < scrollDistance) 
		{ 
			//transform.Translate(transform.forward * -scrollSpeed * Time.deltaTime); 
			transform.Translate(Vector3.up * -scrollSpeed * Time.deltaTime); 
		} 

		if (mousePosY >= Screen.height - scrollDistance) 
		{ 
			//transform.Translate(transform.forward * scrollSpeed * Time.deltaTime); 
			transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime); 
		}


		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			Camera.main.orthographicSize++;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			Camera.main.orthographicSize--;
		}
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax );
	}



	public void GenerateAllLevels(){

		int[,] tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		int x, y;
		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}

		for (x = 0;x < 4;x++){
			tiles [x, 10] = 0;
		}

		mapLayout.Add (tiles); //level zero




		tiles = new int[TileMap.instance.mapSizeX,TileMap.instance.mapSizeY];
		x = 0;
		y = 0;

		// Initialize our map tiles to be grass
		for (x = 0; x < TileMap.instance.mapSizeX; x++) {
			for (y = 0; y < TileMap.instance.mapSizeY; y++) {
				tiles [x, y] = 1;
			}
		}


		//This is ground level
		for (x = 0; x < 30; x++) {
			tiles [x, 20] = (x % 2 == 0) ? 3 : 4;
			tiles [x, 21] = 5;
		}

		//This is the entry.
		//Transparent has index 5
		int index = 6;
		for (y = 20; y <= 21; y++) {
			for (x = 13; x <= 15; x++) {
				tiles [x, y] = index;
				index++;
			}
		}

		tiles [14, 19] = 0;
		tiles [14, 18] = 0;
		tiles [14, 17] = 0;
		tiles [14, 16] = 0;
		tiles [14, 15] = 0;
		tiles [14, 14] = 0;


		//This is the special loop.
		for (x = 13; x <= 15; x++) {
			for (y = 11; y <=13 ; y++) {
				tiles [x, y] = 0;
			}
		}

		//The special tile - Gate
		tiles[14,12] = 12;

		//keep going deeper
		tiles [14, 10] = 0;
		tiles [14, 9] = 0;
		tiles [14, 8] = 0;
		tiles [14, 7] = 0;
		tiles [14, 6] = 0;
		tiles [14, 5] = 0;

		//Where the demon lord lives...
		for(int i=14;i<17;i++){
			for (int j=4;j<=5;j++){
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

		tiles [14, 21] = 0;
		tiles [14, 20] = 0;
		tiles [14, 19] = 0;

		tiles [17, 15] = 12;

	
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
			
		tiles [14, 21] = 0;
		tiles [14, 20] = 0;
		tiles [14, 19] = 0;

		tiles [13, 8] = 12;
		tiles [15, 8] = 12;


		mapLayout.Add (tiles); //this is level 3
	
	}

}
