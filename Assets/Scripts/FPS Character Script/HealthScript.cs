using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HealthScript : MonoBehaviour
{

    private EnemyAnimator enemy_Anim;
    private NavMeshAgent navAgent;
    private EnemyController enemy_Controller;


    public GameObject weapon_Holder;
    public float health = 100f;
    public bool is_Player, is_Zombie;

    private bool is_Dead;
    private EnemyAudio enemyAudio;

    private PlayerStats playerStats;
    private AudioSource playerAudio;

    private void Awake()
    {
        if(is_Zombie)
        {
            enemy_Anim = GetComponent<EnemyAnimator>();
            enemy_Controller = GetComponent<EnemyController>();
            navAgent = GetComponent<NavMeshAgent>();
            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }


        if (is_Player)
        {
            playerStats = GetComponent<PlayerStats>();
            playerAudio = GetComponent<AudioSource>();
        }
       
    }

    public void ApplyDamage(float damage)
    {
        if (is_Dead) return;

        health -= damage;

        if(is_Player)
        {
            // display UI health value
            playerStats.Display_HealthStats(health);
        }

        if(is_Zombie)
        { 
            if (enemy_Controller.Enemy_State == EnemyState.PATROL)
            {
                enemy_Controller.chase_Distance = 50f;
            }
        }

        if(health <= 0f)
        {
            PlayerDied();
            is_Dead = true;
        }

    }


    // for the player and the zombie
    public void PlayerDied()
    {
        if(is_Zombie)
        {
            KillCountManager.instance.AddScore();
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;
            enemy_Controller.enabled = false;

            enemy_Anim.Dead();

            StartCoroutine(DeadSound());

            //EnemyManager spawn more enemies
            EnemyManager.instance.EnemyDied();
        }

        if(is_Player)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            for(int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<EnemyController>().enabled = false;

            }

            // call the enemy manager to stop spawning enemies
            EnemyManager.instance.StopSpawning();

            GetComponent<FPSController>().enabled = false;
            GetComponent<FPSShootingControls>().enabled = false;
            weapon_Holder.SetActive(false);
    
        }

        if(tag == "Player")
        {
            //Invoke("GoToMenu", 3f);
            StartCoroutine("GoToMenu");
        }
        else
        {
            Invoke("TurnOffGameObject", 3f);
        }
    }


    IEnumerator GoToMenu()
    {
        playerAudio.Play();
        yield return new WaitForSeconds(7f);
        PlayerPrefs.SetString("score", KillCountManager.instance.GetScore().ToString());
        PlayerPrefs.SetString("highScore", KillCountManager.instance.GetHighScore().ToString());
        SceneManager.LoadScene("SampleScene");

    }

   /* public void GoToMenu()
    {
        PlayerPrefs.SetString("score", KillCountManager.instance.GetScore().ToString());
        PlayerPrefs.SetString("highScore", KillCountManager.instance.GetHighScore().ToString());
        SceneManager.LoadScene("SampleScene");
    } */


    public void TurnOffGameObject()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DeadSound()
    {
        yield return new WaitForSeconds(0.3f);
        enemyAudio.Play_DeadSound();
    }






} // class
