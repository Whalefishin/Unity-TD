using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KuroSlime : Monster {

	private int split_count;
	public int split_limit;
	public float split_time;
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
		Split ();
	}

//	void Split(){
//		// a slime can only split a certain # of times.
//		if (split_count < split_limit) {
//			if (Time.time - time_created >= split_time) {
//				GameObject Slime = Instantiate (Game_Manager.instance.monsterTypes [1].monsterPrefab, 
//					transform.position, Quaternion.identity);
//				time_created = Time.time;
//				split_count++;
//			}
//		}
//	}

	IEnumerator Split(){
		while (split_count < split_limit){
			yield return new WaitForSeconds (split_time);
			GameObject Slime = Instantiate (Game_Manager.instance.monsterTypes [1].monsterPrefab, 
				transform.position, Quaternion.identity);
			split_count++;
		}
	}
}
