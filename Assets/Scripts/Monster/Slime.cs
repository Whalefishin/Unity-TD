using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster {

	private float time_created;
	private int split_count;
	public int split_limit;
	public float split_time;
	public float fusion_strength;

	// Use this for initialization
	void Start () {
		time_created = Time.time;
		base.Start ();
//		base.speed = 1f;
		split_count = 0;
//		split_limit = 1;
//		split_time = 10f;

//		HP = 3f;
//		MP = 0;
//		attack = 1f;
//		attack_rate = 1f;
//		defense = 0;
//		alertRadius = 2f;
//		fusion_strength = 3f;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		Split ();
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag == "AntiSlime") {
			base.destroy (gameObject);
			base.destroy (other.gameObject);
			//increment dark power
			Game_Manager.instance.increaseDarkPower (fusion_strength);
		}
	}

	void Split(){
		// a slime can only split a certain # of times.
		if (split_count < split_limit) {
			if (Time.time - time_created >= split_time) {
				GameObject Slime = Instantiate (Game_Manager.instance.monsterTypes[0].monsterPrefab, 
					transform.position, Quaternion.identity);
				time_created = Time.time;
				split_count++;
			}
		}
	}
}
