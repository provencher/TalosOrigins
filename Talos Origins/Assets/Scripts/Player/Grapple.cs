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

    public int grappleDistance;

    Player mTalos;

    // Use this for initialization
    void Start()
    {
        mTalos = GameObject.Find("Talos").GetComponent<Player>();
        grapple = GetComponent<DistanceJoint2D>();
        grapple.enabled = false;
        grapple.connectedBody = anchor.gameObject.GetComponent<Rigidbody2D>();
        grapplehooked = false;

        if (grappleDistance == default(int))
        {
            grappleDistance = 4;
        }
    }

    // Update is called once per frame
    void Update () {

       if (Input.GetKeyDown (KeyCode.Mouse1))
       {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;

            targetDirection = (targetPosition - transform.position).normalized;          
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, grappleDistance);

            if (hit.collider != null && hit.collider.gameObject.tag == "Cave")
            {
                moveHook(hit.point);
                lineRenderer.enabled = true;
                drawLine();
                lineRenderer.SetColors(Color.red, Color.red);
                grapplehooked = true;
                mTalos.mUsedDoubleJump = false;
            }                

        }

        if (Input.GetButtonDown("Jump"))
        {
            lineRenderer.enabled = false;
            grapple.enabled = false;
        }

        if (grapplehooked)
        {
            drawLine();

            if (Input.GetButton("Up"))
            {
                grapple.distance -= grapple.distance * 0.01f;
            }
            else if (Input.GetButton("Down"))
            {
                grapple.distance += grapple.distance * 0.01f;
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
