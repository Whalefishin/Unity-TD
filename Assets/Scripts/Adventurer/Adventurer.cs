using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 		

public class Adventurer : Characters {

	public bool isLeader = false;
	public GameObject leader;
	public GameObject follower;
	public bool isTail=false;
	private List<GameObject> enemies;

//	public int tileX;
//	public int tileY;

	public List<Node> pathToDemonLord = null;
	private Vector3 demonLordPos;

//	public float speed;
	private Rigidbody2D rb2D;
	private int nodeIndex = 1;

	public bool foundDemonLord;
	public Vector3 startPosition;

	public float reachableRadius;
//	public float temp_time;
//
//	public float HP;
//	public float MP;
//	public float attack;
//	public float attack_rate;
//	public float defense;



	// Use this for initialization
	protected virtual void Start () {
		base.Start ();
//		tileX = (int)transform.position.x;
//		tileY = (int)transform.position.y;

		rb2D = GetComponent <Rigidbody2D> ();

		if (leader == null) {
			isLeader = true;
		}
		if (follower == null) {
			isTail = true;
		}

		demonLordPos = TileMap.instance.demonLordPos;
		startPosition = transform.position;
		foundDemonLord = false;
		pathToDemonLord = TileMap.instance.returnPathGenerated (startPosition,demonLordPos);
	}
		

	protected virtual void Update() {
		base.Update ();
		//checkIfDead ();
		
		//List <GameObject> enemiesAround = getEnemiesAround();
		List <GameObject> enemiesReachable = getEnemiesReachable ();

//		if (enemiesAround.Count != 0) {
//			if (enemiesReachable.Count != 0) {
//				//print("here");
//				Attack (enemiesReachable[0]);
//			} else {
//				//move towards a random enemy found
//				Vector3 enemyLocation = enemiesAround [Random.Range (0, enemiesAround.Count)].transform.position;
//				moveTowards (enemyLocation);
//			}
//		} else if (isLeader) {
//			if (!foundDemonLord) {
//				findDemonLord ();
//			} else if (foundDemonLord) {
//				followPathBack ();
//			}
//		} else if (!isLeader) {
//			followLeader ();
//		}

		if (enemiesReachable.Count != 0) {
			Attack (enemiesReachable[0]);
		}
		else if (isLeader) {
				if (!foundDemonLord) {
					findDemonLord ();
				} else if (foundDemonLord) {
					followPathBack ();
				}
			} else if (!isLeader) {
				followLeader ();
			}

		if (foundDemonLord && new Vector3(tileX,tileY,0) == startPosition) {
			//Game_Manager.instance.loadNextLevel ();
			Game_Manager.instance.reachedDestination = true;
		} 
	}


	protected virtual List<GameObject> getEnemiesAround(){
		if (TileMap.instance.Monsters != null)
			return TileMap.instance.Monsters;
		return null;
	}

	protected virtual List<GameObject> getEnemiesReachable(){
		List<GameObject> toReturn = new List<GameObject>();
		foreach (GameObject enemy in TileMap.instance.Monsters) {
			if (Vector3.Distance (transform.position, enemy.transform.position) < (reachableRadius + float.Epsilon)) {
				toReturn.Add (enemy);
			}
		}
		return toReturn;
	}


	protected void followLeader(){
		moveTowards (leader.transform.position);
	}
		

	public void findDemonLord() {
		Vector3 pos = new Vector3 (tileX, tileY, 0);
		if (pos == TileMap.instance.DemonLord.transform.position) {
			foundDemonLord = true;
			TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = true;
			Game_Manager.instance.demonLordFound = true;
		}
		moveTowards (TileMap.instance.DemonLord.transform.position);
	}

	public void followPathBack(){
		moveTowardsWithIndicator (startPosition);
	}

	protected void moveTowardsWithIndicator(Vector3 dest){
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
				//TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = true;
			}
		}
	}

	protected virtual GameObject getClosestMonster(){
		float shortestDistance = 10;
		GameObject closestMonster = null;
		foreach (GameObject monster in TileMap.instance.Monsters){
			float distance = Vector3.Distance (monster.transform.position, transform.position);
			if (distance < shortestDistance) {
				shortestDistance = distance;
				closestMonster = monster.gameObject;
			}
		}
		return closestMonster;
	}

	protected void OnDestroy(){
		if (follower != null) {
			follower.GetComponent<Adventurer> ().isLeader = true;
		}
		TileMap.instance.Adventurers.Remove (this.gameObject);
	}

	//	protected virtual void Attack(GameObject monster){
	//		if (Time.time - temp_time > attack_rate) {
	//			monster.GetComponent<Monster> ().HP -= (attack - monster.GetComponent<Monster> ().defense);
	//			temp_time = Time.time;
	//		}
	//	}

	//	protected virtual void checkIfDead(){
	//		if (HP == 0) {
	//			Destroy (this.gameObject);
	//		}
	//	}



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

//	protected void destroy(GameObject toDestroy){
//		Destroy (toDestroy);
//	}
//		

	/*
	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	*/

	//
	//	public void findDemonLord() {
	//		if(nodeIndex == pathToDemonLord.Count){
	//			nodeIndex--;
	//			foundDemonLord = true;
	//			Game_Manager.instance.demonLordFound = true;
	//			TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = true;
	//			return;
	//		}
	//
	//
	//		// Get cost from current tile to next tile
	//		//remainingMovement -= TileMap.instance.CostToEnterTile(pathToDemonLord[0].x, pathToDemonLord[0].y, pathToDemonLord[1].x, pathToDemonLord[1].y );
	//
	//		// Move us to the next tile in the sequence
	//		tileX = pathToDemonLord[nodeIndex].x;
	//		tileY = pathToDemonLord[nodeIndex].y;
	//
	//		//Smooth movement
	//		Vector3 end = new Vector3(tileX,tileY,transform.position.z);
	//
	//		//rotational stuff. Buggy.
	//		/*
	//		if (end.x - transform.position.x < 0.1) {
	//			Vector3 theScale = transform.localScale;
	//			theScale.x = -1;
	//			transform.localScale = theScale;	
	//		} else if (end.x - transform.position.x > 0.1) {
	//			Vector3 theScale = transform.localScale;
	//			theScale.x = 1;
	//			transform.localScale = theScale;
	//		}
	//		*/
	//		float step = speed * Time.deltaTime;
	//		transform.position = Vector3.MoveTowards(transform.position, TileMap.instance.TileCoordToWorldCoord( tileX, tileY ), step);
	//		//rb2D.MovePosition (newPos);
	//
	//		// Remove the old "current" tile
	//		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
	//		if (sqrRemainingDistance < 0.001f) {
	//			nodeIndex++;
	//		}
	//	}
	//
	//
	//	void followPathBack(){
	//		if(nodeIndex == -1){
	//			return;
	//		}
	//
	//		// Move us to the next tile in the sequence
	//		tileX = pathToDemonLord[nodeIndex].x;
	//		tileY = pathToDemonLord[nodeIndex].y;
	//
	//		//transform.position = TileMap.instance.TileCoordToWorldCoord( tileX, tileY );	// Update our unity world position
	//
	//		//Smooth movement
	//		Vector3 end = new Vector3(tileX,tileY,transform.position.z);
	//
	//		//print (transform.position);
	//		//print (TileMap.instance.TileCoordToWorldCoord (tileX, tileY));
	//		float step = speed * Time.deltaTime;
	//		transform.position = Vector3.MoveTowards(transform.position, TileMap.instance.TileCoordToWorldCoord( tileX, tileY ), step);
	//
	//		// Remove the old "current" tile
	//		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
	//		if (sqrRemainingDistance < 0.001f) {
	//			nodeIndex--;
	//		}
	//	}
}




