using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Draggable : MonoBehaviour{
    [SerializeField]
    GameObject ButtonAudio;

    [SerializeField]
	Text Price;

	[SerializeField]
	Text Level;

	[SerializeField]
	int mCost;

	int cost;
	Stack<int> lastCost;

	[SerializeField]
	bool mHasCap;

	int currentUpgradeLevel;
	int originalLevel;

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
		if (GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount > cost)
		{
			if(currentUpgradeLevel == 10 && mHasCap)
			{
				return;
			}

			Instantiate(ButtonAudio, transform.position, Quaternion.identity);
			currentUpgradeLevel++;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount -= cost;
			lastCost.Push(cost);
			cost = cost + cost / 2;
			SetText();
		}
	}

	public void CancelLevelUp ()
	{
		if(currentUpgradeLevel != originalLevel)
		{
            Instantiate(ButtonAudio, transform.position, Quaternion.identity);
            currentUpgradeLevel --;
			cost = cost - (lastCost.Pop()/ 2);
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount += cost;
			SetText();
		}
	
	}

	public void CancelShop(){

		while(currentUpgradeLevel != originalLevel)
		{
			CancelLevelUp();
		}
		
	}

	void SetCost(){

		if (currentUpgradeLevel == 0) {
			cost = mCost;
		}
		else 
		{
			cost = mCost;

			for (int i = 1; i <= currentUpgradeLevel; i++) 
			{
				cost = cost + (cost / 2);
			}
		}
	}

	void SetText()
	{
		Level.text = "Level " + currentUpgradeLevel;
		Price.text = cost.ToString ();
	}
}
