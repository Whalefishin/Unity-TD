using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableCamera : MonoBehaviour {
	public Vector3 original_camera_pos;

	public int scrollDistance = 5; 
	public float scrollSpeed = 20;
	const int orthographicSizeMin = 8;
	const int orthographicSizeMax = 16;

	public bool inverted;

	public float MIN_X, MAX_X, MIN_Y, MAX_Y;
	float position_offset_x = 10; //placement of camera
	float position_offset_y;

	Vector3 camera_offset;

	Quaternion rotation;

	// Use this for initialization
	void Start () {
		position_offset_y = 2+TileMap.instance.mapSizeY / 4f;
//		MIN_X = 0 + position_offset_x;
//		MAX_X = TileMap.instance.mapSizeX - position_offset_x;
//		MIN_Y = 0 + position_offset_y;
//		MAX_Y = TileMap.instance.mapSizeY - position_offset_y;

		MIN_X = 0;
		MAX_X = TileMap.instance.mapSizeX;
		MIN_Y = 0;
		MAX_Y = TileMap.instance.mapSizeY;
		original_camera_pos = transform.position;
		//inverted = true;
		rotation = transform.rotation;

		Vector3 orig_pos = new Vector3 (TileMap.instance.DemonLord.transform.position.x, TileMap.instance.DemonLord.transform.position.y, -10);
		transform.position = orig_pos;

		camera_offset = new Vector3(0,0,-10);

		StartCoroutine (adjustCamera ());
	}

	// Update is called once per frame
	void Update () {
		transform.position = TileMap.instance.DemonLord.transform.position + camera_offset;
		//adjustCamera ();
	}


	IEnumerator adjustCamera(){


		while (true) {

			float mousePosX = Input.mousePosition.x; 
			float mousePosY = Input.mousePosition.y;

				//press H to restore default position
			if (Input.GetKeyDown ("h")) {
				transform.position = original_camera_pos;
			} 
			else if (mousePosX < scrollDistance) { 
				transform.Translate (Vector3.right * -scrollSpeed * Time.deltaTime);
				adjustCamera_helper ();
			} else if (mousePosX >= Screen.width - scrollDistance) { 
				transform.Translate (Vector3.right * scrollSpeed * Time.deltaTime); 
				adjustCamera_helper ();
			} else if (mousePosY < scrollDistance) { 
				//transform.Translate(transform.forward * -scrollSpeed * Time.deltaTime); 
				transform.Translate (Vector3.up * -scrollSpeed * Time.deltaTime); 
				adjustCamera_helper ();
			} else if (mousePosY >= Screen.height - scrollDistance) { 
				//transform.Translate(transform.forward * scrollSpeed * Time.deltaTime); 
				transform.Translate (Vector3.up * scrollSpeed * Time.deltaTime); 
				adjustCamera_helper ();
			} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) { // back
				Camera.main.orthographicSize++;
				adjustCamera_helper ();
			} else if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // forward
				Camera.main.orthographicSize--;
				adjustCamera_helper ();
			} else {
				yield return null;
			}
			yield return null;
		}
	}

	void adjustCamera_helper(){
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax);

		transform.position = new Vector3 (
			Mathf.Clamp (transform.position.x, MIN_X, MAX_X),
			Mathf.Clamp (transform.position.y, MIN_Y, MAX_Y),
			transform.position.z);

		//Mathf.Clamp(transform.position.z, MIN_Z, MAX_Z));
		camera_offset = -TileMap.instance.DemonLord.transform.position + transform.position;
	}



	//	void LateUpdate(){
	//		transform.rotation = rotation;
	//	}

	/*
	void adjustCamera(){

		//press H to restore default position
		if (Input.GetKeyDown("h")){
			transform.position = original_camera_pos;
		}

		float mousePosX = Input.mousePosition.x; 
		float mousePosY = Input.mousePosition.y;

		if (mousePosX < scrollDistance) { 
			transform.Translate (Vector3.right * -scrollSpeed * Time.deltaTime); 
		} 

		if (mousePosX >= Screen.width - scrollDistance) { 
			transform.Translate (Vector3.right * scrollSpeed * Time.deltaTime); 
		}

		if (mousePosY < scrollDistance) { 
			//transform.Translate(transform.forward * -scrollSpeed * Time.deltaTime); 
			transform.Translate (Vector3.up * -scrollSpeed * Time.deltaTime); 
		} 

		if (mousePosY >= Screen.height - scrollDistance) { 
			//transform.Translate(transform.forward * scrollSpeed * Time.deltaTime); 
			transform.Translate (Vector3.up * scrollSpeed * Time.deltaTime); 
		}
			
		if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
		{
			Camera.main.orthographicSize++;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
		{
			Camera.main.orthographicSize--;
		}
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax );

		transform.position = new Vector3 (
			Mathf.Clamp (transform.position.x, MIN_X, MAX_X),
			Mathf.Clamp (transform.position.y, MIN_Y, MAX_Y),
			transform.position.z);
		
			//Mathf.Clamp(transform.position.z, MIN_Z, MAX_Z));
		camera_offset = -TileMap.instance.DemonLord.transform.position + transform.position;
	}
	*/

}
