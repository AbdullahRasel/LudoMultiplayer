using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrnament : MonoBehaviour {

	[Header("Randomize Animation")]
	[SerializeField] private float minSeconds = 6f;
	[SerializeField] private float maxSeconds = 12f;

	private Animator animator;
	private float timer;
	private float delayTime;

	void Start () 
	{
		animator = GetComponent<Animator> ();
		if (minSeconds < 6f)
			minSeconds = 6f;
		if (maxSeconds < 12f)
			maxSeconds = 12f;
		delayTime = maxSeconds;
	}

	void Update ()
	{
		timer += Time.deltaTime;

		if (timer >= delayTime) {
			delayTime = Random.Range (minSeconds, maxSeconds);
			int rand = (int) delayTime;
			if (rand % 2 == 0) {
				animator.SetTrigger ("Rotate");
			} else {
				animator.SetTrigger ("Jump");
			}

			timer = 0f;
		}
	}
}
