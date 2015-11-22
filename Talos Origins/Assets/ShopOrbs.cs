using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopOrbs : MonoBehaviour {
	
	[SerializeField]
	Text BlueOrbs;

	[SerializeField]
	Text RedOrbs;
	
	[SerializeField]
	Text GreenOrbs;
	
	[SerializeField]
	Text YellowOrbs;

	[SerializeField]
	Text Total;

	int red, blue, green, yellow;

	public int totalCount;

	// Use this for initialization
	void Start () {
	
		red = PlayerPrefs.GetInt("Red Orbs");
		green = PlayerPrefs.GetInt("Yellow Orbs");
		blue = PlayerPrefs.GetInt("Blue Orbs");
		yellow = PlayerPrefs.GetInt("Yellow Orbs");

	}
	
	// Update is called once per frame
	void Update () {
	
		BlueOrbs.text = blue.ToString();
		GreenOrbs.text = green.ToString();
		RedOrbs.text = red.ToString();
		YellowOrbs.text = yellow.ToString();

		totalCount = red + green + blue + yellow;
		Total.text = totalCount.ToString();
	}
}
