using UnityEngine;

public class RoomHandler : MonoBehaviour {
	// The highest valued ID currently in use - a new room would use this value + 1
	private int currentMaxID;

	/*
	 * TODO: Handle creation of a new room instance upon being placed by the player, in which the ID will be assigned, and
	 * the position of the room object will be handled as:
	 * 
	 * nearestRoomOnYLevel.transform.position.z +/- (newRoomLength / 2)
	 * 
	 * where + or - depends on the side the new room is being placed on
	 * 
	 * Also, when a room is placed on either side of an existing room, the door object on the corresponding side of each room
	 * involved should be deleted
	 */

	/*
	 * TODO: Handle deletion of a room instance, which should remove all reference to it from other rooms on its y level
	 */
}
