using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
using UnityEngine.UI;
using UnityEditor;


public class Adventurer : Characters {

	public bool isLeader = false;
	public GameObject leader;
	public GameObject follower;
	public bool isTail=false;
	private List<GameObject> enemies;
	private Rigidbody2D rb2D;
	private int nodeIndex = 1;

	public bool foundDemonLord;
	public Vector3 startPosition;

	public float reachableRadius;

	public Vector2 prevTileChanged;
	public GameObject tileToChange;

	public GameObject healthUI;
	public GameObject scouting;
	public float sight_radius;

	public List<GameObject> tauntInContact;

	public int monster_layer;
	public int tile_layer;
	public int combined_layer;



	// Use this for initialization
	protected virtual void Start () {
		base.Start ();

		monster_layer = 1 << LayerMask.NameToLayer ("Monster");
		tile_layer = 1 << LayerMask.NameToLayer ("Tiles");
		combined_layer = tile_layer | monster_layer;


		//rb2D = GetComponent <Rigidbody2D> ();

		prevTileChanged = new Vector2 (0, 0);

		if (leader == null) {
			isLeader = true;
		}
		if (follower == null) {
			isTail = true;
		}
			
		startPosition = transform.position;
		foundDemonLord = false;

		scouting.GetComponent<CircleCollider2D> ().radius = sight_radius;

		//healthUI = GameObject.Find ("adventurerHealth"); //debugging.
		healthUI.SetActive(true);
		//SearchAndDestroy();
		StartCoroutine(SearchAndDestroy());
	}
		

	protected virtual void Update() {
		base.Update ();
		healthUI.GetComponent<Text> ().text = "Health: " + HP.ToString ();
	
		if (TileMap.instance.DemonLord.GetComponent<DemonLord>().dragger == this.gameObject
			&& new Vector2(tileX,tileY) == Game_Manager.instance.return_location) {
			//Game_Manager.instance.loadNextLevel ();
			Game_Manager.instance.reachedDestination = true;
		} 
	}


	protected virtual IEnumerator SearchAndDestroy(){
		while (true) {
			GameObject taunt = getClosestTaunt ();

			if (tauntInContact.Count !=0){
				yield return Attack (tauntInContact [0]);
			}
			else if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			}
			else if (taunt != null) {
				//yield return MoveTowardsGameObjectContactBased (taunt);
				//print ("one step towards taunt");
				yield return MoveOneStepTowardsGameObject(taunt);
			} else if (!Game_Manager.instance.demonLordFound) {
				yield return FindDemonLord ();
				//yield return MoveOneStepTowardsGameObject (TileMap.instance.DemonLord);
			} else if (Game_Manager.instance.demonLordFound && TileMap.instance.DemonLord.GetComponent<DemonLord>().dragger == this.gameObject) {
				yield return MoveOneStepTowards (Game_Manager.instance.return_location);
			}
			else if (Game_Manager.instance.demonLordFound && TileMap.instance.DemonLord.GetComponent<DemonLord>().dragger != this.gameObject){
				//yield return MoveOneStepTowardsGameObject (Game_Manager.instance.current_dragger);
				//print ("guard");
				yield return GuardianMode(Game_Manager.instance.current_dragger);
			}
			yield return null;
		}
	}


	/*
	protected virtual IEnumerator Destroy(){
		while (true) {
			if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			} else {
				yield return null;
			}
		}
	}

	protected void SearchAndDestroy(){
		StartCoroutine (Search ());
		StartCoroutine (Destroy ());
	}
	*/
		
	protected virtual IEnumerator FindDemonLord(){
		GameObject target = TileMap.instance.DemonLord;
		if (target != null) {
			Unit targetTileInfo = target.GetComponent<Unit> ();
			List<Node> path = TileMap.instance.returnPathGenerated (new Vector3 (tileX, tileY, 0),
				new Vector3 (targetTileInfo.tileX, targetTileInfo.tileY, 0));
			if (path != null) {
				int X = path [1].x;
				int Y = path [1].y;

				Vector3 nextStep = new Vector3 (X, Y, 0);
				if (X > tileX) {
					turnRight ();
				} else if (X < tileX) {
					turnLeft ();
				} else if (Y > tileY) {
					turnUp ();
				} else if (Y < tileY) {
					turnDown ();
				}
				while (!Game_Manager.instance.demonLordFound && enemiesInContact.Count ==0 ) {
					if ((transform.position - target.transform.position).sqrMagnitude < 0.5f) {
						UpdateDemonLordFoundStatus (true);
						TileMap.instance.DemonLord.GetComponent<DemonLord> ().dragger = this.gameObject;
						Game_Manager.instance.current_dragger = this.gameObject;
						break;
					}
					else if ((transform.position - nextStep).sqrMagnitude < float.Epsilon){
						tileX = X;
						tileY = Y;
						break;
					}
					else {
						transform.position = Vector3.MoveTowards (transform.position, nextStep, speed * Time.deltaTime);
						yield return null;
					}
				}
//				tileX = X;
//				tileY = Y;
			}
		} else {
			yield break;
		}
	}


	protected virtual IEnumerator GuardianMode(GameObject VIP){
		GameObject taunt = getClosestTaunt ();
		GameObject closestMonster = MonsterInSight ();
		if (enemiesInContact.Count != 0) {
			yield return Attack (enemiesInContact [0]);
		} else if (closestMonster != null) {
			yield return MoveOneStepTowardsGameObject (closestMonster);
			//Shoot(TileMap.instance.DemonLord);
		} else {
			yield return MoveOneStepTowardsGameObject (VIP);
		}
	}

	protected virtual GameObject MonsterInSight(){
		//print ("here");

		List<GameObject> monstersInRange = scouting.GetComponent<Scouting> ().monsters_in_range;
		List<GameObject> tauntsInRange = scouting.GetComponent<Scouting> ().taunts_in_range;
		if (monstersInRange.Count == 0) {
			//print ("no monsters");
			return null;
		}
		if (tauntsInRange.Count != 0) {
			return tauntsInRange [0];
		} else {
			return monstersInRange [0];
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
			if (Vector3.Distance (transform.position, enemy.transform.position) < (reachableRadius + 0.15f)) {
				toReturn.Add (enemy);
			}
		}
		return toReturn;
	}

	protected void OnCollisionEnter2D(Collision2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer && collisionInfo.gameObject.GetComponent<Monster>().taunt_distance !=0) {
			//print("jere");
			tauntInContact.Add (collisionInfo.gameObject);
			enemiesInContact.Add (collisionInfo.gameObject);
		}
		else if (collisionInfo.gameObject.layer == monster_layer) {
			enemiesInContact.Add (collisionInfo.gameObject);
		}
//		if (collisionInfo.gameObject.tag == "DemonLord") {
//			UpdateDemonLordFoundStatus (true);
//		}
	}

	protected void OnCollisionExit2D(Collision2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer && enemiesInContact.Contains(collisionInfo.gameObject)){
			//print("here");
			enemiesInContact.Remove (collisionInfo.gameObject);
		}
		if (collisionInfo.gameObject.layer == monster_layer && tauntInContact.Contains(collisionInfo.gameObject)){
			//print("here");
			tauntInContact.Remove (collisionInfo.gameObject);
		}
	}

//	protected void OnTriggerEnter2D(Collider2D collisionInfo){
//		if (collisionInfo.gameObject.tag == "DemonLord") {
//			UpdateDemonLordFoundStatus (true);
//		}
//	}



//	protected void followLeader(){
//		moveTowards (leader.transform.position);
//	}
//		

//	public void findDemonLord() {
//		Vector3 pos = new Vector3 (tileX, tileY, 0);
//		if (pos == TileMap.instance.DemonLord.transform.position) {
//			foundDemonLord = true;
//			TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = true;
//			Game_Manager.instance.demonLordFound = true;
//		}
//		moveTowards (TileMap.instance.DemonLord.transform.position);
//	}
//

//	public IEnumerator findDemonLord() {
//		Vector3 pos = new Vector3 (tileX, tileY, 0);
//		if (pos == TileMap.instance.DemonLord.transform.position) {
//			foundDemonLord = true;
//			TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = true;
//			Game_Manager.instance.demonLordFound = true;
//		}
//		yield return MoveOneStepTowardsGameObject (TileMap.instance.DemonLord);
//	}


//	public void followPathBack(){
//		moveTowardsWithIndicator (startPosition);
//	}

	/*
	protected override void moveTowards(Vector3 dest){
		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
		List<Node> path = TileMap.instance.returnPathGeneratedAdventurer (new Vector3(tileX,tileY,0),dest);
		// Move us to the next tile in the sequence

		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;

			if (X > tileX) {
				turnRight ();
			} else if (X < tileX) {
				turnLeft ();
			} else if (Y > tileY) {
				turnUp ();
			} else if (Y < tileY) {
				turnDown ();
			}


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

	protected void moveTowardsWithIndicator(Vector3 dest){
		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
		List<Node> path = TileMap.instance.returnPathGeneratedAdventurer (new Vector3(tileX,tileY,0),dest);
		// Move us to the next tile in the sequence

		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;

			if (X > tileX) {
				turnRight ();
			} else if (X < tileX) {
				turnLeft ();
			} else if (Y > tileY) {
				turnUp ();
			} else if (Y < tileY) {
				turnDown ();
			}

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
	*/

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

	protected virtual GameObject getClosestTaunt(){
		float shortestDistance = 10;
		GameObject closestMonster = null;
		foreach (GameObject monster in TileMap.instance.Taunts){
			float distance = Vector3.Distance (monster.transform.position, transform.position);
			if (distance < shortestDistance && distance < monster.GetComponent<Monster>().taunt_distance) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, monster.transform.position- transform.position,100,combined_layer);
				if (hit.transform.gameObject.GetComponent<Monster> () != null) { //this means that the adventure can actually see the taunt.
					shortestDistance = distance;
					closestMonster = monster.gameObject;
				}
			}
		}
		return closestMonster;
	}

	protected void OnDestroy(){
		if (EditorApplication.isPlaying) {
			if (follower != null) {
				follower.GetComponent<Adventurer> ().isLeader = true;
			}
			DemonLord script = TileMap.instance.DemonLord.GetComponent<DemonLord> ();
			if (script.dragger == this.gameObject) { //release the demonlord if dead
				UpdateDemonLordFoundStatus (false);
				script.dragger = null;
				Game_Manager.instance.current_dragger = null;
				script.tileX = tileX;
				script.tileY = tileY;
				//Game_Manager.instance.demonLordFound = false;
			}

			TileMap.instance.Adventurers.Remove (this.gameObject);
		}
	}

	protected void UpdateDemonLordFoundStatus(bool status){
		foundDemonLord = status;
		if (TileMap.instance.DemonLord!=null)
			TileMap.instance.DemonLord.GetComponent<DemonLord> ().beingDragged = status;
		Game_Manager.instance.demonLordFound = status;
	}



	protected void ChangeFrontTileCost(){

//		if (!(prevTileChanged.x == 0 && prevTileChanged.y == 0)) {
//			TileMap.instance.movement_costs [prevTileChanged.x, prevTileChanged.y]--;
//		} 

		if (facing_up) {
			if (prevTileChanged != CoordInFront ()) {
				TileMap.instance.movement_costs [(int)prevTileChanged.x, (int)prevTileChanged.y]--;
				prevTileChanged = new Vector2 (tileX, tileY + 1);
				TileMap.instance.movement_costs [tileX, tileY + 1]++;
			}
		}
		if (facing_down) {
			if (prevTileChanged != CoordInFront ()) {
				TileMap.instance.movement_costs [(int)prevTileChanged.x,(int) prevTileChanged.y]--;
				prevTileChanged = new Vector2 (tileX, tileY - 1);
				TileMap.instance.movement_costs [tileX, tileY - 1]++;
			}
		}
		if (facing_left) {
			if (prevTileChanged != CoordInFront ()) {
				TileMap.instance.movement_costs [(int)prevTileChanged.x, (int)prevTileChanged.y]--;
				prevTileChanged = new Vector2 (tileX - 1, tileY);
				TileMap.instance.movement_costs [tileX - 1, tileY]++;
			}
		}
		if (facing_right) {
			if (prevTileChanged != CoordInFront ()) {
				TileMap.instance.movement_costs [(int)prevTileChanged.x, (int)prevTileChanged.y]--;
				prevTileChanged = new Vector2 (tileX + 1, tileY);
				TileMap.instance.movement_costs [tileX + 1, tileY]++;
			}
		}
	}



	public Vector2 CoordInFront(){
		Vector2 coord = new Vector2(0,0);
		if (facing_up) {
			coord = new Vector2 (tileX, tileY + 1);
		}
		if (facing_down) {
			coord = new Vector2 (tileX, tileY - 1);
		}
		if (facing_left) {
			coord = new Vector2 (tileX -1 , tileY);
		}
		if (facing_right) {
			coord = new Vector2 (tileX + 1, tileY);
		}

		return coord;
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




