using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour {

	public delegate void OnDiceRolled (int diceNum);

	public event OnDiceRolled onDiceRolled;

	[SerializeField] private int minDiceNumber = 1;
	[SerializeField] private int maxDiceNumber = 6;

	private Animator animator;
	private bool rolling;

	private bool enableUserInteraction;
	public bool EnableUserInteraction {
		set { enableUserInteraction = value; }
	}

	void Start () 
	{
		animator = GetComponent<Animator> ();
		rolling = false;
		enableUserInteraction = true;
	}

	void Update ()
	{
		if (!enableUserInteraction || rolling) {
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
			Vector2 pos = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (pos), Vector2.zero);
			if (hit != null) {
				if (hit.transform == transform) {
					StartCoroutine (RollDice ());
				}
			}
		}
	}

	public IEnumerator RollDice ()
	{
		if (!rolling) {
			rolling = true;
			animator.SetTrigger ("RollDice");
			animator.SetInteger ("DiceNum", 0);
			yield return new WaitForSeconds (1f);

			rolling = false;
			int num = Random.Range (minDiceNumber, maxDiceNumber + 1);
			animator.SetInteger ("DiceNum", num);
			
			if (onDiceRolled != null) {
				onDiceRolled (num);
			}
		}

		yield return null;
	}
}
