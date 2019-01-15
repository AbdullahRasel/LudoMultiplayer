using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour {

	[SerializeField] private GameObject gamePlayPreference;
	[SerializeField] private QuestionDialog quitDialog;

	[SerializeField] private TokensRadioGroup tokensRadioGroup;
	[SerializeField] private PlayerCountRadioGroup playerCountRadioGroup;

	private int playerCount = 2;
	private Token.TokenType selectedToken = Token.TokenType.Blue;

	void Start ()
	{
		tokensRadioGroup.onTokenTypeSelected += ((Token.TokenType type) => selectedToken = type);
		playerCountRadioGroup.onPlayerCountSelected += ((int count) => playerCount = count);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (gamePlayPreference.activeSelf) {
				gamePlayPreference.SetActive (false);
			} else {
				quitDialog.ShowDialog ("Are you sure want to quit?", () => Application.Quit (), null);
			}
		}
	}

	public void OnVSComputer ()
	{
		gamePlayPreference.SetActive (true);
	}

	public void OnPlay ()
	{
		Token.TokenPlayer[] players = new Token.TokenPlayer[playerCount];
		Token.TokenType[] types = new Token.TokenType[playerCount];

		for (int i = 0; i < playerCount; i++) {
			players [i] = Token.TokenPlayer.Computer;
			types [i] = (Token.TokenType)i;

			if (types [i] == selectedToken) {
				players [i] = Token.TokenPlayer.Human;
			}
		}

		if ((int)selectedToken >= playerCount) {
			players [playerCount - 1] = Token.TokenPlayer.Human;
			types [playerCount - 1] = selectedToken;
		}

		GameMaster gm = GameMaster.instance;
		gm.SelectedTokens = types;
		gm.SelectedTokenPlayers = players;

		SceneManager.LoadScene ("GamePlay");
	}

}
