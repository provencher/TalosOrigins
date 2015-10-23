﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    float kInvincibilityDuration = 1.0f;
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
        NotifyEnemiesOfPosition();
    }

 

    void Update()
    {
        //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
        CheckGround();

        CheckMove();
        TriggerMelee();

        /*
        bool grounded = CheckGrounded();
        if (!mGrounded && grounded)
        {
            mLandingSound.Play();
        }
        mGrounded = grounded;
        */


        if /*(mGrounded &&*/( Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
        }
        else if (mAllowWallKick && Input.GetButtonDown("Jump"))
        {
            mRigidBody2D.velocity = Vector2.zero;
            mRigidBody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
           // mWallKickSound.Play();
        }



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
        UpdateAnimator();
    }


    void CheckMove()
    {
        if (mWeapon.mMelee)
        {
            mMeleeTrigger = true;
            mMeleeTimer = 0;
            mRunning = false;            
            return;
        }

        mRunning = false;
        if (Input.GetButton("Left"))
        {
            transform.Translate(-Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);
            FaceDirection(Vector2.left);
            mRunning = true;
        }
        if (Input.GetButton("Right"))
        {
            transform.Translate(Vector3.right * mMoveSpeed * Time.deltaTime, Space.World);
            FaceDirection(Vector2.right);
            mRunning = true;
        }
    }

    void FaceDirection(Vector2 faceD)
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

    void StartPos(Vector3 pos)
    {
        transform.position = pos;
        GameObject.Find("Main Camera").SendMessage("StartPos", pos);
    }
    void CheckGround()
    {
        //temp
        if (mRigidBody2D.velocity.y == 0)
        {
            mGrounded = true;
        }
        else
        {
            mGrounded = false;
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
    }

    /*
    void OnCollisionEnter2D(Collision2D coll)
    {    

        if (coll != null && coll.gameObject.tag == "Enemy" && mWeapon.mMelee)
        {
            Vector3 enemyPos = coll.gameObject.transform.position;

            if (((mFacingDirection == Vector2.right) && (enemyPos.x > transform.position.x))
                || ((mFacingDirection == Vector2.left) && (enemyPos.x < transform.position.x))
                || ((mFacingDirection == Vector2.up) && (enemyPos.y > transform.position.y))
                || ((mFacingDirection == Vector2.down) && (enemyPos.y < transform.position.y)))
            {
                coll.gameObject.SendMessage("KilledBySword");
            }
        }
    }
    */
}