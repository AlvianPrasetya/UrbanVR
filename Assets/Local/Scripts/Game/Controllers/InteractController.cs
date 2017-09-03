using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InteractController : MonoBehaviour {

	public Interactable[] spawnables;

	public float translateSpeedMultiplier;
	public float rotateSpeedMultiplier;

	private Interactable draggedInteractable;

	private Camera activeCamera;

	private Vector3 lastCameraEulerAngles;
	private Vector3 lastPosition;

	private HashSet<Collider> interactableOverlaps;

	void Awake() {
		draggedInteractable = null;

		interactableOverlaps = new HashSet<Collider>();
	}

	void Update() {
		UpdateCamera();

		InputSpawn();
		InputDestroy();

		InputSelect();
		Drag();
		InputDrop();

		Pan();
	}

	private void UpdateCamera() {
		activeCamera = GameManagerBase.Instance.cameraManager.GetActiveCamera();
	}

	private void InputSpawn() {
		for (int i = 1; i <= spawnables.Length; i++) {
			if (Input.GetKeyDown(i.ToString())) {
				RaycastHit hitInfo;
				if (Physics.Raycast(activeCamera.transform.position, activeCamera.transform.forward, out hitInfo, Mathf.Infinity, Utils.Layer.TERRAIN | Utils.Layer.OBSTACLE)) {
					Instantiate(spawnables[i - 1], hitInfo.point, Quaternion.identity);
				}
			}
		}
	}

	private void InputDestroy() {
		if (Input.GetMouseButtonDown(Utils.Input.MOUSE_BUTTON_RIGHT)) {
			RaycastHit hitInfo;
			if (Physics.Raycast(activeCamera.transform.position, activeCamera.transform.forward, out hitInfo, Mathf.Infinity, Utils.Layer.OBSTACLE)) {
				Interactable hitInteractable = hitInfo.transform.GetComponentInParent<Interactable>();
				if (hitInteractable != null) {
					Destroy(hitInteractable.gameObject);
				}
			}
		}
	}

	private void InputSelect() {
		if (Input.GetMouseButtonDown(Utils.Input.MOUSE_BUTTON_LEFT)) {
			Select();
		}
	}

	private void InputDrop() {
		if (Input.GetMouseButtonUp(Utils.Input.MOUSE_BUTTON_LEFT)) {
			TryDrop();
		}
	}

	private void Select() {
		if (draggedInteractable != null) {
			// Do not allow select while interacting with another interactable
			return;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(activeCamera.transform.position, activeCamera.transform.forward, out hitInfo, Mathf.Infinity, Utils.Layer.OBSTACLE)) {
			Interactable hitInteractable = hitInfo.transform.GetComponentInParent<Interactable>();
			if (hitInteractable != null) {
				draggedInteractable = hitInteractable;
				lastCameraEulerAngles = activeCamera.transform.eulerAngles;
				lastPosition = transform.position;

				draggedInteractable.AddTriggerEnterCallback(OnInteractableTriggerEnter);
				draggedInteractable.AddTriggerExitCallback(OnInteractableTriggerExit);

				draggedInteractable.StartInteraction();
			}
		}
	}

	private void Drag() {
		if (draggedInteractable != null) {
			Vector3 cameraEulerAngles = activeCamera.transform.eulerAngles;
			float deltaXAngle = CalculateAngularDifference(cameraEulerAngles.x, lastCameraEulerAngles.x);
			float deltaYAngle = CalculateAngularDifference(cameraEulerAngles.y, lastCameraEulerAngles.y);

			lastCameraEulerAngles = cameraEulerAngles;

			Vector2 draggedInteractablePosition = Utils.Flatten(draggedInteractable.transform.position);
			Vector2 translateDirection = (Utils.Flatten(draggedInteractable.transform.position - activeCamera.transform.position)).normalized;
			Vector2 draggedInteractableTranslatedPosition = draggedInteractablePosition + translateDirection * -deltaXAngle * translateSpeedMultiplier;

			RaycastHit hitInfo;
			if (Physics.Raycast(Utils.Unflatten(draggedInteractableTranslatedPosition, 1000.0f), Vector3.down, out hitInfo, Mathf.Infinity, Utils.Layer.TERRAIN)) {
				draggedInteractable.transform.position = Utils.Unflatten(draggedInteractableTranslatedPosition, hitInfo.point.y);
			}
			
			draggedInteractable.transform.RotateAround(activeCamera.transform.position * rotateSpeedMultiplier, Vector3.up, deltaYAngle);
		}
	}

	private void TryDrop() {
		if (interactableOverlaps.Count == 0) {
			// Only allow interaction to end if interactable does not overlap with other object(s)
			draggedInteractable.EndInteraction();

			draggedInteractable.RemoveTriggerEnterCallback(OnInteractableTriggerEnter);
			draggedInteractable.RemoveTriggerExitCallback(OnInteractableTriggerExit);

			draggedInteractable = null;
		}
	}

	private void Pan() {
		if (draggedInteractable != null) {
			Vector2 deltaPosition = Utils.Flatten(transform.position - lastPosition);
			draggedInteractable.transform.position += Utils.Unflatten(deltaPosition);
			lastPosition = transform.position;
		}
	}

	private void OnInteractableTriggerEnter(Collider other) {
		interactableOverlaps.Add(other);
	}

	private void OnInteractableTriggerExit(Collider other) {
		interactableOverlaps.Remove(other);
	}

	private float CalculateAngularDifference(float x, float y) {
		float delta = x - y;

		if (delta > 180) {
			delta -= 360;
		}

		if (delta < -180) {
			delta += 360;
		}

		return delta;
	}

}
