using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BB_Quadrant {

    public float Front_Left;
    public float Front_Right;
    public float Back_Left;
    public float Back_Right;

    public Vector2 Center;
    public Vector2 CenterNoRound;
    public Vector2 CenterRaw;

    public float Weight;

    public BB_Quadrant() {
        Front_Left = 0.0f;
        Front_Right = 0.0f;
        Back_Left = 0.0f;
        Back_Right = 0.0f;
        Center = new Vector2();
        CenterNoRound = new Vector2();
        CenterRaw = new Vector2();
    }

    public BB_Quadrant(Vector4 weights, Vector2 center, float weight) {
        Front_Right = weights.x;
        Front_Left = weights.y;
        Back_Right = weights.z;
        Back_Left = weights.w;
        Center = center;
        CenterRaw = center;
        Weight = weight;
    }
}
