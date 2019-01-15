using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountRadioGroup : MonoBehaviour {

	public delegate void OnPlayerCountSelected(int count);
	public event OnPlayerCountSelected onPlayerCountSelected;

	[SerializeField] private Toggle twoPlayerToggle;
	[SerializeField] private Toggle threePlayerToggle;
	[SerializeField] private Toggle fourPlayerToggle;

	void Start () 
	{
		twoPlayerToggle.onValueChanged.AddListener ((bool b) => {
			CallPlayerCountSelected (2);
		});

		threePlayerToggle.onValueChanged.AddListener ((bool b) => {
			CallPlayerCountSelected (3);
		});

		fourPlayerToggle.onValueChanged.AddListener ((bool b) => {
			CallPlayerCountSelected (4);
		});
	}

	void CallPlayerCountSelected (int count)
	{
		if (onPlayerCountSelected != null) {
			onPlayerCountSelected (count);
		}
	}
}
