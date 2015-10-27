using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour
{   
    //Vector2 velocity;


    [SerializeField]
    float mMoveSpeed;
    [SerializeField]
    float mJumpForce;
    [SerializeField]
    LayerMask mWhatIsGround;
    float kGroundCheckRadius = 0.1f;

    // Animator booleans
    bool mRunning;
    bool mGrounded;
    bool mRising;

    // Invincibility timer
    float kInvincibilityDuration = 0.5f;
    float mInvincibleTimer;
    bool mInvincible;

    // Damage effects
    float kDamagePushForce = 2.5f;


    // Wall kicking
    bool mAllowWallKick;
    public Vector2 mFacingDirection;

    // References to other components and game objects
    Animator mAnimator;
    Rigidbody2D mRigidBody2D;
    Weapon mWeapon;

    // Reference to audio sources
    AudioSource mLandingSound;
    AudioSource mWallKickSound;
    AudioSource mTakeDamageSound;

    [SerializeField]
    GameObject mDeathParticleEmitter;


    int mTotalExp;
    float mMeleeTimer;
    bool mMeleeTrigger;
    int mHealth;
    int mEnemiesKilled;

    Vector2 mShoveDirection;

    Vector3 mExitLocation;
    int mCurrentLevel;
    int mEnemiesRemaining;

    Text exitDistance, enemiesLeft, curLevel, talosHealth, experience, actionPts, invicibleTime;    

    /*
    [SerializeField]
    LifeMeter life;

    List<GroundCheck> mGroundCheckList;
    */

    void Start()
    {
        // Get references to other components and game objects
        
        mRigidBody2D = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mWeapon = transform.FindChild("Weapon").GetComponent<Weapon>();
        mFacingDirection = Vector2.right;
        mTotalExp = 0;
        mMeleeTimer = 0;
        mShoveDirection = Vector2.zero;
        mHealth = 100;
        mInvincibleTimer = 0;


        // UI Text
        /*exitDistance = GameObject.Find("DistanceExit").GetComponent<Text>();
        enemiesLeft = GameObject.Find("EnemiesRemaining").GetComponent<Text>();
        curLevel = GameObject.Find("CurrentLevel").GetComponent<Text>();
        talosHealth = GameObject.Find("TalosHealth").GetComponent<Text>();
        experience = GameObject.Find("Experience").GetComponent<Text>();
        actionPts = GameObject.Find("ActionPoints").GetComponent<Text>();
        invicibleTime = GameObject.Find("Invicible").GetComponent<Text>();*/


        /*
        // Obtain ground check components and store in list
        mGroundCheckList = new List<GroundCheck>();
        GroundCheck[] groundChecksArray = transform.GetComponentsInChildren<GroundCheck>();
        foreach (GroundCheck g in groundChecksArray)
        {
            mGroundCheckList.Add(g);
        }

        // Get audio references
        AudioSource[] audioSources = GetComponents<AudioSource>();
        mLandingSound = audioSources[0];
        mWallKickSound = audioSources[1];
        mTakeDamageSound = audioSources[2];
        */
    }

    /*
    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 5;
    }
    */

    void FixedUpdate()
    {
        CheckDead();
        NotifyEnemiesOfPosition();
        UpdateCameraVelocity();
        UpdateUIText();
    }

    void UpdateUIText()
    {
        GameObject.Find("distance").GetComponent<Text>().text = "Exit Dist: " + ((int)(mExitLocation - transform.position).magnitude).ToString();
        GameObject.Find("enemiesLeft").GetComponent<Text>().text = "Enemies:  " + mEnemiesRemaining.ToString();
        GameObject.Find("curLevel").GetComponent<Text>().text = "Current Level: " + mCurrentLevel.ToString();
        GameObject.Find("health").GetComponent<Text>().text = "Health: " + mHealth.ToString();
        GameObject.Find("experience").GetComponent<Text>().text = "Exp: " + mTotalExp.ToString();
        GameObject.Find("invincibleTime").GetComponent<Text>().text = "Invincible Timer: " + mInvincibleTimer.ToString("F2");
    }
 

    void Update()
    {
        //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);       
        FaceMouse();
        CheckGround();
        CheckInvicible();       
                          
        TranslateInDirection(CheckMove());
        TriggerMelee();
        CheckJump();
        FaceMouse();

        UpdateAnimator();
        /*
        bool grounded = CheckGrounded();
        if (!mGrounded && grounded)
        {
            mLandingSound.Play();
        }
        mGrounded = grounded;
        */


        /*

        mRising = mRigidBody2D.velocity.y > 0.0f;
        UpdateAnimator();

        if (mInvincible)
        {
            mInvincibleTimer += Time.deltaTime;
            if (mInvincibleTimer >= kInvincibilityDuration)
            {
                mInvincible = false;
                mInvincibleTimer = 0.0f;
            }
        }
        */

    }

    void CheckDead()
    {
        if(mHealth <= 0)
        {
            GameObject.Find("MapGenerator").SendMessage("ResetGame");            
        }
    }

    void CheckJump()
    {
        if /*(mGrounded &&*/(Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
        }
        else if (mAllowWallKick && Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.velocity = Vector2.zero;
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
            // mWallKickSound.Play();
        }

    }

    void CheckInvicible()
    {
        if (mInvincible)
        {
            if (mInvincibleTimer > 0)
            {
                mInvincibleTimer -= Time.deltaTime;
            }
            else
            {
                mInvincibleTimer = 0;
                mInvincible = false;
            }
        }
    }

    Vector3 CheckMove()
    {       
        if (mWeapon.mMelee)
        {
            mMeleeTrigger = true;
            mMeleeTimer = 0;
            mRunning = false;            
            return Vector3.zero;
        }

        mRunning = false;
        if (Input.GetButton("Left"))
        {
            //transform.Translate(-Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);           
            //FaceDirection(Vector2.left);
            mRunning = true;
            return Vector3.left;
        }
        if (Input.GetButton("Right"))
        {
            //transform.Translate(Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);
            //FaceDirection(Vector2.right);            
            mRunning = true;
            return Vector3.right;
        }

        return Vector3.zero;
    }

    void TranslateInDirection(Vector3 direction)
    {
        if (direction != null && (Mathf.Abs(mRigidBody2D.velocity.x) < 7.0f))
        {
            //Vector3 direction = target - transform.position;
            direction.z = 0;            
            mRigidBody2D.AddForce(10*direction);
            //transform.position += direction.normalized * mMoveSpeed * Time.deltaTime;
        }
    }

    public Vector3 PlayerVelocity()
    {
        return mRigidBody2D.velocity;
    }
    
 

    void FaceMouse()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            mFacingDirection = Vector2.right;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back);
            mFacingDirection = Vector2.left;
        }
    }

    public void FaceDirection(Vector2 faceD)
    {
        
        if (faceD == Vector2.left)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        if(faceD == Vector2.right)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }
        mFacingDirection = faceD;
    }

   
    void CheckGround()
    {
        //temp
        if (Mathf.Abs(mRigidBody2D.velocity.y) > 0)
        {
            mGrounded = false;
        }
        else
        {
            mGrounded = true;
        }
    }

    void TriggerMelee()
    {
        if (mMeleeTrigger)
        {
            if(mMeleeTimer > 0.2f)
            {
                mMeleeTimer = 0;
                mMeleeTrigger = false;
                GameObject enemy = EnemyInDirection(mFacingDirection);
                if (enemy != null)
                {
                    enemy.SendMessage("KilledBySword");
                }
            }
            else
            {
                mMeleeTimer += Time.deltaTime;
            }         
        }
    }

    void UpdateAnimator()
    {
        mAnimator.SetBool("isGrounded", mGrounded);
        mAnimator.SetBool("isRunning", mRunning);
        mAnimator.SetBool("isMelee", mWeapon.mMelee);
        mAnimator.SetBool("isShoot", mWeapon.mShoot);
    }

    
    GameObject EnemyInDirection(Vector2 direction)
    {        
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

            if (Mathf.Abs(direction.x) > 0)
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
                        increment = boxSize * scaleSize / 2;
                        break;
                    }
                case 2:
                    {
                        increment = (-1) * boxSize * scaleSize / 2;
                        break;
                    }
                default:
                    break;
            }

            StartPosition = transform.position;
            if (Mathf.Abs(direction.x) > 0)
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

            if(hit.collider != null && hit.collider.gameObject.tag == "Enemy")
            {
                return hit.collider.gameObject;
            }          

        }

        return null;
    }

  



    // Notification management
    ///////////////////////////////////////////
    void NotifyEnemiesOfPosition()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SendMessage("UpdatePlayerPosition", transform.position);
        }
    }

    void KilledEnemy(int exp)
    {
        mTotalExp += exp;
        mEnemiesRemaining--;
    }

    void ShovedByEnemy(Vector3 shoveInfo)
    {
        if(!mInvincible)
        {
            mInvincible = true;
            mInvincibleTimer = kInvincibilityDuration;
            mHealth -= (int)shoveInfo.z;

            // Get Shoved
            mRigidBody2D.AddForce(new Vector3(5 * shoveInfo.x, 3, 0), ForceMode2D.Impulse);      
        }
        else
        {
            mShoveDirection = Vector2.zero;
        }
    }
    
    void StartPos(Vector3 pos)
    {
        transform.position = pos;
        GameObject.Find("Main Camera").SendMessage("StartPos", pos);

        mInvincible = true;
        mInvincibleTimer = 1.5f;
    }

    void ExitPos(Vector3 pos)
    {
        mExitLocation = pos;
    }

    void CurrentLevel(int level)
    {
        mCurrentLevel = level;
    }

    void TotalEnemies(int numEnemies)
    {
        mEnemiesRemaining = numEnemies;
    }

    void UpdateCameraVelocity()
    {
        GameObject.Find("Main Camera").SendMessage("PlayerVelocity", mRigidBody2D.velocity);
    }

    void HitByBullet(int damage)
    {
        if(!mInvincible)
        {
            mHealth -= damage;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if (coll.gameObject.tag == "Exit")
            {
                GameObject.Find("MapGenerator").SendMessage("NextLevel");
            }
        }
    }
}