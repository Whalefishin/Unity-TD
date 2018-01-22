using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonLord : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	public float speed;
	public bool beingDragged;
	private Vector3 pos;
	public Transform tr;

	public int tileX;
	public int tileY;

	public Vector3 previousPosition;
	public bool registered;


		

	void Start(){
		tileX = (int)transform.position.x;
		tileY = (int)transform.position.y;
		pos = transform.position;
		tr = transform;
		beingDragged = false;
		speed = 1.0f;
		previousPosition = new Vector3 (tileX-1, tileY, 0);
		registered = false;
	}


	// Update is called once per frame
	void Update () {
		if (beingDragged && !(Game_Manager.instance.returnGameOver())) {
			dragged ();
		} else {
			move ();
		}
	}

	public void move(){
		if (Input.GetKeyDown(KeyCode.D) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x+1,(int)pos.y)) {
			pos += Vector3.right;
			registered = false;
		}
		else if (Input.GetKeyDown(KeyCode.A) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x-1,(int)pos.y)) {
			pos += Vector3.left;
			registered = false;
		}
		else if (Input.GetKeyDown(KeyCode.W) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x,(int)pos.y+1)) {
			pos += Vector3.up;
			registered = false;
		}
		else if (Input.GetKeyDown(KeyCode.S) && tr.position == pos && TileMap.instance.UnitCanEnterTile((int)pos.x,(int)pos.y-1)) {
			pos += Vector3.down;
			registered = false;
		}
		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
		if ((transform.position - pos).sqrMagnitude < float.Epsilon && (!registered)) {
			previousPosition = new Vector3 (tileX, tileY, 0);
			tileX = (int)pos.x;
			tileY = (int)pos.y;
			registered = true;
		}
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
	void dragged(){
		speed = TileMap.instance.Adventurers [0].GetComponent<Adventurer> ().speed;
		for (int i = 0; i < TileMap.instance.Adventurers.Count; i++) {
			if (TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().isTail == true) {
				moveTowards (new Vector3(TileMap.instance.Adventurers [i].GetComponent<Adventurer>().tileX,
					TileMap.instance.Adventurers [i].GetComponent<Adventurer>().tileY,0));
				//transform.position = TileMap.instance.Adventurers [i].GetComponent<Adventurer> ().prevPosition;
			}
		}
	}

	void moveTowards(Vector3 dest){
		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
		List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),dest);
		// Move us to the next tile in the sequence

		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;

			Vector3 nextStep = new Vector3(X,Y,0);

			//transform.position = TileMap.instance.TileCoordToWorldCoord( tileX, tileY );	// Update our unity world position

			//Smooth movement
			//Vector3 end = new Vector3(tileX,tileY,transform.position.z);
			float step = speed * Time.deltaTime;
			//transform.position = Vector3.MoveTowards (transform.position, TileMap.instance.TileCoordToWorldCoord (tileX, tileY), step);
			transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
			if ((transform.position - nextStep).sqrMagnitude < float.Epsilon) {
				tileX = X;
				tileY = Y;
			}
		}
	}

}
