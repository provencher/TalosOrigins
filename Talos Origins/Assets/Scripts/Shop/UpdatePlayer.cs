using UnityEngine;
using System.Collections;

public class UpdatePlayer : MonoBehaviour {

    Player PlayerRef;

    void Start() {
        PlayerRef = Player.playerRef;
    }

	public void UpdateTalosUpgrade(GameObject dropZone){
		string name = dropZone.transform.GetChild(0).name;
	
        	
//		PlayerPrefs.SetInt (name, (PlayerPrefs.GetInt(name) + 1 ));
		Debug.Log ("set " + dropZone.name);
	}

	public void ReturnToLevel(){
		Application.LoadLevel (1);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
