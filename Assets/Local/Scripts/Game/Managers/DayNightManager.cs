using UnityEngine;

/**
 * This script controls the complete behaviour of a day/night cycle including 
 * sun and moon movements.
 */
public class DayNightManager : MonoBehaviour {

	public enum TimeMode {
		SECOND_TO_SECOND, SECOND_TO_MINUTE, SECOND_TO_HOUR
	}
	
	public Transform sun;
	public float sunSize;

	public Transform moon;
	public float moonSize;
	
	// The initial time of day in hours since 00:00
	public float initialTime;

	// Respectively describes the maximum intensity of the sun/moon lights
	public float maxSunLightIntensity;
	public float maxMoonLightIntensity;

	// The time (in hours since 00:00) in which the sun/moon reaches their peaks
	public float peakSunIntensityTime;
	public float peakMoonIntensityTime;

	public float sunshineDuration;
	public float moonshineDuration;

	public float peakDayExposure;
	public float peakNightExposure;

	private Light sunLight;
	private Light moonLight;

	// The current time of day in hours since 00:00
	private float timeOfDay;

	private TimeMode timeMode;

	void Awake() {
		sunLight = sun.GetComponent<Light>();
		moonLight = moon.GetComponent<Light>();

		timeOfDay = initialTime;
		timeMode = TimeMode.SECOND_TO_SECOND;
	}

	void Update() {
		float hoursPassedSinceLastUpdate = UpdateTimeOfDay();
		UpdateSunPosition(hoursPassedSinceLastUpdate);
		UpdateMoonPosition(hoursPassedSinceLastUpdate);

		InputToggleTimeMode();
	}

	/**
	 * This method updates the time of day and returns the delta since the last 
	 * time this function is called.
	 */
	private float UpdateTimeOfDay() {
		float hoursPassedSinceLastUpdate = CalculateHoursPassedSinceLastUpdate();
		timeOfDay += hoursPassedSinceLastUpdate;
		if (timeOfDay >= 24.0f) {
			timeOfDay -= 24.0f;
		}

		float hoursFromSunPeak = CalculateTimeDifference(timeOfDay, peakSunIntensityTime);
		float hoursFromMoonPeak = CalculateTimeDifference(timeOfDay, peakMoonIntensityTime);
		if (hoursFromSunPeak < hoursFromMoonPeak) {
			RenderSettings.sun = sunLight;
			RenderSettings.skybox.SetFloat("_SunSize", sunSize);
		} else {
			RenderSettings.sun = moonLight;
			RenderSettings.skybox.SetFloat("_SunSize", moonSize);
		}

		RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(peakDayExposure, peakNightExposure, hoursFromSunPeak / 12.0f));

		float relativeSunIntensity = 1.0f - Mathf.Pow(hoursFromSunPeak / sunshineDuration, 2.0f);
		sunLight.intensity = relativeSunIntensity * maxSunLightIntensity;

		float relativeMoonIntensity = 1.0f - Mathf.Pow(hoursFromMoonPeak / moonshineDuration, 2.0f);
		moonLight.intensity = relativeMoonIntensity * maxMoonLightIntensity;

		return hoursPassedSinceLastUpdate;
	}

	private void UpdateSunPosition(float deltaTime) {
		sun.RotateAround(Vector3.zero, Vector3.right, 360.0f / 24.0f * deltaTime);
	}

	private void UpdateMoonPosition(float deltaTime) {
		moon.RotateAround(Vector3.zero, Vector3.right, 360.0f / 24.0f * deltaTime);
	}

	private void InputToggleTimeMode() {
		if (Input.GetKeyDown(KeyCode.T)) {
			switch (timeMode) {
				case TimeMode.SECOND_TO_SECOND:
					timeMode = TimeMode.SECOND_TO_MINUTE;
					break;
				case TimeMode.SECOND_TO_MINUTE:
					timeMode = TimeMode.SECOND_TO_HOUR;
					break;
				case TimeMode.SECOND_TO_HOUR:
					timeMode = TimeMode.SECOND_TO_SECOND;
					break;
				default:
					break;
			}
		}
	}

	private float CalculateHoursPassedSinceLastUpdate() {
		float simulatedTimeRatio;
		switch (timeMode) {
			case TimeMode.SECOND_TO_SECOND:
				simulatedTimeRatio = 1;
				break;
			case TimeMode.SECOND_TO_MINUTE:
				simulatedTimeRatio = 60;
				break;
			case TimeMode.SECOND_TO_HOUR:
				simulatedTimeRatio = 3600;
				break;
			default:
				simulatedTimeRatio = 1;
				break;
		}

		return simulatedTimeRatio * Time.deltaTime * Utils.Value.HOURS_IN_SECOND;
	}

	private float CalculateTimeDifference(float timeA, float timeB) {
		if (timeA > timeB) {
			float temp = timeA;
			timeA = timeB;
			timeB = temp;
		}

		return Mathf.Min(timeB - timeA, 24.0f + timeA - timeB);
	}

}
