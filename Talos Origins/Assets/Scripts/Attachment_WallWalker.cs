using UnityEngine;
using System.Collections;

public class Attachment_WallWalker : MonoBehaviour {

    public bool mIsGrounded;
    public float mDistGround;
    public float mDistEdge;
    public float mWalkSpeed;

    public Vector3 mSurfaceNormal;
    public bool USERINPUT = false;    

    private Rigidbody2D mRigidBody2D;
    private Vector3 mLastPosition;

    public Vector3 mNextDirection;
    public Vector3 mLastDirection; 


    // Use this for initialization
    void Start () {
        mIsGrounded = false;
        mDistGround = GetComponent<BoxCollider2D>().size.y/2;
        mDistEdge = GetComponent<BoxCollider2D>().size.x / 2;
        mRigidBody2D = GetComponent<Rigidbody2D>();

        mRigidBody2D.gravityScale = 0;
        mSurfaceNormal = transform.up;
    }

    void FixedUpdate()
    {     
        mRigidBody2D.AddForce(-9.81f * mRigidBody2D.mass * mSurfaceNormal);       
    }

    // Update is called once per frame
    void Update ()
    {
        //ApplyMovementDirection(mNextDirection);        
        //Wait for Next Input        
    }

    public void ApplyMovementDirection(Vector3 direction)
    {
        RaycastHit2D leftInfo, rightInfo;
        if (doubleRaycastDown(out leftInfo, out rightInfo))
        {

            //if moving left
            if (Vector3.Dot(transform.right, direction) >= 0)
            {
                Vector3 leftStart = transform.position + mDistGround * transform.up;
                RaycastHit2D overrideLeftHitInfo = Physics2D.Raycast(leftStart, transform.right, mDistEdge);

                if (overrideLeftHitInfo.collider != null)
                {
                    leftInfo = overrideLeftHitInfo;
                }
            }
            //if moving right
            else
            {
                Vector3 rightStart = transform.position + mDistGround * transform.up;
                RaycastHit2D overrideRightHitInfo = Physics2D.Raycast(rightStart, -transform.right, mDistEdge);

                if (overrideRightHitInfo.collider != null)
                {
                    rightInfo = overrideRightHitInfo;
                }
            }

            // use it to update myNormal and isGrounded            
            mIsGrounded = (leftInfo.distance + rightInfo.distance) / 2 <= mDistGround + 0.3f;//deltaGround;
            mSurfaceNormal = (leftInfo.normal + rightInfo.normal) / 2;
        }
        else
        {
            mIsGrounded = false;
            // assume usual ground normal to avoid "falling forever"
            mSurfaceNormal = Vector3.up;
        }


        Vector3 offset = positionOnTerrain(leftInfo, rightInfo, mDistGround);
        //direction.y = direction.y * 1.5f;
        if (direction.x > 0)
        {
            direction = transform.right;//+ offset;
                                        //CrawlerFaceDirection(Vector2.right);      

        }
        else
        {
            direction = -transform.right;//  + offset;

            //CrawlerFaceDirection(-Vector2.right);
        }
        direction.y *= 2;
        //transform.rotation = Quaternion.LookRotation(transform.forward);
        //direction = transform.right + offset;
        //transform.LookAt(direction);      

        mLastPosition = transform.position;
        

        //mRigidBody2D.velocity = direction * mMoveSpeed; //* Time.deltaTime;

        if (!USERINPUT)
        {
            FaceDirection(direction);
            transform.position += direction * mWalkSpeed * Time.deltaTime;
        }
        else
        {
            mRigidBody2D.AddForce(10 * direction);
        }

        if ((transform.position - mLastPosition).magnitude < (direction * mWalkSpeed * Time.deltaTime).magnitude)   //(currentPosition - transform.position).magnitude < 0.3f)
        {
            //mRigidBody2D.AddForce(mRigidBody2D.mass*(3*Vector3.up + direction));
            //mRigidBody2D.velocity += (Vector2) direction * 2;
            //mRigidBody2D.AddForce(5* transform.up + 5 * transform.localScale.x * transform.right);
        }
    }

    void FaceDirection(Vector2 fDic)
    {
        if (
            (fDic.x > 0 && transform.localScale.x > 0) ||
            (fDic.x < 0 && transform.localScale.x < 0)
            )
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 0);
            //transform.rotation = Quaternion.LookRotation(transform.forward);
        }
        /*
        else
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
        */
    }

    bool doubleRaycastDown(out RaycastHit2D leftHitInfo, out RaycastHit2D rightHitInfo)
    {
        Vector2 leftStart = transform.position + 0.2f * -transform.up + mDistEdge * transform.right;
        Vector2 rightStart = transform.position + 0.2f * -transform.up - mDistEdge * transform.right;

        rightHitInfo = Physics2D.Raycast(leftStart, -transform.up, 0.2f);
        leftHitInfo = Physics2D.Raycast(rightStart, -transform.up, 0.2f);

        Debug.DrawLine(leftStart, leftStart + (Vector2)transform.up * -0.2f, Color.red, 2, false);
        Debug.DrawLine(rightStart, rightStart + (Vector2)transform.up * -0.2f, Color.red, 2, false);


        return rightHitInfo.collider != null && leftHitInfo.collider != null;
    }


    Vector3 positionOnTerrain(RaycastHit2D leftHitInfo, RaycastHit2D rightHitInfo, float boxSizeY)
    {

        Vector3 averageNormal = (leftHitInfo.normal + rightHitInfo.normal) / 2;
        Vector3 averagePoint = (leftHitInfo.point + rightHitInfo.point) / 2;

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
        Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);

        if (mIsGrounded)
        {
            transform.position = averagePoint + transform.up * 0.35f;
            return (averagePoint + transform.up * mDistGround) - transform.position;
        }
        return Vector3.zero;
    }

}
