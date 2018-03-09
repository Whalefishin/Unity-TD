using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Monster {

	int slime_count;

	// Use this for initialization
	void Start () {
		slime_count = 0;
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
//			if (other.gameObject.tag == "SmallPumpkin") {
//				base.evolveTo (other.gameObject, evolution_tree [0]);
//			}
			if (other.gameObject.tag == "Slime") {
				slime_count++;
				base.destroy (other.gameObject);
				if (slime_count == evolution_cost[0]){
					base.destroy (this.gameObject);
					GameObject.Instantiate (evolution_tree[0], transform.position, Quaternion.identity);	
				}
			}
		}
	}

}
