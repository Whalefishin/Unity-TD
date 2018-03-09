using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Adventurer {

	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	protected override IEnumerator SearchAndDestroy () // rogues don't care about taunt
	{
		while (true) {
			if (enemiesInContact.Count != 0) {
				yield return Attack (enemiesInContact [0]);
			} else if (!Game_Manager.instance.demonLordFound) {
				yield return FindDemonLord ();
				//yield return MoveOneStepTowardsGameObject (TileMap.instance.DemonLord);
			} else if (Game_Manager.instance.demonLordFound) {
				yield return MoveOneStepTowards (startPosition);
			}
			yield return null;
		}
	}
}
