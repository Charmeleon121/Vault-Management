using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
	// Input System object and actions
	private InputSystem_Actions keybinds;
	private InputAction buildMenuAction;

	// Text objects on the UI
	public TextMeshProUGUI fpsDisplay, storageDisplay, IODisplay;

	// Pause menu
	public GameObject pauseMenu;

	// Options menu
	public GameObject optionsMenu;
	private TMP_Dropdown resolutionDropdown;
	private Toggle fullScreenToggle, vSyncToggle;
	private TMP_InputField fpsEntry;
	
	// Build menu
	public GameObject buildMenu;
	
	// Timer for updating the UI elements
	private int updateTimer;
	
	void Start() {
		keybinds = new();
		keybinds.Player.Enable();

		buildMenuAction = InputSystem.actions.FindAction("Player/BuildMenu");

		resolutionDropdown = optionsMenu.transform.Find("Resolution Dropdown").gameObject.GetComponent<TMP_Dropdown>();
		fullScreenToggle = optionsMenu.transform.Find("Toggle Fullscreen").gameObject.GetComponent<Toggle>();
		vSyncToggle = optionsMenu.transform.Find("Toggle VSync").gameObject.GetComponent<Toggle>();
		fpsEntry = optionsMenu.transform.Find("Target FPS Entry").gameObject.GetComponent<TMP_InputField>();

		resolutionDropdown.value = 4;
		fullScreenToggle.isOn = true;
		vSyncToggle.isOn = false;
		fpsEntry.text = "60";

		updateTimer = 0;
		Application.targetFrameRate = 60;
		
		buildMenu.transform.position = new Vector3(-1000f, 0f, 0f);
		pauseMenu.transform.position = new Vector3(-1000f, 0f, 0f);
		optionsMenu.transform.position = new Vector3(-1000f, 0f, 0f);
	}
	
	/*
	 * On Update:
	 *
	 * - Update the FPS display
	 * - Handle the update timer to ensure UI elements aren't updating every frame
	 */
	void Update() {
		ToggleBuildMenu();

		if (updateTimer == Mathf.RoundToInt(Application.targetFrameRate / 2)) {
			DisplayFPS();
			updateTimer = 0;
		} else {
			++updateTimer;
		}
	}

	private void ToggleBuildMenu() {
		if (buildMenuAction.triggered) {
			if (buildMenu.transform.position.x == (200f / 1920f) * Screen.width) {
				buildMenu.transform.position = new Vector3(-1000f, 0f, 0f);
			} else {
				buildMenu.transform.position = new Vector3((200f / 1920f) * Screen.width, (540f / 1080f) * Screen.height, 0f);
			}
		}
	}
	
	private void DisplayFPS() {
		float fpsValue = 1f / Time.deltaTime;
		fpsDisplay.text = $"FPS: {fpsValue:n2}/{Application.targetFrameRate}";
			
		if (fpsValue <= Application.targetFrameRate * 0.25f) {
			fpsDisplay.color = Color.red;
		} else if (fpsValue > Application.targetFrameRate * 0.25f && fpsValue <= Application.targetFrameRate * 0.5f) {
			fpsDisplay.color = Color.orange;
		} else if (fpsValue > Application.targetFrameRate * 0.5f && fpsValue <= Application.targetFrameRate * 0.75f) {
			fpsDisplay.color = Color.yellow;
		} else {
			fpsDisplay.color = Color.green;
		}	
	}

	public void DisplayStoredAndIO(float storedPower, float maxPower, float powerIn, float powerOut, float storedFood, float maxFood, float storedWater, float maxWater) {
		storageDisplay.text = $"Power: {storedPower:n2}/{maxPower:n2}\n\nFood:  {storedFood:n2}/{maxFood:n2}\n\nWater: {storedWater:n2}/{maxWater:n2}";
		IODisplay.text = $"I: {powerIn:n2} | O: {powerOut:n2}";
	}

	public void TogglePause(bool state) {
		if (state) {
			Time.timeScale = 0f;
			pauseMenu.transform.position = new Vector3((960f / 1920f) * Screen.width, (540f / 1080f) * Screen.height, 0f);
		} else {
			Time.timeScale = 1f;
			pauseMenu.transform.position = new Vector3(-1000f, 0f, 0f);
		}
	}

	public void ToggleOptions(bool state) {
		if (state) {
			optionsMenu.transform.position = new Vector3((960f / 1920f) * Screen.width, (540f / 1080f) * Screen.height, 0f);
			pauseMenu.transform.position = new Vector3(-1000f, 0f, 0f);
		} else {
			optionsMenu.transform.position = new Vector3(-1000f, 0f, 0f);
			pauseMenu.transform.position = new Vector3((960f / 1920f) * Screen.width, (540f / 1080f) * Screen.height, 0f);
		}
	}

	public void ApplyOptions() {
		string[] resolution = resolutionDropdown.options[resolutionDropdown.value].text.Split(" x ");
		Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), fullScreenToggle.isOn);

		try {
			Application.targetFrameRate = int.Parse(fpsEntry.text);
		} catch (FormatException) {
			// If the player left the field blank, default the target FPS to an "uncapped" value of 1000
			Application.targetFrameRate = 1000;
		}

		QualitySettings.vSyncCount = Convert.ToInt32(vSyncToggle.isOn);

		ToggleOptions(false);
	}

	public void QuitGame() {
		// Implement saving the game
		Application.Quit();
	}
}