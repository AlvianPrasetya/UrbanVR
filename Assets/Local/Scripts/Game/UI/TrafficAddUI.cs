using UnityEngine;

public class TrafficAddUI : MonoBehaviour, InteractableUI {

	public void Interact() {
		GameManagerBase.Instance.trafficManager.AddVehicle();
	}

}
