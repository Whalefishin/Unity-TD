using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : Characters {

//	int tileX;
//	int tileY;
	Rigidbody2D rb2D;	
	//Vector3 nextDest;

	public float spawn_time;
	public int[] gate_cost;
	public float alertRadius;

//	public float temp_time;
//
//	public float HP;
//	public float MP;
//	public float attack;
//	public float attack_rate;
//	public float defense;
//	public float speed;


	// Use this for initialization
	protected virtual void Start () {
		base.Start ();
		speed = 1f;
		temp_time = 0f;
//		tileX = (int)transform.position.x;
//		tileY = (int)transform.position.y;
		rb2D = GetComponent<Rigidbody2D> ();
		nextDest = generateRandomDest ();

		//Add itself into objectManager
		TileMap.instance.Monsters.Add(this.gameObject);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		base.Update ();
		bool foundAdventurer = checkIfAdventurerInSight ();
		if (!foundAdventurer) {
			randomWalk ();
		} else{
			GameObject closestAdventurer = getClosestAdventurer ();
			approachAdventurer (closestAdventurer);
			if (Vector3.Distance(transform.position,closestAdventurer.transform.position) < 1+float.Epsilon){
				//StartCoroutine(Attack(closestAdventurer,attack_rate));
				Attack(closestAdventurer);
			}
		}
	}



	protected virtual bool checkIfAdventurerInSight(){
		foreach (GameObject dude in TileMap.instance.Adventurers){
			if (dude != null) {
				if (Vector3.Distance (dude.transform.position, transform.position) < alertRadius) {
					return true;
				}
			}
		}
		return false;
	}

	protected virtual GameObject getClosestAdventurer(){
		float shortestDistance = 10;
		GameObject closestGuy = null;
		foreach (GameObject dude in TileMap.instance.Adventurers){
			float distance = Vector3.Distance (dude.transform.position, transform.position);
			if (distance < shortestDistance) {
				shortestDistance = distance;
				closestGuy = dude.gameObject;
			}
		}
		return closestGuy;
	}

	protected virtual void approachAdventurer(GameObject dude){
		moveTowards (dude.transform.position);
	}
		

	protected void OnDestroy(){
		TileMap.instance.Monsters.Remove (this.gameObject);
	}
					
//	protected void moveTowards(Vector3 dest){
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
