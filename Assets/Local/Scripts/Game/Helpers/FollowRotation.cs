using UnityEngine;

public class FollowRotation : MonoBehaviour {

	public Transform targetTransform;

	public bool followXAxis;
	public bool followYAxis;
	public bool followZAxis;

	void Update() {
		transform.rotation = Quaternion.Euler(
			followXAxis ? targetTransform.eulerAngles.x : transform.eulerAngles.x, 
			followYAxis ? targetTransform.eulerAngles.y : transform.eulerAngles.y, 
			followZAxis ? targetTransform.eulerAngles.z : transform.eulerAngles.z);
	}

}
