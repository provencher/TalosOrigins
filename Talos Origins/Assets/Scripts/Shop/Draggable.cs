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
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount += cost;
			cost = cost - (lastCost.Peek()/ 2);
			lastCost.Pop();
			SetText();
		}
	
	}

	void SetCost(){

		if (currentUpgradeLevel == 0) {
			cost = mCost;
		}
		else 
		{
			cost = mCost;
			Debug.Log(cost);
			
			for (int i = 1; i <= currentUpgradeLevel; i++) 
			{
				cost = cost + (cost / 2);
				Debug.Log(cost);
			}
		}
	}

	void SetText()
	{
		Level.text = "Level " + currentUpgradeLevel;
		Price.text = cost.ToString ();
	}
}
