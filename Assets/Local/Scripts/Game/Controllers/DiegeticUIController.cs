using UnityEngine;
using UnityEngine.UI;

public class DiegeticUIController : MonoBehaviour {

	public Text timeOfDayText;

	void Update() {
		UpdateTimeOfDay();
	}

	private void UpdateTimeOfDay() {
		float timeOfDay = GameManagerBase.Instance.dayNightManager.TimeOfDay;
		int hour = Mathf.FloorToInt(timeOfDay);
		int minute = Mathf.FloorToInt((timeOfDay - Mathf.FloorToInt(timeOfDay)) * 60.0f);

		timeOfDayText.text = hour.ToString("D2") + ":" + minute.ToString("D2");
	}

}
