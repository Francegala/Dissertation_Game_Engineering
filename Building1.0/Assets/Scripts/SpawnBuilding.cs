	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SpawnBuilding : MonoBehaviour
	{
		
		private float y = 0.5f;
		public GameObject myFloor;
		private spawnHouse spawnH;
		public int counterFloors = 0 ;
		private List<GameObject> floorList;

		public void createBuildings(float x, float z)
		{
			floorList = new List<GameObject>();
			counterFloors = 0;
			//spawnH = gameObject.GetComponentInParent<global::spawnHouse>();
			spawnH = GameObject.Find ("Plane").GetComponent<global::spawnHouse>();
			for (float f = 0; f < spawnH.numofFloors; f++) { 
				float copyX = x; 
				float copyY = y+f; 
				float copyZ = z;
			
			var floor =Instantiate(myFloor);
				floor.name = "Floor"+(++counterFloors);
				floor.transform.parent = this.transform;
				floorList.Add(floor);

			SpawnRooms spawnRooms = floor.GetComponent<SpawnRooms>();
			spawnRooms.createFloor(spawnH, z, copyX, copyY, copyZ);
			} 

		}

	}
