using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCameraMove : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0) * 2 * Time.deltaTime);
    }
}