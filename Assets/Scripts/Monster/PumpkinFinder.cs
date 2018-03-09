using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinFinder : Scouting {

	public GameObject owner;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void OnTriggerEnter2D(Collider2D collisionInfo){
		if (collisionInfo.gameObject!=owner && collisionInfo.gameObject.layer == Game_Manager.instance.monster_layer) {
			if (collisionInfo.gameObject.GetComponent<Monster>().taunt_distance !=0) {
				taunts_in_range.Add (collisionInfo.gameObject);
			}
		}
	}

	protected override void OnTriggerExit2D(Collider2D collisionInfo){
		if (collisionInfo.gameObject.layer == Game_Manager.instance.monster_layer && taunts_in_range.Contains(collisionInfo.gameObject)){
			taunts_in_range.Remove (collisionInfo.gameObject);
		}
	}
}
