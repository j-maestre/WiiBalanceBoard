using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] patrones;
    public Transform player;
    public MovePlayer playerMovement_;
    public float distance;
    public float separation;
    public float TimeToSpawn;
    private float timer;
    public float TimeDecrement_;
    [SerializeField]private int enemies_count;
    private int enemy_index_;

    public int EnemiesToAccelerateSpawn;
    public int EnemiesToAccelerateSpeed;
    public int enemiesToOnlyPatrons_ = 10;
    public bool startOnlyPatrons_ = false;
    public int timeBetweenPatrons_ = 5;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        enemies_count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= TimeToSpawn){
            timer -= TimeToSpawn;

            if(startOnlyPatrons_){
                //Spawn patron
                Debug.Log("Patron");
                SpawnEnemy(patrones[Random.Range(0,patrones.Length)], 3.0f, true);

            }

            if(!startOnlyPatrons_){
                // Spawn single enemy

                enemy_index_ = Random.Range(0, enemies.Length);
                float upDistance = 0.0f;

                // Si el enemigo es la bolita, lo ponemos un poco mas arriba para que pueda esquivarlo en el centro sin saltar
                if(enemy_index_ == 0){
                    upDistance = 3.0f;
                }
                SpawnEnemy(enemies[enemy_index_], upDistance);
            }
            
            
            if (enemies_count == EnemiesToAccelerateSpawn){
                TimeToSpawn -= TimeDecrement_;
            }

            if(enemies_count == EnemiesToAccelerateSpeed){
                playerMovement_.Accelerate();
            }

            if(enemies_count == enemiesToOnlyPatrons_){
                startOnlyPatrons_ = true;
                TimeToSpawn = timeBetweenPatrons_;
            }
        }
    }

    void SpawnEnemy(GameObject enemy, float upDistance = 0.0f, bool isPatron = false){

        Vector3 pos = player.position + player.forward * distance;
        pos.y += upDistance;
        
        
        if(!isPatron){
            // Single enemy    
            int p = Random.Range(0, 3);

            if (enemy_index_ == 2) p = 2;
            
            if (p == 0){
                pos += player.right * separation;
            }

            if (p == 1){
                pos -= player.right * separation;
            }

            if (p == 2){
                pos -= player.up * 2.0f;
            }
        }else{
            // Patron

        }
        
        Instantiate(enemy, pos, Quaternion.identity);
        enemies_count++;
    }
}
