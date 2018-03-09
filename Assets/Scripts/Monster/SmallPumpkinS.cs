using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPumpkinS : Monster {
	// Use this for initialization
	public GameObject pumpkinFinder;

	void Start () {
		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		base.Update ();	
	}

	void OnCollisionEnter2D(Collision2D other){
		base.OnCollisionEnter2D (other);
		Monster script = other.gameObject.GetComponent<Monster> ();
		if (script != null) {
			if (other.gameObject.tag == "SmallPumpkinS") {
				//base.evolveTo (other.gameObject, evolution_tree [0]);
				TileMap.instance.fusionWithIdenticalObject(this.gameObject,evolution_tree[0],this.transform.position);
			}
		}
	}

	protected override IEnumerator SearchAndDestroy(){
		float waiting_time = 0.2f;
		while (true) {
			bool foundAdventurer = checkIfAdventurerInSight ();
			GameObject nearestPumpkin = FindPumpkin();
			if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			}
			else if (foundAdventurer) {
				GameObject closestAdventurer = getClosestAdventurer ();
				//yield return MoveOneStepTowardsGameObject (closestAdventurer);
				yield return MoveTowardsGameObjectContactBased(closestAdventurer);
			}
			else if (nearestPumpkin != null) {
				yield return MoveOneStepTowardsGameObject (nearestPumpkin);
			}
			else{
				yield return new WaitForSeconds (waiting_time);
				yield return randomWalk ();
			}
		}
	}

//	private GameObject FindPumpkin(){
//		foreach (GameObject tank in pumpkinFinder.GetComponent<PumpkinFinder>().taunts_in_range) {
//			if (tank.tag == "SmallPumpkinS") {
//				RaycastHit2D hit = Physics2D.Raycast(transform.position, tank.transform.position- transform.position,
//					100,Game_Manager.instance.monster_tile_layer);
//				print (hit.transform.gameObject);
//				if (hit.transform.gameObject.GetComponent<Monster> () != null) {
//					return hit.transform.gameObject;
//				}
//			}
//		}
//		return null;
//	}

	GameObject FindPumpkin(){
		if (pumpkinFinder.GetComponent<PumpkinFinder> ().taunts_in_range.Count != 0) {
			foreach (GameObject tank in pumpkinFinder.GetComponent<PumpkinFinder>().taunts_in_range) {
				if (tank.tag == "SmallPumpkinS") {
					return tank;
				}
			}
		}
		return null;
	}
}
