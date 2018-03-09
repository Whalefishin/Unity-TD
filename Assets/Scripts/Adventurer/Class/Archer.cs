using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Adventurer {

	//public float sight_radius;
	//public GameObject scouting;
	public GameObject arrow;
	public GameObject firing_spot;
	public float firing_rate;
	public float arrow_speed;
	public float arrow_attack;

	void Start () {
		base.Start ();
		//scouting.GetComponent<CircleCollider2D> ().radius = sight_radius;
		//Shoot(TileMap.instance.DemonLord);
	}

	// Update is called once per frame
	void Update () {
		base.Update ();
		Vector2 targetDir = TileMap.instance.DemonLord.transform.position - transform.position;
		float angle = Vector2.Angle(targetDir, transform.right);
	}

	protected override IEnumerator SearchAndDestroy ()
	{
		while (true) {
			GameObject taunt = getClosestTaunt ();
			GameObject closestMonster = MonsterInSight ();
			if (tauntInContact.Count !=0){
				yield return Attack (tauntInContact [0]);
			}
			else if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			}
			else if (closestMonster != null){
				yield return Shoot (closestMonster);
				//Shoot(TileMap.instance.DemonLord);
			}
			else if (taunt != null) {
				yield return MoveOneStepTowardsGameObject (taunt);
			} else if (!Game_Manager.instance.demonLordFound) {
				yield return FindDemonLord ();
				//yield return MoveOneStepTowardsGameObject (TileMap.instance.DemonLord);
			} else if (Game_Manager.instance.demonLordFound && TileMap.instance.DemonLord.GetComponent<DemonLord>().dragger == this.gameObject) {
				yield return MoveOneStepTowards (Game_Manager.instance.return_location);
			}
			else if (Game_Manager.instance.demonLordFound && TileMap.instance.DemonLord.GetComponent<DemonLord>().dragger != this.gameObject){
				//yield return MoveOneStepTowardsGameObject (Game_Manager.instance.current_dragger);
				yield return GuardianMode(Game_Manager.instance.current_dragger);
			}
			yield return null;
		}
	}

	protected override GameObject MonsterInSight(){
		List<GameObject> monstersInRange = scouting.GetComponent<Scouting> ().monsters_in_range;
		List<GameObject> tauntsInRange = scouting.GetComponent<Scouting> ().taunts_in_range;
		if (monstersInRange.Count == 0) {
			//print ("no monsters");
			return null;
		}

//		int monster_layer = 1 << LayerMask.NameToLayer ("Monster");
//		int tile_layer = 1 << LayerMask.NameToLayer ("Tiles");
//		int combined_layer = tile_layer | monster_layer;

		foreach(GameObject monster in tauntsInRange){
			RaycastHit2D hit = Physics2D.Raycast(transform.position, monster.transform.position- transform.position,100,combined_layer);
			//RaycastHit2D hit = Physics2D.CircleCast(transform.position, monster.transform.position- transform.position,100,combined_layer);
			//Debug.draw (transform.position, monster.transform.position - transform.position,Color.black,5, false);
			if (hit.transform.gameObject.GetComponent<Monster> () != null) {
				return monster;
			}
		}

		foreach(GameObject monster in monstersInRange){
			//RaycastHit2D hitInfo = new RaycastHit2D ();
			//bool hit = Physics2D.Raycast (transform.position, monster.transform.position, out hitInfo);
			RaycastHit2D hit = Physics2D.Raycast(transform.position, monster.transform.position - transform.position,100,combined_layer);
			//Debug.DrawRay (transform.position, monster.transform.position,Color.black,5, false);
			//Debug.DrawRay (transform.position, monster.transform.position - transform.position,Color.black,5, false);
			//			print (hit.transform.gameObject);
			//			print (hit.transform.gameObject.transform.position.x);
			//			print (hit.transform.gameObject.transform.position.y);
			if (hit.transform.gameObject.GetComponent<Monster> () != null) {
				return monster;
			}
		}
		return null;
	}

	protected virtual IEnumerator GuardianMode(GameObject VIP){
		GameObject taunt = getClosestTaunt ();
		GameObject closestMonster = MonsterInSight ();
		if (tauntInContact.Count !=0){
			yield return Attack (tauntInContact [0]);
		}
		else if (enemiesInContact.Count != 0) {
			yield return Attack (enemiesInContact [0]);
		} else if (closestMonster != null) {
			//yield return MoveOneStepTowardsGameObject (closestMonster);
			//Shoot(TileMap.instance.DemonLord);
			yield return Shoot (closestMonster);
		} else {
			yield return MoveOneStepTowardsGameObject (VIP);
		}
	}

//	public GameObject MonsterInSight(){
//		//print ("here");
//
//		List<GameObject> monstersInRange = scouting.GetComponent<Scouting> ().monsters_in_range;
//		List<GameObject> tauntsInRange = scouting.GetComponent<Scouting> ().taunts_in_range;
//		if (monstersInRange.Count == 0) {
//			//print ("no monsters");
//			return null;
//		}
//
//		int monster_layer = 1 << LayerMask.NameToLayer ("Monster");
//		int tile_layer = 1 << LayerMask.NameToLayer ("Tiles");
//		int combined_layer = tile_layer | monster_layer;
//
//		foreach(GameObject monster in tauntsInRange){
//			//RaycastHit2D hitInfo = new RaycastHit2D ();
//			//bool hit = Physics2D.Raycast (transform.position, monster.transform.position, out hitInfo);
//			RaycastHit2D hit = Physics2D.Raycast(transform.position, monster.transform.position- transform.position,100,combined_layer);
//			if (hit.transform.gameObject.GetComponent<Monster> () != null) {
//				return monster;
//			}
//		}
//
//		foreach(GameObject monster in monstersInRange){
//			//RaycastHit2D hitInfo = new RaycastHit2D ();
//			//bool hit = Physics2D.Raycast (transform.position, monster.transform.position, out hitInfo);
//			RaycastHit2D hit = Physics2D.Raycast(transform.position, monster.transform.position - transform.position,100,combined_layer);
//			//Debug.DrawRay (transform.position, monster.transform.position,Color.black,5, false);
//			//Debug.DrawRay (transform.position, monster.transform.position - transform.position,Color.black,5, false);
////			print (hit.transform.gameObject);
////			print (hit.transform.gameObject.transform.position.x);
////			print (hit.transform.gameObject.transform.position.y);
//			if (hit.transform.gameObject.GetComponent<Monster> () != null) {
//				return monster;
//			}
//		}
//		return null;
//	}

	public IEnumerator Shoot(GameObject monster){
		Vector2 targetDir = monster.transform.position - transform.position;
		float angle = Vector2.Angle(targetDir, transform.right);
		//float angle = Vector2.Angle (transform.position, TileMap.instance.DemonLord.transform.position);

		/*
		if (targetDir.y >= 0) { //if the target is above the archer
			if (angle >= 45 && angle <= 135) {
				turnUp ();
			} else if (angle <= 45) {
				turnRight ();
			} else if (angle >= 135) {
				turnLeft ();
			}
			Quaternion rotation = Quaternion.Euler (angle, 0, 0);
			GameObject.Instantiate (arrow, firing_spot.transform.position, rotation);
		} else {//if the target is below the archer
			if (angle >= 45 && angle <= 135) {
				turnDown ();
			} else if (angle <= 45) {
				turnRight ();
			} else if (angle >= 135) {
				turnLeft ();
			}
			Quaternion rotation = Quaternion.Euler (360-angle, 0, 0);
			GameObject.Instantiate (arrow, firing_spot.transform.position, rotation);
		}
		*/

		//Quaternion rotation = Quaternion.LookRotation (targetDir);

//		Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
//		diff.Normalize();
//
//		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
//		transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

		GameObject arrow_ = GameObject.Instantiate (arrow, firing_spot.transform.position, Quaternion.identity);
		arrow_.transform.right = targetDir;
		Arrow script = arrow_.GetComponent<Arrow> ();
		script.direction = monster.transform.position;
		script.speed = arrow_speed;
		script.attack = arrow_attack;
		yield return new WaitForSeconds (firing_rate);
		//arrow.GetComponent<Arrow> ().rb2D.AddForce (targetDir_normal,ForceMode2D.Impulse);
		//print (angle);
	}
}
