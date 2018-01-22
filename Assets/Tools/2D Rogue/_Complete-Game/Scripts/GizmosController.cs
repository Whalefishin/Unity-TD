using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]

public class GizmosController : MonoBehaviour 
{

	private Vector3 screenPoint;
	private Vector3 offset;

	void Start(){
	}

	void OnMouseDown()
	{
		if (Game_Manager.instance.preparationPhase == true) {
			screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		}
	}

	void OnMouseDrag()
	{
		if (Game_Manager.instance.preparationPhase == true) {
			Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x,Input.mousePosition.y, screenPoint.z);

			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
			Vector3 adjustedPos = new Vector3 ((int)curPosition.x, (int)curPosition.y);
			transform.position = adjustedPos;
		}
	}

}