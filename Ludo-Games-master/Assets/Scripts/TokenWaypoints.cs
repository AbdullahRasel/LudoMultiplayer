using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenWaypoints 
{
	public List<Transform> Waypoints {
		get; private set;
	}

	public Transform HomeBase {
		get; private set;
	}

	public Transform CurrentWaypoint {
		get; set;
	}

	public Transform LastWaypoint {
		get; private set;
	}

	public Transform StartingWaypoint {
		get { return Waypoints[0]; }
	}

	public int CurrentWaypointIndex {
		get; private set;
	}

	private int _destWaypointIdx;
	public int DestWaypointIndex {
		get { return _destWaypointIdx; } 
		set {
			_destWaypointIdx = Mathf.Min (Waypoints.Count - 1, Mathf.Max (value, 0));
		}
	}

	public float DistFromPrevWaypoint {
		get; private set;
	}

	public TokenWaypoints (List<Transform> waypoints, Transform homeBase) 
	{
		Waypoints = waypoints;
		HomeBase = homeBase;
		LastWaypoint = HomeBase;
		CurrentWaypointIndex = 0;
		DestWaypointIndex = 0;
		CurrentWaypoint = Waypoints[CurrentWaypointIndex];

		DistFromPrevWaypoint = Vector3.Distance (HomeBase.position, CurrentWaypoint.position);
	}

	public bool IsArrive ()
	{
		return CurrentWaypointIndex == DestWaypointIndex;
	}

	public bool IsFinishPoint ()
	{
		return CurrentWaypointIndex == Waypoints.Count;
	}

	public bool IsInsideHomeBase ()
	{
		return HomeBase == CurrentWaypoint;
	}

	public void GoToNextWaypoint () 
	{
		LastWaypoint = CurrentWaypoint;
		CurrentWaypointIndex = Mathf.Min (CurrentWaypointIndex + 1, DestWaypointIndex);
		CurrentWaypoint = Waypoints[CurrentWaypointIndex];
		DistFromPrevWaypoint = Vector3.Distance (LastWaypoint.position, CurrentWaypoint.position);	
	}

	public void GoToPreviousWaypoint ()
	{
		LastWaypoint = CurrentWaypoint;
		if (CurrentWaypointIndex - 1 < 0) {
			CurrentWaypoint = HomeBase;
		} else {
			CurrentWaypoint = Waypoints[ --CurrentWaypointIndex ];
		}
		
		DistFromPrevWaypoint = Vector3.Distance (LastWaypoint.position, CurrentWaypoint.position);
	}
}