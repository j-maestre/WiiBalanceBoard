using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningSpinner : MonoBehaviour
{
    private float speed = 200.0f;
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 0, Time.deltaTime * speed));
    }
}
