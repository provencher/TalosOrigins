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
    public float distanceThreshold;

    public int mCurrentLevel;
    int mExpGiven;

    LayerMask ignoreLayer, defaultLayer;

    Vector3 playerPosition;

    // Retain index of enemy to properly handle destruction
    int mapGenIndex;

    bool firstLoop = true;
    

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
        mMoveSpeed = 3.0f;
        distanceThreshold = 12f;        

        lastDirection = Vector2.right;        
    }

    void FixedUpdate()
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
        if(firstLoop)
        {
            FlyerInit();
            firstLoop = false;
        }

        if (InTalosRange(playerPosition))
        {
            Vector2 targetDirection = lastDirection;

            int dice6Roll = Random.Range(1, 6);

            if (!DirectionClear(lastDirection) || dice6Roll%5 == 0)
            {
                targetDirection = FindDirectionWithTarget(playerPosition);
                lastDirection = targetDirection;
            }
            
            //Pursue Player            
            TranslateToTarget(transform.position + (Vector3)targetDirection);
        }
    }

    void FlyerInit()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    void WalkerUpdate()
    { }

    void RunnerUpdate()
    { }

    bool InTalosRange(Vector3 target)
    {
        Vector3 distance = target - transform.position;

        if (distance.magnitude < distanceThreshold)
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
            direction.z = 0;
            transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;

            /*
            if (direction.magnitude > stepOverThreshold)
            {
                // If too far, translate at kFollowSpeed
                transform.Translate(direction.normalized * mMoveSpeed * Time.deltaTime);
            }
            else
            {
                target.z = 0;
                // If close enough, just step over
                transform.position = target;                
            }
            */
        }
    }

    Vector2 FindDirectionWithTarget(Vector3 target)
    {        
        Vector3 direction = (target - transform.position).normalized;
        Vector2 ret = direction; 

        if(!(DirectionClear(ret)))
        {
            Vector2 dir1 = new Vector2(direction.x, 0);
            Vector2 dir2 = new Vector2(0, direction.y);

            bool dir1Clear = DirectionClear(dir1);
            bool dir2Clear = DirectionClear(dir2);

            float x = Mathf.Abs(direction.x);
            float y = Mathf.Abs(direction.y);


            int coinFlip = 0;
            if (dir1Clear && dir2Clear)
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
            else if (dir1Clear && !dir2Clear)
            {
                ret = dir1;

            }
            else if (dir2Clear && !dir1Clear)
            {
                ret = dir2;
            }
            else
            {
                dir1 = -1 * dir1;
                dir2 = -1 * dir2;
                dir1Clear = DirectionClear(dir1);
                dir2Clear = DirectionClear(dir2);

                if (dir1Clear && dir2Clear)
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
                else if (dir1Clear && !dir2Clear)
                {
                    ret = dir1;

                }
                else
                {
                    ret = dir2;
                }
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
        float boxSizeX = GetComponent<BoxCollider2D>().size.x;
        float scaleX = transform.localScale.x;

        float boxSizeY = GetComponent<BoxCollider2D>().size.x;
        float scaleY = transform.localScale.x;

        float boxSize, scaleSize;

        for (int i = 0; i < 3; i++)
        {
            increment = 0;

            if (DirectionIsHorizontal(direction))
            {
                boxSize = boxSizeX;
                scaleSize = scaleX;
            }
            else
            {
                boxSize = boxSizeY;
                scaleSize = scaleY;
            }

            switch (i)
            {
                case 1:
                    {
                        increment = boxSize * scaleSize;
                        break;
                    }
                case 2:
                    {
                        increment = (-1) * boxSize * scaleSize;
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

            EndPosition = StartPosition + direction * 0.6f;

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

    void KilledBySword()
    {
        NotifyOfDeath();
    }

    void NotifyOfDeath()
    {
        // Notify Player of kill with experience gained
        GameObject.FindGameObjectWithTag("Player").SendMessage("KilledEnemy", CalculateEXP(mCurrentLevel));

        // Notify Map Generator of index of enemy killed
        GameObject.Find("MapGenerator").SendMessage("KilledEnemy", mapGenIndex);
    }
}
