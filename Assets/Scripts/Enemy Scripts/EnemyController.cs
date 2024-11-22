using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum EnemyState
{
    PATROL,
    CHASE,
    ATTACK
}


public class EnemyController : MonoBehaviour
{


    private EnemyAnimator enemy_Anim;
    private NavMeshAgent navAgent;
    private EnemyState enemy_State;

    public float walk_Speed = 0.5f;
    public float run_Speed = 4f;

    public float chase_Distance = 7f;
    private float current_Chase_Distance;
    public float attack_Distance = 1.8f;
    public float chase_After_Attack_Distance = 2f;
    public float patrol_Radius_Min = 20f, patrol_Radius_Max = 60f;
    public float patrol_For_This_Time = 15f;
    private float patrol_Timer;

    public float wait_Before_Attack = 2f;
    private float attack_Timer;

    private Transform target;

    public GameObject attack_Point;

    private EnemyAudio enemyAudio;

    private void Awake()
    {
        enemy_Anim = GetComponent<EnemyAnimator>();
        navAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;

        enemyAudio = GetComponentInChildren<EnemyAudio>();
    }



    // Start is called before the first frame update
    void Start()
    {
        enemy_State = EnemyState.PATROL;

        patrol_Timer = patrol_For_This_Time;

        // when enemy first gets to the player attack right a way
        // attack_Timer = wait_Before_Attack;


        // memorize the value of chase distance so that we can put it back
          current_Chase_Distance = chase_Distance; 
    }

    // Update is called once per frame
    void Update()
    {


        if(enemy_State == EnemyState.PATROL)
        {
            Turn_Off_AttackPoint();
            Patrol();
        }
        if (enemy_State == EnemyState.CHASE)
        {
            Turn_Off_AttackPoint();
            Chase();
        }
        if (enemy_State == EnemyState.ATTACK)
        {
            Turn_On_AttackPoint();
            Attack();
        }


    }


    public void Patrol()
    {
        // tel nav agent that he can move
        navAgent.isStopped = false;
        navAgent.speed = walk_Speed;

        patrol_Timer += Time.deltaTime;

        if(patrol_Timer > patrol_For_This_Time)
        {
            SetNewRandomDestination();
            patrol_Timer = 0;
        }

        if(navAgent.velocity.sqrMagnitude > 0)
        {
            enemy_Anim.Walk(true);
        }
        else
        {
            enemy_Anim.Walk(false);
        }


        // test the distance between the player and the enemy
        if( Vector3.Distance(transform.position, target.position) <= chase_Distance)
        {
            enemy_Anim.Walk(false);
            enemy_State = EnemyState.CHASE;
            enemyAudio.Play_ChaseSound();
        }

    }
    public void Chase()
    {
        navAgent.isStopped = false;
        navAgent.speed = run_Speed;

        // running towards player
        navAgent.SetDestination(target.position);
        

        if (navAgent.velocity.sqrMagnitude > 0)
        {
            enemy_Anim.Run(true);
        }
        else
        {
            enemy_Anim.Run(false);
        }


        // if the distance between enemy and player is less than attack distance
        if(Vector3.Distance(transform.position, target.position) <= attack_Distance)
        {
            enemy_Anim.Run(false);
            enemy_Anim.Walk(false);
            enemy_State = EnemyState.ATTACK;
            enemyAudio.Play_AttackSound();

            // reset the chase distance to previous value
            if (chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance;
            }
        }
        // if the player runs from the enemy
        else if(Vector3.Distance(transform.position, target.position) > chase_Distance)
        {
            enemy_Anim.Run(false);
            enemy_State = EnemyState.PATROL;

            // reset the patrol timer so that the function can calculate the new patrol destination right away
            patrol_Timer = patrol_For_This_Time;
            if (chase_Distance != current_Chase_Distance)
            {
                chase_Distance = current_Chase_Distance;
            }
        }

    }
    public void Attack()
    { 
        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = false;

       // attack_Timer += Time.deltaTime;


        enemy_Anim.Attack();

       // attack_Timer = 0f;

       // enemyAudio.Play_AttackSound();


       /* if (attack_Timer > wait_Before_Attack)
        {
            enemy_Anim.Attack();

            attack_Timer = 0f;

          //  enemyAudio.Play_AttackSound();
        }*/

        if(Vector3.Distance(transform.position, target.position) > attack_Distance + chase_After_Attack_Distance)
        {
            enemy_State = EnemyState.CHASE;
        }


    }

    public void SetNewRandomDestination()
    {
        float rand_Radius = Random.Range(patrol_Radius_Min, patrol_Radius_Max);

        Vector3 randDir = Random.insideUnitSphere * rand_Radius;
        randDir += transform.position;

        NavMeshHit navHit;


        // if position is out of terrain
        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);

        navAgent.SetDestination(navHit.position);
    }



    void Turn_On_AttackPoint()
    {
        attack_Point.SetActive(true);
    }

    void Turn_Off_AttackPoint()
    {
        if(attack_Point.activeInHierarchy)
                attack_Point.SetActive(false);
    }



    public EnemyState Enemy_State
    {
        get; set;
    }









} // class
