﻿using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {

    float timeToLive;
    float lifeTime;
    Transform player;

    public int type;
    public bool pickedUp = false;

	// Use this for initialization
	void Start () {
        lifeTime = Random.Range(1.5f,3.0f);
        timeToLive = lifeTime;
        player = GameObject.Find("Talos").transform;
	}

    void SeekPlayer()
    {
        if(pickedUp)
        {
            GetComponent<AudioSource>().Play(); 
            Destroy(gameObject);
        }

        Vector3 direction = player.position - transform.position;

        if (direction.magnitude < 2.5f)
        {
            transform.position += direction.normalized * 2 * Time.deltaTime;
        }
    }
	
	// Update is called once per frame
	void Update () {
        SeekPlayer();

        if (timeToLive > 0)
        {
            timeToLive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
	}
}
