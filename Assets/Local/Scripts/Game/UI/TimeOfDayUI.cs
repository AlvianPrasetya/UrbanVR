using UnityEngine;

public class TimeOfDayUI : MonoBehaviour, InteractableUI {

	public void Interact() {
		GameManagerBase.Instance.dayNightManager.ToggleTimeMode();
	}
	
}
