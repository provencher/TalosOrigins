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
	
	void Start()
	{
		originalLevel = currentUpgradeLevel = PlayerPrefs.GetInt (gameObject.name);

		SetText ();

	}	

	void OnDisable(){
		PlayerPrefs.SetInt( gameObject.name, currentUpgradeLevel);
	}
	

	public void LevelUpClick ()
	{

		int totalOrbs = GameObject.Find ("Orbs").GetComponent<ShopOrbs> ().totalOrbsCount;
//		Debug.Log (totalOrbs);

		if (totalOrbs > mCost || currentUpgradeLevel <= 10)
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
//
//	public void OnEndDrag(PointerEventData eventData){
//	
//		this.transform.SetParent (parentToReturnTo);
//		if(gameObject.transform.parent.name == "Viewport")
//		{
//			for(int i=0; i<= 2; i++){
//					
//				if(PlayerPrefs.GetString(Upgrade + i) == gameObject.name)
//				{
//					PlayerPrefs.SetString(Upgrade + i, null);
//
//				}
//
//			}
//		
//
//		}
//
//		GetComponent<CanvasGroup> ().blocksRaycasts = true;
//
//	}
}
