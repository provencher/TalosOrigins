using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	
	[SerializeField]
	int DropZoneIndex;

	string Upgrade = "Upgrade";

	public string CurrentUpgrade;

	public void OnDrop(PointerEventData eventData)
	{
		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		
		GetComponent<Image> ().color = UnityEngine.Color.white;
		d.GetComponent<Image> ().color = UnityEngine.Color.white;
		if (d != null) 
		{
			if(transform.childCount == 0 || gameObject.tag != "Player")
			{
				d.parentToReturnTo = this.transform;

				if(gameObject.tag == "UpgradeSlots")
				{
					CurrentUpgrade = d.gameObject.name;
					GameObject.Find("Orbs").GetComponent<ShopOrbs>().totalOrbsCount -= d.gameObject.GetComponent<Draggable>().mCost;
				}
				else if(gameObject.name == "Viewport")
				{
					GameObject.Find("Orbs").GetComponent<ShopOrbs>().totalOrbsCount += d.gameObject.GetComponent<Draggable>().mCost;

				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData){
		
		try {
			Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
			
			
			if(transform.childCount == 0 || gameObject.tag != "Player"){
				GetComponent<Image> ().color = UnityEngine.Color.green;
			}else if(transform.childCount >= 1 && gameObject.tag == "Player"){
				d.GetComponent<Image> ().color = UnityEngine.Color.red;
			}
		}
		catch (Exception e)
		{
			//Do Nothing
		}
	}
	public void OnPointerExit(PointerEventData eventData){
		GetComponent<Image> ().color = UnityEngine.Color.white;			
	}
}
