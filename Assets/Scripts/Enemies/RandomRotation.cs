using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Min(0)] public float MaxRotationSpeed;
    [Min(0)] public float MinRotationSpeed;

    private float rotationSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed =  Random.Range(MinRotationSpeed, MaxRotationSpeed);
        if (Random.Range(0, 2) == 0)
        {
            rotationSpeed = -rotationSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.forward, rotationSpeed);
    }
}
