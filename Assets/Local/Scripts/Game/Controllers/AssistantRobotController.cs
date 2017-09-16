using UnityEngine;
using System.Collections;

public class AssistantRobotController : MonoBehaviour {

	public float moveSpeed;
	public float rotateSpeed;

	public float maxFollowDistance;
	public float minTrailDistance;

	public float maxReachedAngle;
	public float maxReachedDistance;

	private new Rigidbody rigidbody;

	private Transform playerTransform;
	private Queue playerTrails;
	private Vector3 lastPlayerPosition;
	private Vector3 flattenedTargetPosition;
	private Quaternion targetRotation;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		
		playerTrails = new Queue();
	}

	void Update() {
		UpdateTrail();
	}

	void FixedUpdate() {
		FollowPlayer();
	}

	public void Initialize(Transform playerTransform) {
		this.playerTransform = playerTransform;
		lastPlayerPosition = playerTransform.position;
		flattenedTargetPosition = Utils.Unflatten(Utils.Flatten(transform.position));
		targetRotation = transform.rotation;

		// Detach from player transform
		transform.parent = null;
	}

	private void UpdateTrail() {
		if (Vector3.SqrMagnitude(playerTransform.position - lastPlayerPosition) > minTrailDistance * minTrailDistance) {
			// Only note down player position when player has moved far enough to prevent queue overflow
			playerTrails.Enqueue(playerTransform.position);
			lastPlayerPosition = playerTransform.position;
		}
	}

	private void FollowPlayer() {
		if (playerTransform == null) {
			return;
		}

		float sqrDistanceToPlayer = Vector3.SqrMagnitude(playerTransform.position - transform.position);
		if (sqrDistanceToPlayer < maxFollowDistance * maxFollowDistance) {
			// Still within max follow distance, do nothing
			return;
		}
		
		Vector3 flattenedRobotPosition = Utils.Unflatten(Utils.Flatten(transform.position));

		if (Quaternion.Angle(transform.rotation, targetRotation) > maxReachedAngle) {
			// Rotate towards target rotation until rotation reached
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
			return;
		}

		if (Vector3.SqrMagnitude(flattenedTargetPosition - flattenedRobotPosition) > maxReachedDistance * maxReachedDistance) {
			// Move towards target position until position reached
			Vector3 moveDirection = Utils.Unflatten(Utils.Flatten(transform.forward));
			rigidbody.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
			return;
		}
		
		if (playerTrails.Count == 0) {
			// No more trails to follow, do nothing
			return;
		}
		
		// Robot has reached target position, recalculate target position and rotation
		Vector3 targetPosition = (Vector3) playerTrails.Dequeue();
		flattenedTargetPosition = Utils.Unflatten(Utils.Flatten(targetPosition));
		targetRotation = Quaternion.LookRotation((flattenedTargetPosition - flattenedRobotPosition).normalized, Vector3.up);
	}

}
