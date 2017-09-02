using UnityEngine;

public class Interactable : MonoBehaviour {

	public delegate void OnTriggerEnterCallback(Collider other);
	public delegate void OnTriggerExitCallback(Collider other);

	private OnTriggerEnterCallback triggerEnterCallback;
	private OnTriggerExitCallback triggerExitCallback;

	void OnTriggerEnter(Collider other) {
		if (triggerEnterCallback != null) {
			triggerEnterCallback(other);
		}
	}

	void OnTriggerExit(Collider other) {
		if (triggerExitCallback != null) {
			triggerExitCallback(other);
		}
	}

	public void AddTriggerEnterCallback(OnTriggerEnterCallback triggerEnterCallback) {
		if (this.triggerEnterCallback == null) {
			this.triggerEnterCallback = triggerEnterCallback;
		} else {
			this.triggerEnterCallback += triggerEnterCallback;
		}
	}

	public void AddTriggerExitCallback(OnTriggerExitCallback triggerExitCallback) {
		if (this.triggerExitCallback == null) {
			this.triggerExitCallback = triggerExitCallback;
		} else {
			this.triggerExitCallback += triggerExitCallback;
		}
	}

	public void RemoveTriggerEnterCallback(OnTriggerEnterCallback triggerEnterCallback) {
		if (this.triggerEnterCallback != null) {
			this.triggerEnterCallback -= triggerEnterCallback;
		}
	}

	public void RemoveTriggerExitCallback(OnTriggerExitCallback triggerExitCallback) {
		if (this.triggerExitCallback != null) {
			this.triggerExitCallback -= triggerExitCallback;
		}
	}

	public void StartInteraction() {

	}

	public void EndInteraction() {

	}

}
