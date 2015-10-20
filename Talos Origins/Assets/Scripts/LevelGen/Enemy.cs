using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {   
    public enum eClass { flyer, walker, runner };
    public eClass type;

    Vector2[] possibleDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    Vector3 currentPosition;

    Animator animController;
    Rigidbody2D rb;

    public float mMoveSpeed;
    float stepOverThreshold = 0.15f;

    public int currentLevel, expGiven;

    LayerMask ignoreLayer, defaultLayer;

    

    void Start () {
        //Write Code for Modifying stats based on currentLevel

        //Write logic for setting enemy type
        type = eClass.flyer;
        gameObject.tag = "flyer";

        //Write logic for changing enemy sprite 

        // Initialize variables
        defaultLayer = LayerMask.NameToLayer("Enemy");
        ignoreLayer = LayerMask.NameToLayer("Ignore");

        if (currentLevel == default(int))
        {
            currentLevel = 1;
        }
        if (expGiven == default(int))
        {
            expGiven = (int)50 * (1 + currentLevel / 2);
        }
        if(mMoveSpeed == default(float))
        {
            mMoveSpeed = 5.0f;
        }
    }

    void Update()
    {
        currentPosition = transform.position;

        // Update according to enemy class
        switch (type)
        {
            case eClass.flyer:
                {
                    FlyerUpdate();
                    break;
                }
            case eClass.runner:
                {
                    WalkerUpdate();
                    break;
                }
            case eClass.walker:
                {
                    RunnerUpdate();
                    break;
                }
        }        
    }


    void FlyerUpdate()
    {
        

        
    }

    void WalkerUpdate()
    { }

    void RunnerUpdate()
    { }

    void TranslateToTarget(Vector3 target)
    {
        if (target != null)
        {
            Vector3 direction = target - transform.position;

            if (direction.magnitude > stepOverThreshold)
            {
                // If too far, translate at kFollowSpeed
                transform.Translate(direction.normalized * mMoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // If close enough, just step over
                transform.position = target;
            }
        }
    }

    bool DirectionClear(Vector2 direction)
    {
        bool isClear = true;
        Vector2 StartPosition;
        Vector2 EndPosition;
        RaycastHit2D hit = new RaycastHit2D();
        float increment;
        float boxSize = GetComponent<BoxCollider2D>().size.x;
        float scale = transform.localScale.x;

        for (int i = 0; i < 3; i++)
        {
            increment = 0;
            switch (i)
            {
                case 1:
                    {
                        increment = boxSize * scale / 2;
                        break;
                    }
                case 2:
                    {
                        increment = (-1) * boxSize * scale / 2;
                        break;
                    }
                default:
                    break;
            }

            StartPosition = transform.position;

            if (DirectionIsHorizontal(direction))
            {
                StartPosition.y += increment;
            }
            else
            {
                StartPosition.x += increment;
            }

            EndPosition = StartPosition + direction * 0.38f;

            //Check if clear
            hit = Physics2D.Linecast(StartPosition, EndPosition);
            Debug.DrawLine(StartPosition, EndPosition, Color.red, 2, false);

            isClear = isClear && !(hit.collider != null &&
               (hit.collider.gameObject.tag == "Cave" ||
               hit.collider.gameObject.tag == "Enemy"                
               ));

        }

        return isClear;
    }

    Vector2 FindDirectionTowardsTargetPosition(Vector3 target)
    {
        // Vector difference from position to Digger
        Vector3 direction = target - transform.position;

        Vector2 ret = new Vector2();

        direction.x /= 2;
        direction.y /= 2;

        Vector2 dir1 = NormalizeDirection(new Vector2(direction.x, 0));
        Vector2 dir2 = NormalizeDirection(new Vector2(0, direction.y));

        bool dir1Clear = DirectionClear(dir1);
        bool dir2Clear = DirectionClear(dir2);

        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);


        int coinFlip = 0;

        if (dir1Clear && dir1Clear)
        {
            coinFlip = Random.Range(0, 1);
            if (coinFlip == 0)
            {
                ret = dir1;
            }
            else
            {
                ret = dir2;
            }
        }
        else if (x < y && dir1Clear)
        {
            ret = dir1;

        }
        else if (y < x && dir2Clear)
        {
            ret = dir2;
        }

        else if (dir2Clear && !dir1Clear)
        {
            ret = dir2;
        }
        else if (dir1Clear && !dir2Clear)
        {
            ret = dir1;
        }
        else
        {
            dir1 = -1 * dir1;
            dir2 = -1 * dir2;
            dir1Clear = DirectionClear(dir1);
            dir2Clear = DirectionClear(dir2);

            if (dir1Clear && dir1Clear)
            {
                coinFlip = Random.Range(0, 1);
                if (coinFlip == 0)
                {
                    ret = dir1;
                }
                else
                {
                    ret = dir2;
                }
            }
            else
            {
                int loopCount = 0;
                while (true)
                {
                    loopCount++;
                    coinFlip = Random.Range(0, 99);
                    var direct = possibleDirections[coinFlip % possibleDirections.Length];
                    if (DirectionClear(direct))
                    {
                        ret = direct;
                        break;
                    }
                    else if (loopCount > 15)
                    {
                        if (dir1Clear && !dir2Clear)
                        {
                            ret = dir1;
                        }
                        else
                        {
                            ret = dir2;
                        }
                        break;
                    }
                }
            }
        }

        /*
                if (x > y && dir1Clear)
                {
                    ret = dir1;

                }
                else if (y > x && dir2Clear)
                {
                    ret = dir2;
                }
                else
                {
                    if (dir2Clear && !dir1Clear)
                    {
                        ret = dir2;
                    }
                    else if (dir1Clear && !dir2Clear)
                    {
                        ret = dir1;
                    }
                    else if (DirectionClear(-1 * dir1))
                    {
                        ret = -1 * dir1;
                    }
                    else
                    {
                        ret = -1 * dir2;
                    }

                }
                */
        return ret;
    }


    bool DirectionIsHorizontal(Vector2 direction)
    {
        if (direction.x != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

    Vector2 NormalizeDirection(Vector2 dir)
    {
            Vector2 ret = new Vector2();

            if (dir.x > 0)
            {
                ret.x = 1;
            }
            else if (dir.x < 0)
            {
                ret.x = -1;
            }
            else
            {
                ret.x = 0;
            }

            if (dir.y > 0)
            {
                ret.y = 1;
            }
            else if (dir.y < 0)
            {
                ret.y = -1;
            }
            else
            {
                ret.y = 0;
            }

            return ret;
        }

  

    void SetCollisionWithPlayer(bool enabled)
    {
        if (enabled)
        {
            gameObject.tag = "Enemy";
            gameObject.layer = defaultLayer;
        }
        else
        {
            gameObject.tag = "Ignore";
            gameObject.layer = ignoreLayer;
        }
    }

    // Message Receive function to communicate level and change enemy stats
    void UpdateLevel()
    {

    }    
}
