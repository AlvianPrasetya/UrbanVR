using UnityEngine;

public class AssistantRobotController : MonoBehaviour {

	public float moveSpeed;
	public float maxFollowDistance;

	private new Rigidbody rigidbody;
	private Transform playerTransform;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		playerTransform = null;
	}

	void FixedUpdate() {
		FollowPlayer();
	}

	public void Initialize(Transform playerTransform) {
		this.playerTransform = playerTransform;

		// Detach from player transform
		transform.parent = null;
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

		Vector3 flattenedPlayerPosition = Utils.Unflatten(Utils.Flatten(playerTransform.position));
		Vector3 flattenedRobotPosition = Utils.Unflatten(Utils.Flatten(transform.position));

		// Rotate towards player
		Quaternion targetRotation = Quaternion.LookRotation((flattenedPlayerPosition - flattenedRobotPosition).normalized, Vector3.up);
		transform.rotation = targetRotation;

		// Move towards player
		Vector3 moveDirection = Utils.Unflatten(Utils.Flatten(transform.forward));
		rigidbody.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
	}

}
