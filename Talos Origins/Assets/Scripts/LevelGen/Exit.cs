using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

    public Vector3 startScale;
    int[] primes = { 53, 97, 193, 389 };

    void NewExit(Vector3 position)
    {
        
        transform.position = position;
        transform.Find("Stars_Flyby_Fast").position = transform.position;
        startScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = startScale  + startScale  / 3 * Mathf.Sin(2 * Mathf.PI * Time.time) / primes[Random.Range(0, primes.Length - 1)] / 50;
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll != null)
        {
            if (coll.gameObject.tag == "Player")
            {
                GameObject.Find("MapGenerator").SendMessage("NextLevel");
            }
        }
    }     
}
