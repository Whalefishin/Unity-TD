using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 		


public class RandomGate : Gate {

	//public int gate_level;
	public int spawn_interval;
	public float lambda;
	public bool isOpen;

	public GameObject level_up;
	public GameObject light_button;
	public GameObject shadow_button;

	public bool light_shadow;
	public int gate_cost;
	public int max_level;

	// Use this for initialization
	protected override void Start () {
		lambda = 0;
		isOpen = false;
		light_shadow = false;

		x = (int)transform.position.x;
		y = (int)transform.position.y;

		click_parity = 0;

		GateCanvas = transform.Find ("GateCanvas").gameObject;
		GateCanvas.SetActive (false);
		foreach (Transform child in GateCanvas.transform) {
			child.gameObject.SetActive (false);
		}

		StartCoroutine (SpawnAtRandomLocation ());
		//setupButtons ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}


//	protected override void Spawn(){
//		for(int i=0;i<spawnLocations.Count;i++) {
//			Vector3 location = spawnLocations [i];
//			GameObject toSpawn = location_monster [location];
//			if (Time.time - last_spawn_times[i] >= toSpawn.GetComponent<Monster>().spawn_time) {
//				GameObject.Instantiate (toSpawn, getRandomLocation(), Quaternion.identity);
//				last_spawn_times [i] = Time.time;
//			}
//		}
//	}

	protected IEnumerator SpawnAtRandomLocation(){
		while (true) {
			if (isOpen) {
				Vector3 location = getRandomLocation ();
				yield return SpawnAt (location);
			} else {
				yield return null;
			}
		}
	}

	protected override IEnumerator SpawnAt(Vector3 location){
		GameObject toSpawn = GenerateMonsterToSpawn ();
		if (toSpawn != null) {
			GameObject.Instantiate (toSpawn, location, Quaternion.identity);
			yield return new WaitForSeconds (spawn_interval);
		} else {
			yield return null;
		}
	}

	protected Vector3 getRandomLocation(){
		//return a random empty tile
		return TileMap.instance.emptyLocations [Random.Range (0, TileMap.instance.emptyLocations.Count)];
	}

	public void LevelUp(){
		if (lambda >= max_level) {
			print ("Highest Lvl");
		} else {
			if (SpentResources (gate_cost))
				lambda++;
			level_up.SetActive (false);
		}
	}


	public int SimulatePoisson(){
		float rand = Random.value;
		float prob_mass = 0;
		int k = -1;
		while (prob_mass <= rand) {
			prob_mass += Poisson_pmf (k);
			k++;
		}
		return k;
	}

	public float Poisson_pmf(int k){
		return Mathf.Exp(-1*lambda)* Mathf.Pow(lambda,k)/Factorial(k);
	}

	public GameObject GenerateMonsterToSpawn(){
		if (!isOpen)
			return null;
		int index = SimulatePoisson ();
		if (index >= Game_Manager.instance.poisson_monster_list_light.Count) {
			index = Game_Manager.instance.poisson_monster_list_light.Count - 1;
		}
		if (light_shadow) {
			return Game_Manager.instance.poisson_monster_list_light [index];
		} else {
			return Game_Manager.instance.poisson_monster_list_shadow [index];
		}
	}

	public int Factorial(int x){
		//print (x);
//		if (x == 0)
//			return 1;
//		return x * Factorial (x - 1);
		if (x >= 1) {
			return x * Factorial (x - 1);
		} else {
			return 1;
		}
	}

	protected override void OnMouseOver () {
		//detect left click
		if (Input.GetMouseButtonDown (0)) {
			if (click_parity % 2 == 0) {
				if (Vector3.Distance (TileMap.instance.DemonLord.transform.position, this.gameObject.transform.position) < 1.5) {
					//display the gui
					GateCanvas.SetActive (true);
					if (!isOpen) {
//						foreach (Transform child in GateCanvas.transform) {
//							if (child.gameObject != level_up)
//								child.gameObject.SetActive (true);
//						}
						light_button.SetActive(true);
						shadow_button.SetActive (true);
					} else {
						if (lambda < max_level)
							level_up.SetActive (true);
					}
				}

			}
			else {
				foreach (Transform child in GateCanvas.transform) {
					child.gameObject.SetActive (false);
				}

				GateCanvas.SetActive (false);
			}
			click_parity++;
		}
	}

	public void Light(){
		if (SpentResources(gate_cost)){
			isOpen = true;
			light_shadow = true;
		}
		light_button.SetActive (false);
		shadow_button.SetActive (false);
	}

	public void Shadow(){
		if (SpentResources (gate_cost)) {
			isOpen = true;
			light_shadow = false;
		}
		light_button.SetActive (false);
		shadow_button.SetActive (false);
	}

	bool SpentResources(int cost){
		if (cost <= Game_Manager.instance.tileNumbers [0]) {
			Game_Manager.instance.tileNumbers [0] -= cost;
			return true;
		}
		print ("Not enough resources!");
		return false;
	}
}
