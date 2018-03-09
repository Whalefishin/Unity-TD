using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonLord : Characters {

	private Vector3 screenPoint;
	private Vector3 offset;
	public bool beingDragged;
	private Vector3 pos;
	public Transform tr;

	public Vector3 previousPosition;
	public bool registered;

	public GameObject dragger;

	Vector3 camera_orig_scale;


	void Start(){
		base.Start ();
		pos = transform.position;
		tr = transform;
		beingDragged = false;
		//speed = 1.0f;
		previousPosition = new Vector3 (tileX-1, tileY, 0);
		registered = false;
		//camera_orig_scale = camera.transform.localScale;

		StartCoroutine (MoveBeforeBeingDragged ());
	}


	// Update is called once per frame
	void Update () {
//		if (beingDragged && !(Game_Manager.instance.returnGameOver())){
//			dragged ();
//		}
//		else if (Game_Manager.instance.returnGameOver()){
//			beingDragged = false;
//			//do nothing
//		}else {
//			move ();
//		}
	}

	protected IEnumerator MoveBeforeBeingDragged(){
		while (true) {
			if (Game_Manager.instance.demonLordFound && !(Game_Manager.instance.returnGameOver ())) {
				//print ("dragged");
				yield return Dragged ();
			} else if (Game_Manager.instance.returnGameOver ()) {
				//Game_Manager.instance.demonLordFound = false;
				yield return null;
				//yield return Move ();
			} else {
				yield return Move ();
			}
		}
	}

	protected IEnumerator Move(){
		pos = transform.position;
		if (Input.GetKeyDown (KeyCode.D) && tr.position == pos && TileMap.instance.UnitCanEnterTile ((int)pos.x + 1, (int)pos.y)) {
			pos += Vector3.right;
			turnRight ();
			registered = false;
		} else if (Input.GetKeyDown (KeyCode.A) && tr.position == pos && TileMap.instance.UnitCanEnterTile ((int)pos.x - 1, (int)pos.y)) {
			pos += Vector3.left;
			turnLeft ();
			registered = false;
		} else if (Input.GetKeyDown (KeyCode.W) && tr.position == pos && TileMap.instance.UnitCanEnterTile ((int)pos.x, (int)pos.y + 1)) {
			pos += Vector3.up;
			turnUp ();
			registered = false;
		} else if (Input.GetKeyDown (KeyCode.S) && tr.position == pos && TileMap.instance.UnitCanEnterTile ((int)pos.x, (int)pos.y - 1)) {
			pos += Vector3.down;
			turnDown ();
			registered = false;
		}

		while ((transform.position - pos).sqrMagnitude > float.Epsilon) {
			transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
			yield return null;
		}

		tileX = (int)pos.x;
		tileY = (int)pos.y;

//		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
//		if ((transform.position - pos).sqrMagnitude < float.Epsilon && (!registered)) {
//			previousPosition = new Vector3 (tileX, tileY, 0);
//			tileX = (int)pos.x;
//			tileY = (int)pos.y;
//			registered = true;
//		}
	}

	protected override void turnRight(){
		bool prev_left = facing_left;
		base.turnRight ();

		//camera.transform.localScale = camera_orig_scale;
		//camera.GetComponent<AdjustableCamera> ().inverted = true;
		//Vector3 newPos = new Vector3(transform.position.x,transform.position.y,-10);
		//camera.transform.position = newPos;
	}

	protected override void turnLeft(){
		bool prev_right = facing_right;
		base.turnLeft ();

//		Vector3 theScale = camera_orig_scale;
//		theScale.x *= -1;
//		camera.transform.localScale = theScale;	
		//camera.GetComponent<AdjustableCamera> ().inverted = false;
		//Vector3 newPos = new Vector3(transform.position.x,transform.position.y,-10);
		//camera.transform.position = newPos;
	}


//	void dragged(){
//		speed = TileMap.instance.Adventurers [0].GetComponent<Adventurer> ().speed;
//		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
//			if (TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().isTail == true) {
//				moveTowards (new Vector3(TileMap.instance.Adventurers [i].GetComponent<Adventurer>().tileX,
//					TileMap.instance.Adventurers [i].GetComponent<Adventurer>().tileY,0));
//				//transform.position = TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().prevPosition;
//			}
//		}
//	}

//	IEnumerator Dragged(){
//		speed = TileMap.instance.Adventurers [0].GetComponent<Adventurer> ().speed;
//		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
//			if (TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().foundDemonLord == true) {
//				yield return MoveOneStepTowardsGameObject (TileMap.instance.Adventurers [i]);
//				//transform.position = TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().prevPosition;
//			}
//		}
//	}

	IEnumerator Dragged(){
		speed = TileMap.instance.Adventurers [0].GetComponent<Adventurer> ().speed;
		yield return MoveOneStepTowardsGameObject (dragger);
				//transform.position = TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().prevPosition;
	}



	//	void OnMouseDown()
	//	{
	//		if (Game_Manager.instance.preparationPhase == true) {
	//			screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
	//			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	//		}
	//	}
	//
	//
	//	public void move(){
	//		if (Input.GetKeyDown(KeyCode.D) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x+1,(int)pos.y)) {
	//			pos += Vector3.right;
	//			turnRight ();
	//			registered = false;
	//		}
	//		else if (Input.GetKeyDown(KeyCode.A) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x-1,(int)pos.y)) {
	//			pos += Vector3.left;
	//			turnLeft ();
	//			registered = false;
	//		}
	//		else if (Input.GetKeyDown(KeyCode.W) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x,(int)pos.y+1)) {
	//			pos += Vector3.up;
	//			turnUp ();
	//			registered = false;
	//		}
	//		else if (Input.GetKeyDown(KeyCode.S) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x,(int)pos.y-1)) {
	//			pos += Vector3.down;
	//			turnDown ();
	//			registered = false;
	//		}
	//		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
	//		if ((transform.position - pos).sqrMagnitude < float.Epsilon && (!registered)) {
	//			previousPosition = new Vector3 (tileX, tileY, 0);
	//			tileX = (int)pos.x;
	//			tileY = (int)pos.y;
	//			registered = true;
	//		}
	//	}

	//	void OnMouseDrag()
	//	{
	//		if (Game_Manager.instance.preparationPhase == true) {
	//			Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,Input.mousePosition.y, screenPoint.z);
	//			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
	//			Vector3 adjustedPos = new Vector3 ((int)curPosition.x, (int)curPosition.y);
	//
	//			//only allow the player to relocate to an empty tile
	//			if (TileMap.instance.tiles [(int)adjustedPos.x, (int)adjustedPos.y] == 0) {
	//				transform.position = adjustedPos;
	//			}
	//		}
	//	}
	//


//	void moveTowards(Vector3 dest){
//		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
//		List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),dest);
//		// Move us to the next tile in the sequence
//
//		if (path != null) {
//			int X = path [1].x;
//			int Y = path [1].y;
//
//			Vector3 nextStep = new Vector3(X,Y,0);
//
//			//transform.position = TileMap.instance.TileCoordToWorldCoord( tileX, tileY );	// Update our unity world position
//
//			//Smooth movement
//			//Vector3 end = new Vector3(tileX,tileY,transform.position.z);
//			float step = speed * Time.deltaTime;
//			//transform.position = Vector3.MoveTowards (transform.position, TileMap.instance.TileCoordToWorldCoord (tileX, tileY), step);
//			transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
//			if ((transform.position - nextStep).sqrMagnitude < float.Epsilon) {
//				tileX = X;
//				tileY = Y;
//			}
//		}
//	}

}
