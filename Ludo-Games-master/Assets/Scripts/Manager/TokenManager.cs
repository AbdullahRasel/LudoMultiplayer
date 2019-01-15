using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour {

	public delegate void OnTokenAnimationsDone ();

	public event OnTokenAnimationsDone onTokenAnimationsDone;

	[SerializeField] private GameObject playerBluePrefab;
	[SerializeField] private GameObject playerRedPrefab;
	[SerializeField] private GameObject playerGreenPrefab;
	[SerializeField] private GameObject playerYellowPrefab;

	private WaypointManager waypointManager;
	private HomeBaseManager homeBaseManager;

	private Dictionary<Token.TokenType, Token[]> tokens;
	private List<Token> tokenComps;

	public Dictionary<Token.TokenType, Token[]> Tokens {
		get { return tokens; }
	}

	public Token[] GetTokensOfType(Token.TokenType type)
	{
		return Tokens [type];
	}

	public List<Token> GetLockedTokens(Token.TokenType type)
	{
		List<Token> lockedTokens = new List<Token> ();
		Token[] tokensOfType = GetTokensOfType (type);
		for (int i = 0; i < tokensOfType.Length; i++) {
			if (tokensOfType [i].State == Token.TokenState.Locked)
				lockedTokens.Add (tokensOfType [i]);
		}

		return lockedTokens;
	}

	public List<Token> GetUnlockedTokens(Token.TokenType type)
	{
		List<Token> lockedTokens = new List<Token> ();
		Token[] tokensOfType = GetTokensOfType (type);
		for (int i = 0; i < tokensOfType.Length; i++) {
			if (tokensOfType [i].State != Token.TokenState.Locked &&
			    tokensOfType [i].State != Token.TokenState.Finish) {

				lockedTokens.Add (tokensOfType [i]);
			}
		}

		return lockedTokens;
	}

	public List<Token> GetMovableTokens(Token.TokenType type, int diceNum)
	{
		List<Token> movableTokens = new List<Token> ();
		Token[] tokensOfType = GetTokensOfType (type);
		for (int i = 0; i < tokensOfType.Length; i++) {
			Token currToken = tokensOfType [i];

			if (currToken.State == Token.TokenState.Locked) {
				if (diceNum == 6)
					movableTokens.Add (currToken);
			} else if (currToken.State != Token.TokenState.Finish && currToken.IsValidMove (diceNum)) {
				movableTokens.Add (currToken);
			}
		}

		return movableTokens;
	}

//	public GameObject[] GetTokensOfType(Token.TokenType type)
//	{
//		return Tokens [type];
//	}
//
//	/*
//	 * return token yang dapat dipindahkan / dimainkan
//	 */
//	public List<GameObject> GetMovableTokens (int diceNum, Token.TokenType type)
//	{
//		GameObject[] tokensOfType = GetTokensOfType (type);
//		List<GameObject> movableTokens = new List<GameObject> ();
//		for (int i = 0; i < tokensOfType.Length; i++) {
//			Token token = tokensOfType [i].GetComponent<Token> ();
//
//			if (token.State == Token.TokenState.Locked) {
//				if (diceNum == 6)
//					movableTokens.Add (tokensOfType [i]);
//			} else if (token.State != Token.TokenState.Finish &&
//				token.IsValidMove (diceNum)) {
//				movableTokens.Add (tokensOfType [i]);
//			}
//
//		}
//
//		return movableTokens;
//	}
//
//	public List<GameObject> GetUnlockedTokens(Token.TokenType type)
//	{
//		List<GameObject> unlocked = new List<GameObject> ();
//		GameObject[] tokens = GetTokensOfType (type);
//		for (int i = 0; i < tokens.Length; i++) {
//			Token t = tokens [i].GetComponent<Token> ();
//			if (t.State != Token.TokenState.Locked && t.State != Token.TokenState.Finish) {
//				unlocked.Add (tokens[i]);
//			}
//		}
//
//		return unlocked;
//	}
//
//	public List<GameObject> GetLockedTokens(Token.TokenType type)
//	{
//		List<GameObject> locked = new List<GameObject> ();
//		GameObject[] tokens = GetTokensOfType (type);
//		for (int i = 0; i < tokens.Length; i++) {
//			Token t = tokens [i].GetComponent<Token> ();
//			if (t.State == Token.TokenState.Locked) {
//				locked.Add (tokens[i]);
//			}
//		}
//
//		return locked;
//	}

	public void EnableSelectionMode (Token.TokenType type)
	{
		SetSelectionModeEnable (type, true);
	}

	/*
	 * memilih token yang akan digunakan,
	 * pada array selected dan playerTypes
	 * selected.Length == playerTypes.Length
	 */
	public void Init (
		WaypointManager waypointManager, HomeBaseManager hbManager,
		Token.TokenType[] selected, Token.TokenPlayer[] playerTypes)
	{
		if (selected.Length == playerTypes.Length && selected.Length <= 4) {
			this.waypointManager = waypointManager;
			this.homeBaseManager = hbManager;

			tokens = new Dictionary<Token.TokenType, Token[]> ();

			for (int i = 0; i < selected.Length; i++) {
				CreateTokens (selected[i], playerTypes[i]);
			}

			tokenComps = new List<Token> ();
			foreach (KeyValuePair<Token.TokenType, Token[]> entry in tokens) {
				
				for (int i = 0; i < entry.Value.Length; i++) {
					Token token = entry.Value [i];
					token.onStateChanged += TokenStateChanged;
					token.onTokenSelected += TokenSelected;
					tokenComps.Add (token);
				}

			}

		}
	
	}

	void Start ()
	{
	}

	void TokenStateChanged (Token.TokenState lastState, Token.TokenState newState)
	{
		if (onTokenAnimationsDone != null) {
			bool isDone = 
				newState == Token.TokenState.Idle || newState == Token.TokenState.Locked ||
				newState == Token.TokenState.Finish;
			isDone = isDone && !AnyAnimatingToken ();
			if (isDone) {
				onTokenAnimationsDone ();
			}
		
		}
	}

	/*
	 * ketika ada salah satu token terseleksi
	 * maka token tsb dan seluruh token dgn warna yg sama
	 * harus didisable selection mode-nya
	 */
	void TokenSelected (Token selectedToken)
	{
		SetSelectionModeEnable (selectedToken.Type, false);
	}

	void SetSelectionModeEnable (Token.TokenType type, bool enable)
	{
		Token[] currTokens = GetTokensOfType (type);
		for (int i = 0; i < currTokens.Length; i++)
			currTokens [i].SelectionMode = enable;
	}

	/*
	 * mengecek apakah masih terdapat token yang sedang beranimasi
	 */
	bool AnyAnimatingToken ()
	{
		foreach (Token token in tokenComps) {
			if (token.IsAnimating ()) {
				return true;
			}
		}
		return false;
	}

	void CreateTokens (Token.TokenType tokenType, Token.TokenPlayer playerType)
	{
		Transform[] homeBase = null;
		GameObject tokenPrefab = null;
		List<Transform> waypoint = null;

		switch (tokenType) {
		case Token.TokenType.Blue:
			homeBase = homeBaseManager.BlueHomeBase;
			tokenPrefab = playerBluePrefab;
			waypoint = waypointManager.BlueWaypoints;
			break;
		case Token.TokenType.Red:
			homeBase = homeBaseManager.RedHomeBase;
			tokenPrefab = playerRedPrefab;
			waypoint = waypointManager.RedWaypoints;
			break;
		case Token.TokenType.Green:
			homeBase = homeBaseManager.GreenHomeBase;
			tokenPrefab = playerGreenPrefab;
			waypoint = waypointManager.GreenWaypoints;
			break;
		case Token.TokenType.Yellow:
			homeBase = homeBaseManager.YellowHomeBase;
			tokenPrefab = playerYellowPrefab;
			waypoint = waypointManager.YellowWaypoints;
			break;
		}

		Token[] currTokens = new Token[homeBase.Length];
		tokens.Add (tokenType, currTokens);
		for (int i = 0; i < homeBase.Length; i++) {
			GameObject newTokenGO = Instantiate (tokenPrefab, homeBase [i].position, Quaternion.identity);
			Token token = newTokenGO.GetComponent<Token> ();
			token.SetupWaypoints (waypoint, homeBase[i]);
			token.Player = playerType;

			currTokens [i] = token;
		}
	}

}
