using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	// UIHandler script
	private UIHandler uiHandler;

	// Input System object and actions
	private InputSystem_Actions keybinds;
	private InputAction moveAction, pauseAction;

	// Movement speed multiplier for the camera
	private readonly float speed = 0.25f;

	// Pause state
	private bool isPaused;

	/*
	 * On Awake:
	 * 
	 * - Ensure the GameObject won't be destroyed when changing scenes (probably unnecessary, but keeping it as future-proofing just in case)
	 * - Instantiate and enable the Input System
	 */
	void Awake() {
		DontDestroyOnLoad(gameObject);

		keybinds = new();
		keybinds.Player.Enable();
	}

	/*
	 * On Start:
	 * 
	 * - Instantiate any external scripts
	 * - Instantiate the Input System actions
	 * - Unpause the game
	 * - Set the game speed to 1x
	 */
	void Start() {
		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();

		moveAction = InputSystem.actions.FindAction("Player/Move");
		pauseAction = InputSystem.actions.FindAction("Player/Pause");

		isPaused = false;
	}

	/*
	 * On Update:
	 * 
	 * - Allow the player to toggle the pause state of the game
	 */
	void Update() {
		if (pauseAction.triggered) {
			isPaused = !isPaused;
			uiHandler.TogglePause(isPaused);
		}
	}

	/*
	 * On FixedUpdate:
	 * 
	 * - Move the camera
	 * - Allow the player to change the game speed (options: 1x, 2x, 5x, 10x)
	 */
	void FixedUpdate() {
		Move();
	}

	private void Move() {
		Vector2 inputVector = moveAction.ReadValue<Vector2>();
		Vector3 moveVector = (transform.up * inputVector.y) + (transform.right * inputVector.x);

		if (moveVector.magnitude > 0f) {
			transform.position += speed * moveVector.normalized;
		}
	}
}
