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
    public GameObject Talos;

    int[] primes = { 53, 97, 193, 389, 769, 1543 };

    void Start()
    {
        //Talos = GameObject.Find("Talos");
        direction = GenerateDirection();
        direction.z = 0;
    }

    Vector3 GenerateDirection()
    {
        Vector3 dir = (Talos.transform.position - transform.position).normalized * 0.1f * minDirection.magnitude / 500;
        dir.x *= Random.Range(-primes[Random.Range(0, primes.Length - 1)]/2, primes[Random.Range(0, primes.Length - 1)]/2);
        dir.y *= Random.Range(-primes[Random.Range(0, primes.Length - 1)]/2, primes[Random.Range(0, primes.Length - 1)]/2);
        return dir;
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);


        if (Time.time % 31 == 0)
        {
            if (Time.time % primes[Random.Range(0, primes.Length - 1) ] == 0)
            {
                direction = GenerateDirection();
                direction.z = 0;
            }
        }
    }
}
