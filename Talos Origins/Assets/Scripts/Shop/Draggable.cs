using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
	
	public Transform parentToReturnTo = null;

	[SerializeField]
	enum upgradeType {Health, Jump, Grapple, Bullets, Shield, Breadcrumbs};
	
	string Upgrade = "Upgrade";

	void Start()
	{
		for(int i=0; i<3; i++){
			if(gameObject.name == PlayerPrefs.GetString(Upgrade + i))
		  	{
				Debug.Log (gameObject.name);
				parentToReturnTo = GameObject.Find("DropZone"+ i).transform;
			}
		}
//
//		if (parentToReturnTo = null) {
//			parentToReturnTo = gameObject.transform.parent;
//		}
//
		this.transform.SetParent (parentToReturnTo);
	}

	public void OnBeginDrag(PointerEventData eventData){	
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		parentToReturnTo = this.transform.parent;
		this.transform.SetParent (this.transform.parent.parent);
		
	}
	
	public void OnDrag(PointerEventData eventData){
		gameObject.transform.position = eventData.position;
	}
	
	public void OnEndDrag(PointerEventData eventData){
		this.transform.SetParent (parentToReturnTo);
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}
	
}
