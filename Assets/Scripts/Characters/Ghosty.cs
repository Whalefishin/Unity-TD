using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghosty : Characters {

	// Use this for initialization
	void Start () {
		HP = Mathf.Infinity;
		speed = 2f;
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
		moveTowards (TileMap.instance.DemonLord.GetComponent<DemonLord>().previousPosition);
	}
}
