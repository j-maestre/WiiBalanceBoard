using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    public float damage_;
    // Start is called before the first frame update
    void Start(){
        //damage_ = 0.0f;
    }

    public void SetDamage(float v){
        damage_ = v;
    }
    public float GetDamage(){
        return damage_;
    }

    // Update is called once per frame
    void Update(){
        
    }
}
