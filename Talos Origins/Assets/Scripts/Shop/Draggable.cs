using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
	
	public Transform parentToReturnTo = null;

	[SerializeField]
	Text Price;

	[SerializeField]
	public int mCost;

	string Upgrade = "Upgrade";

	void Start()
	{
		//Check if Talos is already equipped with this upgrade
		for(int i=0; i<3; i++){

			if(gameObject.name == PlayerPrefs.GetString(Upgrade + i))
		  	{
				parentToReturnTo = GameObject.Find("DropZone"+ i).transform;
			}
		}

		Price.text = mCost.ToString ();

		//Set the draggable to the appropriate drop zone
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
		if(gameObject.transform.parent.name == "Viewport")
		{
			for(int i=0; i<= 2; i++){
					
				if(PlayerPrefs.GetString(Upgrade + i) == gameObject.name)
				{
					PlayerPrefs.SetString(Upgrade + i, null);

				}

			}
		

		}

		GetComponent<CanvasGroup> ().blocksRaycasts = true;

	}
}
