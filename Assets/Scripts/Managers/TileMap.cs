using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TileMap : MonoBehaviour {

	public static TileMap instance;

	public List<GameObject> Adventurers;
	public List<GameObject> Monsters;
	public List<GameObject> Taunts;

	//public Dictionary <string, GameObject> Monsters;
	public GameObject DemonLord;

	public TileType[] tileTypes;
	//public MonsterType[] monsterTypes;
	public List<GameObject> buttonTypes;
	public List<GameObject> canvasTypes;

	public int[,] tiles;
	Node[,] graph;

	public int mapSizeX;
	public int mapSizeY;

	public List<Vector3> emptyLocations;

	public Vector2 defaultMapSize;
	public float digging_time;
	public bool is_digging;

	public float[,] movement_costs;

	int fusion_count;

//	public Vector3 startPos;


	//Goes first
	void Awake() {

		//print ("Map awake");

		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);	
		}
		instance = this;


		DemonLord = GameObject.Find ("Player");
		fusion_count = 0;
		is_digging = false;
		//GenerateMapData();
		GenerateMapData(Game_Manager.level);
		GeneratePathfindingGraph();
		AddMovementCosts ();
		GenerateMapVisual();

		AddEmptyLocations ();

	}
		

	void GenerateMapData(int level) {

//		if (Game_Manager.level == 1) {
//
//			generateLevel1 ();
//
//		} else if (Game_Manager.level == Game_Manager.final_level) {
//			// do this later;
//		} else {
//			//This is all the levels except the first and the last.
//		}

		if (level == 0) {
			int x, y;
			tiles = new int[mapSizeX, mapSizeY];
			// Initialize our map tiles to be grass
			for (x = 0; x <mapSizeX; x++) {
				for (y = 0; y < mapSizeY; y++) {
					tiles [x, y] = 1;
				}
			}

			for (x = 0; x < mapSizeX; x++) {
				for (y = 0; y < 2; y++) {
					tiles [x, y] = 15;
				}
			}

			for (x = 0;x < 4;x++){
				tiles [x, 5] = 0;
			}
		} else {
			//print ("here");
			int[,] copy = Game_Manager.instance.mapLayout [level].Clone () as int[,];
			tiles = copy;
			//tiles = Game_Manager.instance.mapLayout [level];
		}
	}

	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity );


				//flip the damn doors
				if (Game_Manager.level == 1) {
					if ((x == 12 && y == 30) || (x == 12 && y == 29)) {
						go.transform.Rotate (0, 180, 0);
					}
				}

//				ClickableTile ct = go.GetComponent<ClickableTile>();
//				ct.tileX = x;
//				ct.tileY = y;
				//ct.map = this;
			}
		}
	}

	public void AddMovementCosts(){
		movement_costs = new float[mapSizeX, mapSizeY];

		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				movement_costs [x, y] = tt.movementCost;
				//print (tt.movementCost);
			}
		}
	}

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY) {

		/*
		TileType tt = tileTypes[ tiles[targetX,targetY] ];

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = tt.movementCost;

		if( sourceX!=targetX && sourceY!=targetY) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}
		*/

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = movement_costs [targetX, targetY];

		if( sourceX!=targetX && sourceY!=targetY) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}

		return cost;

	}

	public float CostToEnterTileAdventurer(int sourceX, int sourceY, int targetX, int targetY) {


		TileType tt = tileTypes[ tiles[targetX,targetY] ];

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = tt.movementCost;

		if( sourceX!=targetX && sourceY!=targetY) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}

		return cost;

	}


	void GeneratePathfindingGraph() {
		// Initialize the array
		graph = new Node[mapSizeX,mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
			}
		}

		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {

				// This is the 4-way connection version:
				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );

				// This is the 8-way connection version (allows diagonal movement)
				// Try left
				/*
				if(x > 0) {
					graph[x,y].neighbours.Add( graph[x-1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x-1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x-1, y+1] );
				}

				// Try Right
				if(x < mapSizeX-1) {
					graph[x,y].neighbours.Add( graph[x+1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x+1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x+1, y+1] );
				}

				// Try straight up and down
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );

				// This also works with 6-way hexes and n-way variable areas (like EU4)
				*/
			}
		}
	}



	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3(x, y, 0);
	}

	public bool UnitCanEnterTile(int x, int y) {

		// We could test the unit's walk/hover/fly type against various
		// terrain flags here to see if they are allowed to enter the tile.

		return tileTypes[ tiles[x,y] ].isWalkable;
	}

	/*
	public void GeneratePathTo(int x, int y) {
		// Clear out our unit's old path.
		Adventurer.GetComponent<Adventurer>().currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
		
		Node source = graph[
		                    Adventurer.GetComponent<Adventurer>().tileX, 
		                    Adventurer.GetComponent<Adventurer>().tileY
		                    ];
		
		Node target = graph[
		                    x, 
		                    y
		                    ];
		
		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		Adventurer.GetComponent<Adventurer>().currentPath = currentPath;
	}
	*/


	void AddEmptyLocations(){
		for (int i = 0; i < mapSizeX; i++) {
			for (int j = 0; j < mapSizeY; j++) {
				if (tiles[i,j] == 0)
					emptyLocations.Add(new Vector3(i,j,0));
			}
		}
	}


	public List<Node> returnPathGenerated(Vector3 start, Vector3 dest) {
		// Clear out our unit's old path

		if( UnitCanEnterTile((int)dest.x,(int)dest.y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return null;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph[
			//Adventurer.GetComponent<Adventurer>().tileX, 
			//Adventurer.GetComponent<Adventurer>().tileY
			(int)start.x,
			(int)start.y
		];

		Node target = graph[
			(int)dest.x, 
			(int)dest.y
		];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return null;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		return currentPath;
	}

	bool isDiggable(int x, int y){
		if (x-1 >=0 && y-1 >=0 && x+1 <=mapSizeX-1 && y+1 <= mapSizeY-1)
			if (tiles [x+1, y] == 0 || tiles [x - 1, y] == 0 || tiles [x, y + 1] == 0 || tiles [x, y - 1] == 0)
				return true;
		
		return false;
	}


//	public bool dig(int x,int y){
//		//if the spot is already empty, do nothing
//		if(tiles[x,y] == 0){
//			return false;
//		} 
//		//if (Game_Manager.instance.digLeft > 0 && isDiggable (x, y)) {
//		if (isDiggable (x, y)) {
//
//
//			//print ("click");
//			GameObject go = Instantiate (tileTypes [0].tileVisualPrefab, new Vector3 (x, y, 0), Quaternion.identity);
//			tiles [x, y] = 0;
////			Game_Manager.instance.digLeft--;
//			emptyLocations.Add(new Vector3(x,y,0));
//			Game_Manager.instance.updateText ();
//			return true;
//		} 
////		else if (Game_Manager.instance.digLeft == 0) {
////			print ("You've used up all the digging power for this round!");
////			return false;
////		}
//		else {
//			print ("This tile is not diggable.");
//			return false;
//		}
//	}

	public IEnumerator dig(GameObject toDestory){
		//if the spot is already empty, do nothing
		ClickableTile script = toDestory.GetComponent<ClickableTile>();
		int x = script.tileX;
		int y = script.tileY;

		if(tiles[x,y] == 0){
			yield break;
		}

		if (is_digging){
			print ("Already digging another tile!!!!!!");
			yield break;
		}

		//if (Game_Manager.instance.digLeft > 0 && isDiggable (x, y)) {
		if (isDiggable (x, y)) {

			is_digging = true;
			yield return new WaitForSeconds (digging_time);

			if (script.tileIndex == 1) {
				Game_Manager.instance.tileNumbers [0]++;
			}
			if (script.tileIndex == 2) {
				Game_Manager.instance.tileNumbers [1]++;
			}
			if (script.tileIndex == 3) {
				Game_Manager.instance.tileNumbers [2]++;
			}

			Destroy(toDestory);
			GameObject go = Instantiate (tileTypes [0].tileVisualPrefab, new Vector3 (x, y, 0), Quaternion.identity);
			tiles [x, y] = 0;
			movement_costs [x, y] = 1;
			//			Game_Manager.instance.digLeft--;
			emptyLocations.Add(new Vector3(x,y,0));
			Game_Manager.instance.updateText ();
		} 
		//		else if (Game_Manager.instance.digLeft == 0) {
		//			print ("You've used up all the digging power for this round!");
		//			return false;
		//		}
		else {
			print ("This tile is not diggable.");
			yield return null;
		}
		is_digging = false;
	}


	public IEnumerator OpenChest(GameObject chest){
		//if the spot is already empty, do nothing
		Chest script = chest.GetComponent<Chest>();
		int x = script.tileX;
		int y = script.tileY;

		if(tiles[x,y] == 0){
			yield break;
		}

		if (is_digging){
			print ("Already digging another tile!!!!!!");
			yield break;
		}

		//if (Game_Manager.instance.digLeft > 0 && isDiggable (x, y)) {
		if (isDiggable (x, y)) {

			is_digging = true;
			yield return new WaitForSeconds (digging_time);

			Game_Manager.instance.tileNumbers [0] += script.treasure_count;

			Destroy(chest);
			GameObject go = Instantiate (tileTypes [0].tileVisualPrefab, new Vector3 (x, y, 0), Quaternion.identity);
			tiles [x, y] = 0;
			movement_costs [x, y] = 1;
			//			Game_Manager.instance.digLeft--;
			emptyLocations.Add(new Vector3(x,y,0));
			Game_Manager.instance.updateText ();
		} 
		//		else if (Game_Manager.instance.digLeft == 0) {
		//			print ("You've used up all the digging power for this round!");
		//			return false;
		//		}
		else {
			print ("This tile is not diggable.");
			yield return null;
		}
		is_digging = false;
	}



	public void fusionWithIdenticalObject(GameObject toDestroy, GameObject toCreate, Vector3 location){
//		fusion_count++;
//		Destroy (toDestroy);
//		if (fusion_count == 2) { //we'll get 2 with two objects calling this method
//			GameObject.Instantiate (toCreate, location, Quaternion.identity);	
//			fusion_count = 0;
//		}

		Monster script = toDestroy.GetComponent<Monster> ();
		if (script != null && !script.fused) {
			fusion_count++;
			Destroy (toDestroy);
			if (fusion_count == 2) { //we'll get 2 with two objects calling this method
				GameObject.Instantiate (toCreate, location, Quaternion.identity);	
				fusion_count = 0;
			}
		}
	}



	public List<Node> returnPathGeneratedAdventurer(Vector3 start, Vector3 dest) {
		// Clear out our unit's old path

		if( UnitCanEnterTile((int)dest.x,(int)dest.y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return null;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph[
			//Adventurer.GetComponent<Adventurer>().tileX, 
			//Adventurer.GetComponent<Adventurer>().tileY
			(int)start.x,
			(int)start.y
		];

		Node target = graph[
			(int)dest.x, 
			(int)dest.y
		];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTileAdventurer(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return null;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		return currentPath;
	}


		
}
