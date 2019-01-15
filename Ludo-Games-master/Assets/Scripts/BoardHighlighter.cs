using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlighter : MonoBehaviour {

	[SerializeField] private GameObject blueHighlight;
	[SerializeField] private GameObject redHighlight;
	[SerializeField] private GameObject greenHighlight;
	[SerializeField] private GameObject yellowHighlight;

	public void Highlight (Token.TokenType section)
	{
		switch (section) {
		case Token.TokenType.Blue:
			blueHighlight.SetActive (true);
			break;
		case Token.TokenType.Red:
			redHighlight.SetActive (true);
			break;
		case Token.TokenType.Green:
			greenHighlight.SetActive (true);
			break;
		case Token.TokenType.Yellow:
			yellowHighlight.SetActive (true);
			break;
		}
	}

	public void StopHighlight ()
	{
		blueHighlight.SetActive (false);
		redHighlight.SetActive (false);
		greenHighlight.SetActive (false);
		yellowHighlight.SetActive (false);
	}

}
