﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {

	private int score;
	public static ScoreTracker Instance;
	public Text ScoreText;
	public Text HighScoreText;

	public int Score
	{
		get
		{
			return score;
		}

		set
		{
			score = value;
			ScoreText.text = score.ToString ();
		}
	}

	void Awake()
	{
		// Reset highscore
		//PlayerPrefs.DeleteAll ();
		Instance = this;

		if (!PlayerPrefs.HasKey ("HighScore"))
		{
			PlayerPrefs.SetInt ("HighScore", 0);
		}

		ScoreText.text = "0";
		HighScoreText.text = PlayerPrefs.GetInt ("HighScore").ToString();
	}

	public void SetHighScore()
	{
		if (PlayerPrefs.GetInt ("HighScore") < ScoreTracker.Instance.Score) {
			PlayerPrefs.SetInt ("HighScore", ScoreTracker.Instance.Score);
			ScoreTracker.Instance.HighScoreText.text = ScoreTracker.Instance.Score.ToString ();
			ScoreText.text = ScoreTracker.Instance.Score.ToString ();
		}
	}

}
