using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public GameObject other;

	// Use this for initialization
	void Start () {
		if (other != null)
			Debug.DrawRay (transform.position, other.transform.position-transform.position,Color.black,5, false);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.DrawRay (transform.position, other.transform.position,Color.black,5, false);
	}
}
