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

    public float mMoveSpeed;
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
    public bool mInRange = false;

    bool hookedGrapple = false;

    //For animator
    Animator mAnimator;
    //crawer
    public Vector2 crawlerFacedirection;
    bool crawlerIsWalking;
    bool crawlerIsShooting;
    float shootingTime=1.0f;
    float shootEndTime;
    bool  crawlerIsHit;
    float hitTime = 0.5f;
    float hitEndTime;

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
        mMoveSpeed = 2.0f;
        distanceThreshold = 12f;
        mHealth = 100;

        lastDirection = Vector2.right;
        nbTimesDied = 0;

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
    }

    void CrawlerUpdate()
    {
        if (firstLoop)
        {
            CrawlerInit();
            firstLoop = false;
        }

        CrawlerCheckMove();        
       
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
        targetDirection.y = 0;

        //Pursue Player         
        TranslateGroundToTarget(transform.position + (Vector3)targetDirection);

        CrawlerFaceDirection(targetDirection);
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

            EndPosition = StartPosition + direction * 0.6f;

            //Check if clear
            hit = Physics2D.Linecast(StartPosition, EndPosition);
            Debug.DrawLine(StartPosition, EndPosition, Color.red, 2, false);

            isClear = isClear && !(hit.collider != null &&
               (hit.collider.gameObject.tag == "Cave" ||
               hit.collider.gameObject.tag == "Enemy" ||
               hit.collider.gameObject.tag == "Asteroid"
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

    public void IsHooked()
    {
        hookedGrapple = true;
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
