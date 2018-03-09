using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterType{

	public string name;
	public GameObject monsterPrefab;
	public bool isSpawnable;
	public int min_level;
}
