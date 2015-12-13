using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ParralaxItem : MonoBehaviour
{
    public Vector3 minDirection;
    public Vector3 maxDirection;
    Vector3 offset;

    public Vector3 rotationAxis;
    public float rotationSpeed;
    Vector3 direction;
    public GameObject Talos;

    float timeCounter = 0;

    float[] primes = { 53f, 97f};
    public bool alive = true;
    public bool destroyed = false;

    float distance;
    float currentLevel;
    Vector3 StartPos;

    public void ResetPos()
    {
        Vector3 newPos = StartPos + new Vector3(Random.Range(0.1f, primes[Random.Range(0, primes.Length - 1)]) / 15f, Random.Range(0.1f, primes[Random.Range(0, primes.Length - 1)]) / 15f, 0);
        newPos.z = 0;
        transform.position = newPos;        
    }

    void Start()
    {
        //Talos = GameObject.Find("Talos");  
        direction = GenerateDirection();
        direction.z = 0;   
        //ResetPos();        
    }

    Vector3 GenerateDirection()
    {
        Vector3 dir = (Talos.transform.position - transform.position).normalized * Random.Range(0.1f, primes[Random.Range(0, primes.Length - 1)] / 100f) * 0.2f * minDirection.magnitude;
        dir.z = 0;
        return dir;
    }

    public IEnumerator destroy()
    {
       while (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(0.0005f*Time.deltaTime, 0.0005f*Time.deltaTime, 0.001f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        destroyed = false;
        yield break;
    }



    void Update()
    {
        if(!alive)
        {
            StartCoroutine(destroy());
        }

        transform.position += direction * Time.deltaTime;
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        timeCounter += Time.deltaTime;

        if (Vector3.Distance(Talos.transform.position, transform.position) > 35)
        {      
            direction = GenerateDirection();
            direction.z = 0;
            timeCounter = 0;
        }
    }
}
