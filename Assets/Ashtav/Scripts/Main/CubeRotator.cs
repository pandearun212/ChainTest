using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    void Update()
    {
        /*Rotates cube in main scene.*/
        transform.Rotate(Vector3.forward * 100 * Time.deltaTime, Space.Self);
    }
}
