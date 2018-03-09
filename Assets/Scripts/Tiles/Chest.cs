using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ClickableTile {

	public int treasure_count;

	// Use this for initialization
	void Start () {
		base.Start ();
	}

//	void OnTriggerEnter2D(){
//		Game_Manager.instance.tileNumbers [0] += treasure_count;
//		Destroy (this.gameObject);
//	}

	protected override void OnMouseOver ()
	{
		if (Input.GetMouseButtonDown (1)){ //right click to dig
			if (Vector3.Distance (this.transform.position, TileMap.instance.DemonLord.transform.position) < 1.1f) {
				StartCoroutine(TileMap.instance.OpenChest(this.gameObject));
			} else {
				print ("Not Close Enough to the Chest!");
			}
		}
	}

}
