using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    public GameObject b_;
    public BoardController board_;
    public Rigidbody rb_;
    public float rotationSpeed_ = 2.0f;
    public float maxRotation_ = 50.0f;
    public float speed_ = 5.0f;
    private GameObject player_;
    public float lastWeight_;

    public float jumpForce_ = 5.0f;

    public float duration_ = 2.0f;
    public AnimationCurve curve;

    public float jumpSensivity_ = 1.3f;
    public bool debug_ = true;
    public bool jumpDebug_ = false;
    private bool jumping_ = false;
    private float timer_ = 0.0f;
    public float seconds_to_start_ = 3.0f;

    public UnityEvent OnPlayerJump;
    public UnityEvent OnPlayerLand;

    public Material normalMat_;
    public Material shieldMat_;

    private MeshRenderer mesh_;
    public int vidas_ = 3;
    public GameObject heart1_;
    public GameObject heart2_;
    public GameObject heart3_;
    public AudioSource playerHitted_;
    public AudioSource healthUp_;
    public AudioSource loseSound_;
    public AudioSource background_;


    public UnityEvent OnPlayerDie;
    public UnityEvent OnStartWave;
    public UnityEvent OnEndWave;
    
   
   
    [SerializeField]private bool haveShield_ = false;
    // Start is called before the first frame update
    void Start(){
        rb_ = GetComponent<Rigidbody>();
        b_ = GameObject.Find("BoardController");
        player_ = GameObject.FindWithTag("Player");
        board_ = b_.GetComponent<BoardController>();
        lastWeight_ = board_.maxWeight_;
        mesh_ = this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        mesh_.material = normalMat_;
    }

    // Update is called once per frame
    void Update(){

        timer_ += Time.deltaTime; 

        // Rotation
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -board_.bbCenter.x * speed_);
        
        if (!(Mathf.Abs(transform.eulerAngles.z) < maxRotation_) && !(Mathf.Abs(transform.eulerAngles.z) > 360.0f - maxRotation_)){
            //Debug.LogWarning("Al carrer");
            //rb_.isKinematic = false;
            //player_.GetComponent<Rigidbody>().isKinematic = false;
        }

        if(Mathf.Abs(transform.eulerAngles.z) >= maxRotation_ && !debug_ && timer_ >= seconds_to_start_){
            

        }
        if (timer_ >= seconds_to_start_)
            //Debug.Log("Angle " + Mathf.Abs(transform.eulerAngles.z) + " " + maxRotation_);
        
        Jump(); 
        lastWeight_ = BoardController.TotalWeight;
    }



    void Jump(){
        if(!jumping_){

            if(lastWeight_ <= BoardController.TotalWeight/jumpSensivity_){
                // Ha saltao
                StartCoroutine("Jumping");
                jumpDebug_ = false;
                jumping_ = true;
            }

            if(jumpDebug_){
                StartCoroutine("Jumping");
                jumpDebug_ = false;
                jumping_ = true;
            }
        }
    }

    IEnumerator Jumping(){
       
        //Debug.Log("--------- Jump --------------");
        OnPlayerJump.Invoke();
        jumpDebug_ = false;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0.0f;
        float last_strenght = -1.0f;
        while(elapsedTime < duration_){
            elapsedTime += Time.deltaTime;
            float strenght = curve.Evaluate(elapsedTime / duration_);
            transform.position = new Vector3(transform.position.x,startPosition.y + (strenght * jumpForce_),transform.position.z);

            if(last_strenght == strenght){
                elapsedTime = duration_;
            }

            last_strenght = strenght;
            
            yield return null;
        }

        transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
        jumping_ = false;
        OnPlayerLand.Invoke();
    }

    void AddLive(int value){
        vidas_ += value;

        heart1_.SetActive(false);
        heart2_.SetActive(false);
        heart3_.SetActive(false);

        if(vidas_ == 3){
            heart1_.SetActive(true);
            heart2_.SetActive(true);
            heart3_.SetActive(true);
        }

        if(vidas_ == 2){
            heart1_.SetActive(true);
            heart2_.SetActive(true);
        }
        
        if(vidas_ == 1){
            heart1_.SetActive(true);
        }

        if(vidas_ <= 0){
            OnPlayerDie.Invoke();
            loseSound_.Play();
            background_.Stop();
        }

    }

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Shield")){
            haveShield_ = true;
            mesh_.material = shieldMat_;
        }

        /*if (other.CompareTag("Enemy")){
            if (haveShield_)
            {
                haveShield_ = false;
                mesh_.material = normalMat_;
            }
            else
            {
                if (!debug_){
                    Destroy(gameObject);
                    OnPlayerDie.Invoke();
                }
            }
        }*/

        if(other.CompareTag("CoinMala") || other.CompareTag("Enemy")){
            // Quitar una vida
            AddLive(-1);
            Destroy(other.gameObject);
            playerHitted_.Play();
        }

        if(other.CompareTag("StartWave")){
            OnStartWave.Invoke();
        }
        if(other.CompareTag("EndWave")){
            OnEndWave.Invoke();
        }

        if(other.CompareTag("Heart")){
            AddLive(1);
            Destroy(other.gameObject);
            healthUp_.Play();
        }

        if(other.CompareTag("Boss")){
            AddLive(-5);
        }
    }
}
