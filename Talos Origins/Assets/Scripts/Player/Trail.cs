using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trail : MonoBehaviour {
	
	// Breadcrumb trailling
	[SerializeField]
	public bool trailActivated;
	[SerializeField]
	float mCrumbSeperation;
	[SerializeField]
	int mMaxCrumbs;

	int maxCrumbsWithLevel;

	[SerializeField]
	GameObject mBreadcrumbPrefab;
	bool breadcrumbsActivated;
	int crumbCount;
	Vector3 lastCrumbPosition;
	List<GameObject> trail;
	int trailLevel;

	bool crumbNearBy;

	void Start () {
		crumbCount = 0;
		trail = new List<GameObject>();
		trailLevel = PlayerPrefs.GetInt ("Breadcrumbs");
		trailActivated = trailLevel == 0 ? false: true;
		crumbNearBy = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (trailActivated && trailLevel > 1)
		{
			BreadcrumbsHandler();
			maxCrumbsWithLevel =  mMaxCrumbs + (trailLevel * 5);
		}
		
		if (Input.GetKeyDown(KeyCode.R))
		{
			ResetTrail();
		}        
	}
	
	public void ResetTrail()
	{
		trailActivated = !trailActivated; 

		if(!trailActivated){
			foreach (GameObject crumb in trail)
			{
				//housekeeping
				Destroy(crumb);
			}
		}        
	}
	
	void BreadcrumbsHandler()
	{
		if (crumbCount == 0) {
			lastCrumbPosition = transform.position;
			GameObject mBreadcrumb = (GameObject)Instantiate (mBreadcrumbPrefab, transform.position, Quaternion.identity);
			trail.Add (mBreadcrumb);
			crumbCount++;
		} else {

			foreach (GameObject crumb in trail) 
			{
				if (Vector3.Distance (crumb.transform.position, transform.position) < mCrumbSeperation)
				{
					crumbNearBy = true;
					break;
				}
				else
				{
					crumbNearBy = false;
				}
			}
		
			if (!crumbNearBy) 
			{
				lastCrumbPosition = transform.position + Vector3.up;
			
				if (crumbCount < maxCrumbsWithLevel) 
				{
					GameObject mBreadcrumb = (GameObject)Instantiate (mBreadcrumbPrefab, lastCrumbPosition, Quaternion.identity);
					trail.Add (mBreadcrumb);
					crumbCount++;
				} 
				else
				{
					GameObject oldestCrumb = trail [0];
				
					trail.RemoveAt (0);
					oldestCrumb.transform.position = lastCrumbPosition;
					trail.Add (oldestCrumb);

				}
			}
		}

	}

	public void SetTrailLevel(int level)
	{
		this.trailLevel = level;
	}
	
}
