using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public int tileX;
	public int tileY;

	protected virtual void Start () {
		tileX = (int)transform.position.x;
		tileY = (int)transform.position.y;
	}
}
