using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour {

	/*
	 * waypoints merupakan game object yang memiliki child utk
	 * public waypoint dan private waypoint
	 * Waypoints
	 * 		|- PublicWaypoints
	 * 		|- PrivateWaypoints
	 * 			|- PrivateBlueWaypoints
	 * 			|- PrivateRedWaypoints
	 * 			|- PrivateGreenWaypoints
	 * 			|- PrivateYellowWaypoints
	 */
	[SerializeField] private GameObject waypoints;

	private List<Transform> blueWaypoints;
	private List<Transform> redWaypoints;
	private List<Transform> greenWaypoints;
	private List<Transform> yellowWaypoints;

	public List<Transform> BlueWaypoints
	{
		get { return blueWaypoints; }
	}

	public List<Transform> RedWaypoints 
	{
		get { return redWaypoints; }
	}

	public List<Transform> GreenWaypoints 
	{
		get { return greenWaypoints; }
	}

	public List<Transform> YellowWaypoints
	{
		get { return yellowWaypoints; }
	}

	void Start () 
	{
		blueWaypoints = new List<Transform> ();
		redWaypoints = new List<Transform> ();
		greenWaypoints = new List<Transform> ();
		yellowWaypoints = new List<Transform> ();

		/*
		 * Pulic waypoints merupakan waypoint yang dapat dilalui oleh semua
		 * token
		 */
		Transform pubWaypoints = waypoints.transform.Find ("PublicWaypoints");

		SetWaypointsFor (pubWaypoints, blueWaypoints, 	"BlueStartingPoint", 	"BlueGateway", 	 "PrivateBlueWaypoints");
		SetWaypointsFor (pubWaypoints, redWaypoints, 	"RedStartingPoint", 	"RedGateway", 	 "PrivateRedWaypoints");
		SetWaypointsFor (pubWaypoints, greenWaypoints, 	"GreenStartingPoint", 	"GreenGateway",  "PrivateGreenWaypoints");
		SetWaypointsFor (pubWaypoints, yellowWaypoints, "YellowStartingPoint", 	"YellowGateway", "PrivateYellowWaypoints");
	}

	private void SetWaypointsFor (Transform pubWaypoints, List<Transform> outWaypoints,
		string startingPointName, string gatewayPointName, string privateWaypointsName)
	{
		int index = 0;
		int childCount = pubWaypoints.childCount;
		int breakTolerant = childCount * 2;
		int breakLoop = 0;
		bool done = false;
		bool startingPointFound = false;
		while (!done && breakLoop++ < breakTolerant) {
			Transform waypoint = pubWaypoints.GetChild (index);
			if (!startingPointFound) {
				if (waypoint.CompareTag (startingPointName)) {
					startingPointFound = true;
					outWaypoints.Add (waypoint);
				}
			} else {
				outWaypoints.Add (waypoint);
			}

			index = (++index) % childCount;

			if (waypoint.CompareTag (gatewayPointName) && startingPointFound) {
				done = true;
			}
		}

		Transform privTransform = waypoints.transform.Find ("PrivateWaypoints/"+privateWaypointsName);
		for (int i = 0; i < privTransform.childCount; i++) {
			outWaypoints.Add (privTransform.GetChild (i));
		}
	
	}
}
