using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

    [SerializeField]
    GameObject anchor;

    [SerializeField]
    LineRenderer lineRenderer;

    DistanceJoint2D grapple;
    Vector3 targetPosition;
    Vector3 targetDirection;
    bool grapplehooked;

	// Use this for initialization
	void Start () {
        grapple = GetComponent<DistanceJoint2D>();
        grapple.enabled = false;
        grapple.connectedBody = anchor.gameObject.GetComponent<Rigidbody2D>();
        grapplehooked = false;
        
    }

    // Update is called once per frame
    void Update () {

       if (Input.GetKeyDown (KeyCode.Mouse1))
       {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;

            targetDirection = targetPosition - transform.position;
            targetDirection.Normalize();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection);

            moveHook(hit.point);
            drawLine();
            lineRenderer.enabled = true;
            lineRenderer.SetColors(Color.red,Color.red);
            grapplehooked = true;

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            lineRenderer.enabled = false;
            grapple.enabled = false;

        }

        if (grapplehooked)
        {
            drawLine();

            if (Input.GetKey(KeyCode.Space))
            {
                grapple.distance -= grapple.distance*0.01f;
            }
        }
	}

    void drawLine()
    {
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
        lineRenderer.SetPosition(1, new Vector3(anchor.transform.position.x, anchor.transform.position.y, -1));
    }

    void moveHook(Vector3 anchorPosition)
    {
              
        anchor.gameObject.transform.position = anchorPosition;

        float distance = Mathf.Abs(Vector3.Distance(anchorPosition, transform.position));
      
        grapple.distance = distance;
        grapple.enabled = true;
    }
}
