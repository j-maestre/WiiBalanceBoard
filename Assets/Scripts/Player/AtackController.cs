using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AtackController : MonoBehaviour
{
    [SerializeField]private int coinsAcumulated_ = 0;
    public float pitchIncrement_ = 0.1f;
    public float originalPitch_;
    public AudioSource coinAudio_;
    public AudioSource shotAudio_;
    public GameObject shotBall_;

    private GameObject ballInMovement_;
    private float ballInMovementScale_;
    public Transform bossPosition_;
    private bool ballMoving_ = false;

    public CinemachineVirtualCamera BossCam_;

    public float distanceFirstSlowmo_ = 50.0f;
    public float distanceSecondSlowmo_ = 23.5f;

    private float radius_;
    public ParticleSystem particles_;

    // Start is called before the first frame update
    void Start(){
        originalPitch_ = coinAudio_.pitch;
        bossPosition_ =  GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update(){


        if(ballMoving_){
            Vector3 center = ballInMovement_.GetComponent<Transform>().position;
            Vector3 scale = ballInMovement_.GetComponent<Transform>().localScale;
            radius_ = ballInMovement_.GetComponent<SphereCollider>().radius;


            Vector3 distance = bossPosition_.position - center;
            Vector3 newPoint = ballInMovement_.transform.TransformPoint(distance.normalized * radius_);
            distance = bossPosition_.position - newPoint;
            
            // First Slowmo
            if(distance.magnitude <= radius_ + distanceFirstSlowmo_){
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.0002f;
                BossCam_.Priority = 15;

                // Second Slowmo
                if(distance.magnitude <= radius_ + distanceSecondSlowmo_){
                    Time.timeScale = 0.01f;
                    Time.fixedDeltaTime = 0.00002f;
                    ballMoving_ = false;
                }
            }
        }
        var emision = particles_.emission;
        emision.rateOverTime = coinsAcumulated_ * 2.0f;
        
    }

    public void Shot(){
        // Solo puede disparar si tiene almenos una
        if(coinsAcumulated_ > 0){
            GameObject ball = Instantiate(shotBall_, transform.position + (transform.forward * 2.0f), Quaternion.identity);
            float scale = coinsAcumulated_ * 1.5f;
            ball.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.2f, 3.0f) * 60.0f;
            ball.GetComponent<ShotController>().SetDamage((float) coinsAcumulated_);
            shotAudio_.Play(0);
            coinsAcumulated_ = 0;
            coinAudio_.pitch = originalPitch_;
            ballInMovement_ = ball;
            ballInMovementScale_ = scale;
            ballMoving_ = true;
        }

    }

    void OnTriggerEnter(Collider other){
       
        if(other.CompareTag("Coin")){
      
            coinsAcumulated_++;
            coinAudio_.pitch += pitchIncrement_;
            coinAudio_.Play(0);
            Destroy(other.gameObject);

        }
        
        if(other.CompareTag("CoinTocha")){
      
            coinsAcumulated_+=5;
            coinAudio_.pitch += pitchIncrement_;
            coinAudio_.Play(0);
            Destroy(other.gameObject);

        }
    }
}
