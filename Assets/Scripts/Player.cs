using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	// Input System object
	private InputSystem_Actions keybinds;

	// Variables relating to player movement
	private InputAction moveAction;
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
	 * - Instantiate the Input System actions
	 * - Unpause the game
	 * - Set the game speed to 1x
	 */
	void Start() {
		moveAction = InputSystem.actions.FindAction("Player/Move");

		isPaused = false;
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

	private void SwapPauseState() {
		isPaused = !isPaused;
		Time.timeScale = Convert.ToInt32(isPaused);
	}
}
