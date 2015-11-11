﻿using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{

    public int maxHealth = 100;
    public int curHealth = 100;
    public float healthBarLength;

    int healthBarScreenRatio = 10;

    Vector3 offset;
    

    // Use this for initialization
    void Start()
    {
        Vector3 boxSize = gameObject.GetComponent<BoxCollider2D>().size;
        Vector3 scale = transform.localScale;

        offset = new Vector3(boxSize.x / 2 * scale.x, boxSize.y /2 * scale.y, 0);


        healthBarLength = Screen.width / healthBarScreenRatio;       
    }    

    // Update is called once per frame
    void Update()
    {
        AddjustCurrentHealth(0);
    }
    
    void OnGUI()
    {       
        
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position + offset); 
        GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), curHealth + "/" + maxHealth);
    }

    public void AddjustCurrentHealth(int adj)
    {
        curHealth += adj;

        if (curHealth < 0)
            curHealth = 0;        

        if (curHealth > maxHealth)
            curHealth = maxHealth;        

        if (maxHealth < 1)
            maxHealth = 1;

        healthBarLength = (Screen.width / healthBarScreenRatio) * (curHealth / (float)maxHealth);
    }

}