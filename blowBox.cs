using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blowBox : MonoBehaviour
{
    static public float maxYpos = 3.0f;

    public Material activeMaterial;
    public Vector2Int coordinates;

    private Vector3 actBoxScale;

    public void Start()
    {
        actBoxScale = new Vector3(transform.localScale.x * 0.69f, transform.localScale.y * 1.2f, transform.localScale.z * 0.69f);
    }

    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }

    public void Activate()
    {
        if (activeMaterial != null)
            GetComponent<Renderer>().material = activeMaterial;

        transform.position = new Vector3(transform.position.x, maxYpos, transform.position.z);
        transform.localScale = actBoxScale;

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Bang( Material newMat )
    {
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        GetComponent<Renderer>().material = newMat;
    }
}
