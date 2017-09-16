using UnityEngine;

/**
 * This script controls the complete behaviour of a day/night cycle including 
 * sun and moon movements.
 */
public class DayNightManager : MonoBehaviour {

	public enum TimeMode {
		SECOND_TO_SECOND, SECOND_TO_MINUTE, SECOND_TO_HOUR
	}

	public Transform sunMoonPivot;

	public Light sun;
	public float sunSize;

	public Light moon;
	public float moonSize;
	
	// The initial time of day in hours since 00:00
	public float initialTime;

	public TimeMode timeMode;

	// Respectively describes the maximum intensity of the sun/moon lights
	public float maxSunLightIntensity;
	public float maxMoonLightIntensity;

	// The time (in hours since 00:00) in which the sun reaches its peak
	public float peakSunIntensityTime;

	public float sunshineDuration;
	public float moonshineDuration;

	public float peakDayExposure;
	public float peakNightExposure;

	public float peakDayAtmosphereThickness;
	public float peakNightAtmosphereThickness;

	// The time (in hours since 00:00) in which the moon reaches its peak (always peakSunIntensityTime + 12h)
	private float peakMoonIntensityTime;

	// The current time of day in hours since 00:00
	private float timeOfDay;

	void Awake() {
		peakMoonIntensityTime = AddHours(peakSunIntensityTime, 12.0f);

		timeOfDay = initialTime;
	}

	void Update() {
		UpdateTimeOfDay();
		UpdateLightSource();
		UpdateSunMoonRotation();

		InputToggleTimeMode();
	}

	public float TimeOfDay {
		get {
			return timeOfDay;
		}
	}

	public void ToggleTimeMode() {
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

	private void UpdateTimeOfDay() {
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

		timeOfDay = AddHours(timeOfDay, simulatedTimeRatio * Time.deltaTime * Utils.Value.HOURS_IN_SECOND);
	}

	private void UpdateLightSource() {
		float hoursFromSunPeak = CalculateHoursBetween(timeOfDay, peakSunIntensityTime);
		float hoursFromMoonPeak = CalculateHoursBetween(timeOfDay, peakMoonIntensityTime);

		if (hoursFromSunPeak < hoursFromMoonPeak) {
			RenderSettings.sun = sun;
			RenderSettings.skybox.SetFloat("_SunSize", sunSize);
		} else {
			RenderSettings.sun = moon;
			RenderSettings.skybox.SetFloat("_SunSize", moonSize);
		}

		RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(peakDayExposure, peakNightExposure, hoursFromSunPeak / 12.0f));
		RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(peakDayAtmosphereThickness, peakNightAtmosphereThickness, hoursFromSunPeak / 12.0f));

		float relativeSunIntensity = 1.0f - Mathf.Pow(hoursFromSunPeak / sunshineDuration, 2.0f);
		sun.intensity = relativeSunIntensity * maxSunLightIntensity;

		float relativeMoonIntensity = 1.0f - Mathf.Pow(hoursFromMoonPeak / moonshineDuration, 2.0f);
		moon.intensity = relativeMoonIntensity * maxMoonLightIntensity;
	}

	private void UpdateSunMoonRotation() {
		float zAngle = timeOfDay * 15.0f - 90.0f;
		sunMoonPivot.rotation = Quaternion.Euler(sunMoonPivot.eulerAngles.x, sunMoonPivot.eulerAngles.y, zAngle);
	}

	private void InputToggleTimeMode() {
		if (Input.GetKeyDown(KeyCode.T)) {
			ToggleTimeMode();
		}
	}

	private float CalculateHoursBetween(float timeA, float timeB) {
		if (timeA > timeB) {
			float temp = timeA;
			timeA = timeB;
			timeB = temp;
		}

		return Mathf.Min(timeB - timeA, 24.0f + timeA - timeB);
	}

	private float CalculateHoursFromTo(float from, float to) {
		if (to > from) {
			return to - from;
		} else {
			return to + 24.0f - from;
		}
	}

	private float AddHours(float time, float hoursToAdd) {
		time += hoursToAdd;
		if (time >= 24.0f) {
			time -= 24.0f;
		}

		return time;
	}

}
