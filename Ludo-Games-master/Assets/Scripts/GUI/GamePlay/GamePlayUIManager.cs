using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayUIManager : MonoBehaviour {

	[SerializeField] private QuestionDialog questionDialog;

	void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			questionDialog.ShowDialog ("Are you sure want to end this game?", GoToMainMenu, null);
		}
	}

	void GoToMainMenu () 
	{
		SceneManager.LoadScene ("MainMenu");
	}
}
