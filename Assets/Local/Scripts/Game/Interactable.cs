using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

	public delegate void OnTriggerEnterCallback(Collider other);
	public delegate void OnTriggerExitCallback(Collider other);

	private OnTriggerEnterCallback triggerEnterCallback;
	private OnTriggerExitCallback triggerExitCallback;
	
	private Renderer[] renderers;
	private List<Material> originalMaterials;

	void Awake() {
		renderers = GetComponentsInChildren<Renderer>();

		originalMaterials = new List<Material>();
		foreach (Renderer renderer in renderers) {
			originalMaterials.Add(renderer.material);
		}
	}

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

	public void Materialize() {
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material = originalMaterials[i];
		}
	}

	public void Dematerialize() {
		foreach (Renderer renderer in renderers) {
			renderer.material = null;
		}
	}

}
