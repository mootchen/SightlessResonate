using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 1;
    public flot timeToRotate = 2;
    public float speedWalk = 5;
    // If we want the monster to speed up when a sound is made or it senses the player?
    // public float speedRun = 8;

    // Monster's "sense" radius
    public float monsterVisionRadius = 5;
    public float monsterVisionAngle = 90;

    // Variables on player and terrain (can be updated and changed to work with the map
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution 1f;
    public int edgeIterations = 1;
    public float edgeDistance = 0.5f;

    // Patrol setup
    public Transform[] patrolWaypoints;
    int monster_WaypointsIndex;

    // Player character poitioning/location sense
    Vector playerLastPosition = Vector.zero;
    Vector monster_PlayerPosition;

    float monster_waitTime;
    float monster_TimeToRotate;
    bool monster_PlayerInRange;
    bool monster_PlayerNear;
    bool monster_IsPatrol;
    bool monster_FoundPlayer;


    // Start is called before the first frame update
    void Start()
    {
        // Initializing variables
        monster_PlayerPosition = Vector.zero;
        monster_IsPatrol = true;
        monster_FoundPlayer = false;
        monster_PlayerInRange = false;
        monster_waitTime = startWaitTime;
        monster_TimeToRotate = timeToRotate;

        monster_WaypointsIndex = 0;
        navMeshAgent= GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);

    }

    // Update is called once per frame
    void Update()
    {
        EnvironmentView();

        if (!monster_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }
    
    private void Chasing()
    {
        monster_PlayerNear = false;
        playerLastPosition = Vector.zero;
        if (!monster_FoundPlayer)
        {
            Move(speedWalk); // Speed can be changed here if we want a running mechanic when the monster is close enough or hears a sounds
            navMeshAgent.setDestination(monster_PlayerPosition);
        }
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (monster_waitTime <= 0 && !monster_FoundPlayer && Vector.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 3f)
            {
                monster_IsPatrol = true;
                monster_PlayerNear = false;
                Moving(speedWalk);
                monster_TimeToRotate = timeToRotate;
                monster_waitTime = startWaitTime;
                navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);

            }
            else
            {
                if(Vector.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 1f)
                {
                    StopMoving();
                    monster_waitTime -= Time.deltaTime;
                }
            }
        }
    }
    private void Patroling()
    {
        if(monster_PlayerNear)
        {
            if(monster_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                StopMoving();
                monster_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            monster_PlayerNear = false;
            playerLastPosition = Vector.zero;
            navMeshAgent.setDestination(patrolWaypoints[monster_WaypointsIndex].position);
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if(monster_waitTime <= 0)
                {
                    NextPoint();
                    Moving(speedWalk);
                    monster_waitTime = startWaitTime;
                }
                else
                {
                    StopMoving();
                    monster_timeToRotate -= Time.deltaTime;
                }
            }
        }
    }
    
    // Start moving
    void Moving(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    // Stop moving
    void StopMoving()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    // Moving onto next waypoint/patrol
    public void NextPoint()
    {
        monster_WaypointsIndex = (monster_WaypointsIndex + 1) % patrolWaypoints.Length;
        navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);
    }

    //Methods

    void FoundPlayer()
    {
        monster_FoundPlayer = true;
    }

    void LookingPlayer(Vector player)
    {
        navMeshAgent.SetDestination(player);
        if(Vector.Distance(transform.position, player) <= 0.5)
        {
            if(monster_waitTime <= 0)
            {
                monster_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);
                monster_waitTime = startWaitTime;
                monster_TimeToRotate = timeToRotate;
=            }
            else
            {
                Stop();
                monster_waitTime -= Time.deltaTime;

            }
        }
    }

    // Monster ability to sense player and the terrain
    void EnvironmentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for(int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector dirToPlayer = (player.position - transform.position).normalized;
            if (Vector.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position,dirToPlayer, dstToPlayer, obstacleMask))
                {
                    monster_PlayerInRange = true;
                    monster_IsPatrol = false;

                }
                else
                {
                    monster_PlayerInRange = false;
                }
            }
            if(Vector.Distance(transform.position, player.position) > viewRadius)
            {
                monster_PlayerInRange = false;

            }
        }
        if (monster_PlayerInRange)
        {
            monster_PlayerPosition = playerLastPosition.transform.position;
        }
    }
}
