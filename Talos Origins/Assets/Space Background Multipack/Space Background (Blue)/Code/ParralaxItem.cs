using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ParralaxItem : MonoBehaviour
{
    public Vector3 minDirection;
    public Vector3 maxDirection;

    public Vector3 rotationAxis;
    public float rotationSpeed;
    Vector3 direction;
    GameObject Talos;

    int[] primes = { 53, 97, 193, 389, 769, 1543 };

    void Start()
    {
        Talos = GameObject.Find("Talos");
        direction = new Vector3(Random.Range(minDirection.x, maxDirection.x), Random.Range(minDirection.y, maxDirection.y), 0);
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        if(Time.time % 31 == 0)
        {
            if(Time.time % primes[Random.Range(0, primes.Length)- 1] == 0)
            {
                direction = (Talos.transform.position - transform.position).normalized * 0.1f * primes[Random.Range(0, primes.Length) - 1]/100;
                direction.z = 0;
            }            
        }

    }
}
