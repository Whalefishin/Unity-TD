using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {

	public void Quit(){
		Application.Quit ();
	}

	public void Retry(){
		Game_Manager.instance.ReloadLevel ();
	}
}
