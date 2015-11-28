﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour{

	[SerializeField]
	Text Price;

	[SerializeField]
	Text Level;

	[SerializeField]
	int mCost;

	int cost;

	int currentUpgradeLevel;
	int originalLevel;

    public bool resetStat = false;


	void OnEnable()
	{
        if (resetStat)
        {
            PlayerPrefs.SetInt(gameObject.name, 0);
            resetStat = false;
        }
        originalLevel = currentUpgradeLevel = PlayerPrefs.GetInt (gameObject.name);      
        SetText();
		cost = mCost + currentUpgradeLevel * (mCost/2);

	}	

	void OnDisable()
	{
		PlayerPrefs.SetInt( gameObject.name, currentUpgradeLevel);
	}
	

	public void LevelUpClick ()
	{
		if (GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount > cost && currentUpgradeLevel <= 10)
		{
			currentUpgradeLevel++;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount -= cost;
			SetText();
			cost = mCost + currentUpgradeLevel * (mCost/2);
		}
	}

	public void CancelLevelUp ()
	{
		if(currentUpgradeLevel != originalLevel)
		{
			currentUpgradeLevel --;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount += cost;
			cost = mCost + currentUpgradeLevel * (mCost/2);
			SetText();
		}
	
	}

	void SetText()
	{
		Level.text = "Level " + currentUpgradeLevel;
		Price.text = cost.ToString ();
	}
}
