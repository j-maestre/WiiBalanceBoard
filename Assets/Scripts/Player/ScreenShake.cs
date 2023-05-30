using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{

    private CinemachineVirtualCamera cinemaVirtual_;
    public float shakeInstensity_ = 20.0f;
    public float shakeTime_ = 1.0f;
    private float timer_ = 0.0f;
    private bool shaking_ = false;

    private CinemachineBasicMultiChannelPerlin perlin_;
    public Transform playerTr_;
    

    // Start is called before the first frame update
    void Start(){
        cinemaVirtual_ = GetComponent<CinemachineVirtualCamera>();
        perlin_ = cinemaVirtual_.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update(){

        if(shaking_){
            timer_ -= Time.deltaTime;
            if(timer_ <= 0.0f){
                StopShake();
            }
        }

        
    }

    public IEnumerator Shake(){

      cinemaVirtual_.Follow = null;
      Vector3 originalPosition_ = transform.position;

      float timer_ = 0.0f;

      while (timer_ < shakeTime_){

        transform.position = originalPosition_ + Random.insideUnitSphere;
        timer_ += Time.deltaTime;

        yield return null;
      }
      cinemaVirtual_.Follow = playerTr_;
    }


    public void StartShake(){
        Debug.Log("SHAKEEEE");
        StartCoroutine("Shake");
        /*shaking_ = true;
        perlin_.m_AmplitudeGain = shakeInstensity_;
        timer_ = shakeTime_;*/
        
    }

    public void StopShake(){
        perlin_.m_AmplitudeGain = 0.0f;
        timer_ = 0.0f;
    }


    
}
