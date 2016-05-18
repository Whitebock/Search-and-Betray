using UnityEngine;
using System.Collections;
/*Markus Röse
  Script to visual represent bullets in the scene
  */


public class Tracer : MonoBehaviour {
   
    Ray myRay; //Ray which was send
    RaycastHit myRaycastHit; //Hit point of bullet

    LineRenderer myLineRenderer; //LineRenderer for visual representation in scene

    int distance = 0;

	
	void Update () {
		//Leave method if fields are not properly set
		if(myLineRenderer == null)
			return;
		
        distance+=2;
		
		//Set points of linerenderer for visual representation
        myLineRenderer.SetPosition(0, myRay.GetPoint(distance - 1));
        myLineRenderer.SetPosition(1, myRay.GetPoint(distance));
		
		//When desired point is reached the gameObject destroys itself
        if (CheckFinish())
        {
            Destroy(gameObject);
        }
	}

    /// <summary>
    /// Method set necessary popertys to set flight path of tracer.
    /// </summary>
    /// <param name="myRay">Ray which was send when weapon was shot.</param>
    /// <param name="hit">The hit object of the raycast.</param>
    public void StartTracer(Ray myRay, RaycastHit hit)
    {
		if (hit.distance > 300)
		{
			hit = new RaycastHit();
			hit.point = myRay.GetPoint(300);
			hit.distance = 300;
		}

        myLineRenderer = gameObject.GetComponent<LineRenderer>();
        this.myRay = myRay;
        this.myRaycastHit = hit;
    }

    /// <summary>
    /// Method to check if desired point is reached
    /// </summary>
    /// <returns>A bool which indicates if desired point is reached.</returns>
    bool CheckFinish()
    {
        if (distance >= myRaycastHit.distance)
        {
            return true;
        }

        return false;
    }
}
