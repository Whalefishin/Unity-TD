using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster {

	private int split_count;
	public int split_limit;
	public float split_time;
	public float fusion_strength;
	public GameObject marker;

	// Use this for initialization
	void Start () {
		base.Start ();
		split_count = 0;
		StartCoroutine(Split());
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		//Split ();
	}

	void OnCollisionEnter2D(Collision2D other){
		base.OnCollisionEnter2D (other);
		if (other.gameObject.tag == "KuroSlime") {
			base.evolveTo (other.gameObject, evolution_tree [0]);
		}
	}

	IEnumerator Split(){
		while (split_count < split_limit){
			yield return new WaitForSeconds (split_time);
			GameObject Slime = Instantiate (Game_Manager.instance.monsterTypes [0].monsterPrefab, 
				                  transform.position, Quaternion.identity);
			split_count++;
		}
	}
}
