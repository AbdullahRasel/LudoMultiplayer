using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

	public bool isSafe = false;
	public bool Safe {
		get { return isSafe; }
		set { isSafe = value; }
	}

	private List<Token> tokens;
	public List<Token> Tokens {
		get { return tokens; }
	}

	void Start ()
	{
		tokens = new List<Token> ();
	}

	public void AddToken (Token token)
	{
		tokens.Add (token);
		ResizeTokens ();
	}

	public void RemoveToken (Token token) 
	{
		tokens.Remove (token);
		ResizeTokens ();

		token.transform.localScale = new Vector3 (1f, 1f, 1f);
	}

	void ResizeTokens ()
	{
		if (Tokens.Count > 1) {
			float scale = 1.0f - ((float)Tokens.Count / 6.0f);
			float offset = scale / 5.0f;

			List<Token> t = new List<Token> ();
			t.AddRange (Tokens);
			if (Tokens.Count % 2 != 0) {
				Tokens [0].transform.localScale = new Vector3 (scale, scale);
				Tokens [0].transform.position = transform.position;
				t.Remove (Tokens [0]);
			}

			int inc = 1;
			for (int i = 0; i < t.Count; i++) {
				t [i].transform.localScale = new Vector3 (scale, scale);

				if (i % 3 == 0) {
					t [i].transform.position = transform.position + new Vector3 (offset * inc, offset * inc);
				} else if (i % 3 == 1) {
					t [i].transform.position = transform.position + new Vector3 (-offset * inc, -offset * inc);
				} else {
					inc++;
				}
			}
		} else if (Tokens.Count == 1) {
			Tokens [0].transform.localScale = new Vector3 (1f, 1f);
			Tokens [0].transform.position = transform.position;
		}
	}
}
