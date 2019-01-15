using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokensRadioGroup : MonoBehaviour {

	public delegate void OnTokenTypeSelected(Token.TokenType type);
	public event OnTokenTypeSelected onTokenTypeSelected;

	[SerializeField] private Toggle blueToggle;
	[SerializeField] private Toggle redToggle;
	[SerializeField] private Toggle greenToggle;
	[SerializeField] private Toggle yellowToggle;

	void Start ()
	{
		blueToggle.onValueChanged.AddListener ((bool b) => {
			CallTokenSelected (Token.TokenType.Blue);
		});

		redToggle.onValueChanged.AddListener ((bool b) => {
			CallTokenSelected (Token.TokenType.Red);
		});

		greenToggle.onValueChanged.AddListener ((bool b) => {
			CallTokenSelected (Token.TokenType.Green);
		});

		yellowToggle.onValueChanged.AddListener ((bool b) => {
			CallTokenSelected (Token.TokenType.Yellow);
		});
	}


	void CallTokenSelected (Token.TokenType type)
	{
		if (onTokenTypeSelected != null) {
			onTokenTypeSelected (type);
		}
	}
}
