using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbController : MonoBehaviour {
       
    [SerializeField]
    public GameObject blueOrbPrefab;

    [SerializeField]
    public GameObject greenOrbPrefab;

    [SerializeField]
    public GameObject redOrbPrefab;

    [SerializeField]
    public GameObject yellowOrbPrefab;

    

    public void SpawnOrb(int colorNum, int upgrade)
    {
        Vector2 offSet = new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(0, 1.2f));
        GameObject orbInstance = null;
        switch (colorNum)
        {
            case 0:
                orbInstance = Instantiate(blueOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity) as GameObject;                
                break;
            case 1:
                orbInstance = Instantiate(greenOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity) as GameObject;
                break;
            case 2:
                orbInstance = Instantiate(redOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity) as GameObject;
                break;
            case 3:
                orbInstance = Instantiate(yellowOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity) as GameObject;
                break;

        }        
        orbInstance.GetComponent<Orb>().type = colorNum;      

        if(upgrade > -1)
        {
            orbInstance.GetComponent<Orb>().timeToLive = 120;
            orbInstance.GetComponent<Orb>().upgrade = upgrade;
            orbInstance.GetComponent<Orb>().transform.localScale *= 4;
            orbInstance.GetComponent<Light>().range *= 4;
            orbInstance.GetComponent<Light>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        }
        else
        {
            orbInstance.GetComponent<Orb>().timeToLive = 10;
        }
        

    }   
          
    
}
