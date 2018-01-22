using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 		


public class RandomGate : Gate {

	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}


	protected override void Spawn(){
		for(int i=0;i<spawnLocations.Count;i++) {
			Vector3 location = spawnLocations [i];
			GameObject toSpawn = location_monster [location];
			if (Time.time - last_spawn_times[i] >= toSpawn.GetComponent<Monster>().spawn_time) {
				GameObject.Instantiate (toSpawn, getRandomLocation(), Quaternion.identity);
				last_spawn_times [i] = Time.time;
			}
		}
	}

	Vector3 getRandomLocation(){
		List<Vector3> tiles = new List<Vector3>();
		//first, get all empty tiles
		for (int i=0;i<TileMap.instance.mapSizeX;i++){
			for (int j = 0; j < TileMap.instance.mapSizeY; j++) {
				if (TileMap.instance.tiles[i,j]==0){
					tiles.Add (new Vector3 (i,j,0));
				}
			}
		}
		//return a random empty tile
		return tiles [Random.Range (0, tiles.Count)];
	}
}
