using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
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

    int staticPositionCounter = 0;

    int mCurrentLevel;
    int mExpGiven;
    float mDamageModifier;
    public int mHealth;

    LayerMask ignoreLayer, defaultLayer;

    Vector3 playerPosition;

    int nbTimesDied;

    public float mScaleValue = 1;

    // Retain index of enemy to properly handle destruction
    int mapGenIndex;

    bool firstLoop = true;
    bool secondLoop = false;
    public bool mInRange = false;

    float distGround, distEdge;

    List<int> collectedOrbs;

    //For animator
    Animator mAnimator;
    Animator AnimatorFlyer;

    //crawler
    public Vector2 crawlerFacedirection;
    bool crawlerIsWalking;
    bool crawlerIsShooting;
    bool crawlerIsHit;
    bool crawlerIsFalling;

    bool flyerIsMovingUp;
    bool flyerIsMovingDown;

    Vector2 curNormal = Vector2.up;
    Vector2 surfaceNormal = Vector2.zero;

    bool mIsGrounded;
    
    [SerializeField]
    public GameObject LaserGreenHit;    //LaserGreenHit Prefab

    [SerializeField]
    public GameObject Explosion; 		//Explosion Prefab

    [SerializeField]
    public GameObject HitByShotAudio;        //Explosion Prefab

    public bool isBoss = false;

    
    void Start()
    {
        //Write Code for Modifying stats based on currentLevel
        collectedOrbs = new List<int>();
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
        distanceThreshold = 15;
        mHealth = Random.Range(mCurrentLevel, mCurrentLevel * Random.Range(1, 5));

        lastDirection = Vector2.right;
        nbTimesDied = 0;
        mBoxCollider = GetComponent<BoxCollider2D>();


        distGround = mBoxCollider.size.y / 1.95f * transform.localScale.y;
        distEdge = mBoxCollider.size.x / 1.95f * transform.localScale.x;//(box.yMin + (box.yMin - box.yMax)) / 2;

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

    void FixedUpdate()
    {
        if (type == eClass.Crawler)
        {           
            // Add directional force
            mRigidBody2D.AddForce(-9.81f * mRigidBody2D.mass * surfaceNormal);
        }
    }


    void CheckDead()
    {
        gameObject.GetComponent<EnemyHealthBar>().curHealth = mHealth;
        if (mHealth <= 0)
        {
            mHealth = 0;
            NotifyOfDeath();
        }
    }

    void FlyerInit()
    {

        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        mDamageModifier = 5;
        //mHealth = Random.Range(mCurrentLevel, mCurrentLevel * Random.Range(1, 15));

        mHealth =Mathf.CeilToInt( Mathf.Pow(2, mCurrentLevel) * mScaleValue);

        gameObject.GetComponent<EnemyHealthBar>().maxHealth = mHealth;

        if(isBoss)
        {
            transform.localScale *= 8;            
            mHealth *= 10;
            mDamageModifier *= 2;
            mMoveSpeed *= 2;       
        }

        //mRigidBody2D.mass *= mScaleValue;

        distGround = mBoxCollider.size.y / 1.95f * transform.localScale.y;
        distEdge = mBoxCollider.size.x / 1.95f * transform.localScale.x;//(box.yMin + (box.yMin - box.yMax)) / 2;

        AnimatorFlyer = GetComponent<Animator>();
        flyerIsMovingUp = false;
        flyerIsMovingDown = false;
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

        if (Vector3.Dot(mRigidBody2D.velocity, transform.right) > 0.75f)
        {
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }
        else if (Vector3.Dot(mRigidBody2D.velocity, -transform.right) > 0.75f)
        {
            transform.localScale = new Vector3( Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }

        lastDirection = targetDirection;

        //Pursue Player            
        TranslateAerialToTarget(transform.position + (Vector3)targetDirection);

        if (gameObject.GetComponent<Rigidbody2D>().velocity.y > 0.2f)
        {
            flyerIsMovingUp = true;
            flyerIsMovingDown = false;
        }
        else if (gameObject.GetComponent<Rigidbody2D>().velocity.y < -0.2f)
        {
            flyerIsMovingDown = true;
            flyerIsMovingUp = false;
        }
        else
        {
            flyerIsMovingDown = false;
            flyerIsMovingUp = false;
        }

        FlyerUpdateAnimator();
    }



    void CrawlerInit()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        mDamageModifier = 5;
        mHealth = Mathf.CeilToInt(mScaleValue * Random.Range(mCurrentLevel, mCurrentLevel * Random.Range(3,5)));

        gameObject.GetComponent<EnemyHealthBar>().maxHealth = mHealth;

        lastDirection = transform.right;

        crawlerFacedirection = Vector2.left;
        //animator
        mAnimator = GetComponent<Animator>();
        //mAnimator.enabled = true;
        crawlerIsWalking = true;
        crawlerIsShooting = false;
        crawlerIsHit = false;

        mMoveSpeed = 1.4f;

        curNormal = transform.up;
        mRigidBody2D.freezeRotation = true;

        surfaceNormal = ChooseRandomDirection();
        mRigidBody2D.gravityScale = 0;

        //mRigidBody2D.mass *= mScaleValue;

        if (isBoss)
        {
            transform.localScale *= 10;       
            mHealth *= 10;
            mDamageModifier *= 2;


        }

        distGround = mBoxCollider.size.y / 1.95f * transform.localScale.y;
        distEdge = mBoxCollider.size.x / 1.95f * transform.localScale.x;//(box.yMin + (box.yMin - box.yMax)) / 2;
    }

    void CrawlerUpdate()
    {
        if (firstLoop)
        {
            CrawlerInit();
            firstLoop = false;
        }


        Vector2 targetDirection = (DirectionClear(lastDirection)? lastDirection : -lastDirection);
        


        int d30Roll = Random.Range(1, 30);
        if (d30Roll == 5)
        {

            targetDirection = FindDirectionWithTarget(playerPosition);

            if (targetDirection.x > 0)
            {
                targetDirection = transform.right;//+ 0.1f * (Vector3)targetDirection;//+ offset;
                                                                                    //CrawlerFaceDirection(Vector2.right);      

            }
            else if (targetDirection.x < 0)
            {
                targetDirection = -transform.right;// + 0.1f * (Vector3)targetDirection;//  + offset;

                //CrawlerFaceDirection(-Vector2.right);
            }

            //targetDirection.y *= 1.5f;  
            /*
            if (staticPositionCounter > 50)
            {
                targetDirection = -targetDirection;
                staticPositionCounter = 0;
            }
            */

            //direction.y *= 2;
            
        }

        CrawlerFaceDirection(targetDirection);
        crawlerFacedirection = targetDirection;



        CrawlerMove(targetDirection);

        //Pursue Player         
        //TranslateGroundToTarget(transform.position + (Vector3)targetDirection);

        //MoveCrawlerMoveCrawlerAlongWalls(true);

        //MoveCrawler();
        TranslateGroundToDirection(targetDirection);

      


        //CrawlerFaceDirection(targetDirection);
        CrawlerUpdateAnimator();
        lastDirection = targetDirection;
    }

    void CrawlerFaceDirection(Vector3 fDic)
    {
        // if moving right
        
        if (Vector3.Dot(fDic, transform.right) > 0.75f)
        {
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }
        else if (Vector3.Dot(fDic, -transform.right) > 0.75f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 0);
        }
        //
        
    }

    void CrawlerMove(Vector3 direction)
    {
        RaycastHit2D leftInfo, rightInfo;
        if (doubleRaycastDown(out leftInfo, out rightInfo))
        {
            //if moving right
            if ((direction - transform.right).magnitude < 0.01f)
            {
                Vector3 rightStart = transform.position + transform.localScale.y * 1.75f * transform.up;
                RaycastHit2D overrideRightHitInfo = Physics2D.Raycast(rightStart, -transform.right, distEdge);

                if (overrideRightHitInfo.collider != null)
                {
                    rightInfo = overrideRightHitInfo;
                }
            }
            //if moving left
            else
            {
                Vector3 leftStart = transform.position + transform.localScale.y * 1.75f * transform.up;
                RaycastHit2D overrideLeftHitInfo = Physics2D.Raycast(leftStart, -transform.right, distEdge);

                if (overrideLeftHitInfo.collider != null)
                {
                    leftInfo = overrideLeftHitInfo;
                }
            }

            // use it to update myNormal and isGrounded            
            mIsGrounded = (leftInfo.distance + rightInfo.distance) / 2 <= distGround + transform.localScale.x * 1.5f;//deltaGround;
            surfaceNormal = (leftInfo.normal + rightInfo.normal) / 2;

        }
        else
        {
            mIsGrounded = false;
            // assume usual ground normal to avoid "falling forever"
            surfaceNormal = transform.up;
        }


        Vector3 offset = positionOnTerrain(leftInfo, rightInfo, distGround);
        //direction.y = direction.y * 1.5f;
       
        //transform.rotation = Quaternion.LookRotation(transform.forward);
        //direction = transform.right + offset;
        //transform.LookAt(direction);      

        currentPosition = transform.position;
        //transform.position += direction * mMoveSpeed * Time.deltaTime;

        //mRigidBody2D.velocity = direction * mMoveSpeed; //* Time.deltaTime;


        

        if ((transform.position - currentPosition).magnitude < (direction * mMoveSpeed * Time.deltaTime).magnitude)   //(currentPosition - transform.position).magnitude < 0.3f)
        {

            //mRigidBody2D.AddForce(mRigidBody2D.mass*(3*Vector3.up + direction));
            //mRigidBody2D.velocity += (Vector2) direction * 2;
            //mRigidBody2D.AddForce(5* transform.up + 5 * transform.localScale.x * transform.right);
        }
    }

    void CrawlerUpdateAnimator()
    {
        mAnimator.SetBool("isWalking", crawlerIsWalking);
        mAnimator.SetBool("isShooting", crawlerIsShooting);
        mAnimator.SetBool("isHit", crawlerIsHit);
    }
    void FlyerUpdateAnimator()
    {
        AnimatorFlyer.SetBool("isMovingUp", flyerIsMovingUp);
        AnimatorFlyer.SetBool("isMovingDown", flyerIsMovingDown);
    }
    void WalkerUpdate()
    { }

    void WalkerInit()
    { }

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
        Vector2 leftStart = transform.position + transform.localScale.y  * -transform.up + distEdge * transform.right;
        Vector2 rightStart = transform.position + transform.localScale.y  * -transform.up - distEdge * transform.right;

        rightHitInfo = Physics2D.Raycast(leftStart, -transform.up, transform.localScale.y);
        leftHitInfo = Physics2D.Raycast(rightStart, -transform.up, transform.localScale.y);

        Debug.DrawLine(leftStart, leftStart + (Vector2)transform.up * -transform.localScale.y, Color.red, 2, false);
        Debug.DrawLine(rightStart, rightStart + (Vector2)transform.up * -transform.localScale.y, Color.red, 2, false);


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
            transform.position = averagePoint + transform.up * transform.localScale.y * 1.65f;
            return (averagePoint + transform.up * 0.30f) - transform.position;
        }
        return Vector3.zero;
    }



    void TranslateGroundToDirection(Vector3 direction)
    {
        currentPosition = transform.position;
        //Vector3 direction = target - transform.position;
        direction.z = 0;

        transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;
        
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

        StartPosition = transform.position;
        //StartPosition.y += offset;

        EndPosition = StartPosition + direction * transform.localScale.magnitude * mBoxCollider.size.y / 1.65f;


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

    // Each level, enemies do an extra 5% damage
    public float CalculateDamage()
    { 
        return Mathf.Pow(2, mCurrentLevel) * mScaleValue;
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

        if (isBoss)
        {
            GameObject.Find("Talos").SendMessage("ExitPos", transform.position);
            GameObject.Find("Exit").SendMessage("NewExit", transform.position);
        }

        // Notify Map Generator of index of enemy killed
        //GameObject.Find("MapGenerator").SendMessage("KilledEnemy", mapGenIndex);
        Destroy(gameObject);

    }

    void ShovedByEnemy(Vector2 shoveDir)
    {
        switch (type)
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
        if(isBoss)
        {
            numOrbs *= mCurrentLevel;
        }

        for (int i = 0; i < numOrbs + collectedOrbs.Count; i++)
        {
            if (i < numOrbs)
            {
                gameObject.GetComponentInParent<OrbController>().SpawnOrb(Random.Range(0, 2));
            }else
            {
                gameObject.GetComponentInParent<OrbController>().SpawnOrb(collectedOrbs[i - numOrbs]);
            }
        }

    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if (coll.gameObject.tag == "Player")
            {
                coll.gameObject.SendMessage("ShovedByEnemy", new Vector3(lastDirection.x, lastDirection.y, CalculateDamage()));
            }
            else if(false && coll.gameObject.tag == "Orb")
            {                
                collectedOrbs.Add(coll.gameObject.GetComponent<Orb>().type);
                coll.gameObject.GetComponent<Orb>().pickedUp = true;
            }        
            else if(coll.gameObject.tag == "Bullet" || coll.gameObject.tag == "enemyBullet")
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
                    DropOrbs(Mathf.CeilToInt(Mathf.Pow(mScaleValue, mCurrentLevel * mScaleValue)));
                    NotifyOfDeath();
                    nbTimesDied++;

                    if (nbTimesDied > 1)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        


       
        
    }
}