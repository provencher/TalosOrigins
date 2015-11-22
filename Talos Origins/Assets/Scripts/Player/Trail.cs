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
	[SerializeField]
	GameObject mBreadcrumbPrefab;
	bool breadcrumbsActivated;
	int crumbCount;
	Vector3 lastCrumbPosition;
	List<GameObject> trail;
	

	void Awake(){
		trailActivated = false;
	}

	void Start () {
		crumbCount = 0;
		trail = new List<GameObject>();
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
			lastCrumbPosition = transform.position;
			GameObject mBreadcrumb = (GameObject)Instantiate(mBreadcrumbPrefab, transform.position, Quaternion.identity);
			trail.Add(mBreadcrumb);
			crumbCount++;
		}
		
		if (Vector3.Distance(lastCrumbPosition, transform.position) > mCrumbSeperation )
		{
			lastCrumbPosition = transform.position + Vector3.up;
			
			if (crumbCount < mMaxCrumbs)
			{
				GameObject mBreadcrumb = (GameObject)Instantiate(mBreadcrumbPrefab, lastCrumbPosition, Quaternion.identity);
				trail.Add(mBreadcrumb);
				Debug.Log("breadcrumb dropped");
				crumbCount++;
			}
			else
			{
				GameObject oldestCrumb = trail[0];
				
				trail.RemoveAt(0);
				oldestCrumb.transform.position = lastCrumbPosition;
				trail.Add(oldestCrumb);
				
				
				//trail.Add((GameObject)Instantiate(mBreadcrumbPrefab, transform.position, Quaternion.identity));
				Debug.Log("breadcrumb moved");
				
				//trail[crumbCount % mMaxCrumbs].transform.position = transform.position;
			}
			
			
			
			
			
		}
	}
	
}
