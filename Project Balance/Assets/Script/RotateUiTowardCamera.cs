using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUiTowardCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
