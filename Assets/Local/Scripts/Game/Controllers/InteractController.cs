using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractController : MonoBehaviour {

	public Interactable[] spawnables;
	public Transform playerCamera;
	public Transform pointer;

	public float translateSpeedMultiplier;
	public float rotateSpeedMultiplier;

	private Interactable draggedInteractable;

	private Vector3 lastCameraEulerAngles;
	private Vector3 lastPosition;

	private HashSet<Collider> interactableOverlaps;

	void Awake() {
		draggedInteractable = null;

		interactableOverlaps = new HashSet<Collider>();
	}

	void Update() {
		InputSpawn();
		InputDestroy();
		
		InputSelect();
		Drag();
		InputDrop();

		Pan();

		UpdatePointerPosition();
	}

	private void InputSpawn() {
		for (int i = 1; i <= spawnables.Length; i++) {
			if (Input.GetKeyDown(i.ToString())) {
				RaycastHit hitInfo;
				if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo, Mathf.Infinity, Utils.Layer.TERRAIN)) {
					Instantiate(spawnables[i - 1], hitInfo.point, Quaternion.identity);
				}
			}
		}
	}

	private void InputDestroy() {
		if (Input.GetMouseButtonDown(Utils.Input.MOUSE_BUTTON_RIGHT)) {
			RaycastHit hitInfo;
			if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo, Mathf.Infinity, Utils.Layer.INTERACTABLE)) {
				Interactable hitInteractable = hitInfo.transform.GetComponentInParent<Interactable>();
				if (hitInteractable != null) {
					Destroy(hitInteractable.gameObject);
				}
			}
		}
	}

	private void InputSelect() {
		if (Input.GetMouseButtonDown(Utils.Input.MOUSE_BUTTON_LEFT)) {
			if (!SelectUI()) {
				Select();
			}
		}
	}

	private void InputDrop() {
		if (Input.GetMouseButtonUp(Utils.Input.MOUSE_BUTTON_LEFT)) {
			TryDrop();
		}
	}

	private bool SelectUI() {
		if (draggedInteractable != null) {
			// Do not allow select while interacting with another interactable
			return false;
		}

		PointerEventData ped = new PointerEventData(EventSystem.current);
		ped.position = playerCamera.GetComponent<Camera>().WorldToScreenPoint(playerCamera.position + playerCamera.forward);
		Logger.Log(ped.position.ToString());
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(ped, raycastResults);

		foreach (RaycastResult raycastResult in raycastResults) {
			InteractableUI interactableUI = raycastResult.gameObject.GetComponent<InteractableUI>();
			interactableUI.Interact();
		}

		if (raycastResults.Count == 0) {
			return false;
		}

		return true;
	}

	private void Select() {
		if (draggedInteractable != null) {
			// Do not allow select while interacting with another interactable
			return;
		}

		RaycastHit hitInfo;
		if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo, Mathf.Infinity, Utils.Layer.INTERACTABLE)) {
			Interactable hitInteractable = hitInfo.transform.GetComponentInParent<Interactable>();
			if (hitInteractable != null) {
				draggedInteractable = hitInteractable;
				lastCameraEulerAngles = playerCamera.eulerAngles;
				lastPosition = transform.position;

				draggedInteractable.AddTriggerEnterCallback(OnInteractableTriggerEnter);
				draggedInteractable.AddTriggerExitCallback(OnInteractableTriggerExit);

				draggedInteractable.StartInteraction();
			}
		}
	}

	private void Drag() {
		if (draggedInteractable != null) {
			Vector3 cameraEulerAngles = playerCamera.eulerAngles;
			float deltaXAngle = CalculateAngularDifference(cameraEulerAngles.x, lastCameraEulerAngles.x);
			float deltaYAngle = CalculateAngularDifference(cameraEulerAngles.y, lastCameraEulerAngles.y);

			lastCameraEulerAngles = cameraEulerAngles;

			Vector2 draggedInteractablePosition = Utils.Flatten(draggedInteractable.transform.position);
			Vector2 translateDirection = (Utils.Flatten(draggedInteractable.transform.position - playerCamera.position)).normalized;
			Vector2 draggedInteractableTranslatedPosition = draggedInteractablePosition + translateDirection * -deltaXAngle * translateSpeedMultiplier;

			RaycastHit hitInfo;
			if (Physics.Raycast(Utils.Unflatten(draggedInteractableTranslatedPosition, 1000.0f), Vector3.down, out hitInfo, Mathf.Infinity, Utils.Layer.TERRAIN)) {
				draggedInteractable.transform.position = Utils.Unflatten(draggedInteractableTranslatedPosition, hitInfo.point.y);
			}
			
			draggedInteractable.transform.RotateAround(playerCamera.position * rotateSpeedMultiplier, Vector3.up, deltaYAngle);
		}
	}

	private void TryDrop() {
		if (draggedInteractable != null && interactableOverlaps.Count == 0) {
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

	private void UpdatePointerPosition() {
		RaycastHit hitInfo;
		if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo, Mathf.Infinity)) {
			pointer.position = playerCamera.position + playerCamera.forward * hitInfo.distance * 0.9f;
			pointer.localScale = new Vector3(
				hitInfo.distance * 0.01f,
				hitInfo.distance * 0.01f,
				hitInfo.distance * 0.01f);
		} else {
			float farClipDistance = playerCamera.GetComponent<Camera>().farClipPlane;
			pointer.position = playerCamera.position + playerCamera.forward * farClipDistance * 0.9f;
			pointer.localScale = new Vector3(
				farClipDistance * 0.01f,
				farClipDistance * 0.01f,
				farClipDistance * 0.01f);
		}

		pointer.LookAt(playerCamera.position);
	}

	private void OnInteractableTriggerEnter(Collider other) {
		interactableOverlaps.Add(other);

		if (interactableOverlaps.Count == 1) {
			draggedInteractable.Dematerialize();
		}
	}

	private void OnInteractableTriggerExit(Collider other) {
		interactableOverlaps.Remove(other);

		if (interactableOverlaps.Count == 0) {
			draggedInteractable.Materialize();
		}
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
