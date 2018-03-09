using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

	public List<GameObject> enemiesInShield;
	public GameObject owner;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer) {
			//print("jere");
			Monster script = collisionInfo.gameObject.GetComponent<Monster>();
			script.attack = script.attack * owner.GetComponent<Paladin> ().shield_damage_modifier;
			enemiesInShield.Add (collisionInfo.gameObject);
		}
	}

	void OnCollisionExit2D(Collision2D collisionInfo){
		int monster_layer = LayerMask.NameToLayer ("Monster");
		if (collisionInfo.gameObject.layer == monster_layer && enemiesInShield.Contains(collisionInfo.gameObject)){
			Monster script = collisionInfo.gameObject.GetComponent<Monster>();
			script.attack = script.attack / owner.GetComponent<Paladin> ().shield_damage_modifier;
			enemiesInShield.Remove (collisionInfo.gameObject);
		}
	}
}
