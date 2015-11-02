using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {


	public void OnDrop(PointerEventData eventData)
	{
		GetComponent<Image> ().color = UnityEngine.Color.clear;
		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		d.GetComponent<Image> ().color = UnityEngine.Color.white;
		if (d != null) {
			if(transform.childCount < 4 || gameObject.tag != "Player"){
				d.parentToReturnTo = this.transform;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData){
		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		if (d != null) {

			if(transform.childCount < 4 || gameObject.tag != "Player"){
				GetComponent<Image> ().color = UnityEngine.Color.green;
			}else if(transform.childCount >= 4 && gameObject.tag == "Player"){
				d.GetComponent<Image> ().color = UnityEngine.Color.red;
			}
		}
	}
	public void OnPointerExit(PointerEventData eventData){
		GetComponent<Image> ().color = UnityEngine.Color.clear;

	}
}
