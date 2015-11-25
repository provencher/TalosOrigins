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
	Text TotalOrbs;

	[SerializeField]
	Image TotalFrame;

	int red, blue, green, yellow;

	public int totalOrbsCount;

	// Use this for initialization
	void Start () {
	
		Cursor.visible = true;

		red    = PlayerPrefs.GetInt("Red Orbs");
		green  = PlayerPrefs.GetInt("Yellow Orbs");
		blue   = PlayerPrefs.GetInt("Blue Orbs");
		yellow = PlayerPrefs.GetInt("Yellow Orbs");
		
		if(totalOrbsCount < 0){
			totalOrbsCount = 0;
		}

		totalOrbsCount = PlayerPrefs.GetInt ("Total Orbs");
		totalOrbsCount += (5 * blue + 3 * red + 2 * green +  1 * yellow);
	}
	
	// Update is called once per frame
	void Update () {
	
		BlueOrbs.text   = "+" + blue.ToString() + " (x 5)";
		GreenOrbs.text  = "+" + green.ToString() + " (x 3)";
		RedOrbs.text    = "+" + red.ToString() + " (x 2)";
		YellowOrbs.text = "+" + yellow.ToString() + " (x 1)";

		UpdateTotalOrbsFrame ();
	}

	void OnDestroy(){

		PlayerPrefs.SetInt ("Total Orbs", totalOrbsCount);
	}

	void UpdateTotalOrbsFrame(){
		
		if (totalOrbsCount < 0) 
		{
			TotalFrame.color = new Color (255, 0, 0, 255);
		} 
		else
		{
			TotalFrame.color = Color.white;
		}

		TotalOrbs.text = totalOrbsCount.ToString();

	}
}
