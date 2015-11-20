using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	
	[SerializeField]
	int DropZoneIndex;

	string Upgrade = "Upgrade";

	string CurrentUpgrade;

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

				PlayerPrefs.SetString( Upgrade + DropZoneIndex, d.gameObject.name);
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
