using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public enum eClass { flyer, walker, runner };
    public eClass type;

    Vector2[] possibleDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    Vector2 lastDirection;
    Vector3 currentPosition;

    Animator animController;
    Rigidbody2D rb;

    public float mMoveSpeed;
    float stepOverThreshold = 0.05f;
    public float exploreDistanceThreshold;

    public int mCurrentLevel;
    int mExpGiven;

    LayerMask ignoreLayer, defaultLayer;

    Vector3 playerPosition;

    // Retain index of enemy to properly handle destruction
    int mapGenIndex;

    bool firstLoop = true;

    void Init()
    {
        //Write Code for Modifying stats based on currentLevel

        //Write logic for setting enemy type
        type = eClass.flyer;
        gameObject.tag = "flyer";

        //Write logic for changing enemy sprite 

        // Initialize variables
        defaultLayer = LayerMask.NameToLayer("Enemy");
        ignoreLayer = LayerMask.NameToLayer("Ignore");

        mCurrentLevel = 1;
        mMoveSpeed = 5.0f;
        exploreDistanceThreshold = 5f;

        lastDirection = Vector2.right;     
    }


    void Start() {
        //Write Code for Modifying stats based on currentLevel

        //Write logic for setting enemy type
        type = eClass.flyer;
        gameObject.tag = "Enemy";

        //Write logic for changing enemy sprite 

        // Initialize variables
        defaultLayer = LayerMask.NameToLayer("Enemy");
        ignoreLayer = LayerMask.NameToLayer("Ignore");
        
        mCurrentLevel = 1; 
        mMoveSpeed = 5.0f; 
        exploreDistanceThreshold = 5f;        

        lastDirection = Vector2.right;        
    }

    void FixedUpdate()
    {
        if(firstLoop)
        {
            //Init();
            firstLoop = false;
        }

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
        Vector3 targetPosition = transform.position;

        if (ExploreCave(playerPosition))
        {
            if (!DirectionClear(lastDirection))
            {
                lastDirection = ChooseRandomDirection();                               
            }
            targetPosition.x += lastDirection.x;
            targetPosition.y += lastDirection.y;     
        }
        else
        {
            //Pursue Player
            Vector2 pursuit = FindDirectionWithTarget(playerPosition);
            targetPosition.x += pursuit.x;
            targetPosition.y += pursuit.y;
        }

        TranslateToTarget(targetPosition);
    }

    void WalkerUpdate()
    { }

    void RunnerUpdate()
    { }

    bool ExploreCave(Vector3 target)
    {
        Vector3 distance = target - transform.position;

        if (distance.magnitude > exploreDistanceThreshold)
        {
            return true;
        }
        return false;
    }

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

    Vector2 FindDirectionWithTarget(Vector3 target)
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
               ret = ChooseRandomDirection();
            }
        }
      
        return ret;
    }

    Vector2 ChooseRandomDirection()
    {
        int loopCount = 0;
        int randomInt;
        Vector2 direct;
        while (true)
        {
            loopCount++;
            randomInt = Random.Range(0, 99);
            direct = possibleDirections[randomInt % possibleDirections.Length];
            if (DirectionClear(direct))
            {
                return direct;               
            }
            else if (loopCount > 10)
            {
                return -1* lastDirection;
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

    int CalculateEXP(int level)
    {
        return (int)50 * (1 + level / 2);
    }

    // Collision Detection
    void OnCollisionEnter2D(Collision2D other)
    {
        // add code to detect collision with bullet        
    }

    // MESSAGING FUNCTIONS   
    ////////////////////////////////////////////////////////////
    void UpdatePlayerPosition(Vector3 playerPos)
    {
        playerPosition = playerPos;
    }    

    void UpdateEnemyIndex(int index)
    {
        mapGenIndex = index;
    }    
    
    void UpdateLevel(int level)
    {
        // Message Receive function to communicate level and change enemy stats
        mCurrentLevel = level;
    }

    void NotifyOfDeath()
    {
        // Notify Player of kill with experience gained
        GameObject.FindGameObjectWithTag("Player").SendMessage("KilledEnemy", CalculateEXP(mCurrentLevel));

        // Notify Map Generator of index of enemy killed
        GameObject.Find("MapGenerator").SendMessage("KilledEnemy", mapGenIndex);
    }
}
