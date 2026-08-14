using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomHandler : MonoBehaviour {
	// Input System object
	private InputSystem_Actions keybinds;
	
	private InputAction clickAction;
	
	// The highest valued ID currently in use - a new room would use this value + 1
	private int currentMaxID;
	
	private List<GameObject[]> layers = new List<GameObject[]>();
	
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
		
		GameObject[] startLayer = new GameObject[10];
		startLayer[0] = GameObject.FindWithTag("Ladder");
		layers.Add(startLayer);
	}
	
	void FixedUpdate() {
		GameObject activeGhost = GameObject.FindWithTag("Ghost");
		
		if (activeGhost != null) {
			Vector2 rawMousePos = Mouse.current.position.ReadValue();
			float depth = Vector3.Dot(new Vector3(0f, 0f, 0f) - Camera.main.transform.position, Camera.main.transform.forward);
			Vector3 mousePos = new Vector3(rawMousePos.x, rawMousePos.y, depth);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			worldPos.x = 0f;
			
			GameObject closestRoom = FindClosestRoom(worldPos);
			Vector3 snapPosition;
			if (worldPos.z <= closestRoom.transform.position.z) {
				// Place to the right of the closest room
				snapPosition = new Vector3(0f, closestRoom.transform.position.y, (closestRoom.transform.position.z - (closestRoom.transform.localScale.z * 2f)) - (activeGhost.transform.localScale.z * 2f));
			} else {
				// Place to the left of the closest room
				snapPosition = new Vector3(0f, closestRoom.transform.position.y, (closestRoom.transform.position.z + (closestRoom.transform.localScale.z * 2f)) + (activeGhost.transform.localScale.z * 2f));
			}
		
			activeGhost.transform.position = snapPosition;
			
			if (clickAction.triggered) {
				GameObject room = Instantiate(normalRoomPrefab, snapPosition, Quaternion.Euler(0f, 0f, 0f));
				room.GetComponent<Room>().ID = currentMaxID + 1;
				
				int targetY = Mathf.RoundToInt(snapPosition.y + 1.25f);
				
				if (layers.Count < targetY) {
					GameObject[] layer = new GameObject[10];
					layer[0] = room;
				} else {
					for (int i = 0; i < layers[targetY].Length; ++i) {
						if (layers[targetY][i].GetType() == typeof(GameObject)) {
							layers[targetY][i] = room;
							break;
						}
					}
				}
				
				++currentMaxID;
				Destroy(activeGhost.gameObject);
				activeGhost = null;
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

	public void CreateRoom() {
		Vector3 placementPos = GameObject.FindWithTag("Ghost Room").transform.position;
	}

	private GameObject FindClosestRoom(Vector3 position) {
		float minDistance = 1000f;
		GameObject selected = null;
		
		for (int yLevel = 0; yLevel < layers.Count; ++yLevel) {
			foreach (GameObject room in layers[yLevel]) {
				if (room != null) {
					Vector3 distance = position - room.transform.position;
					float distNorm = Mathf.Sqrt(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2));
			
					if (distNorm < minDistance) {
						minDistance = distNorm;
						selected = room;
					}
				}
			}
		}
		
		return selected; // TODO: Make this return the y level too - relying on ingame y pos is not sufficient
	}
}
