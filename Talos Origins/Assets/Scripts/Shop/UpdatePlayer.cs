using UnityEngine;
using System.Collections;

public class UpdatePlayer : MonoBehaviour {

	public void ReturnToLevel(){
	
		if(GameObject.Find ("Orbs").GetComponent<ShopOrbs>().totalOrbsCount >= 0){
			PlayerPrefs.SetString( "Upgrade0", GameObject.Find ("DropZone0").GetComponent<DropZone>().CurrentUpgrade);
			PlayerPrefs.SetString( "Upgrade1", GameObject.Find ("DropZone1").GetComponent<DropZone>().CurrentUpgrade);
			PlayerPrefs.SetString( "Upgrade2", GameObject.Find ("DropZone2").GetComponent<DropZone>().CurrentUpgrade);

			Application.LoadLevel (0);
		}
	}

}
