using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickableTile : Unit {

	public int tileIndex;
	//public TileMap map;

	void Start(){
		base.Start ();
	}

	protected virtual void OnMouseOver() {
		//print ("er");
		if (Input.GetMouseButtonDown (0)){ //right click to dig
			if (!EventSystem.current.IsPointerOverGameObject ()) {
				if (Vector3.Distance (this.transform.position, TileMap.instance.DemonLord.transform.position) < 1.1f) {
					StartCoroutine (TileMap.instance.dig (this.gameObject));
				} else {
					print ("Can only dig adjacent tiles!");
				}
			}
		}
	}

}
