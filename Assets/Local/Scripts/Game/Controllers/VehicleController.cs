using UnityEngine;

public class VehicleController : MonoBehaviour {

	public float maxAcceleration;
	public float maxVelocity;
	public float maxAvoidAcceleration;
	public float lookAheadRadius;
	public float lookAheadDistance;
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
		steeringAcceleration += Queue();

		steeringAcceleration = Vector2.ClampMagnitude(steeringAcceleration, maxAcceleration);

		rigidbody.AddForce(Utils.Unflatten(steeringAcceleration), ForceMode.Acceleration);
		
		Quaternion currentRotation = Quaternion.LookRotation(transform.forward, Vector2.up);
		Quaternion targetRotation = Quaternion.LookRotation(Utils.Unflatten(Utils.Flatten(rigidbody.velocity)), Vector2.up);

		transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 0.1f);
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
		if (Physics.SphereCast(transform.position, lookAheadRadius, transform.forward, out hitInfo, lookAheadDistance, Utils.Layer.VEHICLE | Utils.Layer.OBSTACLE)) {
			Vector2 threatPosition = Utils.Flatten(hitInfo.transform.position);
			Vector2 lookAheadPosition = Utils.Flatten(transform.position + transform.forward * lookAheadDistance);

			avoidAcceleration = (lookAheadPosition - threatPosition).normalized * maxAvoidAcceleration;
		}

		return avoidAcceleration;
	}

	private Vector2 Queue() {
		return Vector2.zero;
	}

	private Vector2 Seek(Vector2 targetPosition) {
		Vector2 desiredVelocity = (targetPosition - Utils.Flatten(transform.position)).normalized * maxVelocity;
		Vector2 steeringAcceleration = Vector2.ClampMagnitude(desiredVelocity - Utils.Flatten(rigidbody.velocity), maxAcceleration);

		return steeringAcceleration;
	}

}