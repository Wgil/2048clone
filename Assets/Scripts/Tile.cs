using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

	public bool mergedThisTurn = false;
	public int indRow;
	public int indCol;

	public int Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
			if (number == 0)
				SetEmpty ();
			else
			{
				ApplyStyleFromHolder();
				SetVisible ();
			}
		}
	}

	private int number;

	private Text TileText;
	private Image TileImage;
	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator> ();
		TileText = GetComponentInChildren<Text> ();
		TileImage = transform.Find ("NumberedCell").GetComponent<Image> ();
	}

	public void PlayMergeAnimation()
	{
		anim.SetTrigger ("Merge");
	}

	public void PlayAppearAnimation()
	{
		anim.SetTrigger ("Appear");
	}

	void ApplyStyleFromHolder()
	{
		int indexColor = TileStyleHolder.Instance.GetColorIndex (number);
		TileText.text = number.ToString ();
		TileText.color = TileStyleHolder.Instance.TileStyles [indexColor].TextColor;
		TileImage.color = TileStyleHolder.Instance.TileStyles [indexColor].TileColor;
	}

	private void SetVisible()
	{
		TileImage.enabled = true;
		TileText.enabled = true;
	}

	private void SetEmpty()
	{
		TileImage.enabled = false;
		TileText.enabled = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
