using UnityEngine;

public class VehicleController : MonoBehaviour {

	public float maxForwardSpeed;
	public float maxAcceleration;
	public float maxDeceleration;
	public float maxAngularSpeed;

	public Waypoint nextWaypoint;
	public Waypoint nextNextWaypoint;

	private new Rigidbody rigidbody;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
	}

	/*void Update() {
		float steerAngle = Navigate();

		Logger.Log(steerAngle.ToString());

		rigidbody.AddForce(
			steerVector * Mathf.Lerp(maxAcceleration, 0.0f, Vector3.Dot(rigidbody.velocity, transform.forward) / maxForwardSpeed * steerAngle / 180.0f),
			ForceMode.Acceleration);

		
	}*/

	void FixedUpdate() {
		Navigate();
	}

	private Vector2 steerVector;
	private Vector2 vectorToNextWaypoint;
	private Vector2 forwardDirectionVector;

	private void Navigate() {
		Vector2 nextWaypointPositionVector = Utils.Flatten(nextWaypoint.transform.position);
		Vector2 vehiclePositionVector = Utils.Flatten(transform.position);

		Vector2 desiredVelocity = (nextWaypointPositionVector - vehiclePositionVector).normalized * maxForwardSpeed;
		Vector2 steering = Vector2.ClampMagnitude(desiredVelocity - Utils.Flatten(rigidbody.velocity), maxAcceleration);

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity + Utils.Unflatten(steering) * Time.fixedDeltaTime, maxForwardSpeed);

		if (Vector2.Distance(Utils.Flatten(transform.position), Utils.Flatten(nextWaypoint.transform.position)) < 1.0f) {
			nextWaypoint = nextNextWaypoint;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + Utils.Unflatten(vectorToNextWaypoint));

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position + Utils.Unflatten(forwardDirectionVector * 20));

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + Utils.Unflatten(steerVector * 20));
	}

}