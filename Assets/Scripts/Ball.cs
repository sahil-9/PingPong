using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	public float moveSpeed = 20.0f;
	public Vector2 ballDirection = Vector2.left;

	private GameObject paddlePlayer, paddleComputer;

	private float playerPaddleHeight, playerPaddleWidth, computerPaddleHeight, computerPaddleWidth;
	private float playerPaddleMaxX, playerPaddleMaxY, playerPaddleMinX, playerPaddleMinY;
	private float computerPaddleMaxX, computerPaddleMaxY, computerPaddleMinX, computerPaddleMinY;
	private float ballWidth, ballHeight;

	public float topBounds = 9.4f, bottomBounds = -9.4f;

	public int speedIncreasedInterval = 20;
	public float speedIncreaseBy = 1.0f;
	private float speedIncreaseTimer = 0;

	private float bounceAngle, maxAngle = 45.0f;
	private float vx, vy;

	private bool collidedWithComputer, collidedWithPlayer, collidedWithWall;

	private Game game;
	private bool assignedPoint;

	// Use this for initialization
	void Start () {

		game = GameObject.Find ("Game").GetComponent<Game> ();
			
		if(moveSpeed < 0) {
			moveSpeed = -1 * moveSpeed;
		}
			
		paddlePlayer = GameObject.Find ("player_paddle");
		paddleComputer = GameObject.Find ("computer_paddle");

		playerPaddleHeight = paddlePlayer.transform.GetComponent<SpriteRenderer> ().bounds.size.y;
		playerPaddleWidth= paddlePlayer.transform.GetComponent<SpriteRenderer> ().bounds.size.x;
		computerPaddleHeight = paddleComputer.transform.GetComponent<SpriteRenderer> ().bounds.size.y;
		computerPaddleWidth = paddleComputer.transform.GetComponent<SpriteRenderer> ().bounds.size.x;
		ballHeight = transform.GetComponent<SpriteRenderer> ().bounds.size.y;
		ballWidth= transform.GetComponent<SpriteRenderer> ().bounds.size.x;

		//Setting the boundaries of the edges of computer+player paddle
		playerPaddleMaxX = paddlePlayer.transform.localPosition.x + playerPaddleWidth / 2;
		playerPaddleMinX = paddlePlayer.transform.localPosition.x - playerPaddleWidth / 2;
		computerPaddleMaxX = paddleComputer.transform.localPosition.x - computerPaddleWidth / 2;
		computerPaddleMinX = paddleComputer.transform.localPosition.x + computerPaddleWidth / 2;

		bounceAngle = GetRandomBounceAngle ();

		vx = moveSpeed * Mathf.Cos (bounceAngle);
		vy = moveSpeed * -Mathf.Sin (bounceAngle);
	}
	
	// Update is called once per frame
	void Update () {
		if (game.gameState != Game.GameState.Paused) {
			Move ();

			UpdateSpeedIncrease ();	
		}
	}

	public void UpdateSpeedIncrease() {
		if (speedIncreaseTimer >= speedIncreasedInterval) {
			speedIncreaseTimer = 0;
			if (moveSpeed > 0)
				moveSpeed += speedIncreaseBy;
			else
				moveSpeed -= speedIncreaseBy;
		} else {
			speedIncreaseTimer += Time.deltaTime;
		}
	}

	bool CheckCollision() {
		//getting upper and lower boundaries of the player and computer paddle
		playerPaddleMaxY = paddlePlayer.transform.localPosition.y + playerPaddleHeight / 2;
		playerPaddleMinY = paddlePlayer.transform.localPosition.y - playerPaddleHeight / 2;
		computerPaddleMaxY = paddleComputer.transform.localPosition.y + computerPaddleHeight / 2;
		computerPaddleMinY = paddleComputer.transform.localPosition.y - computerPaddleHeight / 2;
	
		//check the X collision of ball with paddle
		if (transform.localPosition.x - ballWidth / 2 < playerPaddleMaxX && transform.localPosition.x + ballWidth / 2 > playerPaddleMinX) {
			//check the Y collision of the ball with paddle
			//top collision and bottom collision with the paddle respt
			if (transform.localPosition.y - ballHeight / 2 < playerPaddleMaxY && transform.localPosition.y + ballHeight / 2 > playerPaddleMinY) {

				ballDirection = Vector2.right;
				collidedWithPlayer = true; 
				transform.localPosition = new Vector3 (playerPaddleMaxX + (float)(ballWidth / 1.8), transform.localPosition.y, transform.localPosition.z);
				return true;
			} else {
				if (!assignedPoint) {
					assignedPoint = true;
					game.ComputerPoint ();
				}
			}
		}

		if (transform.localPosition.x + ballWidth / 2 > computerPaddleMaxX && transform.localPosition.x - ballWidth / 2 < computerPaddleMinX) {
			if (transform.localPosition.y - ballHeight / 2 < computerPaddleMaxY && transform.localPosition.y + ballHeight / 2 > computerPaddleMinY) {
				//remove both below statements if you wanna work with the statements in move function
				ballDirection = Vector2.left;
				collidedWithComputer = true;
				transform.localPosition = new Vector3 (computerPaddleMaxX - (float)(ballWidth / 1.8), transform.localPosition.y, transform.localPosition.z);
				return true;
			} else {
				if (!assignedPoint) {
					assignedPoint = true;
					game.PlayerPoint ();
				}
			}
		}

		if (transform.localPosition.y > topBounds) {
			transform.localPosition = new Vector3 (transform.localPosition.x, topBounds, transform.localPosition.z);
			collidedWithWall = true;
			return true;
		}

		if (transform.localPosition.y < bottomBounds) {
			transform.localPosition = new Vector3 (transform.localPosition.x, bottomBounds, transform.localPosition.z);
			collidedWithWall = true;
			return true;
		}

		return false;
	}

	void Move() {
		/*if (CheckCollision () ) {
			ballDirection.x = ballDirection.x * -1;
		}
		transform.localPosition += (Vector3)ballDirection * moveSpeed * Time.deltaTime;*/
		if (!CheckCollision ()) {
			vx = moveSpeed * Mathf.Cos (bounceAngle);
			if (moveSpeed < 0)
				vy = moveSpeed * -Mathf.Sin (bounceAngle);
			else
				vy = moveSpeed * Mathf.Sin (bounceAngle);

			transform.localPosition += new Vector3 (ballDirection.x * vx * Time.deltaTime, vy * Time.deltaTime, 0);
		} else {
			if (moveSpeed < 0)
				moveSpeed = -1 * moveSpeed;
				
			if (collidedWithPlayer) {
				collidedWithPlayer = false;
				float relativeIntersectY = paddlePlayer.transform.localPosition.y - transform.localPosition.y;
				float normalisedRelativeIntersectionY = (relativeIntersectY/(playerPaddleHeight/2));

				bounceAngle = normalisedRelativeIntersectionY * (maxAngle * Mathf.Deg2Rad);
			}

			if (collidedWithComputer) {
				collidedWithComputer= false;
				float relativeIntersectY = paddleComputer.transform.localPosition.y - transform.localPosition.y;
				float normalisedRelativeIntersectionY = (relativeIntersectY/(computerPaddleHeight/2));

				bounceAngle = normalisedRelativeIntersectionY * (maxAngle * Mathf.Deg2Rad);
			}

			else if (collidedWithWall) {
				collidedWithWall = false;
				bounceAngle = -bounceAngle;
			}
		}
	}

	float GetRandomBounceAngle(float minDegrees=160f, float maxDegrees=260f) {
		float minRad = minDegrees * Mathf.PI / 180;
		float maxRad= maxDegrees * Mathf.PI / 180;

		return Random.Range (minRad, maxRad);
	}
}
