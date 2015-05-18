using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {


	public static GameMaster GM;

	public int fencesUsed = 0;
	public int slimesCaptured = 0;

	public GameObject score;
	public Text scoreDetails;
	public Text mainScore;

	public GameObject startMenu;

	public enum GameState {
		Paused,
		Playing,

		Finished
	}


	public GameState gameState;

	void Awake () {
		if (GM == null) {
			GM = this;
		}
		score.SetActive (false);
	}

	// Use this for initialization
	void Start () {

	}

	public void PlayAgain () {
		Application.LoadLevel (Application.loadedLevel);
	}

	public void PlayGame () {
		startMenu.SetActive (false);
		gameState = GameState.Playing;
	}

	public void FinishGame () {
		gameState = GameState.Finished;
		score.SetActive (true);

		scoreDetails.text = "SLIMES CAPTURED: " + slimesCaptured + "\n\nFENCES USED: " + fencesUsed + " \n\n";
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == GameState.Paused || gameState == GameState.Finished) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

		if (Input.GetButtonDown("Pause")) {
			if (gameState == GameState.Paused) {
				gameState = GameState.Playing;
			} else {
				gameState = GameState.Paused;
			}
		}
	}
}
