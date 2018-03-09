using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPumpkin : Monster {
	// Use this for initialization
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
			if (other.gameObject.tag == "Slime") {
				evolution_dict [evolution_tree[0]]++;
				base.destroy (other.gameObject);
				if (evolution_dict [evolution_tree[0]] == evolution_cost[0]){
					base.destroy (this.gameObject);
					GameObject.Instantiate (evolution_tree[0], transform.position, Quaternion.identity);	
				}
				else if (evolution_dict [evolution_tree[2]] == 1 && evolution_dict [evolution_tree[0]]==1){
					base.destroy (this.gameObject);
					GameObject.Instantiate (evolution_tree[1], transform.position, Quaternion.identity);	
				}

			} 
			else if (other.gameObject.tag == "SmallPumpkin") {
				//base.evolveTo (other.gameObject, evolution_tree [1]);
				TileMap.instance.fusionWithIdenticalObject(this.gameObject,evolution_tree[1],this.transform.position);
			}

			else if (other.gameObject.tag == "KuroSlime"){
				evolution_dict [evolution_tree[2]]++;
				base.destroy (other.gameObject);
				if (evolution_dict [evolution_tree[2]] == evolution_cost[2]){
					base.destroy (this.gameObject);
					GameObject.Instantiate (evolution_tree[2], transform.position, Quaternion.identity);	
				}
				else if (evolution_dict [evolution_tree[2]] == 1 && evolution_dict [evolution_tree[0]]==1){
					base.destroy (this.gameObject);
					GameObject.Instantiate (evolution_tree[1], transform.position, Quaternion.identity);	
				}
			}
		}
	}
}
