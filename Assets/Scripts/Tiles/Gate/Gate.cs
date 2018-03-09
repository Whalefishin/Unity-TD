using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gate : Unit {

	//these two are in fact redundant, just a hassle to clean them up
	protected int x;
	protected int y;

	public GameObject GateCanvas;
//	public Button Left;
//	public Button Right;
//	public Button Up;
//	public Button Down;
//
//	public Vector2 canvas_size;
//	public Vector3 canvas_offset;
//	public Vector3 button_offset;
//
//	public List<GameObject> allButtons;
//	public List<GameObject> allSubCanvas;
//
//	public List<Vector3> spawnLocations;
//	public List<GameObject> spawnTypes;
//	public Dictionary<Vector3,GameObject> location_monster;
//	public Dictionary<Vector3,GameObject> location_marker;

	public int click_parity;


	// Use this for initialization
	protected virtual void Start () {
		x = (int)transform.position.x;
		y = (int)transform.position.y;

//		location_monster = new Dictionary<Vector3,GameObject> ();
//		location_marker = new Dictionary<Vector3,GameObject> ();
//
//		canvas_size = new Vector2(38.41f,100f);
//		canvas_offset = new Vector3 (1.65f, 0,0);
//		button_offset = new Vector3 (0, -0.5f, 0);

		click_parity = 0;

		GateCanvas = transform.Find ("GateCanvas").gameObject;
		GateCanvas.SetActive (false);
		foreach (Transform child in GateCanvas.transform) {
			child.gameObject.SetActive (false);
		}

		//setupButtons ();
//		setupMonsterChoices ();
//		StartCoroutine (SetFalseIfTooFar());

		/*
		foreach (GameObject button in allButtons){
			GameObject childCanvas = GameObject.Instantiate (TileMap.instance.canvasTypes[0]);
			childCanvas.transform.SetParent (button.transform,false);
			allSubCanvas.Add (childCanvas);
		}
		*/
	}
	

	protected void Update(){
//		if (Vector3.Distance (TileMap.instance.DemonLord.transform.position, this.gameObject.transform.position) > 1.5 &&
//			GateCanvas.activeSelf) {
//			SetFalse ();
//		}

		//Spawn();
	}

	protected void SetFalse(){
		foreach (Transform child in GateCanvas.transform) {
			child.gameObject.SetActive (false);
		}
		GateCanvas.SetActive (false);
		click_parity++;
	}

	protected IEnumerator SetFalseIfTooFar(){
		float check_in_time = 1f;
		while (true) {
			yield return new WaitForSeconds (check_in_time);
			if (GateCanvas.activeSelf && Vector3.Distance (TileMap.instance.DemonLord.transform.position, this.gameObject.transform.position) > 1.5) {
				SetFalse ();
			}
		}
	}

//	protected virtual void OnMouseOver () {
//		//detect right click
//		if (Input.GetMouseButtonDown (0)) {
//			if (click_parity % 2 == 0) {
//				if (Vector3.Distance (TileMap.instance.DemonLord.transform.position, this.gameObject.transform.position) < 1.5) {
//					//display the gui
//					GateCanvas.SetActive (true);
//					foreach (Transform child in GateCanvas.transform) {
//						if (TileMap.instance.tiles [x + 1, y] == 0 && child.name == "Right") {
//							child.gameObject.SetActive (true);
//						}
//						if (x - 1 >= 0 && TileMap.instance.tiles [x - 1, y] == 0 && child.name == "Left") {
//							child.gameObject.SetActive (true);
//						}
//						if (TileMap.instance.tiles [x, y + 1] == 0 && child.name == "Up") {
//							child.gameObject.SetActive (true);
//						}
//						if (y - 1 >= 0 && TileMap.instance.tiles [x, y - 1] == 0 && child.name == "Down") {
//							child.gameObject.SetActive (true);
//						}
//
//					}
//				}
//
//			}
//
//			else {
//				foreach (Transform child in GateCanvas.transform) {
//					child.gameObject.SetActive (false);
//				}
//
//				GateCanvas.SetActive (false);
//			}
//			click_parity++;
//		}
//	}

	protected virtual void OnMouseOver () {
	}


//	protected void setupButtons(){
//		Left = transform.Find ("GateCanvas/Left").gameObject.GetComponent<Button> ();
//		Right = transform.Find ("GateCanvas/Right").gameObject.GetComponent<Button> ();
//		Up = transform.Find ("GateCanvas/Up").gameObject.GetComponent<Button> ();
//		Down = transform.Find ("GateCanvas/Down").gameObject.GetComponent<Button> ();
//
//		allButtons.Add (Up.gameObject);
//		allButtons.Add (Down.gameObject);
//		allButtons.Add (Left.gameObject);
//		allButtons.Add (Right.gameObject);
//
//
//		Left.onClick.AddListener (() => {enableMonsterButtons(Left.gameObject);});
//		Right.onClick.AddListener (() => {enableMonsterButtons(Right.gameObject);});
//		Up.onClick.AddListener (() => {enableMonsterButtons(Up.gameObject);});
//		Down.onClick.AddListener (() => {enableMonsterButtons(Down.gameObject);});
//	}

//
//	protected void setupMonsterChoices(){
//		//int j = 0;
//		foreach (GameObject button in allButtons) {
//			//setup the sub-canvas
//			GameObject childCanvas = GameObject.Instantiate (TileMap.instance.canvasTypes[0]);
//			childCanvas.transform.SetParent (button.transform,false);
//			//childCanvas.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
//			//childCanvas.GetComponent<Canvas> ().worldCamera = Camera.main;
//			childCanvas.GetComponent<RectTransform> ().sizeDelta = canvas_size;
//			childCanvas.GetComponent<RectTransform> ().position = button.transform.position + canvas_offset;
//
//			for (int i = 0; i <	Game_Manager.instance.monsterTypes.Length; i++) {
//				MonsterType type = Game_Manager.instance.monsterTypes [i];
//				if (Game_Manager.level >= type.min_level && type.isSpawnable) {
//					GameObject monsterButtonObject = GameObject.Instantiate (TileMap.instance.buttonTypes [0]);
//					monsterButtonObject.transform.SetParent (childCanvas.transform, false);
//					monsterButtonObject.transform.position += button_offset * i;
//					//monsterButtonObject.transform.SetParent (button.transform+canvas_offset, false);
//
//					Text monsterText = monsterButtonObject.GetComponentInChildren<Text> ();
//					//The text displayed on the button is the monster name
//					//monsterText.text = Game_Manager.instance.monsterTypes [i].name;
//					monsterText.text = type.name;
//
//					Button monsterButton = monsterButtonObject.GetComponent<Button> ();
//
//					//Add onclick stuff
//
//					//monsterButton.onClick.AddListener (closeGUI);
//					monsterButton.onClick.AddListener (() => {
//						addSpawn (monsterButtonObject);
//					});
//					monsterButton.onClick.AddListener (() => {
//						closeGUIAndMyself (monsterButtonObject);
//					});
//
//					//initially set these sub-buttons as inactive
//					monsterButtonObject.SetActive (false);
//				}
//			}
//		}
//	}




//	protected void enableMonsterButtons(GameObject this_button){
//		foreach (GameObject button in allButtons) {
//			GameObject subcanvas = button.GetComponentInChildren<Canvas> ().gameObject;
//			//hide all the other sub_buttons
//			if (button != this_button) {
//				//hide all the other buttons' canvas so that they don't just display their own buttons
//				subcanvas.GetComponent<Canvas>().enabled = false;
//				//also disable their buttons
//				foreach (Transform child in subcanvas.transform) {
//					child.gameObject.SetActive (false);
//				}
//			} 
//			//display all of this button's sub_buttons
//			else {
//				subcanvas.GetComponent<Canvas>().enabled = true;
//				foreach (Transform child in subcanvas.transform) {
////					if (child.GetComponentInChildren<Text> ().text == "Up" && TileMap.instance.tiles[x,y] == 0) {
////						child.gameObject.SetActive (true);
////					}
//					child.gameObject.SetActive (true);
//				}
//			}
//		}
//	}



//
//	protected virtual void openGate(string direction, GameObject monster){
//
//		int[] gate_cost = monster.GetComponent<Monster> ().gate_cost;
//
//
//		//zeroth element is darkpower, the others follow tileNumbers
//		if (gate_cost [0] <= Game_Manager.instance.darkPower
//		    && gate_cost [1] <= Game_Manager.instance.tileNumbers [0]
//		    && gate_cost [2] <= Game_Manager.instance.tileNumbers [1]
//		    && gate_cost [3] <= Game_Manager.instance.tileNumbers [2]) {
//			for (int i = 0; i < gate_cost.Length; i++) {
//				if (i == 0) {
//					Game_Manager.instance.darkPower -= gate_cost [i];
//				} else {
//					Game_Manager.instance.tileNumbers [i - 1] -= gate_cost [i];
//				}
//			}
//
//			Vector3 spawnLocation = new Vector3 (0, 0, 0);
//			if (direction == "Up") {
//				spawnLocation = new Vector3 (x, y + 1, 0);
//			} else if (direction == "Down") {
//				spawnLocation = new Vector3 (x, y - 1, 0);
//			} else if (direction == "Left") {
//				spawnLocation = new Vector3 (x - 1, y, 0);
//			} else {
//				spawnLocation = new Vector3 (x + 1, y, 0);
//			}
//
//			if (spawnLocations.Contains (spawnLocation)) {//see if the player is trying to overwrite gates
//				if (location_monster [spawnLocation] == monster) { // if the player is trying to spawn the same monster, probably a misclick
//					Game_Manager.instance.tileNumbers [0] += gate_cost [1];
//				} else {
//					location_monster [spawnLocation] = monster;
//
//					//destroy and replace marker
//					Destroy (location_marker [spawnLocation]);
//					if (monster.name == "Slime"){
//						GameObject toInstantiate = GameObject.Instantiate(monster.GetComponent<Slime> ().marker,spawnLocation,Quaternion.identity);
//						location_marker [spawnLocation] = toInstantiate;
//					}
//					else if (monster.name == "KuroSlime"){
//						GameObject toInstantiate = GameObject.Instantiate(monster.GetComponent<KuroSlime> ().marker,spawnLocation,Quaternion.identity);
//						location_marker [spawnLocation] = toInstantiate;
//					}
//				}
//			} else {
//				spawnLocations.Add (spawnLocation);
//				location_monster.Add (spawnLocation, monster);
//				StartCoroutine (SpawnAt (spawnLocation));
//
//				//add the marker
//				if (monster.name == "Slime"){
//					GameObject toInstantiate = GameObject.Instantiate(monster.GetComponent<Slime> ().marker,spawnLocation,Quaternion.identity);
//					location_marker.Add (spawnLocation, toInstantiate);
//				}
//				else if (monster.name == "KuroSlime"){
//					GameObject toInstantiate = GameObject.Instantiate(monster.GetComponent<KuroSlime> ().marker,spawnLocation,Quaternion.identity);
//					location_marker.Add(spawnLocation,toInstantiate);
//				}
//			}
//			//location_monster.Add (spawnLocation, monster);
//		} else {
//			print ("Not enough resources!");
//		}
//	}
		

//	protected virtual void Spawn(){
//		for(int i=0;i<spawnLocations.Count;i++) {
//			Vector3 location = spawnLocations [i];
//			GameObject toSpawn = location_monster [location];
//			if (Time.time - last_spawn_times[i] >= toSpawn.GetComponent<Monster>().spawn_time) {
//				GameObject.Instantiate (toSpawn, location, Quaternion.identity);
//				last_spawn_times [i] = Time.time;
//			}
//		}
//	}

	protected virtual IEnumerator SpawnAt(Vector3 location){
		//GameObject toSpawn = location_monster [location];
//		while (true) {
//			GameObject toSpawn = location_monster [location];
//			GameObject.Instantiate (toSpawn, location, Quaternion.identity);
//			yield return new WaitForSeconds (toSpawn.GetComponent<Monster> ().spawn_time);
//		}
		yield return null;
	}

	/*
	protected virtual IEnumerator Spawn(){
		for(int i=0;i<spawnLocations.Count;i++) {
			Vector3 location = spawnLocations [i];
			StartCoroutine (SpawnAt (location));
		}
	}
	*/


//	protected virtual void addSpawn(GameObject monsterButtonObject){
//		//need two information: the direction of the gate, and the type of monster
//		GameObject monsterToSpawn;
//
//		//get monster to spawn
//		Text monsterName = monsterButtonObject.GetComponentInChildren<Text>();
//		monsterToSpawn = Game_Manager.instance.monsterDic [monsterName.text];
//
//		//get location, which is encoded in the text.
//		openGate (monsterButtonObject.transform.parent.parent.Find("ButtonText").GetComponent<Text>().text, monsterToSpawn);
//	}
//


	protected void closeGUIAndMyself(GameObject self){
		//self.transform.parent.gameObject.SetActive (false);
		self.SetActive (false);
		GateCanvas.SetActive (false);
	}
}
