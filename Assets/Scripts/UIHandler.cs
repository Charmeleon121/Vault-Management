using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIHandler : MonoBehaviour {
	// Input System object
	private InputSystem_Actions keybinds;

	// Player object
	private Player player;

	// Text objects on the UI
	public TextMeshProUGUI fpsDisplay;
	
	// Build menu
	private InputAction buildMenuAction;
	public GameObject buildMenu;
	
	// Timer for updating the UI elements
	private int updateTimer;
	
	void Start() {
		keybinds = new();
		keybinds.Player.Enable();

		player = GameObject.Find("Main Camera").GetComponent<Player>();
		
		buildMenuAction = InputSystem.actions.FindAction("Player/BuildMenu");
		
		updateTimer = 0;
		Application.targetFrameRate = 60;
		
		buildMenu.transform.position = new Vector3(-200f, 540f, 0f);
	}
	
	/*
	 * On Update:
	 *
	 * - Update the FPS display
	 * - Handle the update timer to ensure UI elements aren't updating every frame
	 */
	void Update() {
		ToggleBuildMenu();

		if (updateTimer == 30) {
			DisplayFPS();
			updateTimer = 0;
		} else {
			++updateTimer;
		}
	}

	private void ToggleBuildMenu() {
		if (buildMenuAction.triggered) {
			if (buildMenu.transform.position.x == 200f) {
				buildMenu.transform.position = new Vector3(-200f, 540, 0f);
			} else {
				buildMenu.transform.position = new Vector3(200f, 540, 0f);
			}
		}
	}
	
	private void DisplayFPS() {
		float fpsValue = 1f / Time.deltaTime;
		string fpsString = fpsValue.ToString("n2");
		fpsDisplay.text = $"FPS: {fpsString}/{Application.targetFrameRate}";
			
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
}