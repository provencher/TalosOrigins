using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trail : MonoBehaviour {

    // Breadcrumb trailling
    [SerializeField]
    bool trailActivated;
    [SerializeField]
    float mCrumbSeperation;
    [SerializeField]
    int mMaxCrumbs;
    [SerializeField]
    GameObject mBreadcrumbPrefab;
    bool breadcrumbsActivated;
    int crumbCount;
    Vector3 InitialCrumbPosition;
    List<GameObject> trail;

    // Use this for initialization
    void Start () {
        crumbCount = 0;
        trail = new List<GameObject>();
        trailActivated = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (trailActivated)
        {
            BreadcrumbsHandler();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTrail();
        }        
    }

    public void ResetTrail()
    {
        trailActivated = false; 
               
        foreach (GameObject crumb in trail)
        {
            //housekeeping
            Destroy(crumb);
        }        
    }

    void BreadcrumbsHandler()
    {
        if (crumbCount == 0)
        {
            InitialCrumbPosition = transform.position;
            GameObject mBreadcrumb = (GameObject)Instantiate(mBreadcrumbPrefab, transform.position, Quaternion.identity);
            trail.Add(mBreadcrumb);
            crumbCount++;
        }

        if (Vector3.Distance(InitialCrumbPosition, transform.position) > mCrumbSeperation * crumbCount)
        {
            if (crumbCount < mMaxCrumbs)
            {
                GameObject mBreadcrumb = (GameObject)Instantiate(mBreadcrumbPrefab, transform.position, Quaternion.identity);
                trail.Add(mBreadcrumb);
                Debug.Log("breadcrumb dropped");
            }
            else
            {
                trail[crumbCount % mMaxCrumbs].transform.position = transform.position;
            }

            crumbCount++;

        }
    }

}
