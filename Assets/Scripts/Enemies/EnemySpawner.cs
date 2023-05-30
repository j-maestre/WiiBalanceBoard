using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnInfo
{
    public enum SpawnPosition
    {
        Right,
        Left,
        Center,
        CenterDown,
        Random
    };
    [Range(0.0f, 100.0f)] public float chance;
    public GameObject prefab;
    public SpawnPosition position;
};

public class EnemySpawner : MonoBehaviour
{
    [Space] [Header("Spawn Position")]
    [SerializeField] float distance;
    [SerializeField] float LeftRigntSeparation;
    [SerializeField] float DownSeparation;
    [SerializeField] Transform StartSpawnPosition;

    [Space] [Header("Spawn Time")]
    [SerializeField] float SpawnTime;

    [Space] [Header("Enemies Info")]
    [SerializeField] SpawnInfo[] info;

    [HideInInspector] public bool active_;
    int info_index;
    float timer_;
    float enemySpeed_ = 75.0f;

    // Start is called before the first frame update
    void Start()
    {
         info_index = Random.Range(0, info.Length);
    }

    // Update is called once per frame 
    void Update()
    {
        if (active_)
        {
            timer_ += Time.deltaTime;
            if (timer_ >= SpawnTime)
            {
                timer_ -= SpawnTime;
                bool spawned = false;
                while(!spawned && info.Length > 0)
                {
                    /*if (Random.Range(0.0f, 99.0f) < info[info_index].chance)
                    {
                        info_index++;
                        info_index %= info.Length;
                        spawned = true;
                        SpawnEnemy(info[info_index]);
                    }*/
                }
            }
        }
    }

    public void SendEnemy(){
        timer_ += Time.deltaTime;
        if (timer_ >= SpawnTime){
            timer_ -= SpawnTime;
            bool spawned = false;
            while(!spawned && info.Length > 0)
            {
                if (Random.Range(0.0f, 99.0f) < info[info_index].chance)
                {
                    info_index++;
                    info_index %= info.Length;
                    spawned = true;
                    SpawnEnemy(info[info_index]);
                }
            }
        }
    }

    void SpawnEnemy(SpawnInfo enemyInfo)
    {
        Vector3 center = StartSpawnPosition.position - (StartSpawnPosition.forward * distance);
        Vector3 right = center + (StartSpawnPosition.right * LeftRigntSeparation);
        Vector3 left = center - (StartSpawnPosition.right * LeftRigntSeparation);
        Vector3 centerDown = center - (StartSpawnPosition.up * DownSeparation);

        GameObject object_Spawned;
        Vector3 position = center;
        AudioSource audioPrefab = enemyInfo.prefab.GetComponent<AudioSource>();

        if (enemyInfo.position == SpawnInfo.SpawnPosition.Random){        
            switch(Random.Range(0, 4)){
            case 0:
                //object_Spawned = Instantiate(enemyInfo.prefab, center, Quaternion.identity);
                position = center;
                break;
            case 1:
                //object_Spawned = Instantiate(enemyInfo.prefab, right, Quaternion.identity);
                position = right;
                audioPrefab.panStereo = 0.8f;
                break;
            case 2:
                //object_Spawned = Instantiate(enemyInfo.prefab, left, Quaternion.identity);
                position = left;
                audioPrefab.panStereo = -0.8f;
                break;
            case 3:
                //object_Spawned = Instantiate(enemyInfo.prefab, centerDown, Quaternion.identity);
                position = centerDown;
                break;
            }

        }


        if (enemyInfo.position == SpawnInfo.SpawnPosition.Right){
            position = right;
            audioPrefab.panStereo = 0.8f;
            //object_Spawned = Instantiate(enemyInfo.prefab, right, Quaternion.identity);
        }

        if (enemyInfo.position == SpawnInfo.SpawnPosition.Left){
            position = left;
            audioPrefab.panStereo = -0.8f;
            //object_Spawned = Instantiate(enemyInfo.prefab, left, Quaternion.identity);
        }

        if (enemyInfo.position == SpawnInfo.SpawnPosition.Center){
            position = center;
            //object_Spawned = Instantiate(enemyInfo.prefab, center, Quaternion.identity);
        }

        if (enemyInfo.position == SpawnInfo.SpawnPosition.CenterDown){
            position = centerDown;
            //object_Spawned = Instantiate(enemyInfo.prefab, centerDown, Quaternion.identity);
        }

        object_Spawned = Instantiate(enemyInfo.prefab, position, Quaternion.identity);
        object_Spawned.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, -enemySpeed_);


    }
}
