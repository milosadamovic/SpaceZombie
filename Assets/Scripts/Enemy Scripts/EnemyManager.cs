using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager instance;

    [SerializeField]
    private GameObject zombie_Prefab;

    public Transform[] zombie_SpawnPoints;

    [SerializeField]
    private int zombie_Count;

    private int initial_zombieCount;

    public float wait_Before_Spawn_Enemies_Time = 10f;


    private void Awake()
    {
        MakeInstance();
    }


    private void Start()
    {
        initial_zombieCount = zombie_Count;

        SpawnEnemies();
        StartCoroutine(CheckToSpawnEnemies());
    }

    void MakeInstance()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    IEnumerator CheckToSpawnEnemies()
    {
        yield return new WaitForSeconds(wait_Before_Spawn_Enemies_Time);
        SpawnEnemies();

        StartCoroutine(CheckToSpawnEnemies());
    }


    void SpawnEnemies()
    {

        for(int i = 0; i < zombie_Count; i++)
        {
            Instantiate(zombie_Prefab, zombie_SpawnPoints[i].position, Quaternion.identity);
        }

        zombie_Count = 0;
    }


    public void EnemyDied()
    {
        zombie_Count++;

        if (zombie_Count > initial_zombieCount)
            zombie_Count = initial_zombieCount;
    }


    public void StopSpawning()
    {
        StopCoroutine(CheckToSpawnEnemies());
    }



}// class
