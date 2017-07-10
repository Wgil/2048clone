using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public enum GameState
{
	Playing,
	GameOver,
	WaitingForMoveToEnd
}

public class GameManager : MonoBehaviour {
		
	public int BaseNumber;

	//Coroutine identifiers to stop after move made to avoid stop the YouWon coroutine with StopAllCoroutines()
	private IEnumerator MoveOneLineDownIndexCoroutineIdentifier;
	private IEnumerator MoveOneLineUpIndexCoroutineIdentifier;
	private IEnumerator MoveCoroutineIdentifier;

	public GameState State = GameState.Playing;
	//Show a slider in the inspector instead of an input
	[Range(0,2f)]
	public float delay;
	public float panelDelay = 3f;
	private bool moveMade;
	private bool[] lineMoveComplete = new bool[4]{true, true, true, true};

	public GameObject YouWonText;
	public GameObject PlayAgainButton;
	public GameObject GameOverText;
	public Text GameOverScoreText;
	public GameObject GameOverPanel;

	private Tile[,] AllTiles = new Tile[4,4];
	private List <Tile[]> columns = new List<Tile[]> ();
	private List <Tile[]> rows = new List<Tile[]> ();
	private List<Tile> EmptyTiles = new List<Tile>();

	// Use this for initialization
	void Start () {
		Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile> ();
		foreach (Tile tile in AllTilesOneDim)
		{
			tile.Number = 0;
			AllTiles [tile.indRow, tile.indCol] = tile;
			EmptyTiles.Add (tile);
		}

		columns.Add (new Tile[]{ AllTiles[0,0], AllTiles[1,0], AllTiles[2,0], AllTiles[3,0] });
		columns.Add (new Tile[]{ AllTiles[0,1], AllTiles[1,1], AllTiles[2,1], AllTiles[3,1] });
		columns.Add (new Tile[]{ AllTiles[0,2], AllTiles[1,2], AllTiles[2,2], AllTiles[3,2] });
		columns.Add (new Tile[]{ AllTiles[0,3], AllTiles[1,3], AllTiles[2,3], AllTiles[3,3] });

		rows.Add (new Tile[]{ AllTiles[0,0], AllTiles[0,1], AllTiles[0,2], AllTiles[0,3] });
		rows.Add (new Tile[]{ AllTiles[1,0], AllTiles[1,1], AllTiles[1,2], AllTiles[1,3] });
		rows.Add (new Tile[]{ AllTiles[2,0], AllTiles[2,1], AllTiles[2,2], AllTiles[2,3] });
		rows.Add (new Tile[]{ AllTiles[3,0], AllTiles[3,1], AllTiles[3,2], AllTiles[3,3] });

		Generate ();
		Generate ();
	}

	public void GameOver()
	{
		GameOverScoreText.text = ScoreTracker.Instance.Score.ToString ();
		GameOverPanel.SetActive (true);
		ScoreTracker.Instance.SetHighScore ();
	}

	IEnumerator YouWon()
	{
		GameOverText.SetActive(false);
		PlayAgainButton.SetActive (false);
		YouWonText.SetActive(true);

		ScoreTracker.Instance.SetHighScore ();

		GameOverPanel.SetActive(true);
		yield return new WaitForSeconds (panelDelay);

		GameOverPanel.SetActive(false);
	}

	bool CanMove()
	{
		if (EmptyTiles.Count > 0)
			return true;
		else
		{
			// Check available merge in columns
			for (int i = 0; i < columns.Count; i++)
			{
				for (int j = 0; j < rows.Count - 1; j++)
				{
					if (AllTiles [j, i].Number == AllTiles [j + 1, i].Number)
						return true;
				}
			}

			// Check available merge in rows
			for (int i = 0; i < rows.Count; i++)
			{
				for (int j = 0; j < columns.Count - 1; j++)
				{
					if (AllTiles [i, j].Number == AllTiles [i, j+1].Number)
						return true;
				}
			}
		}

		return false;
	}

	public void NewGameButtonHandler()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	bool MakeOneMoveDownIndex(Tile[] LineOfTiles)
	{
		for (int i =0; i< LineOfTiles.Length-1; i++) 
		{
			//MOVE BLOCK 
			if (LineOfTiles[i].Number == 0 && LineOfTiles[i+1].Number != 0)
			{
				LineOfTiles[i].Number = LineOfTiles[i+1].Number;
				LineOfTiles[i+1].Number = 0;
				return true;
			}
			// MERGE BLOCK
			if (LineOfTiles[i].Number!= 0 && LineOfTiles[i].Number == LineOfTiles[i+1].Number &&
				LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i+1].mergedThisTurn == false)
			{
				LineOfTiles[i].Number *= BaseNumber;
				LineOfTiles[i+1].Number = 0;
				LineOfTiles[i].mergedThisTurn = true;
				LineOfTiles [i].PlayMergeAnimation ();
				ScoreTracker.Instance.Score += LineOfTiles [i].Number;
				//If Base number = 2, then BaseNumber^11 = 2048
				if (LineOfTiles [i].Number == Math.Pow(BaseNumber, 11))
					StartCoroutine (YouWon());
				return true;
			}
		}
		return false;
	}

	bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
	{
		for (int i = LineOfTiles.Length-1; i > 0; i--)
		{
			//MOVE BLOCK
			if (LineOfTiles [i].Number == 0 && LineOfTiles [i - 1].Number != 0)
			{
				LineOfTiles [i].Number = LineOfTiles [i - 1].Number;
				LineOfTiles [i - 1].Number = 0;

				return true;
			}
			//MERGE BLOCK
			if (LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i-1].Number &&
				LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i-1].mergedThisTurn == false)
			{
				LineOfTiles [i].Number *= BaseNumber;
				LineOfTiles [i - 1].Number = 0;
				LineOfTiles [i].mergedThisTurn = true;
				LineOfTiles [i].PlayMergeAnimation ();
				ScoreTracker.Instance.Score += LineOfTiles [i].Number;
				//If Base number = 2, then BaseNumber^11 = 2048
				if (LineOfTiles [i].Number == Math.Pow(BaseNumber, 11))
					StartCoroutine (YouWon());
				return true;
			}
		}

		return false;
	}

	IEnumerator MoveOneLineUpIndexCoroutine(Tile [] line, int index)
	{
		lineMoveComplete [index] = false;
		while (MakeOneMoveUpIndex (line))
		{
			moveMade = true;
			yield return new WaitForSeconds (delay);

		}
		lineMoveComplete [index] = true;
	}

	IEnumerator MoveOneLineDownIndexCoroutine(Tile [] line, int index)
	{
		lineMoveComplete [index] = false;
		while (MakeOneMoveDownIndex (line))
		{
			moveMade = true;
			yield return new WaitForSeconds (delay);

		}
		lineMoveComplete [index] = true;
	}

	void Generate()
	{
		if (EmptyTiles.Count > 0) {
			int indexForNewNumber = UnityEngine.Random.Range (0, EmptyTiles.Count);
			int randomNum = UnityEngine.Random.Range (0, 10);
			if (randomNum == 0) {
				EmptyTiles [indexForNewNumber].Number = BaseNumber * BaseNumber;			
			} else
			{
				EmptyTiles [indexForNewNumber].Number = 1024;
			}
			EmptyTiles [indexForNewNumber].PlayAppearAnimation ();
			
			EmptyTiles.RemoveAt (indexForNewNumber);
		}
	}
	
	// Update is called once per frame
	/*void Update ()
	{
		if (Input.GetKeyDown (KeyCode.G))
			Generate ();
	}*/

	private void ResetMergeFlags()
	{
		foreach (Tile t in AllTiles)
		{
			t.mergedThisTurn = false;
		}
	}

	private void UpdateEmptyTiles()
	{
		EmptyTiles.Clear ();
		foreach (Tile t in AllTiles)
		{
			if (t.Number == 0)
			{
				EmptyTiles.Add (t);
			}
		}
	}

	public void Move(MoveDirection md)
	{
		Debug.Log (md.ToString () + " move.");
		moveMade = false;
		ResetMergeFlags ();
		if (delay > 0)
		{
			//Needed to stop all coroutines after all moves made.
			MoveCoroutineIdentifier = MoveCoroutine (md);
			StartCoroutine (MoveCoroutineIdentifier);
		}
		else
		{
			for (int i = 0; i < rows.Count; i++)
			{
				switch (md)
				{
				case MoveDirection.Down:
					while (MakeOneMoveUpIndex(columns[i])) 
					{
						moveMade = true;				
					}	
					break;
				case MoveDirection.Left:
					while (MakeOneMoveDownIndex(rows[i])) 
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Right:
					while (MakeOneMoveUpIndex(rows[i])) 
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Up:
					while (MakeOneMoveDownIndex(columns[i]))
					{
						moveMade = true;
					}
					break;
				}
			}

			if (moveMade)
			{
				UpdateEmptyTiles ();
				Generate ();

				if (!CanMove ())
					GameOver ();
			}
		}
	}

	public IEnumerator MoveCoroutine(MoveDirection md)
	{
		State = GameState.WaitingForMoveToEnd;
		bool MovedUp = false;
		//start moving each line with delays depending on MoveDirection md
		switch (md)
		{
		case MoveDirection.Down:
			for (int i = 0; i < columns.Count; i++)
			{
				MoveOneLineUpIndexCoroutineIdentifier = MoveOneLineUpIndexCoroutine (columns [i], i);
				StartCoroutine (MoveOneLineUpIndexCoroutineIdentifier);
				MovedUp = true;
			}
		break;
		case MoveDirection.Left:
			for (int i = 0; i < rows.Count; i++)
			{
				MoveOneLineDownIndexCoroutineIdentifier = MoveOneLineDownIndexCoroutine (rows [i], i);
				StartCoroutine (MoveOneLineDownIndexCoroutineIdentifier);	
			}
			break;
		case MoveDirection.Right:
			for (int i = 0; i < rows.Count; i++)
			{
				MoveOneLineUpIndexCoroutineIdentifier = MoveOneLineUpIndexCoroutine (rows [i], i);
				StartCoroutine (MoveOneLineUpIndexCoroutineIdentifier);	
				MovedUp = true;
			}
			break;
		case MoveDirection.Up:
			for (int i = 0; i < columns.Count; i++)
			{
				MoveOneLineDownIndexCoroutineIdentifier = MoveOneLineDownIndexCoroutine (columns [i], i);
				StartCoroutine (MoveOneLineDownIndexCoroutineIdentifier);	
			}
			break;
		}

		// Wait until the move is over in all lines
		while (!(lineMoveComplete [0] && lineMoveComplete [1] && lineMoveComplete [2] && lineMoveComplete [3]))
			yield return null;

		if (moveMade)
		{
			UpdateEmptyTiles ();
			Generate ();

			if (!CanMove ())
				GameOver ();
		}

		State = GameState.Playing;
		StopCoroutine(MoveCoroutineIdentifier);
		if (MovedUp)
			StopCoroutine(MoveOneLineUpIndexCoroutineIdentifier);
		else
			StopCoroutine(MoveOneLineDownIndexCoroutineIdentifier);
		

	}
}
