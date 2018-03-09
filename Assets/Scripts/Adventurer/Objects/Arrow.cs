using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

	public Rigidbody2D rb2D;
	public Vector3 direction;
	public float speed;
	public float attack;

	// Use this for initialization
	void Start () {
		//rb2D.AddForce (new Vector2 (4, 0),ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
		if (direction != null) {
			//transform.position = Vector3.MoveTowards (transform.position, direction,speed*Time.deltaTime);
			transform.position += transform.right * Time.deltaTime * speed;
		}
	}

	void OnCollisionEnter2D(Collision2D collisionInfo){
		Monster mon_script = collisionInfo.gameObject.GetComponent<Monster> ();
		if (mon_script != null) {
			mon_script.HP -= (attack - mon_script.defense);
		}
		Destroy (this.gameObject);
	}
}
