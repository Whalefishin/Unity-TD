using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scouting : MonoBehaviour {

	public List<GameObject> monsters_in_range;
	public List<GameObject> taunts_in_range;

	// Use this for initialization
	void Start () {
		//this.gameObject.GetComponent<CircleCollider2D> ().radius = scout_radius;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected virtual void OnTriggerEnter2D(Collider2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer) {
			monsters_in_range.Add (collisionInfo.gameObject);
			if (collisionInfo.gameObject.GetComponent<Monster>().taunt_distance !=0) {
				taunts_in_range.Add (collisionInfo.gameObject);
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer && monsters_in_range.Contains(collisionInfo.gameObject)){
			monsters_in_range.Remove (collisionInfo.gameObject);
		}
		if (collisionInfo.gameObject.layer == monster_layer && taunts_in_range.Contains(collisionInfo.gameObject)){
			taunts_in_range.Remove (collisionInfo.gameObject);
		}
	}

}
