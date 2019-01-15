using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
	private const float DISTANCE_TOLERANT = 0.01f;

	public enum TokenType 
	{
		Blue, Red, Green, Yellow
	};

	public enum TokenPlayer
	{
		Human, Computer
	};

	public enum TokenState 
	{
		Locked, Unlocking, Idle, MovingForward, ReturningHome, Finish
	};


	public delegate void OnStateChanged (Token.TokenState lastState, Token.TokenState newState);
	public delegate void OnTokenSelected(Token selectedToken);

	public event OnStateChanged onStateChanged;
	public event OnTokenSelected onTokenSelected;

	[SerializeField] private Token.TokenType type;
	[SerializeField] private AudioClip jumpAudioClip;
	[SerializeField] private GameObject tokenTrailPrefab;
	[SerializeField] private float moveForwardSpeed = 2;
	[SerializeField] private float moveScaleFactor = 0.3f;
	[SerializeField] private float returnSpeed = 8;

	private GameObject indicatorGO;
	private SpriteRenderer renderer;
	private TokenWaypoints waypoints;

	public int CurrentWaypointIndex {
		get { return waypoints.CurrentWaypointIndex; }
	}

	public Square CurrentSquare {
		get { return waypoints.CurrentWaypoint.GetComponent<Square> (); }
	}

	private bool selectionMode;
	public bool SelectionMode {
		get { return selectionMode; }
		set {
			selectionMode = value;
			GetComponent<BoxCollider2D> ().enabled = selectionMode;
			if (selectionMode) {
				// play selection animation on this token
				indicatorGO.GetComponent<Animator> ().SetTrigger ("Landed");
			} else {
				// stop selection animation on this token
				indicatorGO.GetComponent<Animator> ().SetTrigger ("Idle");
			}
		}
	}

	private TokenState state = TokenState.Locked;
	public TokenState State {
		get { return state; }
		private set {
			state = value;
		}
	}

	public TokenPlayer Player {
		get; set;
	}

	public Color TraceColor {
		get; set;
	}

	public TokenType Type {
		get { return type; }
	}

	//
	public void SetupWaypoints (List<Transform> waypoints, Transform homeBase)
	{
		this.waypoints = new TokenWaypoints(waypoints, homeBase);
	}

	public Square GetPreviousSquareFrom(int fromIdx)
	{
		int idx = fromIdx - 1;
		if (idx < 0 || idx > waypoints.Waypoints.Count)
			return null;
		return waypoints.Waypoints [idx].GetComponent<Square> ();
	}

	public Square GetPreviousSquare (int offset)
	{
		int idx = CurrentWaypointIndex - offset;
		if (idx < 0)
			return null;
		return waypoints.Waypoints [idx].GetComponent<Square> ();
	}

	public Square GetNextSquare (int offset)
	{
		int idx = Mathf.Abs (CurrentWaypointIndex + offset);
		if (idx >= waypoints.Waypoints.Count)
			return null;
		return waypoints.Waypoints [idx].GetComponent<Square> ();
	}

	public void SelectToken ()
	{
		if (SelectionMode) {
			onTokenSelected (this);
		}
	}

	public void Unlock ()
	{
		SetState (TokenState.Unlocking);
	}

	public void Lock ()
	{
		SetState (TokenState.Locked);
	}

	public bool IsAnimating () 
	{
		return !(State == TokenState.Idle || State == TokenState.Locked);
	}

	public bool IsValidMove (int count) 
	{
		return waypoints.DestWaypointIndex + count < waypoints.Waypoints.Count;
	}

	public bool MoveToken (int count)
	{
		if (IsValidMove (count)) {
			waypoints.DestWaypointIndex += count;

			RemoveFromCurrentSquare ();
			waypoints.GoToNextWaypoint ();
			StartMoving ();
			return true;
		}

		return false;
	}

	void Start ()
	{
		renderer = GetComponent<SpriteRenderer> ();
		indicatorGO = transform.Find ("Indicator").gameObject;
		SelectionMode = false;
	}

	void LateUpdate ()
	{
		renderer.sortingOrder = (int) -Camera.main.WorldToScreenPoint(transform.position).y;
	}

	void FixedUpdate ()
	{
		if (State == TokenState.MovingForward) {
			float distance = MoveTowardsNextWaypoint (moveForwardSpeed);

			ScaleBaseOnDistance (distance);

			if (distance < DISTANCE_TOLERANT) {
				if (waypoints.CurrentWaypointIndex < waypoints.DestWaypointIndex) {
					waypoints.GoToNextWaypoint ();
					LeaveTrail ();
				} else if (waypoints.IsArrive ()) {
					AsignToCurrentSquare ();
					LeaveTrail ();
					StopMoving ();
				}

				SoundManager.instance.PlaySFX (jumpAudioClip);
			}
		} else if (State == TokenState.ReturningHome) {
			float distance = MoveTowardsNextWaypoint (returnSpeed);

			if (distance < DISTANCE_TOLERANT) {
				if (waypoints.CurrentWaypointIndex > waypoints.DestWaypointIndex) {
					waypoints.GoToPreviousWaypoint ();
				} else if (waypoints.IsArrive ()) {
					if (waypoints.CurrentWaypoint != waypoints.HomeBase) {
						waypoints.GoToPreviousWaypoint ();
					} else {
						Lock ();
					}
				}
			}
		} else if (State == TokenState.Unlocking) {
			float distance = MoveToStartingPoint (moveForwardSpeed * 2.0f);

			if (distance < DISTANCE_TOLERANT) {
				AsignToCurrentSquare ();
				StopMoving ();
			}
		}
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Vector2 pos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			Debug.DrawRay (new Vector3(pos.x, pos.y), Vector3.zero, Color.cyan);
		}

		if (SelectionMode) {
			if (Input.GetMouseButtonDown (0)) {
				Vector2 pos = 
					Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
				Vector2 size = new Vector2 (0.4f, 0.4f);

				RaycastHit2D hit = Physics2D.BoxCast (pos, size, 0f, Vector2.zero);

				// Debug
				Color color = hit ? Color.red : Color.green;
				Utils.DrawBoxCast2D (pos, size, 0f, Vector2.zero, Mathf.Infinity, color);
				//

				if (hit && hit.transform == transform && onTokenSelected != null) {
					onTokenSelected (this);
				}

			}
		}
	}

	/*
	* Move this object towards current waypoint &
	* return the remaining distance between 
	* new position and current waypoint position
	*/
	float MoveTowardsNextWaypoint (float speed)
	{
		transform.position = 
			Vector2.MoveTowards (transform.position, waypoints.CurrentWaypoint.position, speed * Time.deltaTime);;
		return Vector3.Distance (transform.position, waypoints.CurrentWaypoint.position);
	}

	float MoveToStartingPoint (float speed)
	{
		transform.position = 
			Vector2.Lerp (transform.position, waypoints.StartingWaypoint.position, speed * Time.deltaTime);
		return Vector3.Distance (transform.position, waypoints.StartingWaypoint.position);
	}

	void ScaleBaseOnDistance (float distance)
	{
		float ratio = 
			Mathf.Min (waypoints.DistFromPrevWaypoint - distance, distance) / (waypoints.DistFromPrevWaypoint / 2);
		float scale = (ratio * moveScaleFactor) + 1f;
		transform.localScale = new Vector3 (scale, scale, 1f);
	}

	void StartMoving ()
	{
		SetState (TokenState.MovingForward);
	}

	void StopMoving ()
	{
		if (waypoints.IsFinishPoint ()) {
			SetState (TokenState.Finish);
		} else {
			SetState (TokenState.Idle);
		}
	}

	void LeaveTrail ()
	{
		GameObject trace = Instantiate (tokenTrailPrefab, transform.position, Quaternion.identity);
	}

	void ReturnToHome ()
	{
		SetState (TokenState.ReturningHome);
		RemoveFromCurrentSquare ();
		waypoints.GoToPreviousWaypoint ();
		waypoints.DestWaypointIndex = 0;
	}

	void AsignToCurrentSquare ()
	{
		Square square = CurrentSquare;
		if (square != null) {
			square.AddToken (this);
			if (square.isSafe)
				return;

			List<Token> toBeReturned = new List<Token> ();
			List<Token> tokens = square.Tokens;
			foreach (Token token in tokens) {
				if (token.Type != Type) {
					toBeReturned.Add (token);
				}
			}

			foreach (Token token in toBeReturned) {
				token.ReturnToHome ();
			}
		}
	}

	void RemoveFromCurrentSquare ()
	{
		Square square = CurrentSquare;
		if (square != null) {
			square.RemoveToken (this);
		}
	}

	public void SetState (TokenState newState) 
	{
		bool changeState = false;
		switch (State) {
		case TokenState.Locked:
			if (newState == TokenState.Unlocking) {
				changeState = true;
			}
			break;
		case TokenState.Unlocking:
			if (newState == TokenState.Idle) {
				changeState = true;
			}
			break;
		case TokenState.Idle:
			if (newState == TokenState.MovingForward || newState == TokenState.ReturningHome) {
				changeState = true;
			}
			break;
		case TokenState.MovingForward:
			if (newState == TokenState.Idle || newState == TokenState.Finish) {
				changeState = true;
			}
			break;
		case TokenState.ReturningHome:
			if (newState == TokenState.Locked) {
				changeState = true;
			}
			break;
		}

		if (changeState) {
			Token.TokenState lastState = State;
			State = newState;
			if (onStateChanged != null) {
				onStateChanged (lastState, State);
			}
		}
	}
}
