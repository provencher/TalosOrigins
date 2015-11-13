using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{

    public Transform parentToReturnTo = null;
    [SerializeField]
    enum upgradeType {Health, Jump, Grapple, Bullets, Shield, Breadcrumbs};

	public void OnBeginDrag(PointerEventData eventData){	
		parentToReturnTo = this.transform.parent;
		this.transform.SetParent (this.transform.parent.parent);

		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	public void OnDrag(PointerEventData eventData){
		gameObject.transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData){
		this.transform.SetParent (parentToReturnTo);
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

}
