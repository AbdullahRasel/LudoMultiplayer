using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour {
	private static int BACK_RADIUS = 6;

	public event Token.OnTokenSelected onTokenSelected;

	public DiceManager DiceManager {
		get;
		set;
	}

	public TokenManager TokenManager {
		get;
		set;
	}

	private Token.TokenType currentTokenType;
	private bool isStarted;

	struct MoveScoreParameterWeights {
		public int ToFinishPoint;
		public int ToSafeSquare;
		public int NoTokenBehind;
		public int BeatsOther;

		public int GetSum()
		{
			return ToFinishPoint + ToSafeSquare + NoTokenBehind + BeatsOther;
		}
	}

	struct StayRiskScoreParameterWeights {
		public int AnyTokenBehind;
	}

	private static MoveScoreParameterWeights defMoveScoreParamWeights = new MoveScoreParameterWeights ();
	private static StayRiskScoreParameterWeights defStayRiskScoreParamWeights = new StayRiskScoreParameterWeights();

	void Start ()
	{
		isStarted = false;

		defMoveScoreParamWeights.ToFinishPoint = 35;
		defMoveScoreParamWeights.ToSafeSquare = 25;
		defMoveScoreParamWeights.NoTokenBehind = 20;
		defMoveScoreParamWeights.BeatsOther = 20;

		defStayRiskScoreParamWeights.AnyTokenBehind = 25;
	}
		
	public void StartTurn (Token.TokenType tokenType) 
	{
		isStarted = true;
		currentTokenType = tokenType;
		StartCoroutine (DiceManager.GetCurrentActiveDice ().RollDice ());
	}

	public void Play (int diceNum)
	{
		if (!isStarted)
			return;

		Token selected = null;

		List<Token> unlocked = TokenManager.GetUnlockedTokens (currentTokenType);
		List<Token> locked = TokenManager.GetLockedTokens (currentTokenType);

		if (unlocked.Count <= 0 && diceNum == 6) {

			selected = locked [0];

		} else if (unlocked.Count == 1 && diceNum != 6) {

			selected = unlocked [0];

		} else if (locked.Count > 0 && diceNum == 6) {

			selected = locked [0];

		} else {

			List<Token> movable = TokenManager.GetMovableTokens (currentTokenType, diceNum);
			Dictionary<int, List<Token>> sameScore = new Dictionary<int, List<Token>> ();

			int lastScore = -defMoveScoreParamWeights.GetSum ();
			int currScore = 0;

			foreach (Token currToken in movable) {
				currScore = GetFinalScore (currToken, movable, diceNum);
				if (currScore >= lastScore) {

					List<Token> listTokensOfSameScore;
					if (sameScore.ContainsKey (currScore)) {
						listTokensOfSameScore = sameScore [currScore];
					} else {
						listTokensOfSameScore = new List<Token> ();
						sameScore.Add (currScore, listTokensOfSameScore);
					}

					listTokensOfSameScore.Add (currToken);

					lastScore = currScore;
				}
			}

			Debug.Log ("Highest Score: " + lastScore);

			List<Token> highestSameScore = sameScore [lastScore];
			if (highestSameScore.Count > 1 && IsNoTokenBehind (highestSameScore[0], diceNum) < 0) {

				int lastIdx = 1000;
				foreach (Token currToken in highestSameScore) {
					if (currToken.CurrentWaypointIndex <= lastIdx) {
						selected = currToken;
						lastIdx = currToken.CurrentWaypointIndex;
					}
				}

				Debug.Log ("Choose the lowest idx : " + lastIdx);

			} else if (highestSameScore.Count > 0) {

				selected = highestSameScore [0];

			}

		}

		if (onTokenSelected != null) {
			onTokenSelected (selected);
		}

		isStarted = false;
	}

	int GetFinalScore(Token currToken, List<Token> others, int diceNum)
	{
		int moveScore = GetMoveScore (defMoveScoreParamWeights, currToken, diceNum);
		int otherStayRisk = 0;
		foreach (Token other in others) {
			if (other != currToken)
				otherStayRisk += GetStayRiskScore (defStayRiskScoreParamWeights, other);
		}

		return moveScore - otherStayRisk;
//		return moveScore;
	}

	int GetStayRiskScore(StayRiskScoreParameterWeights paramWeights, Token token)
	{
		if (IsInSafeSquare (token))
			return 0;

		return IsInBeatRange (token) * paramWeights.AnyTokenBehind;
	}

	int GetMoveScore(MoveScoreParameterWeights paramWeights, Token token, int diceNum)
	{
		int moveScore = 0;
		moveScore += IsToFinishPoint (token, diceNum) * paramWeights.ToFinishPoint;
		moveScore += IsToSafeSquare (token, diceNum) * paramWeights.ToSafeSquare;
		moveScore += IsNoTokenBehind (token, diceNum) * paramWeights.NoTokenBehind;
		moveScore += IsBeatsOther (token, diceNum) * paramWeights.BeatsOther;

		return moveScore;
	}

	int IsToFinishPoint(Token token, int diceNum)
	{
		Square nextSquare = token.GetNextSquare (diceNum);
		if (nextSquare != null && nextSquare.gameObject.CompareTag ("FinishPoint"))
			return 1;
		return 0;
	}

	int IsToSafeSquare(Token token, int diceNum)
	{
		Square nextSquare = token.GetNextSquare (diceNum);
		if (nextSquare != null && nextSquare.isSafe)
			return 1;
		return 0;
	}

	int IsNoTokenBehind(Token token, int diceNum)
	{
		int nextIdx = token.CurrentWaypointIndex + diceNum;
		for (int i = 0; i < BACK_RADIUS; i++) {
			Square s = token.GetPreviousSquareFrom (nextIdx);
			if (s != null) {
				foreach (Token behind in s.Tokens) {
					if (behind.Type != token.Type)
						return 0;
				}
			}
			nextIdx--;
		}

		return 1;
	}

	int IsBeatsOther(Token token, int diceNum) 
	{
		Square nextSquare = token.GetNextSquare (diceNum);
		if (nextSquare.isSafe)
			return 0;
		if (nextSquare != null) {
			foreach (Token other in nextSquare.Tokens) {
				if (other.Type != token.Type)
					return 1;
			}
		}

		return 0;
	}
		
	int IsInBeatRange(Token token) 
	{
		if (token.CurrentSquare != null) {
			for (int i = 0; i < BACK_RADIUS; i++) {
				Square prevSquare = token.GetPreviousSquare (i + 1);
				if (prevSquare != null) {
					foreach (Token other in prevSquare.Tokens) {
						if (other.Type != token.Type)
							return 1;
					}
				}
			}
		}

		return 0;
	}

	bool IsInSafeSquare(Token token)
	{
		if (token.CurrentSquare == null)
			return true;
		return token.CurrentSquare.isSafe;
	}
}
