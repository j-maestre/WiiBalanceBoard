using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimed : MonoBehaviour
{
    [Min(0)] public float TimeToDestroy;

    private float Timer;
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= TimeToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
