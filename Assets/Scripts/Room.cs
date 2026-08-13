using UnityEngine;

public class Room : MonoBehaviour {
	public int ID;

	/*
	 * On instantiation:
	 * 
	 * - Assign the room its ID
	 */
	public Room(int newID) {
		ID = newID;
	}
}
