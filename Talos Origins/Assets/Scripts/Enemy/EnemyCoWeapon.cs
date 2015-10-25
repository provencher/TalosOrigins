﻿using UnityEngine;
using System.Collections;

public class EnemyCoWeapon : MonoBehaviour {

    [SerializeField]
    float ShootInterval;
    float lastShootTime;
    float mRandomTime;
    [SerializeField]
    GameObject mEnemyCoBulletPrefab;

    // Use this for initialization
    void Start () {
        mRandomTime = UnityEngine.Random.Range(3.0f, 5.0f);
    }
	
	// Update is called once per frame
	void Update () {
        float shootTime = Time.time;
        
        while (shootTime > mRandomTime)
        {
            shootTime -= mRandomTime;
        }
        if (shootTime < 1)
        {
            Shoot();
        }
	}
    void Shoot()
    {
        if (Time.time - lastShootTime > ShootInterval)
        {
            Vector2 enemyFaceDirection=transform.parent.GetComponent<Enemy>().crawlerFacedirection;
            Vector3 bulletPosition = transform.parent.position+(Vector3)enemyFaceDirection*0.5f;
            Vector3 mBulletDirection = enemyFaceDirection;

            GameObject mBullet = (GameObject)Instantiate(mEnemyCoBulletPrefab, bulletPosition, Quaternion.identity);
            mBullet.GetComponent<EnemyCoBullet>().setDirection((Vector3)enemyFaceDirection);
            //mBullet.GetComponent<Bullet>().SetDirection(mBulletDirection.normalized, mTalos.PlayerVelocity());

            lastShootTime = Time.time;
        }
    }
 }