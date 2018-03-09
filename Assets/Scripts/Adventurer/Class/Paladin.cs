using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Adventurer {

	public GameObject shield_up;
	public GameObject shield_down;
	public GameObject shield_left;
	public GameObject shield_right;
	public float shield_damage_modifier;

	// Use this for initialization
	void Start () {
		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	protected override IEnumerator MoveOneStepTowards (Vector3 dest)
	{
		yield return base.MoveOneStepTowards (dest);
		UpdateShieldStatus ();
	}

	protected override IEnumerator MoveOneStepTowardsGameObject (GameObject target)
	{
		yield return base.MoveOneStepTowardsGameObject (target);
		UpdateShieldStatus ();
	}

	protected override IEnumerator MoveTowardsGameObjectContactBased (GameObject target)
	{
		yield return base.MoveTowardsGameObjectContactBased (target);
		UpdateShieldStatus ();
	}

	protected override IEnumerator FindDemonLord ()
	{
		yield return base.FindDemonLord ();
		UpdateShieldStatus ();
	}

	void UpdateShieldStatus(){
		if (facing_up) {
			shield_up.SetActive (true);
			shield_down.SetActive (false);
			shield_left.SetActive (false);
			shield_right.SetActive (false);
		}
		else if (facing_down) {
			shield_up.SetActive (false);
			shield_down.SetActive (true);
			shield_left.SetActive (false);
			shield_right.SetActive (false);
		}
		else if (facing_left) {
			shield_up.SetActive (false);
			shield_down.SetActive (false);
			shield_left.SetActive (true);
			shield_right.SetActive (false);
		}
		else if (facing_right) {
			shield_up.SetActive (false);
			shield_down.SetActive (false);
			shield_left.SetActive (false);
			shield_right.SetActive (true);
		}
	}

	void ReduceShieldedDamage(){ //if a monster attacks in the shield, the damage is reduced by the modifier

	}
}
