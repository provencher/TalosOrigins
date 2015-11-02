using UnityEngine;
using System.Collections;

public class UpdatePlayer : MonoBehaviour {

	public void UpdateTalosUpgrade(GameObject dropZone){
		string name = dropZone.transform.GetChild(0).name;

		PlayerPrefs.SetInt (name, (PlayerPrefs.GetInt(name) + 1 ));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
