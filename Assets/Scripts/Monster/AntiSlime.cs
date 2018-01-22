using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiSlime : Monster {

	private float time_created;
	private int split_count;
	public int split_limit;
	public float split_time;

	// Use this for initialization
	void Start () {
		time_created = Time.time;
		base.Start ();
//		base.speed = 1f;
		split_count = 0;
//		split_limit = 1;
//		split_time = 10f;
//		HP = 2;
//		MP = 0;
//		attack = 1;
//		defense = 0;
	}

	// Update is called once per frame
	void Update () {
		base.Update ();
		Split ();
	}

	void OnCollisionEnter2D(Collision2D other){
//		if (other.gameObject.tag == "Slime") {
//			base.destroy (gameObject);
//			base.destroy (other.gameObject);
//			//do sth else here
//		}
//		if (other.gameObject.tag == "Adventurer") {
//			Adventurer script = other.gameObject.GetComponent<Adventurer> ();
//			//flat reduction
//			script.HP -= (attack - script.defense);
//		}
	}

	void Split(){
		// a slime can only split a certain # of times.
		if (split_count < split_limit) {
			if (Time.time - time_created >= split_time) {
				GameObject Slime = Instantiate (Game_Manager.instance.monsterTypes [1].monsterPrefab, 
					transform.position, Quaternion.identity);
				time_created = Time.time;
				split_count++;
			}
		}
	}
}
