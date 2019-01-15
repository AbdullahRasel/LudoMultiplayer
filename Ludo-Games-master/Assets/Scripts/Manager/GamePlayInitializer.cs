using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayInitializer : MonoBehaviour {
	
	void Start ()
	{
		GameMaster gm = GameMaster.instance;

		DiceManager diceManager = GetComponent<DiceManager> ();
		TokenManager tokenManager = GetComponent<TokenManager> ();
		TurnManager turnManager = GetComponent<TurnManager> ();
		WaypointManager waypointManager = GetComponent<WaypointManager> ();
		HomeBaseManager hbManager = GetComponent<HomeBaseManager> ();
		OpponentController opponentCtrl = GetComponent<OpponentController> ();
		BoardHighlighter highlighter = GameObject.Find ("Board").GetComponent<BoardHighlighter> ();

		diceManager.Init (gm.SelectedTokens);
		opponentCtrl.DiceManager = diceManager;

		tokenManager.Init (waypointManager, hbManager, gm.SelectedTokens, gm.SelectedTokenPlayers);
		turnManager.Init (tokenManager, diceManager, opponentCtrl, highlighter);

		turnManager.StartFirstTurn ();
	}

}
