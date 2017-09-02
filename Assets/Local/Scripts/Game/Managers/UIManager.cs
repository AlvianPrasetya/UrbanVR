using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * This class controls UI behaviour during the game.
 */
public class UIManager : MonoBehaviour {

	public delegate void OnChatMessageSentCallback(PhotonPlayer sendingPlayer, string message);

	public Text targetInfoText;
	public Text standingsText;
	public Text announcementText;
	public Text chatText;
	public InputField chatInputField;

	public float announcementFadeTime;
	
	private OnChatMessageSentCallback chatMessageSentCallback;

	void Awake() {
		chatInputField.interactable = false;
	}
	
	void Update() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Locked;
		
		InputCheckChatInputField();
	}

	public void AddChatMessageSentCallback(OnChatMessageSentCallback chatMessageSentCallback) {
		if (this.chatMessageSentCallback == null) {
			this.chatMessageSentCallback = chatMessageSentCallback;
		} else {
			this.chatMessageSentCallback += chatMessageSentCallback;
		}
	}

	public void Announce(string announcementString) {
		announcementText.text = announcementString;
		StartCoroutine(FadeAnnouncementCoroutine());
	}

	public void ResetCursor() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void UpdateStandingsText(string standingsString) {
		standingsText.text = standingsString;
	}

	public void UpdateChatText(string chatString) {
		chatText.text = chatString;
	}

	private void InputCheckChatInputField() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			CheckChatInputField();
		}
	}

	private void CheckChatInputField() {
		if (chatInputField.interactable) {
			SendChatMessage();
			chatInputField.DeactivateInputField();
			chatInputField.interactable = false;
		} else {
			chatInputField.interactable = true;
			chatInputField.Select();
			chatInputField.ActivateInputField();
		}
	}

	private void SendChatMessage() {
		if (chatInputField.text != "") {
			Logger.Log("Sending chat message " + chatInputField.text);

			if (chatMessageSentCallback != null) {
				chatMessageSentCallback(PhotonNetwork.player, chatInputField.text);
			}

			chatInputField.text = "";
		}
	}

	private IEnumerator FadeAnnouncementCoroutine() {
		float currentFadeTime = announcementFadeTime;

		while (currentFadeTime > 0.0f) {
			announcementText.color = new Color(
				announcementText.color.r, 
				announcementText.color.g, 
				announcementText.color.b, 
				currentFadeTime / announcementFadeTime);
			currentFadeTime -= Time.deltaTime;
			yield return null;
		}

		// Ensure alpha is 0 before coroutine is finished
		announcementText.color = new Color(
				announcementText.color.r,
				announcementText.color.g,
				announcementText.color.b,
				0.0f);
	}

}
