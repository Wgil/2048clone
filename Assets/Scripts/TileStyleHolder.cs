using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileStyle
{
	public Color32 TileColor;
	public Color32 TextColor;
}

public class TileStyleHolder : MonoBehaviour {

	// Singleton
	public static TileStyleHolder Instance;

	private GameManager gm;

	public TileStyle[] TileStyles;

	void Awake()
	{
		Instance = this;
		gm = GameObject.FindObjectOfType<GameManager> ();
	}

	public int GetColorIndex(int number)
	{
		int index = 0; 
		while (number > gm.BaseNumber)
		{
			number = number / gm.BaseNumber;
			index++;
		}

		return index;
	}
}
