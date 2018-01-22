using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public int tileIndex;
	//public TileMap map;

	void Start(){
		tileX = (int)transform.position.x;
		tileY = (int)transform.position.y;
	}

	void OnMouseOver() {
		if (Input.GetMouseButtonDown (1)){
			if (Vector3.Distance (this.transform.position, TileMap.instance.DemonLord.transform.position) < 1.1f) {
				if (TileMap.instance.dig (tileX, tileY)) {
					Destroy (gameObject);
					if (tileIndex == 1) {
						Game_Manager.instance.tileNumbers [0]++;
					}
					if (tileIndex == 2) {
						Game_Manager.instance.tileNumbers [1]++;
					}
					if (tileIndex == 3) {
						Game_Manager.instance.tileNumbers [2]++;
					}
				}
			} else {
				print ("Can only dig adjacent tiles!");
			}
		}
	}

}
