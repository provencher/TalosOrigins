﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopOrbs : MonoBehaviour {
	
	[SerializeField]
	Text TotalOrbs;

	[SerializeField]
	Image TotalFrame;

	int red, blue, green, yellow;
	int redDiff, blueDiff, greenDiff, yellowDiff;
	

	public int totalOrbsCount = 0;

	void Start(){
		red = blue = green = yellow = 0;
	}

	// Use this for initialization
	void OnEnable () {
	
		Cursor.visible = true;
        UpdateTotalOrbsFrame();
        /*
		redDiff = PlayerPrefs.GetInt ("Red Orbs") - red;
		greenDiff = PlayerPrefs.GetInt ("Green Orbs") - green;
		blueDiff = PlayerPrefs.GetInt ("Blue Orbs") - blue;
		yellowDiff = PlayerPrefs.GetInt ("Yellow Orbs") - yellow;

		red    = PlayerPrefs.GetInt("Red Orbs");
		green  = PlayerPrefs.GetInt("Green Orbs");
		blue   = PlayerPrefs.GetInt("Blue Orbs");
		yellow = PlayerPrefs.GetInt("Yellow Orbs");

		if(totalOrbsCount < 0){
			totalOrbsCount = 0;
		}
        */
        totalOrbsCount = PlayerPrefs.GetInt ("Total Orbs",0);
		//totalOrbsCount += (5 * blueDiff + 3 * redDiff + 2 * greenDiff +  1 * yellowDiff);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTotalOrbsFrame ();

		if(Input.GetKeyDown(KeyCode.Backslash))
		{
			totalOrbsCount += 1000;
		}
	}

	void OnDisable(){
		PlayerPrefs.SetInt ("Total Orbs", totalOrbsCount);
		Cursor.visible = false;
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
