using UnityEngine;
using System.Collections;

public class EnemyCoWeapon : MonoBehaviour {

    [SerializeField]
    float ShootInterval;
    float shootTime;
    float mRandomTime;
    [SerializeField]
    GameObject mEnemyCoBulletPrefab;

    [SerializeField]
    GameObject shotAudio;

    Enemy mParent;


    // Use this for initialization
    void Start () {
        mRandomTime = Random.Range(1, ShootInterval + 5);
        shootTime = mRandomTime;
        mParent = transform.parent.GetComponent<Enemy>();
    }
	
	// Update is called once per frame
	void Update () {

        if (mParent.mInRange)
        {       
            if (shootTime < 0)
            {
                Shoot();
                mRandomTime = Random.Range(3, ShootInterval + 5);
                shootTime = mRandomTime;
            }
            else
            {
                shootTime -= Time.deltaTime;
            }
        }       
	}
    void Shoot()
    {
        if (mParent.GetComponent<Enemy>().mInRange)
        {
            Vector2 enemyFaceDirection=transform.parent.GetComponent<Enemy>().crawlerFacedirection;
            Vector3 bulletPosition = transform.parent.position+(Vector3)enemyFaceDirection* mParent.GetComponent<BoxCollider2D>().size.x/6+ Vector3.up * mParent.GetComponent<BoxCollider2D>().size.y/6;
            Vector3 mBulletDirection = enemyFaceDirection;

            GameObject mBullet = (GameObject)Instantiate(mEnemyCoBulletPrefab, bulletPosition, Quaternion.identity);
            Instantiate(shotAudio, transform.position, Quaternion.identity);

            mBullet.GetComponent<EnemyCoBullet>().setDirection(enemyFaceDirection);
            mBullet.SendMessage("SetDamage", mParent.CalculateDamage());           
        }
    }
 }
