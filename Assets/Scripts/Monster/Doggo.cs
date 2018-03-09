using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggo : Monster {

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
	}
}
