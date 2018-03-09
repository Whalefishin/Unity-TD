using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : Characters {


	Rigidbody2D rb2D;	
	public float spawn_time;
	public int[] gate_cost;
	public float alertRadius;
	public string name;
	public List<GameObject> evolution_tree;
	public List<int> evolution_cost;
	public Dictionary<GameObject,int> evolution_dict;
	public float taunt_distance;
	//public List<GameObject> enemiesInContact;
	public bool adventurer_in_contact;
	public bool fused;


	protected virtual void Start () {
		base.Start ();
		//speed = 1f;
		temp_time = 0f;
		fused = false;
//		tileX = (int)transform.position.x;
//		tileY = (int)transform.position.y;
		rb2D = GetComponent<Rigidbody2D> ();
		//nextDest = generateRandomDest ();
		nextDest = new Vector3(Mathf.Infinity,Mathf.Infinity,0);

		//Add itself into objectManager
		TileMap.instance.Monsters.Add(this.gameObject);
		if (taunt_distance > 0) {
			TileMap.instance.Taunts.Add (this.gameObject);
		}
		evolution_dict = new Dictionary<GameObject,int> ();
		foreach (GameObject monster in evolution_tree) {
			evolution_dict.Add (monster, 0);
		}
		adventurer_in_contact = false;

		//SearchAndDestory();
		StartCoroutine(SearchAndDestroy());
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		base.Update ();
//		bool foundAdventurer = checkIfAdventurerInSight ();
//		if (!foundAdventurer) {
//			randomWalk ();
//		} else{
//			GameObject closestAdventurer = getClosestAdventurer ();
//			if (Vector3.Distance (transform.position, closestAdventurer.transform.position) < 1 + 0.15f) {
//				//StartCoroutine(Attack(closestAdventurer,attack_rate));
//				Attack (closestAdventurer);
//			} else {
//				approachAdventurer (closestAdventurer);
//			}
//		}
	}

	protected virtual IEnumerator SearchAndDestroy(){
		float waiting_time = 0.2f;
		while (true) {
			bool foundAdventurer = checkIfAdventurerInSight ();

			if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			}
			else if (foundAdventurer) {
				GameObject closestAdventurer = getClosestAdventurer ();
				//yield return MoveOneStepTowardsGameObject (closestAdventurer);
				yield return MoveTowardsGameObjectContactBased(closestAdventurer);
			}
			else{
				yield return new WaitForSeconds (waiting_time);
				yield return randomWalk ();
			}
		}
	}

//	protected virtual IEnumerator Destory(){
//		while (true) {
//			if (enemiesInContact.Count != 0) {
//				yield return Attack (enemiesInContact [0]);
//			} else {
//				yield return null;
//			}
//		}
//	}
//
//	protected virtual void SearchAndDestory(){
//		StartCoroutine (Search ());
//		StartCoroutine (Destory ());
//	}

	protected virtual void OnCollisionEnter2D(Collision2D collisionInfo){
		if (collisionInfo.gameObject.CompareTag("Adventurer")){
			enemiesInContact.Add (collisionInfo.gameObject);
		}
	}

	protected virtual void OnCollisionExit2D(Collision2D collisionInfo){
		if (collisionInfo.gameObject.CompareTag("Adventurer") && enemiesInContact.Contains(collisionInfo.gameObject)){
			enemiesInContact.Remove (collisionInfo.gameObject);
		}
	}



	protected virtual bool checkIfAdventurerInSight(){
		foreach (GameObject dude in TileMap.instance.Adventurers){
			if (dude != null && dude.activeSelf == true) {
				if (Vector3.Distance (dude.transform.position, transform.position) < alertRadius) {
					RaycastHit2D hit = Physics2D.Raycast(transform.position, dude.transform.position- transform.position,
						100,Game_Manager.instance.adventurer_tile_layer);
					if (hit.transform.gameObject.GetComponent<Adventurer> () != null) {
						nextDest = new Vector3 (Mathf.Infinity, Mathf.Infinity, 0);
						return true;
					}
				}
			}
		}
		return false;
	}
		

	protected virtual GameObject getClosestAdventurer(){
		float shortestDistance = Mathf.Infinity;
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

//	protected virtual void approachAdventurer(GameObject dude){
//		moveTowards(new Vector3(dude.GetComponent<Characters>().tileX,dude.GetComponent<Characters>().tileY,0));
//		//moveTowards (dude.transform.position);
//	}
		

	protected void OnDestroy(){
		TileMap.instance.Monsters.Remove (this.gameObject);
		if (taunt_distance > 0) {
			TileMap.instance.Taunts.Remove (this.gameObject);
		}
	}

	protected void evolveTo(GameObject toDestroy, GameObject toCreate){

		Monster script = toDestroy.GetComponent<Monster> ();

		if (script != null && !script.fused) {
			GameObject.Instantiate (toCreate, transform.position, Quaternion.identity);	
			toDestroy.GetComponent<Monster> ().fused = true;
			Destroy (toDestroy);
			Destroy (gameObject);
		}
	}

	//	protected override void Attack(GameObject toAttack){
	//		if (toAttack.activeSelf == true) {
	//			if (Time.time - temp_time > attack_rate) {
	//				float deduction = Mathf.Max (attack - toAttack.GetComponent<Characters> ().defense, 0);
	//				Vector2 adventurer_front = toAttack.GetComponent<Adventurer> ().CoordInFront ();
	////				print (adventurer_front.x);
	////				print (adventurer_front.y);
	////				print ("monster position:");
	////				print (tileX);
	////				print (tileY);
	//				if (adventurer_front.x == tileX && adventurer_front.y == tileY) {
	//					toAttack.GetComponent<Characters> ().HP -= (deduction * toAttack.GetComponent<Adventurer> ().shield_def_modifier);
	//				} else {
	//					toAttack.GetComponent<Characters> ().HP -= deduction;
	//				}
	//				//toAttack.GetComponent<SpriteRenderer> ().color = new Color (1*deduction/attack_max_color_change, 0, 0);
	//				temp_time = Time.time;
	//			}
	//		}
	//	}


}
