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
        mMoveSpeed = 1.75f;
        distanceThreshold = 12f;
        mHealth = 100;

        lastDirection = Vector2.right;
        nbTimesDied = 0;
        mBoxCollider = GetComponent<BoxCollider2D>();       


        distGround = mBoxCollider.siz.e
        distEdge = (box.yMin + (box.yMin - box.yMax)) / 2;

        distEdge = mBoxCollider.bounds.extents.x - mBoxCollider.bounds.center.x + 1;

        mCurrentLevel = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().currentLevel;
    }
    

    void FixedUpdate()
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



        crawlerFacedirection = Vector2.left;
        //animator
        mAnimator = GetComponent<Animator>();
        //mAnimator.enabled = true;
        crawlerIsWalking = true;
        crawlerIsShooting = false;
        crawlerIsHit = false;

        curNormal = transform.up;
        mRigidBody2D.freezeRotation = true;
    }

    void CrawlerUpdate()
    {
        /*
        if (Mathf.Abs(mRigidBody2D.velocity.y) <= 0.001f)
        {
            mRigidBody2D.isKinematic = true;
        }
        */

        if (firstLoop)
        {
            CrawlerInit();
            firstLoop = false;
            secondLoop = true;
            mRigidBody2D.velocity = transform.right;
        }


        mRigidBody2D.AddForce(-10 * mRigidBody2D.mass * curNormal);
        /*
         if (!Grounded())
         {
             mRigidBody2D.gravityScale = 1;
         }
         else
         {
             mRigidBody2D.gravityScale = 0;
         }
         */

        //CrawlerCheckMove();        

        
        Vector2 targetDirection = lastDirection;

        int d30Roll = Random.Range(1, 30);
        if (!DirectionClear(lastDirection) || d30Roll == 5)
        {
            targetDirection = FindDirectionWithTarget(playerPosition);
        }       

        lastDirection = targetDirection;
        //targetDirection.y = 0;

        CrawlerMove(targetDirection);
        //Pursue Player         
        //TranslateGroundToTarget(transform.position + (Vector3)targetDirection);

        //MoveCrawlerMoveCrawlerAlongWalls(true);

        //MoveCrawler();

        //CrawlerFaceDirection(targetDirection);
        CrawlerUpdateAnimator();
        
    }

    void CrawlerFaceDirection(Vector2 fDic)
    {
        if (fDic.x < 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
    }

    void CrawlerMove(Vector3 direction)
    {
        // jump code - jump to wall or simple jump
        //if (crawlerIsFalling) return; // abort Update while jumping to a wall

       
        /*
        if (Input.GetButtonDown("Jump"))
        { // jump pressed:
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, jumpRange))
            { // wall ahead?
                JumpToWall(hit.point, hit.normal); // yes: jump to the wall
            }
            else if (mIsGrounded)
            { // no: if grounded, jump up
                mRigidBody2D.velocity += jumpSpeed * myNormal;
            }
        }
        */

        // movement code - turn left/right with Horizontal axis:
        transform.Rotate(0, direction.x * turnSpeed * Time.deltaTime, 0);
        // update surface normal and isGrounded:


        RaycastHit2D leftInfo, rightInfo; 
        if (doubleRaycastDown(out leftInfo, out rightInfo))
        { // use it to update myNormal and isGrounded
            float distance = Mathf.Max(leftInfo.distance, rightInfo.distance);
            mIsGrounded = distance <= distGround + deltaGround;
            surfaceNormal = leftInfo.normal;
        }
        else
        {
            mIsGrounded = false;
            // assume usual ground normal to avoid "falling forever"
            surfaceNormal = Vector3.up;
        }

        curNormal = Vector3.Lerp(curNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
        // find forward direction with new myNormal:
        Vector3 myForward = Vector3.Cross(transform.right, curNormal);
        // align character to the new myNormal while keeping the forward direction:
        Quaternion targetRot = Quaternion.LookRotation(myForward, curNormal);
        transform.rotation = Quaternion.FromToRotation(curNormal, surfaceNormal);//Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);
        // move the character forth/back with Vertical axis:
        transform.Translate(0, 0, direction.y * mMoveSpeed * Time.deltaTime);
    }    

    void CrawlerCheckMove()
    {
        if (mRigidBody2D.velocity == Vector2.zero)
        {
            crawlerIsWalking = false;
        }
        else
        {
            crawlerIsWalking = true;




            if (FindDirectionWithTarget(playerPosition).x < 0)
            {
                crawlerFacedirection = Vector2.left;
            }
            else
            {
                crawlerFacedirection = Vector2.right;
            }
        }
    }

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
        Vector2 leftStart = transform.position + distGround * transform.up + distEdge * transform.right;
        Vector2 rightStart = transform.position + distGround * transform.up - distEdge * transform.right;

        rightHitInfo = Physics2D.Raycast(leftStart, -transform.up, distGround/*, groundLayer*/);
        leftHitInfo  = Physics2D.Raycast(rightStart, -transform.up, distGround/*, groundLayer*/);

        Debug.DrawLine(leftStart, (Vector2)(transform.position + transform.up * -distGround), Color.red, 2, false);
        Debug.DrawLine(rightStart, (Vector2)(transform.position + transform.up * -distGround), Color.red, 2, false);



        return rightHitInfo.collider != null && leftHitInfo.collider != null;
    }


    void positionOnTerrain(RaycastHit2D leftHitInfo, RaycastHit2D rightHitInfo, float boxSizeY)
    {      
        
        Vector3 averageNormal = (leftHitInfo.normal + rightHitInfo.normal) / 2;
        Vector3 averagePoint = (leftHitInfo.point + rightHitInfo.point) / 2;

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
        Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90);
        transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);

        currentPosition = transform.position;
        transform.position = averagePoint + transform.up * boxSizeY;

        mRigidBody2D.velocity = (transform.position - currentPosition).normalized;
    }

    void MoveCrawler()
    {

        float boxSizeX = GetComponent<BoxCollider2D>().size.x * transform.localScale.x ;
        float boxSizeY = GetComponent<BoxCollider2D>().size.y * transform.localScale.y ;
        //int groundLayer = LayerMask.NameToLayer("Map");

        Vector3 transformUp = transform.up;
        Vector3 transformRight = transform.right;

        RaycastHit2D leftHitInfo, rightHitInfo;

        //doubleRaycastDown(out leftHitInfo, out rightHitInfo, transformUp, transformRight, boxSizeX, boxSizeY, groundLayer);

        Vector2 leftStart = transform.position + boxSizeY * transformUp + boxSizeX * transformRight;
        Vector2 rightStart = transform.position + boxSizeY * transformUp - boxSizeX * transformRight;

        rightHitInfo = Physics2D.Raycast(leftStart, -transformUp, boxSizeY, groundLayer);
        leftHitInfo = Physics2D.Raycast(rightStart, -transformUp, boxSizeY, groundLayer);

        
        if (mRigidBody2D.velocity.sqrMagnitude > 0)
        {          

            //if moving left
            if (Vector3.Dot((transform.position - currentPosition).normalized, transform.right) >= 0)
            {
                leftStart = transform.position + boxSizeY * transformUp;
                RaycastHit2D overrideLeftHitInfo = Physics2D.Raycast(leftStart, transform.right, boxSizeY, groundLayer);

                if (overrideLeftHitInfo)
                {
                    leftHitInfo = overrideLeftHitInfo;
                }
            }
            //if moving right
            else
            {
                rightStart = transform.position + boxSizeY * transformUp;
                RaycastHit2D overrideRightHitInfo = Physics2D.Raycast(rightStart, -transform.right, boxSizeY, groundLayer);

                if (overrideRightHitInfo)
                {
                    rightHitInfo = overrideRightHitInfo;
                }
            }
        }
        

        // Reposition Crawler
        if (leftHitInfo && rightHitInfo)
        {
            positionOnTerrain(leftHitInfo, rightHitInfo, boxSizeY);
        }
        else
        {
            mRigidBody2D.isKinematic = false;
        }
    }
    
    /*
    void MoveCrawlerAlongWalls(bool forward)
    {
        int directionModif = (forward) ? 1 : -1;
        float boxSizeX = GetComponent<BoxCollider2D>().size.x * transform.localScale.x / 1.8f;
        float boxSizeY = GetComponent<BoxCollider2D>().size.y  * transform.localScale.y / 1.8f;

        int groundLayer = LayerMask.NameToLayer("Map");

        if(crawlerIsWalking)
        {
            Vector2 direction = (Vector2)transform.right * directionModif * boxSizeX;
            // Check for cave wall
            RaycastHit2D rayHit1 = Physics2D.Raycast(transform.position, direction, boxSizeX, groundLayer);

            if (rayHit1.collider != null )//&& rayHit1.gameObject.tag == "Asteroid" || rayHit1.transform.gameObject.tag == "Cave")
            {                
                hitNormal = rayHit1.normal;
                crawlerIsWalking = false;
            }
            else
            {
                //mRigidBody2D.gravityScale = 1;
                //return;
            }

            Debug.DrawLine(transform.position, (Vector2)transform.position + (Vector2)transform.right * directionModif * boxSizeX, Color.red, 2, false);

            // Check for no floor
            Vector2 checkRear = (Vector2)transform.position + (-(Vector2)transform.right * directionModif * boxSizeX);
            RaycastHit2D rayHit2 = Physics2D.Raycast(checkRear, - transform.up, boxSizeX, groundLayer);

            if(rayHit2.collider != null)// && rayHit2.collider.tag == "Asteroid" || rayHit2.collider.tag == "Cave")
            {
                //Floor exists             
            }
            else
            {
                // Find the floor around the corner
                Vector2 checkPos = (Vector2)transform.position  - (Vector2)transform.right * boxSizeX * directionModif + (Vector2)transform.up * -1 * boxSizeY;

                RaycastHit2D rayHit3 = Physics2D.Raycast(checkPos, -transform.right * directionModif, boxSizeX, groundLayer);
                Debug.DrawLine(checkPos, checkPos- (Vector2)transform.right * directionModif * boxSizeX, Color.red, 2, false);
                if (rayHit3.collider != null)// && rayHit3.collider.tag == "Asteroid" || rayHit3.collider.tag == "Cave")
                {                    
                    Debug.Log("HitNormal " + rayHit3.normal);
                    hitNormal = rayHit3.normal;
                    crawlerIsWalking = false;
                }
                Debug.DrawLine(transform.position, transform.position + transform.up * (-1) * boxSizeY, Color.red, 2, false);
                MoveCrawler(forward);
            }
        }
        else
        {
            curNormal = Vector2.Lerp(curNormal, hitNormal, 4.0f * Time.deltaTime);
            Quaternion groundTilt = Quaternion.FromToRotation(Vector2.up, curNormal);
            transform.rotation = groundTilt;

            float check = (curNormal - hitNormal).sqrMagnitude;
            if( check < 0.001)
            {
                groundTilt = Quaternion.FromToRotation(Vector2.up, hitNormal);
                transform.rotation = groundTilt;
                crawlerIsWalking = true;
            }
        }
    }
    */

    void MoveCrawler(bool forward)
    {
        int directionModif = (forward) ? 1 : -1;
        transform.position -= transform.right * directionModif * mMoveSpeed * Time.deltaTime;
    }

    void TranslateGroundToTarget(Vector3 target)
    {
        if (target != null)
        {
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

        for (int i = 0; i < 1; i++)
        {
            /*
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
                        increment = boxSize /1.5f  * scaleSize;
                        break;
                    }
                case 2:
                    {
                        increment = (-1) * boxSize/1.5f * scaleSize;
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
              */
            StartPosition = transform.position;

            EndPosition = StartPosition + direction * 0.6f;

            //Check if clear
            hit = Physics2D.Linecast(StartPosition, EndPosition);
            Debug.DrawLine(StartPosition, EndPosition, Color.red, 2, false);

            isClear = isClear && !(hit &&
               (hit.collider.tag == "Cave" ||
               hit.collider.tag == "Enemy" ||
               hit.collider.tag == "Asteroid"
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
            Instantiate(LaserGreenHit, transform.position, transform.rotation);         //Instantiate LaserGreenHit 
            

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
                Instantiate(Explosion, transform.position, transform.rotation);       //Instantiate Explosion   
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
