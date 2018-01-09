using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class envBox : MonoBehaviour
{
    public static float envSpeed = 1.0f;
    public static bool envStatus = true;

    public float dY = 1.0f;                                         // Maximum position.Y value of box
    public float speed = 0.1f;                                      // Local movement speed
    public float offset = 0.0f;                                     // 

    private float dest;                                             // Current movement dirrection

    void Start ()
    {
        dest = Random.value > 0.5f ? dY + offset : -1.0f * dY + offset;
        transform.position = new Vector3(transform.position.x, Random.Range(-1.0f * dY + offset, dY + offset), transform.position.z);
	}
	
	void Update ()
    {
        if (envStatus)
        {
            if (transform.position.y > dY + offset) dest = -1.0f * dY + offset;
            else if (transform.position.y < -1.0f * dY + offset) dest = dY + offset;

            int sign = dY + offset > dest ? -1 : 1;
            transform.Translate(0, sign * Time.deltaTime * speed * 0.5f * Demo.generalSpeed * envSpeed, 0);
        }
        else if ( transform.position.y > -1.0 * dY + offset)
        {
            transform.Translate(0, -1.0f * Time.deltaTime * speed * 2.0f * Demo.generalSpeed * envSpeed, 0);
        }
    }
}
