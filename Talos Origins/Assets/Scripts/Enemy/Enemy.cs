using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public enum eClass { flyer, walker, runner, Crawler };
    public eClass type;

    Vector2[] possibleDirections = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    Vector2 lastDirection;
    Vector3 currentPosition;

    Animator mAnimController;
    Rigidbody2D mRigidBody2D;
    BoxCollider2D mBoxCollider;

    public float mMoveSpeed;
    float turnSpeed = 90;
    float lerpSpeed = 10;
    float deltaGround = 0.2f;
    float stepOverThreshold = 0.05f;
    public float distanceThreshold;

    int mCurrentLevel;
    int mExpGiven;
    float mDamageModifier;
    public int mHealth;

    LayerMask ignoreLayer, defaultLayer;

    Vector3 playerPosition;

    int nbTimesDied;

    // Retain index of enemy to properly handle destruction
    int mapGenIndex;

    bool firstLoop = true;
    bool secondLoop = false;
    public bool mInRange = false;

    float distGround, distEdge;

    //For animator
    Animator mAnimator;

    //crawler
    public Vector2 crawlerFacedirection;
    bool crawlerIsWalking;
    bool crawlerIsShooting;
    bool  crawlerIsHit;
    bool crawlerIsFalling;

    Vector2 curNormal = Vector2.up;
    Vector2 surfaceNormal = Vector2.zero;

    bool mIsGrounded;

    int groundLayer = LayerMask.NameToLayer("Map");



    [SerializeField]
    public GameObject LaserGreenHit;    //LaserGreenHit Prefab

    [SerializeField]
    public GameObject Explosion; 		//Explosion Prefab

    [SerializeField]
    public GameObject HitByShotAudio;        //Explosion Prefab



    void Start() {
        //Write Code for Modifying stats based on currentLevel

        //Write logic for setting enemy type
        //type = eClass.flyer;
        gameObject.tag = "Enemy";

        mRigidBody2D = GetComponent<Rigidbody2D>();
        

        //Write logic for changing enemy sprite 

        // Initialize variables
        defaultLayer = LayerMask.NameToLayer("Enemy");
        ignoreLayer = LayerMask.NameToLayer("Ignore");

        mCurrentLevel = 1;
        mMoveSpeed = 1.5f;
        distanceThreshold = 12f;
        mHealth = 100;

        lastDirection = Vector2.right;
        nbTimesDied = 0;
        mBoxCollider = GetComponent<BoxCollider2D>();


        distGround = mBoxCollider.size.y /2.2f * transform.localScale.y;
        distEdge = mBoxCollider.size.x / 2.2f * transform.localScale.x;//(box.yMin + (box.yMin - box.yMax)) / 2;

        //distEdge = mBoxCollider.bounds.extents.x - mBoxCollider.bounds.center.x + 1;

        mCurrentLevel = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().currentLevel;
    }
    

    void Update()
    {
        CheckDead();
        currentPosition = transform.position;

        mInRange = InTalosRange(playerPosition);
        if (mInRange)
        {
            mRigidBody2D.isKinematic = false;

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
                case eClass.Crawler:
                    {
                        CrawlerUpdate();
                        break;
                    }
            }
        }
        else
        {
            mRigidBody2D.isKinematic = true;
        }
    }



    void CheckDead()
    {
        gameObject.GetComponent<EnemyHealthBar>().curHealth = mHealth;
        if (mHealth <= 0)
        {
            NotifyOfDeath();
        }
    }

    void FlyerInit()
    {

        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        mDamageModifier = 5;
        mHealth = 45 + (mCurrentLevel * 5);
        gameObject.GetComponent<EnemyHealthBar>().maxHealth = mHealth;
    }


    void FlyerUpdate()
    {
        if (firstLoop)
        {
            FlyerInit();
            firstLoop = false;
        }

        
            Vector2 targetDirection = lastDirection;

            int d30Roll = Random.Range(1, 30);
            if (!DirectionClear(lastDirection) || d30Roll == 5)
            {
                targetDirection = FindDirectionWithTarget(playerPosition);               
            }
            else if (d30Roll == 15)
            {
                targetDirection = ChooseRandomDirection();
            }
            
            lastDirection = targetDirection;

            //Pursue Player            
            TranslateAerialToTarget(transform.position + (Vector3)targetDirection);
        
    }

   

    void CrawlerInit()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        mDamageModifier = 5;
        mHealth = 45 + (mCurrentLevel * 5);

        gameObject.GetComponent<EnemyHealthBar>().maxHealth = mHealth;

        lastDirection = transform.right;

        crawlerFacedirection = Vector2.left;
        //animator
        mAnimator = GetComponent<Animator>();
        //mAnimator.enabled = true;
        crawlerIsWalking = true;
        crawlerIsShooting = false;
        crawlerIsHit = false;

        curNormal = transform.up;
        mRigidBody2D.freezeRotation = true;

        surfaceNormal = ChooseRandomDirection();
        mRigidBody2D.gravityScale = 0;

    }

    void CrawlerUpdate()
    {        
        if (firstLoop)
        {
            CrawlerInit();
            firstLoop = false;   
        }
        Vector2 targetDirection = lastDirection;

        int d30Roll = Random.Range(1, 30);
        if (!DirectionClear(lastDirection) || d30Roll == 5)
        {
            targetDirection = FindDirectionWithTarget(playerPosition);
        }        

        lastDirection = targetDirection;

        mRigidBody2D.AddForce(-9.81f * mRigidBody2D.mass * surfaceNormal);

        CrawlerMove(lastDirection);
        //Pursue Player         
        //TranslateGroundToTarget(transform.position + (Vector3)targetDirection);

        //MoveCrawlerMoveCrawlerAlongWalls(true);

        //MoveCrawler();
        CrawlerFaceDirection(lastDirection);

        //CrawlerFaceDirection(targetDirection);
        CrawlerUpdateAnimator();
        
    }

    void CrawlerFaceDirection(Vector2 fDic)
    {
        if (
            (fDic.x > 0 && transform.localScale.x > 0) ||
            (fDic.x < 0 && transform.localScale.x < 0)
            )
        {
            transform.localScale = new Vector3(-1* transform.localScale.x, transform.localScale.y, 0);
            //transform.rotation = Quaternion.LookRotation(transform.forward);
        }
        /*
        else
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
        */
    }

    void CrawlerMove(Vector3 direction)
    {      
        RaycastHit2D leftInfo, rightInfo; 
        if (doubleRaycastDown(out leftInfo, out rightInfo))
        {
            
            //if moving left
            if (Vector3.Dot(transform.right, direction) >= 0)
            {
                Vector3 leftStart = transform.position + distGround * transform.up;
                RaycastHit2D overrideLeftHitInfo = Physics2D.Raycast(leftStart, transform.right, distEdge);

                if (overrideLeftHitInfo.collider != null)
                {
                    leftInfo = overrideLeftHitInfo;
                }
            }
            //if moving right
            else
            {
                Vector3 rightStart = transform.position + distGround * transform.up;
                RaycastHit2D overrideRightHitInfo = Physics2D.Raycast(rightStart, -transform.right, distEdge);

                if (overrideRightHitInfo.collider != null)
                {
                    rightInfo = overrideRightHitInfo;
                }
            }           
            
            // use it to update myNormal and isGrounded            
            mIsGrounded = (leftInfo.distance + rightInfo.distance)/2 <= distGround + 0.3f;//deltaGround;
            surfaceNormal = (leftInfo.normal + rightInfo.normal) / 2;                

        }
        else
        {
            mIsGrounded = false;
            // assume usual ground normal to avoid "falling forever"
            surfaceNormal = Vector3.up;
        }

        
        Vector3 offset = positionOnTerrain(leftInfo, rightInfo, distGround);
        //direction.y = direction.y * 1.5f;
        if(direction.x > 0)
        {
            direction = transform.right;//+ offset;
            //CrawlerFaceDirection(Vector2.right);      
                        
        }else
        {
            direction = -transform.right;//  + offset;
            
            //CrawlerFaceDirection(-Vector2.right);
        }

        //transform.rotation = Quaternion.LookRotation(transform.forward);
        //direction = transform.right + offset;
        //transform.LookAt(direction);  
        TranslateGroundToTarget(direction);


        if ((transform.position - currentPosition).magnitude < (direction * mMoveSpeed * Time.deltaTime).magnitude)   //(currentPosition - transform.position).magnitude < 0.3f)
        {
            //mRigidBody2D.AddForce(mRigidBody2D.mass*(3*Vector3.up + direction));
            //mRigidBody2D.velocity += (Vector2) direction * 2;
           //mRigidBody2D.AddForce(5* transform.up + 5 * transform.localScale.x * transform.right);
        }
    }

    /* 
497     void MoveCrawlerAlongWalls(bool forward) 
498     { 
499         int directionModif = (forward) ? 1 : -1; 
500         float boxSizeX = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 1.8f; 
501         float boxSizeY = GetComponent<BoxCollider2D>().size.y  * transform.localScale.y / 1.8f; 
502  
503         int groundLayer = LayerMask.NameToLayer("Map"); 
504  
505         if(crawlerIsWalking) 
506         { 
507             Vector2 direction = (Vector2)transform.right * directionModif * boxSizeX; 
508             // Check for cave wall 
509             RaycastHit2D rayHit1 = Physics2D.Raycast(transform.position, direction, boxSizeX, groundLayer); 
510  
511             if (rayHit1.collider != null )//&& rayHit1.gameObject.tag == "Asteroid" || rayHit1.transform.gameObject.tag == "Cave") 
512             {                 
513                 hitNormal = rayHit1.normal; 
514                 crawlerIsWalking = false; 
515             } 
516             else 
517             { 
518                 //mRigidBody2D.gravityScale = 1; 
519                 //return; 
520             } 
521  
522             Debug.DrawLine(transform.position, (Vector2)transform.position + (Vector2)transform.right * directionModif * boxSizeX, Color.red, 2, false); 
523  
524             // Check for no floor 
525             Vector2 checkRear = (Vector2)transform.position + (-(Vector2)transform.right * directionModif * boxSizeX); 
526             RaycastHit2D rayHit2 = Physics2D.Raycast(checkRear, - transform.up, boxSizeX, groundLayer); 
527  
528             if(rayHit2.collider != null)// && rayHit2.collider.tag == "Asteroid" || rayHit2.collider.tag == "Cave") 
529             { 
530                 //Floor exists              
531             } 
532             else 
533             { 
534                 // Find the floor around the corner 
535                 Vector2 checkPos = (Vector2)transform.position  - (Vector2)transform.right * boxSizeX * directionModif + (Vector2)transform.up * -1 * boxSizeY; 
536  
537                 RaycastHit2D rayHit3 = Physics2D.Raycast(checkPos, -transform.right * directionModif, boxSizeX, groundLayer); 
538                 Debug.DrawLine(checkPos, checkPos- (Vector2)transform.right * directionModif * boxSizeX, Color.red, 2, false); 
539                 if (rayHit3.collider != null)// && rayHit3.collider.tag == "Asteroid" || rayHit3.collider.tag == "Cave") 
540                 {                     
541                     Debug.Log("HitNormal " + rayHit3.normal); 
542                     hitNormal = rayHit3.normal; 
543                     crawlerIsWalking = false; 
544                 } 
545                 Debug.DrawLine(transform.position, transform.position + transform.up * (-1) * boxSizeY, Color.red, 2, false); 
546                 MoveCrawler(forward); 
547             } 
548         } 
549         else 
550         { 
551             curNormal = Vector2.Lerp(curNormal, hitNormal, 4.0f * Time.deltaTime); 
552             Quaternion groundTilt = Quaternion.FromToRotation(Vector2.up, curNormal); 
553             transform.rotation = groundTilt; 
554  
555             float check = (curNormal - hitNormal).sqrMagnitude; 
556             if( check < 0.001) 
557             { 
558                 groundTilt = Quaternion.FromToRotation(Vector2.up, hitNormal); 
559                 transform.rotation = groundTilt; 
560                 crawlerIsWalking = true; 
561             } 
562         } 
563     } 
564     */


    void CrawlerUpdateAnimator()
    {
        mAnimator.SetBool("isWalking", crawlerIsWalking);
        mAnimator.SetBool("isShooting", crawlerIsShooting);
        mAnimator.SetBool("isHit", crawlerIsHit);
    }

    void WalkerUpdate()
    { }

    void WalkerInit()
    {    }

    void RunnerUpdate()
    { }

    void RunnerInit()
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

    void TranslateAerialToTarget(Vector3 target)
    {    
        if (target != null)
        {
            Vector3 direction = target - transform.position;
            direction.z = 0;
            //transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;
            mRigidBody2D.velocity = mRigidBody2D.velocity + (Vector2)(direction.normalized * mMoveSpeed * Time.deltaTime);
        }
    }

    bool doubleRaycastDown(out RaycastHit2D leftHitInfo, out RaycastHit2D rightHitInfo)
    {    
        Vector2 leftStart = transform.position + 0.2f * - transform.up + distEdge * transform.right;
        Vector2 rightStart = transform.position + 0.2f * -transform.up - distEdge * transform.right;       

        rightHitInfo = Physics2D.Raycast(leftStart, -transform.up, 0.2f);
        leftHitInfo  = Physics2D.Raycast(rightStart, -transform.up, 0.2f);
      
        Debug.DrawLine(leftStart, leftStart + (Vector2)transform.up *-0.2f, Color.red, 2, false);
        Debug.DrawLine(rightStart, rightStart + (Vector2)transform.up * -0.2f, Color.red, 2, false);
        

        return rightHitInfo.collider != null && leftHitInfo.collider != null;
    }


    Vector3 positionOnTerrain(RaycastHit2D leftHitInfo, RaycastHit2D rightHitInfo, float boxSizeY)
    {      
        
        Vector3 averageNormal = (leftHitInfo.normal + rightHitInfo.normal) / 2;
        Vector3 averagePoint = (leftHitInfo.point + rightHitInfo.point) / 2;

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
        Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360* Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z); 

        if(mIsGrounded)
        {
            transform.position = averagePoint + transform.up * 0.35f;
            return (averagePoint + transform.up * distGround) - transform.position;
        }
        return Vector3.zero;               
    }

    

    void TranslateGroundToTarget(Vector3 target)
    {
        if (target != null)
        {
            currentPosition = transform.position;
            Vector3 direction = target - transform.position;
            direction.z = 0;          

            transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;        
        }     
    }

    Vector2 FindDirectionWithTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector2 ret = direction;

        if (!(DirectionClear(ret)))
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
                return -1 * lastDirection;
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
        /*
       if(type == eClass.Crawler)
        {
            if(direction.x > 0)
            {
                direction = transform.right;
            }
            else
            {
                direction = -transform.right;
            }
        }
        */
        
        StartPosition = transform.position;
        //StartPosition.y += offset;

        EndPosition = StartPosition + direction * 0.51f;
        

        //Check if clear
        hit = Physics2D.Linecast(StartPosition, EndPosition);
        Debug.DrawLine(StartPosition, EndPosition, Color.red, 2, false);

        isClear = isClear && !(hit &&
           (hit.collider.tag == "Cave" ||
           hit.collider.tag == "Enemy" ||
           hit.collider.tag == "Asteroid"
           ));
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
        return 50 * (int)(1 + level / 2);
    }

    // Each level, enemies do an extra 5% damage
    public float CalculateDamage()
    {
        return mDamageModifier * (1 + mCurrentLevel / 20);
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

    void ShovedByEnemy(Vector2 shoveDir)
    {
        switch(type)
        {
            case eClass.flyer:
                {
                    //mRigidBody2D.AddForce(shoveDir, ForceMode2D.Impulse);
                    break;
                }
            default:
                break;
        }
    }

    void HitByBullet(int damage)
    {
        mHealth -= damage;
    }

    void DropOrbs(int numOrbs)
    {
        for (int i = 0; i < numOrbs; i++)
        {
            gameObject.GetComponentInParent<OrbController>().SpawnOrb(Random.Range(0,4));
        }

    }


    //Shove Player and provide him with damage and direction
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if (coll.gameObject.tag == "Player")
            {
                coll.gameObject.SendMessage("ShovedByEnemy", new Vector3(lastDirection.x, lastDirection.y, CalculateDamage()));
            }                      
        }


        //Excute if the object tag was equal to one of these
        if (coll.gameObject.tag == "Bullet" || coll.gameObject.tag == "enemyBullet")
        {
            Instantiate(HitByShotAudio, transform.position, Quaternion.identity);
            Instantiate(LaserGreenHit, transform.position, Quaternion.identity);         //Instantiate LaserGreenHit 
            

            //Check the Health if greater than 0
            if (mHealth > 0)
            {
                if (coll.gameObject.tag == "enemyBullet")
                {
                    mHealth -= coll.gameObject.GetComponent<EnemyCoBullet>().mDamage;
                    coll.gameObject.GetComponent<EnemyCoBullet>().mHit = true;
                    //Destroy(coll.gameObject);
                }
                else
                {
                    mHealth -= coll.gameObject.GetComponent<Bullet>().mDamage;
                    coll.gameObject.GetComponent<Bullet>().mAlive = false;
                }               
                
            }
                                                                              //Decrement Health by 1

            //Check the Health if less or equal 0
            if (mHealth <= 0)
            {               
                Instantiate(Explosion, transform.position, Quaternion.identity);       //Instantiate Explosion   
                DropOrbs(5);                                                                                                     
                NotifyOfDeath();
                nbTimesDied++;

                if(nbTimesDied > 1)
                {
                    Destroy(gameObject);
                }
            }
        }
    }   
}
