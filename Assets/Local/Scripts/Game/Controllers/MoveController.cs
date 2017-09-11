using UnityEngine;

/**
 * This class controls the movement behaviour of an entity and the game controls 
 * bound to such behaviour.
 */
public class MoveController : Photon.MonoBehaviour {

	public Transform playerCamera;

	public float moveSpeed;

	private new Rigidbody rigidbody;
	
	private int throttle;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();

		throttle = 0;
	}
	
	void Update() {
		if (photonView.isMine) {
			InputMove();
		}
	}

	void FixedUpdate() {
		if (photonView.isMine) {
			Move();
		}
	}

	private void InputMove() {
		float scroll = Input.GetAxis(Utils.Axis.SCROLL_WHEEL);
		if (scroll > 0.0f) {
			throttle = Mathf.Clamp(throttle + 1, -1, 1);
		} else if (scroll < 0.0f) {
			throttle = Mathf.Clamp(throttle - 1, -1, 1);
		}
	}

	private void Move() {
		rigidbody.MovePosition(transform.position + throttle * playerCamera.transform.forward * moveSpeed * Time.fixedDeltaTime);
	}

}
