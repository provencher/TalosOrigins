using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralMusic : MonoBehaviour {

    [SerializeField]
    public GameObject A;

    [SerializeField]
    public GameObject A_1;

    [SerializeField]
    public GameObject A1;

    [SerializeField]
    public GameObject B;

    [SerializeField]
    public GameObject B_1;

    [SerializeField]
    public GameObject B1;

    [SerializeField]
    public GameObject C;

    [SerializeField]
    public GameObject C1;

    [SerializeField]
    public GameObject C2;

    [SerializeField]
    public GameObject D;

    [SerializeField]
    public GameObject D1;

    [SerializeField]
    public GameObject E;

    [SerializeField]
    public GameObject E1;

    [SerializeField]
    public GameObject F;

    [SerializeField]
    public GameObject F1;

    [SerializeField]
    public GameObject G;

    [SerializeField]
    public GameObject G1;

    List<GameObject> musicNotes;

    float timeCounter = 0;
    int currentNote = 0;
    
    void Awake()
    {
        musicNotes = new List<GameObject>();
        musicNotes.Add(A);
        musicNotes.Add(B);
        musicNotes.Add(B_1);
        musicNotes.Add(B1);
        musicNotes.Add(C);
        musicNotes.Add(C1);
        musicNotes.Add(C2);
        musicNotes.Add(D);
        musicNotes.Add(D1);
        musicNotes.Add(E);
        musicNotes.Add(E1);
        musicNotes.Add(F);
        musicNotes.Add(F1);
        musicNotes.Add(G);
        musicNotes.Add(G1);
    }



    // Update is called once per frame
    void Update () {
        timeCounter += Time.deltaTime;

	    if(Mathf.CeilToInt(timeCounter) % 2 == 0)
        {            
            int randomNumberNotes = Random.Range(1, 3);
            int nextNote = Random.Range(0, musicNotes.Count/2);

            int noteToPlay = (currentNote + nextNote) % musicNotes.Count;



            switch (randomNumberNotes)
            {
                case 1:
                    Instantiate(musicNotes[noteToPlay], transform.position, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(musicNotes[noteToPlay], transform.position, Quaternion.identity);
                    Instantiate(musicNotes[(noteToPlay + 1)% (musicNotes.Count)], transform.position, Quaternion.identity);
                    break;               
                default:
                    Instantiate(musicNotes[noteToPlay], transform.position, Quaternion.identity);
                    break;
            }
        }
	}
}
