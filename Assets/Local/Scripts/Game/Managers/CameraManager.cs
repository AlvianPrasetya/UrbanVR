using UnityEngine;

/**
 * This class manages the camera lifecycles throughout the entire game.
 * All callbacks related to camera events are to be directed to a method 
 * within this class.
 */
public class CameraManager : MonoBehaviour {
	
	public enum ViewMode {
		FIRST_PERSON,
		BIRDS_EYE
	}

	public Camera sceneCamera;
	public Camera playerCamera;

	void Start() {
		sceneCamera.gameObject.SetActive(true);
	}

	public Camera GetActiveCamera() {
		if (sceneCamera.gameObject.GetActive()) {
			return sceneCamera;
		} else {
			return playerCamera;
		}
	}

	public void OnPlayerSpawned(GameObject playerEntity, bool isLocalInstance) {
		playerCamera = playerEntity.GetComponentInChildren<Camera>();

		if (isLocalInstance) {
			// Parent and point scene camera at player entity
			sceneCamera.transform.parent = playerEntity.transform;
			sceneCamera.transform.localPosition = new Vector3(0.0f, 5.0f, 0.0f);
			sceneCamera.transform.LookAt(playerEntity.transform);

			// Disable scene camera on local instance
			sceneCamera.gameObject.SetActive(false);
			// Enable player camera on local instance
			playerCamera.gameObject.SetActive(true);
		} else {
			// Disable player camera on remote instances
			playerCamera.gameObject.SetActive(false);
		}
	}

	public void OnToggledViewMode(ViewMode viewMode, bool isLocalInstance) {
		if (isLocalInstance) {
			if (viewMode == ViewMode.FIRST_PERSON) {
				// Disable scene camera on local instance
				sceneCamera.gameObject.SetActive(false);

				// Enable player camera on local instance
				playerCamera.gameObject.SetActive(true);
			} else {
				// Disable player camera on local instance
				playerCamera.gameObject.SetActive(false);

				// Enable scene camera on local instance
				sceneCamera.gameObject.SetActive(true);
			}
		}
	}

}
