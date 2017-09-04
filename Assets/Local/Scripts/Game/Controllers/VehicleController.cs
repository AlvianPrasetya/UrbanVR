using UnityEngine;
using System.Linq;
using System.Collections;

public class VehicleController : MonoBehaviour {

	public float maxAcceleration;
	public float maxDeceleration;
	public float maxVelocity;
	public float maxAvoidAcceleration;

	public float lookAheadDistance;
	public float lookAheadRadius;

	public float queueAheadDistance;
	public float queueAheadRadius;

	public float waypointReachedDistance;

	public Waypoint prevWaypoint;
	public Waypoint nextWaypoint;

	private new Rigidbody rigidbody;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
	}

	void Update() {
		Steer();
	}

	private void Steer() {
		Vector2 steeringAcceleration = Vector2.zero;
		steeringAcceleration += FollowPath();
		steeringAcceleration += AvoidCollision();
		//Queue();

		steeringAcceleration = Vector2.ClampMagnitude(steeringAcceleration, maxAcceleration);

		rigidbody.AddForce(Utils.Unflatten(steeringAcceleration), ForceMode.Acceleration);
		
		Quaternion currentRotation = Quaternion.LookRotation(transform.forward, Vector2.up);
		Quaternion targetRotation = Quaternion.LookRotation(Utils.Unflatten(Utils.Flatten(rigidbody.velocity)), Vector2.up);

		rigidbody.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, 0.1f));
	}
	
	private Vector2 FollowPath() {
		Vector2 vehiclePosition = Utils.Flatten(transform.position);
		Vector2 waypointPosition = Utils.Flatten(nextWaypoint.transform.position);

		if (Vector2.SqrMagnitude(waypointPosition - vehiclePosition) < waypointReachedDistance * waypointReachedDistance) {
			// Vehicle has arrived at the next waypoint, update next waypoint
			foreach (Waypoint waypoint in nextWaypoint.neighbours) {
				if (!waypoint.Equals(prevWaypoint)) {
					prevWaypoint = nextWaypoint;
					nextWaypoint = waypoint;
					break;
				}
			}
		}

		return Seek(Utils.Flatten(nextWaypoint.transform.position));
	}

	private Vector2 AvoidCollision() {
		Vector2 avoidAcceleration = Vector2.zero;

		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position, lookAheadRadius, transform.forward, out hitInfo, lookAheadDistance, Utils.Layer.VEHICLE | Utils.Layer.OBSTACLE | Utils.Layer.INTERACTABLE)) {
			Vector2 vehiclePosition = Utils.Flatten(transform.position);
			Vector2 vehicleDirection = Utils.Flatten(transform.forward).normalized;
			Vector2 threatPosition = Utils.Flatten(hitInfo.transform.position);
			Vector2 threatDirection = (threatPosition - vehiclePosition).normalized;

			float direction = Utils.Direction(vehicleDirection, threatDirection);
			// Check whether threat is to the left or right of the forward direction
			if (direction <= 0.0f) {
				return Utils.Flatten(-transform.right).normalized * Mathf.Lerp(maxAvoidAcceleration, 0.0f, hitInfo.distance / lookAheadDistance);
			} else {
				return Utils.Flatten(transform.right).normalized * Mathf.Lerp(maxAvoidAcceleration, 0.0f, hitInfo.distance / lookAheadDistance);
			}
		}

		return avoidAcceleration;
	}

	private Vector2 Queue() {
		Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * queueAheadDistance, queueAheadRadius, Utils.Layer.VEHICLE)
			.OrderBy(h => Vector2.SqrMagnitude(Utils.Flatten(h.transform.position) - Utils.Flatten(transform.position))).ToArray();

		Vector2 selfPosition = Utils.Flatten(transform.position);
		Vector2 selfDirection = Utils.Flatten(rigidbody.velocity).normalized;
		foreach (Collider collider in colliders) {
			if (collider.Equals(GetComponent<Collider>())) {
				continue;
			}

			Vector2 threatPosition = Utils.Flatten(collider.transform.position);
			Vector2 threatDirection = Utils.Flatten(collider.GetComponent<VehicleController>().rigidbody.velocity).normalized;

			float det = threatDirection.x * selfDirection.y - selfDirection.x * threatDirection.y;
			float selfDistanceToIntersection = 1.0f / det * ((selfPosition.x - threatPosition.x) * threatDirection.y - (selfPosition.y - threatPosition.y) * threatDirection.x);
			float threatDistanceToIntersection = 1.0f / det * ((selfPosition.x - threatPosition.x) * selfDirection.y - (selfPosition.y - threatPosition.y) * selfDirection.x);

			if (threatDistanceToIntersection > 0.0f && selfDistanceToIntersection > 0.0f) {
				if (selfDistanceToIntersection > threatDistanceToIntersection) {
					// Slow down
					return -rigidbody.velocity.normalized * Mathf.Lerp(0, maxDeceleration, rigidbody.velocity.magnitude / maxVelocity);
				}
			}
		}

		return Vector2.zero;
	}

	private Vector2 Seek(Vector2 targetPosition) {
		Vector2 desiredVelocity = (targetPosition - Utils.Flatten(transform.position)).normalized * maxVelocity;
		Vector2 steeringAcceleration = Vector2.ClampMagnitude(desiredVelocity - Utils.Flatten(rigidbody.velocity), maxAcceleration);

		return steeringAcceleration;
	}

}