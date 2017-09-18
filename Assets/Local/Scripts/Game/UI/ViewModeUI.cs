using UnityEngine;

public class ViewModeUI : MonoBehaviour, InteractableUI {

	public void Interact() {
		GameManagerBase.Instance.cameraManager.ToggleViewMode();
	}

}
