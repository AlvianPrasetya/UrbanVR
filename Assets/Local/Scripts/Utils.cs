using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {
	
	public static readonly string GAME_VERSION = "v0.1";

	public static readonly float RESPAWN_TIME = 5.0f;

	public class Network {
		public static readonly int SEND_RATE = 15;
		public static readonly int SEND_RATE_ON_SERIALIZE = 15;

		// The base delay (ms) used to sync serializations between clients
		public static int BASE_SYNC_DELAY = 67;
		public static int SYNC_BUFFER_SIZE = 10;
	}

	public class Scene {
		public static readonly string LOGIN = "Login";
		public static readonly string LOBBY = "Lobby";
		public static readonly string ROOM = "Room";
		public static readonly string GAME = "Demo";
	}

	public class Resource {
		public static readonly string PLAYER = "Player";
	}

	public class Tag {
		public static readonly string TERRAIN = "Terrain";
		public static readonly string OBSTACLE = "Obstacle";
		public static readonly string VEHICLE = "Vehicle";
	}

	public class Layer {
		public static readonly int TERRAIN = 1 << LayerMask.NameToLayer("Terrain");
		public static readonly int OBSTACLE = 1 << LayerMask.NameToLayer("Obstacle");
		public static readonly int VEHICLE = 1 << LayerMask.NameToLayer("Vehicle");
	}

	public class Key {
		public static readonly string GAME_MODE = "GameMode";
		public static readonly string PING = "Ping";
	}

	public class Input {
		public static readonly string HORIZONTAL = "Horizontal";
		public static readonly string VERTICAL = "Vertical";
		public static readonly string MOUSE_X = "Mouse X";
		public static readonly string MOUSE_Y = "Mouse Y";
		public static readonly int MOUSE_BUTTON_LEFT = 0;
		public static readonly int MOUSE_BUTTON_RIGHT = 1;
		public static readonly int MOUSE_BUTTON_MIDDLE = 2;
		public static readonly KeyCode[] KEY_CODES_CHANGE_WEAPON = {
			KeyCode.Alpha1, 
			KeyCode.Alpha2, 
			KeyCode.Alpha3
		};
	}

	public class Physics {
		// Gravitational constant (big G)
		public static readonly float G = 6.754e-11f;
	}

	public class Value {
		public static readonly float PI = Mathf.PI;
		public static readonly float HOURS_IN_SECOND = 1.0f / 3600.0f;
		public static readonly float EPS = 0.001f;
	}

	/**
	 * This method "flattens" a 3D vector into its 2D form by removing 
	 * the y-component (up) of the vector.
	 */
	public static Vector2 Flatten(Vector3 vector) {
		return new Vector2(vector.x, vector.z);
	}

	/**
	 * This method "unflattens" a 2D vector into its 3D form by adding in 
	 * the y-component (up) to the vector.
	 */
	public static Vector3 Unflatten(Vector2 vector, float y = 0.0f) {
		return new Vector3(vector.x, y, vector.y);
	}

	public static IEnumerator TransformLerpPosition(Transform targetTransform, Vector3 startPosition,
		Vector3 endPosition, float lerpTime) {
		float time = 0.0f;
		while (time < 1.0f) {
			targetTransform.localPosition = Vector3.Lerp(
				startPosition, 
				endPosition, 
				time
			);

			time += Time.deltaTime / lerpTime;
			yield return null;
		}

		targetTransform.localPosition = endPosition;
	}

	public static IEnumerator TransformSlerpRotation(Transform targetTransform, Quaternion startRotation,
		Quaternion endRotation, float slerpTime) {
		float time = 0.0f;
		while (time < 1.0f) {
			targetTransform.localRotation = Quaternion.Slerp(
				startRotation, 
				endRotation, 
				time
			);

			time += Time.deltaTime / slerpTime;
			yield return null;
		}

		targetTransform.localRotation = endRotation;
	}

	public static IEnumerator CameraLerpFieldOfView(Camera targetCamera, float startFieldOfView,
		float endFieldOfView, float lerpTime) {
		float time = 0.0f;
		while (time < 1.0f) {
			targetCamera.fieldOfView = Mathf.Lerp(
				startFieldOfView,
				endFieldOfView,
				time
			);

			time += Time.deltaTime / lerpTime;
			yield return null;
		}

		targetCamera.fieldOfView = endFieldOfView;
	}

}

public enum GameMode {
	
	FREE_FOR_ALL = 0, 
	TEAM_DEATHMATCH = 1, 
	CAPTURE_THE_FLAG = 2

}