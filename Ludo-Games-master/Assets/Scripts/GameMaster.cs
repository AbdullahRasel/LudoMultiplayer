using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;

	public Token.TokenType[] SelectedTokens {
		get;
		set;
	}

	public Token.TokenPlayer[] SelectedTokenPlayers {
		get;
		set;
	}

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (instance);
	}

	void Start ()
	{
		SelectedTokens = new Token.TokenType[]{
			Token.TokenType.Blue, Token.TokenType.Red, Token.TokenType.Green, Token.TokenType.Yellow
		};

		SelectedTokenPlayers = new Token.TokenPlayer[] {
			Token.TokenPlayer.Computer, Token.TokenPlayer.Computer, Token.TokenPlayer.Computer, Token.TokenPlayer.Computer
		};
	}


}
