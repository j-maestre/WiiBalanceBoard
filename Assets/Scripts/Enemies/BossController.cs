using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{

    public float maxLive_ = 35.0f;
    public float actualLive_;
    public float liveToAccelerate_ = 5.0f;
    private float liveToAccelerateAux_;

    private bool isAngry_ = false;
    public float timeAngry_ = 1.5f;
    private float timer_ = 0.0f;


    public Material normalMat_;
    public Material angryMat_;
    public Material agrietadoMat_;
    public Material superAgrietadoMat_;
    public Material ultraAngryMat_;
    public Material deathMat_;
    private MeshRenderer mesh_;

    public CinemachineVirtualCamera BossCam_;
    public CinemachineVirtualCamera BossCamWaveModeOn_;
    public MovePlayer movePlayer_;
    public AudioSource background_;
    public AudioHighPassFilter audioFilter_;
    public AudioSource winSound_;
    public float cuttOfFrecuency_ = 400.0f;
    public float pitchIncreaser_ = 0.07f;
    public ParticleSystem particles_;
    public float newParticlesAngry_ = 10.0f;
    public float newParticlesSuperAngry_ = 100.0f;
    public float newParticlesUltraAngry_ = 300.0f;

    public EnemySpawner spawner_;
    public int state_ = 0;


    private float timerWaveAnimation = 0.0f;
    private bool startWaveAnimation_ = false;
    private bool endWaveAnimation_ = false;
    public float timeWaveAnimation_ = 2.0f;

    public GameObject canvasWin_;

    public UnityEvent bossDeath_;
    // Start is called before the first frame update
    void Start(){
        actualLive_ = maxLive_;
        mesh_ = GetComponent<MeshRenderer>();
        mesh_.material = normalMat_;
        liveToAccelerateAux_ = liveToAccelerate_;
    }

    // Update is called once per frame
    void Update(){

        if(isAngry_){
            timer_ += Time.unscaledDeltaTime;
 
            if(timer_ >= timeAngry_){
                timer_ = 0.0f;
                isAngry_ = false;
                mesh_.material = normalMat_;
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f;
                BossCam_.Priority = 0;
                if(actualLive_ <= maxLive_-liveToAccelerate_){
                    movePlayer_.Accelerate();
                    liveToAccelerate_ += liveToAccelerateAux_;
                    background_.pitch += pitchIncreaser_;
                    particles_.Play();
                    state_++;
                    ChangeMaterial();
                    mesh_.material = normalMat_;
                }

            }
        }

        if(state_ == 3){
            WaveMode();
        }

        if(startWaveAnimation_){
            timerWaveAnimation += Time.deltaTime;
            if(timerWaveAnimation >= timeWaveAnimation_){
                // Canbiar material, esperar, quitar prioridad a la camara
                mesh_.material = ultraAngryMat_;
                var emision = particles_.emission;
                emision.rateOverTime = newParticlesUltraAngry_;
                if(timerWaveAnimation >= timeWaveAnimation_+1.0f){
                    BossCamWaveModeOn_.Priority = 0;
                    startWaveAnimation_ = false;
                    timerWaveAnimation = 0.0f;
                    particles_.Play();
                }
            }
        }

        if(endWaveAnimation_){
            timerWaveAnimation += Time.deltaTime;
            if(timerWaveAnimation >= timeWaveAnimation_){
                state_ = 1;
                ChangeMaterial();
                mesh_.material = normalMat_;
                if(timerWaveAnimation >= timeWaveAnimation_+1.0f){
                    endWaveAnimation_ = false;
                    timerWaveAnimation = 0.0f;
                    BossCamWaveModeOn_.Priority = 0;
                    particles_.Play();
                }
            }
        }

        if(actualLive_ <= 0){
            // Muerte
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            canvasWin_.SetActive(true);
            mesh_.material = deathMat_;
            bossDeath_.Invoke();
            background_.Stop();
            winSound_.Play();
        }

        
    }

    public void StartWaveMode(){
        state_ = 3;
        BossCamWaveModeOn_.Priority = 15;
        startWaveAnimation_ = true;
        audioFilter_.cutoffFrequency = cuttOfFrecuency_;
    }
    
    public void EndWaveMode(){
        state_ = 2;
        BossCamWaveModeOn_.Priority = 15;
        endWaveAnimation_ = true;
        background_.Play();
        audioFilter_.cutoffFrequency = 10.0f;
    }

    void WaveMode(){
        spawner_.SendEnemy();
    }


    void ChangeMaterial(){
        if(state_ == 1){
            normalMat_ = agrietadoMat_;
            var emision = particles_.emission;
            emision.rateOverTime = newParticlesAngry_;
            particles_.Play();
        }

        if(state_ == 2){
            normalMat_ = superAgrietadoMat_;
            var emision = particles_.emission;
            emision.rateOverTime = newParticlesSuperAngry_;
            particles_.Play();
        }

    }

    void OnTriggerEnter(Collider other){

        if(other.CompareTag("Shot")){
            Debug.Log("Damage recived " + other.GetComponent<ShotController>().GetDamage());
            actualLive_ -= other.GetComponent<ShotController>().GetDamage();
            mesh_.material = angryMat_;
            isAngry_ = true;
            timer_ = 0.0f;
        }

        Debug.LogWarning("A chocao algo");

        if(other.CompareTag("FinishLine")){
            // Dejar de mover al boss
            transform.parent = null;
            Debug.LogWarning("A CHUPARLAAAAA");
        }
    }
}
