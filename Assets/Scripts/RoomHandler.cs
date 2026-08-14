using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomHandler : MonoBehaviour {
	// Input System object
	private InputSystem_Actions keybinds;
	
	private InputAction clickAction;
	
	// The highest valued ID currently in use - a new room would use this value + 1
	private int currentMaxID;

	private List<List<GameObject>> layers = new List<List<GameObject>>();
	
	// The room prefabs available
	public GameObject normalRoomGhost;
	public GameObject normalRoomPrefab;
	
	/*
	 * On Start:
	 *
	 * - Initialize the maxID to 0 (replace this in future when loading saved games is implemented!)
	 */
	void Start() {
		keybinds = new();
		keybinds.Player.Enable();
		
		clickAction = InputSystem.actions.FindAction("Player/Click");
		
		currentMaxID = 0;
		
		List<GameObject> startLayer = new List<GameObject>();
		startLayer.Add(GameObject.FindWithTag("Ladder"));
		layers.Add(startLayer);
	}
	
	/*
	 * On FixedUpdate:
	 * 
	 * - Move the current ghost object in such a way that it snaps to nearby rooms for easy construction
	 * - When there is a ghost object in play, and the player clicks, replace it with a new room at that position
	 * - Ensure the new room has an incremented ID number, and that it's been added to the List of available rooms
	 */
	void FixedUpdate() {
		GameObject activeGhost = GameObject.FindWithTag("Ghost");
		
		if (activeGhost != null) {
			Vector2 rawMousePos = Mouse.current.position.ReadValue();
			float depth = Vector3.Dot(new Vector3(0f, 0f, 0f) - Camera.main.transform.position, Camera.main.transform.forward);
			Vector3 mousePos = new Vector3(rawMousePos.x, rawMousePos.y, depth);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			worldPos.x = 0f;
			
			Tuple<GameObject, int> closestRoomData = FindClosestRoom(worldPos);
			GameObject closestRoom = closestRoomData.Item1;
			int roomYPos = closestRoomData.Item2;

			char doorSide;
			Vector3 snapPosition;
			if (worldPos.z < closestRoom.transform.position.z) {
				// Place to the right of the closest room
				snapPosition = new Vector3(0f, closestRoom.transform.position.y, (closestRoom.transform.position.z - (closestRoom.transform.localScale.z * 2f)) - (activeGhost.transform.localScale.z * 2f));
				doorSide = 'R';
			} else {
				// Place to the left of the closest room
				snapPosition = new Vector3(0f, closestRoom.transform.position.y, (closestRoom.transform.position.z + (closestRoom.transform.localScale.z * 2f)) + (activeGhost.transform.localScale.z * 2f));
				doorSide = 'L';
			}
		
			activeGhost.transform.position = snapPosition;
			
			if (clickAction.triggered) {
				GameObject room = Instantiate(normalRoomPrefab, snapPosition, Quaternion.Euler(0f, 0f, 0f));
				room.GetComponent<Room>().ID = currentMaxID + 1;

				// Destroy the door closest to the newly placed room
				if (doorSide == 'L') {
					DestroyImmediate(closestRoom.transform.Find("Door L").gameObject);
					DestroyImmediate(room.transform.Find("Door R").gameObject);
				} else {
					DestroyImmediate(closestRoom.transform.Find("Door R").gameObject);
					DestroyImmediate(room.transform.Find("Door L").gameObject);
				}

				if (layers.Count < roomYPos) {
					List<GameObject> layer = new List<GameObject>();
					layer.Add(room);
					layers.Add(layer);
				} else {
					layers[roomYPos].Add(room);
				}
				
				++currentMaxID;
				Destroy(activeGhost);
			}
		}
	}
	
	public void PlaceGhostRoom(int size) {
		Vector2 rawMousePos = Mouse.current.position.ReadValue();
		float depth = Vector3.Dot(new Vector3(0f, 0f, 0f) - Camera.main.transform.position, Camera.main.transform.forward);
		Vector3 mousePos = new Vector3(rawMousePos.x, rawMousePos.y, depth);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
		worldPos.x = 0f;
		
		if (size == 1) {
			GameObject ghost = Instantiate(normalRoomGhost, worldPos, Quaternion.Euler(0f, 0f, 0f));
			ghost.tag = "Ghost";
		}
	}

	private Tuple<GameObject, int> FindClosestRoom(Vector3 position) {
		float minDistance = 1000f;
		GameObject selectedRoom = null;
		int selectedYLevel = -1;
		
		for (int yLevel = 0; yLevel < layers.Count; ++yLevel) {
			foreach (GameObject room in layers[yLevel]) {
				if (room != null) {
					Vector3 distance = position - room.transform.position;
					float distNorm = Mathf.Sqrt(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2));
			
					if (distNorm < minDistance) {
						minDistance = distNorm;
						selectedRoom = room;
						selectedYLevel = yLevel;
					}
				}
			}
		}
		
		return new Tuple<GameObject, int>(selectedRoom, selectedYLevel);
	}
}
