using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {


	public void OnDrop(PointerEventData eventData)
	{
		GetComponent<Image> ().color = UnityEngine.Color.white;
		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		d.GetComponent<Image> ().color = UnityEngine.Color.white;
		if (d != null) {
			if(transform.childCount == 0 || gameObject.tag != "Player"){
				d.parentToReturnTo = this.transform;

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
