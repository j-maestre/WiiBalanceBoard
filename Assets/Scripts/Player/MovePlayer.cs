using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MovePlayer : MonoBehaviour
{
    public float speed_ = 5.0f;
    public float aceleration_ = 5.0f;
    public CinemachineVirtualCamera cam_;
    public float newFOV_ = 70.0f;
    private bool startToFov_ = false;
    public float increment_ = 0.1f;

    private bool startStop_ = false;


    // Start is called before the first frame update
    void Start(){
        
    }

    public void Accelerate(){
        speed_ += aceleration_;
        startToFov_ = true;

    }

    public void Stop(){
        startStop_ = true;
    }

    public void HardStop(){
        speed_ = 0.0f;
    }

    // Update is called once per frame
    void Update(){
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (speed_ * Time.deltaTime)); 


        if(startToFov_){
            if(cam_.m_Lens.FieldOfView < newFOV_){
                cam_.m_Lens.FieldOfView += increment_ * Time.deltaTime;
            }

            if(cam_.m_Lens.FieldOfView >= newFOV_){
                cam_.m_Lens.FieldOfView = newFOV_;
                startToFov_ = false;
            }
        }


        if(startStop_){
            if(speed_ > 0){
                speed_ -= Time.deltaTime;
            }else{
                speed_ = 0.0f;
            }
        }
        
    }
}
