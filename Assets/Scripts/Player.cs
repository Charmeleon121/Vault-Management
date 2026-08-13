using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	// Input System object
	private InputSystem_Actions keybinds;

	// Variables relating to player movement
	private InputAction moveAction;
	private readonly float speed = 0.25f;

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
	 * - Instantiate the movement action (WASD/joystick/etc.) from the Input System
	 */
	void Start() {
		moveAction = InputSystem.actions.FindAction("Player/Move");
	}

	/*
	 * On FixedUpdate:
	 * 
	 * - Move the camera
	 */
	void FixedUpdate() {
		Vector2 inputVector = moveAction.ReadValue<Vector2>();
		Vector3 moveVector = (transform.up * inputVector.y) + (transform.right * inputVector.x);

		if (moveVector.magnitude > 0f) {
			transform.position += speed * moveVector.normalized;
		}
	}
}
