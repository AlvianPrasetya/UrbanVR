using UnityEngine;

public class TrafficSubUI : MonoBehaviour, InteractableUI {

	public void Interact() {
		GameManagerBase.Instance.trafficManager.SubVehicle();
	}

}
