using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour {


	public int tileX;
	public int tileY;
	public float temp_time;
	public float HP;
	public float MP;
	public float attack;
	public float attack_rate;
	public float defense;
	public float speed;
	private float attack_max_color_change;

	public Vector3 nextDest;

	// Use this for initialization
	protected virtual void Start () {
		tileX = (int)transform.position.x;
		tileY = (int)transform.position.y;
		temp_time = 0;
		attack_max_color_change = 10f;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		checkIfDead ();
	}

	protected virtual void Attack(GameObject toAttack){
		if (Time.time - temp_time > attack_rate) {
			float deduction = Mathf.Max (attack - toAttack.GetComponent<Characters> ().defense, 0);
			toAttack.GetComponent<Characters> ().HP -= deduction;
			//toAttack.GetComponent<SpriteRenderer> ().color = new Color (1*deduction/attack_max_color_change, 0, 0);
			temp_time = Time.time;
		}
	}

	protected void destroy(GameObject toDestroy){
		Destroy (toDestroy);
	}

	protected void moveTowards(Vector3 dest){
		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
		List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),dest);
		// Move us to the next tile in the sequence

		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;

			Vector3 nextStep = new Vector3(X,Y,0);

			//transform.position = TileMap.instance.TileCoordToWorldCoord( tileX, tileY );	// Update our unity world position

			//Smooth movement
			//Vector3 end = new Vector3(tileX,tileY,transform.position.z);
			float step = speed * Time.deltaTime;
			//transform.position = Vector3.MoveTowards (transform.position, TileMap.instance.TileCoordToWorldCoord (tileX, tileY), step);
			transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
			if ((transform.position - nextStep).sqrMagnitude < float.Epsilon) {
				tileX = X;
				tileY = Y;
			}
		}
	}

	protected void randomWalk(){
		transform.position = Vector3.MoveTowards (transform.position, nextDest, speed * Time.deltaTime);
	
		float sqrRemainingDistance = (transform.position - nextDest).sqrMagnitude;
		if (sqrRemainingDistance < float.Epsilon) {
			tileX = (int)nextDest.x;
			tileY = (int)nextDest.y;
			//if we arrived at one destinition, generate the next one.
			nextDest = generateRandomDest ();
		}
	}

	protected Vector3 generateRandomDest(){
		List <Vector3> walkables = new List<Vector3>();
		//Vector3 currPos = transform.position;
		Vector3 currPos = new Vector3(tileX,tileY,0);
		if ((int)currPos.y + 1 < TileMap.instance.mapSizeY && TileMap.instance.tiles [(int)currPos.x, (int)currPos.y + 1] == 0) {
			walkables.Add (new Vector3 ((int)currPos.x, (int)currPos.y + 1,0));
		}
		if ((int)currPos.y - 1 >= 0 && TileMap.instance.tiles [(int)currPos.x, (int)currPos.y - 1] == 0) {
			walkables.Add (new Vector3 ((int)currPos.x, (int)currPos.y - 1,0));
		}
		if ((int)currPos.x+1 < TileMap.instance.mapSizeX && TileMap.instance.tiles [(int)currPos.x+1, (int)currPos.y] == 0) {
			walkables.Add (new Vector3 ((int)currPos.x+1, (int)currPos.y,0));
		}
		if ((int)currPos.x-1 >=0 && TileMap.instance.tiles [(int)currPos.x-1, (int)currPos.y] == 0) {
			walkables.Add (new Vector3 ((int)currPos.x-1, (int)currPos.y,0));
		}
		Vector3 toWalk = walkables [Random.Range (0, walkables.Count)];
		return toWalk;
	}
	protected virtual void checkIfDead(){
		if (HP <= 0) {
			Destroy (this.gameObject);
		}
	}
}
