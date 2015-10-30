using UnityEngine;
using System.Collections;

public class MouseUsage : MonoBehaviour {

	Vector3 mousePosition;
	// Use this for initialization
	void Start () {
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector3(mousePosition.x, mousePosition.y, -1f);
	}
}
