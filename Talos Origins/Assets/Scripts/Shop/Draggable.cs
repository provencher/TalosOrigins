﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Draggable : MonoBehaviour{
	[SerializeField]
	GameObject AddAudio;

	[SerializeField]
	GameObject NothingAudio;

    [SerializeField]
	Text Price;

	[SerializeField]
	Text Level;

	[SerializeField]
	int mCost;

	int cost;
	Stack<int> lastCost;

	[SerializeField]
	public bool mHasCap;



	public int currentUpgradeLevel;
	int originalLevel;
    int currentCost;

    public bool resetStat = false;


	void OnEnable()
	{
		lastCost = new Stack<int> ();
        if (resetStat)
        {
            PlayerPrefs.SetInt(gameObject.name, 0);
			resetStat = false;
        }
        originalLevel = currentUpgradeLevel = PlayerPrefs.GetInt (gameObject.name);
        /*
        currentCost = PlayerPrefs.GetInt(gameObject.name + "-cost", mCost);
        if(currentCost == 0)
        {
            PlayerPrefs.SetInt(gameObject.name + "-cost", mCost);
            currentCost = mCost;
        }
        */

		SetText ();
		SetCost ();
		lastCost.Push(cost);
		//Instantiate(ButtonAudio, transform.position, Quaternion.identity);

    }	

	void OnDisable()
	{
		PlayerPrefs.SetInt( gameObject.name, currentUpgradeLevel);
		//Instantiate(ButtonAudio, transform.position, Quaternion.identity);
    }
	

	public void LevelUpClick ()
	{
		if (GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount >= cost)
		{
			if(currentUpgradeLevel == 10 && mHasCap)
			{
				return;
			}

			Instantiate(AddAudio, transform.position, Quaternion.identity);
			currentUpgradeLevel++;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount -= cost;
			lastCost.Push(cost);
            cost =Mathf.CeilToInt( cost * 2 * Mathf.Pow(1.05f, currentUpgradeLevel));        
			SetText();
		}
	}

	public void CancelLevelUp (bool audio)
	{
		if(currentUpgradeLevel != originalLevel)
		{
			if(audio)
			{
				Instantiate(NothingAudio, transform.position, Quaternion.identity);
			}
		
			currentUpgradeLevel --;
			cost = lastCost.Pop();
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount += cost;
			SetText();
		}
	
	}

	public void CancelShop(){

		Instantiate(NothingAudio, transform.position, Quaternion.identity);

		while(currentUpgradeLevel != originalLevel)
		{
			CancelLevelUp(false);
		}
		
	}

	void SetCost(){

        //cost = PlayerPrefs.GetInt(gameObject.name + "-cost", mCost);

        
		if (currentUpgradeLevel == 0) {
			cost = mCost;
		}
		else 
		{
			cost = mCost;

			for (int i = 1; i <= currentUpgradeLevel; i++) 
			{
				cost = Mathf.CeilToInt( cost * 2 * Mathf.Pow(1.05f, i));
			}
		}        
    }

	void SetText()
	{
		Level.text = "Level " + currentUpgradeLevel;
		Price.text = cost.ToString ();
	}
}
