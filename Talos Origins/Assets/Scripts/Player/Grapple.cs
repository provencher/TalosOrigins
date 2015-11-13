using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {
    [SerializeField]
    GameObject grappleHitAudio;

    [SerializeField]
    GameObject grappleFireAudio;

    [SerializeField]
    GameObject anchor;

    [SerializeField]
    LineRenderer lineRenderer;

    DistanceJoint2D grapple;
    Vector3 targetPosition;
    Vector3 targetDirection;
    public bool grapplehooked;

    public int grappleDistance;

    GameObject hookedObject = null;
    Vector3 hitPoint;

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

            if (hit.collider.gameObject != null && hit.collider.gameObject.tag == "Cave" || hit.collider.gameObject.tag == "Asteroid")
            {
                Instantiate(grappleFireAudio, transform.position, Quaternion.identity);
                Instantiate(grappleHitAudio, transform.position, Quaternion.identity);
                grappleHitAudio.GetComponent<audioPrefabController>().delay = 0.1f;               


                hitPoint = hit.point;
                moveHook(hit.point);
                lineRenderer.enabled = true;
                drawLine();
                lineRenderer.SetColors(Color.red, Color.red);
                grapplehooked = true;
                mTalos.mUsedDoubleJump = false;

                if (hit.collider.gameObject.tag == "Asteroid")                    
                {
                    hit.collider.gameObject.GetComponent<Asteroid_Script>().IsHooked();                  
                }               
                else
                {
                    hookedObject = null;                    
                }
            }                

        }

        UpdateAnchor();

        if (Input.GetButtonDown("Jump")|| (hookedObject == null && hitPoint == Vector3.zero))
        {
            unHook();
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
                unHook();
            }
        }
	}

    void UpdateAnchor()
    {
        if(hookedObject != null && hitPoint != Vector3.zero)
        {
            anchor.transform.position = hookedObject.transform.position;
            hitPoint = hookedObject.transform.position;
        }
    }

    public void unHook()
    {
        lineRenderer.enabled = false;
        grapple.enabled = false;
        hitPoint = Vector3.zero;
        hookedObject = null;
    }


    void drawLine()
    {        
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1));
        lineRenderer.SetPosition(1, new Vector3(anchor.transform.position.x, anchor.transform.position.y, -1));        
    }

    void moveHook(Vector3 anchorPosition)
    {
              
        anchor.gameObject.transform.position = anchorPosition;
        hitPoint = anchorPosition;

        float distance = Mathf.Abs(Vector3.Distance(anchorPosition, transform.position));
      
        grapple.distance = distance;
        grapple.enabled = true;
    }
}
