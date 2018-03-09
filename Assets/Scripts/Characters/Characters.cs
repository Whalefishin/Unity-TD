using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : Unit {


	public float temp_time;
	public float HP;
	public float MP;
	public float attack;
	public float attack_rate;
	public float defense;
	public float speed;
	private float attack_max_color_change;
	public List<GameObject> enemiesInContact;

	public Vector3 nextDest;

	public bool facing_up;
	public bool facing_down;
	public bool facing_left;
	public bool facing_right;

	//public float guardian_radius;

	Vector3 original_scale; 

	// Use this for initialization
	protected virtual void Start () {
		base.Start ();
		temp_time = 0;
		attack_max_color_change = 10f;
		facing_up = false;
		facing_down = true;
		facing_left = false;
		facing_right = false;
		//facing_right_animator = true;
		original_scale = transform.localScale;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		checkIfDead ();
	}
		

	protected virtual IEnumerator Attack(GameObject toAttack){
		if (toAttack!= null && toAttack.activeSelf == true) {
			float deduction = Mathf.Max (attack - toAttack.GetComponent<Characters> ().defense, 0);
			toAttack.GetComponent<Characters> ().HP -= deduction;
			yield return new WaitForSeconds (attack_rate);
		} else {
			yield return null;
		}
	}

	protected void destroy(GameObject toDestroy){
		Destroy (toDestroy);
	}

	protected virtual void turnRight(){
//		if (facing_right_animator) {
//			facing_right_animator = !(facing_right_animator);
//			Vector3 theScale = transform.localScale;
//			theScale.x *= -1;
//			transform.localScale = theScale;
//		}
		transform.localScale = original_scale;

		facing_up = false;
		facing_down = false;
		facing_left = false;
		facing_right = true;
	}
	protected virtual void turnLeft(){
		Vector3 theScale = original_scale;
		theScale.x *= -1;
		transform.localScale = theScale;

		facing_up = false;
		facing_down = false;
		facing_left = true;
		facing_right = false;
	}
	protected virtual void turnUp(){
		facing_up = true;
		facing_down = false;
		facing_left = false;
		facing_right = false;
	}
	protected virtual void turnDown(){
		facing_up = false;
		facing_down = true;
		facing_left = false;
		facing_right = false;
	}

	/*
	protected virtual void moveTowards(Vector3 dest){
		//List<Node> path = TileMap.instance.returnPathGenerated (transform.position,dest);
		List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),dest);
		// Move us to the next tile in the sequence

		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;

			if (X > tileX) {
				turnRight ();
			} else if (X < tileX) {
				turnLeft ();
			} else if (Y > tileY) {
				turnUp ();
			} else if (Y < tileY) {
				turnDown ();
			}


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

	
	protected virtual IEnumerator MoveTowardsGameObject(GameObject target){
		Unit targetTileInfo = target.GetComponent<Unit> ();
		while (target != null && target.activeSelf && (transform.position - target.transform.position).sqrMagnitude > float.Epsilon){
			List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),
				new Vector3(targetTileInfo.tileX,targetTileInfo.tileY,0));
			if (path != null) {
				int X = path [1].x;
				int Y = path [1].y;
				Vector3 nextStep = new Vector3 (X, Y, 0);
				if (X > tileX) {
					turnRight ();
				} else if (X < tileX) {
					turnLeft ();
				} else if (Y > tileY) {
					turnUp ();
				} else if (Y < tileY) {
					turnDown ();
				}
				while ((transform.position - nextStep).sqrMagnitude > float.Epsilon) {
					if (enemiesInContact.Contains (target)) {
						break;
					} else {
						float step = speed * Time.deltaTime;
						transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
						yield return null;
					}
				}
				tileX = X;
				tileY = Y;
			} else {
				yield break;
			}
			yield return null;
		}
	}
	*/

	protected virtual IEnumerator MoveTowardsGameObjectContactBased(GameObject target){
		Unit targetTileInfo = target.GetComponent<Unit> ();
		while (target != null && target.activeSelf && enemiesInContact.Count ==0){
			List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),
				new Vector3(targetTileInfo.tileX,targetTileInfo.tileY,0));
			if (path != null) {
				int X = path [1].x;
				int Y = path [1].y;
				Vector3 nextStep = new Vector3 (X, Y, 0);
				if (X > tileX) {
					turnRight ();
				} else if (X < tileX) {
					turnLeft ();
				} else if (Y > tileY) {
					turnUp ();
				} else if (Y < tileY) {
					turnDown ();
				}
				while (enemiesInContact.Count ==0 && target!=null) {
					if ((transform.position - nextStep).sqrMagnitude < float.Epsilon) {
						tileX = X;
						tileY = Y;
						break;
					}
					float step = speed * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
					yield return null;
				}
//				tileX = X;
//				tileY = Y;
			} else {
				yield break;
			}
			yield return null;
		}
	}

	protected virtual IEnumerator MoveOneStepTowardsGameObject(GameObject target){
		if (target != null) {
			Unit targetTileInfo = target.GetComponent<Unit> ();
			List<Node> path = TileMap.instance.returnPathGenerated (new Vector3 (tileX, tileY, 0),
				                  new Vector3 (targetTileInfo.tileX, targetTileInfo.tileY, 0));
			if (path != null) {
				int X = path [1].x;
				int Y = path [1].y;
				Vector3 nextStep = new Vector3 (X, Y, 0);
				if (X > tileX) {
					turnRight ();
				} else if (X < tileX) {
					turnLeft ();
				} else if (Y > tileY) {
					turnUp ();
				} else if (Y < tileY) {
					turnDown ();
				}
				while (enemiesInContact.Count ==0 && target!=null) {
//					if (enemiesInContact.Contains (target)) {
//						break;
//					} else {
					if ((transform.position - nextStep).sqrMagnitude < float.Epsilon){
						tileX = X;
						tileY = Y;
						break;
					}
					//print ("here");
					float step = speed * Time.deltaTime;
					transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
					yield return null;
//					}
				}
//				tileX = X;
//				tileY = Y;
			}
		} else {
			yield break;
		}
	}

	protected virtual IEnumerator MoveOneStepTowards(Vector3 dest){
		List<Node> path = TileMap.instance.returnPathGenerated (new Vector3(tileX,tileY,0),
			new Vector3(dest.x,dest.y,0));
		if (path != null) {
			int X = path [1].x;
			int Y = path [1].y;
			Vector3 nextStep = new Vector3 (X, Y, 0);
			if (X > tileX) {
				turnRight ();
			} else if (X < tileX) {
				turnLeft ();
			} else if (Y > tileY) {
				turnUp ();
			} else if (Y < tileY) {
				turnDown ();
			}
//			while (enemiesInContact.Count ==0 && (transform.position - nextStep).sqrMagnitude > float.Epsilon) {
//				float step = speed * Time.deltaTime;
//				transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
//				yield return null;
//			}
//			//print ("sdfsd");
//			tileX = X;
//			tileY = Y;
			while (enemiesInContact.Count ==0) {
				//					if (enemiesInContact.Contains (target)) {
				//						break;
				//					} else {
				if ((transform.position - nextStep).sqrMagnitude < float.Epsilon){
					tileX = X;
					tileY = Y;
					break;
				}
				//print ("here");
				float step = speed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position, nextStep, step);
				yield return null;
				//					}
			}
		}
	}


	protected IEnumerator randomWalk(){
		if (nextDest.x == Mathf.Infinity) {
			nextDest = generateRandomDest ();
		}

		if (nextDest.x > tileX) {
			turnRight ();
		} else if (nextDest.x < tileX) {
			turnLeft ();
		} else if (nextDest.y > tileY) {
			turnUp ();
		} else if (nextDest.y < tileY) {
			turnDown ();
		}

		while ((transform.position - nextDest).sqrMagnitude > float.Epsilon) {
			transform.position = Vector3.MoveTowards (transform.position, nextDest, speed * Time.deltaTime);
			yield return null;
		}

		tileX = (int)nextDest.x;
		tileY = (int)nextDest.y;
		nextDest = generateRandomDest ();
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

	//	protected void randomWalk(){
	//		if (nextDest.x == Mathf.Infinity) {
	//			nextDest = generateRandomDest ();
	//		}
	//
	//		if (nextDest.x > tileX) {
	//			turnRight ();
	//		} else if (nextDest.x < tileX) {
	//			turnLeft ();
	//		} else if (nextDest.y > tileY) {
	//			turnUp ();
	//		} else if (nextDest.y < tileY) {
	//			turnDown ();
	//		}
	//
	//		transform.position = Vector3.MoveTowards (transform.position, nextDest, speed * Time.deltaTime);
	//	
	//		float sqrRemainingDistance = (transform.position - nextDest).sqrMagnitude;
	//		if (sqrRemainingDistance < float.Epsilon) {
	//			tileX = (int)nextDest.x;
	//			tileY = (int)nextDest.y;
	//			//if we arrived at one destinition, generate the next one.
	//			nextDest = generateRandomDest ();
	//		}
	//	}

	//	protected virtual void AttackOld(GameObject toAttack){
	//		if (toAttack.activeSelf == true) {
	//			if (Time.time - temp_time > attack_rate) {
	//				float deduction = Mathf.Max (attack - toAttack.GetComponent<Characters> ().defense, 0);
	//				toAttack.GetComponent<Characters> ().HP -= deduction;
	//				//toAttack.GetComponent<SpriteRenderer> ().color = new Color (1*deduction/attack_max_color_change, 0, 0);
	//				temp_time = Time.time;
	//			}
	//		}
	//	}
}
