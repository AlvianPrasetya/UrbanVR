using UnityEngine;
using System.Collections.Generic;

public class TrafficManager : MonoBehaviour {

	public VehicleController vehiclePrefab;

	public int numVehiclesToSpawn;

	private bool simulationRunning;
	private List<VehicleController> spawnedVehicles;
	private Waypoint[] waypoints;

	void Awake() {
		simulationRunning = false;
		spawnedVehicles = new List<VehicleController>();
		waypoints = FindObjectsOfType<Waypoint>();
	}

	void Update() {
		InputToggleSimulation();
	}

	private void InputToggleSimulation() {
		if (Input.GetKeyDown(KeyCode.L)) {
			AddVehicle();
			//ToggleSimulation();
		}
	}

	private void AddVehicle() {
		Random.InitState(System.DateTime.Now.Millisecond);
		Waypoint sourceWaypoint = null;
		Waypoint targetWaypoint = null;
		Vector3 spawnPosition = Vector3.zero;
		Quaternion spawnRotation = Quaternion.identity;

		int sourceWaypointId = Random.Range(0, waypoints.Length);
		int targetWaypointId = Random.Range(0, waypoints[sourceWaypointId].neighbours.Length);

		sourceWaypoint = waypoints[Random.Range(0, waypoints.Length)];
		targetWaypoint = sourceWaypoint.neighbours[Random.Range(0, sourceWaypoint.neighbours.Length)];

		spawnPosition = sourceWaypoint.transform.position
			+ (targetWaypoint.transform.position - sourceWaypoint.transform.position) * Random.Range(0.2f, 0.8f);
		spawnRotation = Quaternion.LookRotation(
			(targetWaypoint.transform.position - sourceWaypoint.transform.position).normalized,
			Vector3.up);

		VehicleController vehicle = Instantiate(vehiclePrefab, spawnPosition, spawnRotation);
		vehicle.prevWaypoint = sourceWaypoint;
		vehicle.nextWaypoint = targetWaypoint;

		spawnedVehicles.Add(vehicle);
	}

	private void ToggleSimulation() {
		if (simulationRunning) {
			simulationRunning = false;
			foreach (VehicleController spawnedVehicle in spawnedVehicles) {
				Destroy(spawnedVehicle.gameObject);
			}
			spawnedVehicles.Clear();
		} else {
			simulationRunning = true;

			Random.InitState(System.DateTime.Now.Millisecond);
			for (int i = 0; i < numVehiclesToSpawn; i++) {
				bool recalculateSpawnPosition = true;

				Waypoint sourceWaypoint = null;
				Waypoint targetWaypoint = null;
				Vector3 spawnPosition = Vector3.zero;
				Quaternion spawnRotation = Quaternion.identity;

				// Recalculate spawn position while current calculated position is too close to another spawned vehicle position
				while (recalculateSpawnPosition) {
					int sourceWaypointId = Random.Range(0, waypoints.Length);
					int targetWaypointId = Random.Range(0, waypoints[sourceWaypointId].neighbours.Length);

					sourceWaypoint = waypoints[Random.Range(0, waypoints.Length)];
					targetWaypoint = sourceWaypoint.neighbours[Random.Range(0, sourceWaypoint.neighbours.Length)];

					spawnPosition = sourceWaypoint.transform.position
						+ (targetWaypoint.transform.position - sourceWaypoint.transform.position) * Random.Range(0.2f, 0.8f);
					spawnRotation = Quaternion.LookRotation(
						(targetWaypoint.transform.position - sourceWaypoint.transform.position).normalized,
						Vector3.up);

					recalculateSpawnPosition = false;
					foreach (VehicleController spawnedVehicle in spawnedVehicles) {
						if (Vector3.Distance(spawnedVehicle.transform.position, spawnPosition) < 10.0f) {
							recalculateSpawnPosition = true;
							break;
						}
					}
				}

				// Spawn the vehicle and set waypoints information
				VehicleController vehicle = Instantiate(vehiclePrefab, spawnPosition, spawnRotation);
				vehicle.prevWaypoint = sourceWaypoint;
				vehicle.nextWaypoint = targetWaypoint;

				spawnedVehicles.Add(vehicle);
			}
		}
	}

}
