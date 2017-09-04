using UnityEngine;

public class PlayerController : Photon.MonoBehaviour {

	public delegate void OnPlayerSpawnedCallback(GameObject playerEntity, bool isLocalInstance);
	public delegate void OnToggledViewModeCallback(CameraManager.ViewMode viewMode, bool isLocalInstance);

	private new Rigidbody rigidbody;

	private OnPlayerSpawnedCallback playerSpawnedCallback;
	private OnToggledViewModeCallback toggledViewModeCallback;

	private CameraManager.ViewMode viewMode;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();

		AddPlayerSpawnedCallback(GameManagerBase.Instance.cameraManager.OnPlayerSpawned);
		AddPlayerSpawnedCallback(OnPlayerSpawned);

		AddToggledViewModeCallback(GameManagerBase.Instance.cameraManager.OnToggledViewMode);

		viewMode = CameraManager.ViewMode.FIRST_PERSON;
	}

	void Start() {
		if (playerSpawnedCallback != null) {
			playerSpawnedCallback(gameObject, photonView.isMine);
		}
	}

	private void Update() {
		InputToggleView();
	}

	public string NickName {
		get {
			return photonView.owner.NickName;
		}
	}

	public void AddPlayerSpawnedCallback(OnPlayerSpawnedCallback playerSpawnedCallback) {
		if (this.playerSpawnedCallback == null) {
			this.playerSpawnedCallback = playerSpawnedCallback;
		} else {
			this.playerSpawnedCallback += playerSpawnedCallback;
		}
	}

	public void AddToggledViewModeCallback(OnToggledViewModeCallback toggledViewModeCallback) {
		if (this.toggledViewModeCallback == null) {
			this.toggledViewModeCallback = toggledViewModeCallback;
		} else {
			this.toggledViewModeCallback += toggledViewModeCallback;
		}
	}

	private void OnPlayerSpawned(GameObject playerEntity, bool isLocalInstance) {
		if (isLocalInstance) {
			// Enable physics on local instance
			rigidbody.isKinematic = false;
		} else {
			// Disable physics on remote instances
			rigidbody.isKinematic = true;
		}
	}

	private void InputToggleView() {
		if (Input.GetKeyDown(KeyCode.V)) {
			if (viewMode == CameraManager.ViewMode.FIRST_PERSON) {
				viewMode = CameraManager.ViewMode.BIRDS_EYE;
			} else {
				viewMode = CameraManager.ViewMode.FIRST_PERSON;
			}

			toggledViewModeCallback(viewMode, photonView.isMine);
		}
	}

}
