using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbController : MonoBehaviour {
    public GameObject attachedParent; 
    
    [SerializeField]
    public GameObject blueOrbPrefab;

    [SerializeField]
    public GameObject greenOrbPrefab;

    [SerializeField]
    public GameObject redOrbPrefab;

    [SerializeField]
    public GameObject yellowOrbPrefab;


    public void SpawnOrb(int colorNum)
    {
        Vector2 offSet = new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(0, 1.2f));
        switch (colorNum)
        {
            case 0:
                Instantiate(blueOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;
            case 1:
                Instantiate(greenOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;
            case 2:
                Instantiate(redOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;
            case 3:
                Instantiate(yellowOrbPrefab, (Vector2)gameObject.GetComponentInParent<Transform>().position + offSet, Quaternion.identity);
                break;

        }
    }   
          
    
}
