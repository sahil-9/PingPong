using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	private GameObject ball;

	private int computerScore;
	private int playerScore;
	private Hudder hudder;

	private GameObject hudderCanvas;
	private GameObject paddleComputer;

	public int winningScore = 2;

	public enum GameState {
		Playing,
		GameOver,
		Paused,
		Launched
	};

	public GameState gameState = GameState.Launched;

	// Use this for initialization
	void Start () {
		paddleComputer = GameObject.Find ("computer_paddle");
		hudderCanvas = GameObject.Find ("Hudder_Canvas");
		hudder = hudderCanvas.GetComponent<Hudder> ();
		hudder.playAgain.text = "PRESS SPACEBAR TO PLAY";
	}
	
	// Update is called once per frame
	void Update () {
		CheckScore ();
		CheckInput (); 
	}

	void CheckInput() {
		if (gameState == GameState.Paused || gameState == GameState.Playing) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				PauseResumeGame();
			}
		}if(gameState == GameState.Launched || gameState == GameState.GameOver) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				StartGame ();
			}
		}


	}

	void CheckScore() {
		if (playerScore >= winningScore || computerScore >= winningScore) {
			if (playerScore >= winningScore && computerScore < playerScore - 1) {
				//player wins
				PlayerWins();

			}else if (computerScore >= winningScore && playerScore < computerScore - 1) {
				//computer wins
				ComputerWins();
			}
		}
	}

	private void PlayerWins() {
		hudder.winPlayer.enabled = true;
		Gameover ();
	}

	private void ComputerWins() {
		hudder.winComputer.enabled = true;
		Gameover ();
	}

	void SpawnBall() {
		ball = GameObject.Instantiate ((GameObject)Resources.Load("Prefabs/ball", typeof(GameObject)));
		ball.transform.localPosition = new Vector3 (12, 0, -2);
	}

	public void ComputerPoint() {
		computerScore++;
		hudder.computerScore.text = computerScore.ToString ();
		NextRound ();
	}

	public void PlayerPoint() {
		playerScore++;
		hudder.playerScore.text = playerScore.ToString ();
		NextRound ();
	}

	private void StartGame() {
		playerScore = 0;
		computerScore = 0;

		hudder.playerScore.text = "0";
		hudder.computerScore.text = "0";
		hudder.winPlayer.enabled = false;
		hudder.winComputer.enabled = false;

		hudder.playAgain.enabled = false;
		gameState = GameState.Playing;

		paddleComputer.transform.localPosition = new Vector3 (paddleComputer.transform.localPosition.x, 0, paddleComputer.transform.localPosition.z);
		SpawnBall ();
	}

	private void NextRound() {
		if (gameState == GameState.Playing) {
			paddleComputer.transform.localPosition = new Vector3 (paddleComputer.transform.localPosition.x, 0, paddleComputer.transform.localPosition.z);
			GameObject.Destroy (ball.gameObject);
			SpawnBall ();
		}
	}

	private void Gameover() {
		GameObject.Destroy (ball.gameObject);
		hudder.playAgain.text = "PRESS SPACEBAR TO PLAY AGAIN!";
		hudder.playAgain.enabled = true;
		gameState = GameState.GameOver;
	}

	private void PauseResumeGame() {
		if (gameState == GameState.Paused) {
			gameState = GameState.Playing;
			hudder.playAgain.enabled = false;
		} else {
			gameState = GameState.Paused;
			hudder.playAgain.text = "GAME IS PAUSED. SPACEBAR TO RESUME";
			hudder.playAgain.enabled = true;
		}
	}
}
