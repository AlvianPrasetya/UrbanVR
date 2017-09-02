using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject Spawn(GameObject spawnedPrefab) {
		return Instantiate(spawnedPrefab, transform.position, transform.rotation);
	}

	public GameObject NetworkedSpawn(string spawnedPrefabName, byte group) {
		return PhotonNetwork.Instantiate(spawnedPrefabName, transform.position, transform.rotation, group);
	}

}
