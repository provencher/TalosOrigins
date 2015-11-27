using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour{

	[SerializeField]
	Text Price;

	[SerializeField]
	Text Level;

	[SerializeField]
	public int mCost;

	int currentUpgradeLevel;
	int originalLevel;
	
	void OnEnable()
	{
		originalLevel = currentUpgradeLevel = PlayerPrefs.GetInt (gameObject.name);
		SetText();

	}	

	void OnDisable()
	{
		PlayerPrefs.SetInt( gameObject.name, currentUpgradeLevel);
	}
	

	public void LevelUpClick ()
	{
		if (GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount > mCost && currentUpgradeLevel <= 10)
		{
			currentUpgradeLevel++;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount -= mCost;
			SetText();
		}
	}

	public void CancelLevelUp ()
	{
		if(currentUpgradeLevel != originalLevel)
		{
			currentUpgradeLevel --;
			GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount += mCost;
			SetText();
		}
	
	}

	void SetText()
	{
		Level.text = "Level " + currentUpgradeLevel;
		Price.text = mCost.ToString ();
	}
}
