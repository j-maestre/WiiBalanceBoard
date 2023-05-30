using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{

    public GameObject canvasEnd_;


    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void Restart(){
        SceneManager.LoadScene(1);
    }

    public void EnGame(){
        canvasEnd_.SetActive(true);
    }

}
