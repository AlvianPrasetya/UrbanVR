using UnityEngine;

public class PlayerController : Photon.MonoBehaviour {

	public AssistantRobotController assistantRobot;

	public delegate void OnPlayerSpawnedCallback(GameObject playerEntity, bool isLocalInstance);
	public delegate void OnToggledViewModeCallback(CameraManager.ViewMode viewMode, bool isLocalInstance);

	private new Rigidbody rigidbody;

	private OnPlayerSpawnedCallback playerSpawnedCallback;

	private CameraManager.ViewMode viewMode;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();

		AddPlayerSpawnedCallback(GameManagerBase.Instance.cameraManager.OnPlayerSpawned);
		AddPlayerSpawnedCallback(OnPlayerSpawned);

		viewMode = CameraManager.ViewMode.FIRST_PERSON;
	}

	void Start() {
		if (playerSpawnedCallback != null) {
			playerSpawnedCallback(gameObject, photonView.isMine);
		}

		// Initialize assistant robot for player-following behaviour
		assistantRobot.Initialize(transform);
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

	private void OnPlayerSpawned(GameObject playerEntity, bool isLocalInstance) {
		if (isLocalInstance) {
			// Enable physics on local instance
			rigidbody.isKinematic = false;
		} else {
			// Disable physics on remote instances
			rigidbody.isKinematic = true;
		}
	}

}
