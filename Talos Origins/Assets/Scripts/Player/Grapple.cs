using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

    [SerializeField]
    GameObject Anchor;

    DistanceJoint2D grapple;
    Vector3 targetPosition;

	// Use this for initialization
	void Start () {
        grapple = GetComponent<DistanceJoint2D>();
        grapple.enabled = false;
        grapple.connectedBody = Anchor.gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update () {

       if (Input.GetKeyDown (KeyCode.Mouse1))
       {
           targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
           targetPosition.z = 0;

            Anchor.gameObject.transform.position = targetPosition;


            float distance = Mathf.Abs(Vector3.Distance(targetPosition, transform.position));
            Debug.Log(distance);

            grapple.distance = distance;
            grapple.enabled = true;
                
            

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            grapple.enabled = false;

        }

	}
}
