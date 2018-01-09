using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour {

    public GameObject bSphere;          //  Center of the beam - beam sphere
    public GameObject vRay, hRay;       //  Vertival and horizontal beam rays

    public float beamY;

    public void TurnOff ()
    {
//      Reseting scale and position of beam rays

        vRay.transform.localScale = hRay.transform.localScale = new Vector3(vRay.transform.localScale.x, 0.0f, vRay.transform.localScale.z);
        vRay.transform.localPosition = hRay.transform.localPosition = Vector3.zero;

        bSphere.SetActive(false);
    }

    public void TurnOn (Vector2 pivot, Quaternion directions)
    {
        //      pivot       -  Precalculated coordinates to place the beam
        //      directions  -  1 or 0 value of x, y, z, w corresponds to top, bottom, right, left directions of beam expanding

        bSphere.transform.position = new Vector3(pivot.x, beamY, pivot.y);
        bSphere.SetActive(true);

//      Scaling beams        

        vRay.transform.localScale = new Vector3(vRay.transform.localScale.x, vRay.transform.localScale.y + 0.5f * directions.x + 0.5f * directions.y, vRay.transform.localScale.z);
        hRay.transform.localScale = new Vector3(hRay.transform.localScale.x, hRay.transform.localScale.y + 0.5f * directions.z + 0.5f * directions.w, hRay.transform.localScale.z);

//      Moving beams if it's necessary

        vRay.transform.localPosition = new Vector3(vRay.transform.localPosition.x, 0.0f, vRay.transform.localPosition.z - 0.5f * (1.0f - directions.x) + 0.5f * (1.0f - directions.y));
        hRay.transform.localPosition = new Vector3(hRay.transform.localPosition.x - 0.5f * (1.0f - directions.z) + 0.5f * (1.0f - directions.w), 0.0f, hRay.transform.localPosition.z);
    }
}
