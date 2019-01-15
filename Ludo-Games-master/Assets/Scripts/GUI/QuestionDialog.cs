using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionDialog : MonoBehaviour {

	public delegate void OnYesClicked();
	public delegate void OnNoClicked();

	private OnYesClicked onYesCallback;
	private OnNoClicked onNoCallback;

	[SerializeField] private Text textMsg;

	public void ShowDialog (string msg, OnYesClicked yesCallback, OnNoClicked noCallback) 
	{
		textMsg.text = msg;
		onYesCallback = yesCallback;
		onNoCallback = noCallback;

		gameObject.SetActive (true);
	}

	public void Yes ()
	{
		if (onYesCallback != null)
			onYesCallback ();
		gameObject.SetActive (false);
	}

	public void No ()
	{
		if (onNoCallback != null)
			onNoCallback ();
		gameObject.SetActive (false);
	}
}
