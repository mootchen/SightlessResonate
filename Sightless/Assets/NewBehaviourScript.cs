using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 1;
    public float timeToRotate = 2;
    public float speedWalk = 5;
    // If we want the monster to speed up when a sound is made or it senses the player?
    // public float speedRun = 8;

    // Monster's "sense" radius
    public float monsterVisionRadius = 5;
    public float monsterVisionAngle = 90;

    // Variables on player and terrain (can be updated and changed to work with the map
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 1;
    public float edgeDistance = 0.5f;

    // Patrol setup
    public Transform[] patrolWaypoints;
    int monster_WaypointsIndex;

    // Player character poitioning/location sense
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 monster_PlayerPosition;

    public float monster_waitTime;
    private float monster_TimeToRotate;
    public bool monster_PlayerInRange;
    bool monster_PlayerNear;
    bool monster_IsPatrol;
    public bool monster_FoundPlayer;

    public float viewRadius;

    public float viewAngle;

    public SimpleSonarReplacementMain script;

    private float lastFire;

    public Color ringColor = new Color(1.0f, 0.2f, 0.2f, 1);


    // Start is called before the first frame update
    void Start()
    {
        // Initializing variables
        monster_PlayerPosition = Vector3.zero;
        monster_IsPatrol = true;
        monster_FoundPlayer = false;
        monster_PlayerInRange = false;
        monster_waitTime = startWaitTime;
        monster_TimeToRotate = timeToRotate;

        monster_WaypointsIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;
        navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);

    }

    // Update is called once per frame
    void Update()
    {
        EnvironmentView();

        if(Time.time > lastFire) {
            lastFire = Time.time + 5;
            script.StartSonarRing(transform.position, 15f, ringColor);
        }

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
        playerLastPosition = Vector3.zero;
        if(monster_PlayerInRange) {
            navMeshAgent.SetDestination(monster_PlayerPosition);
        } else {
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (monster_waitTime <= 0.1f && !monster_FoundPlayer)
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
                    monster_waitTime -= Time.deltaTime;
                    if(monster_waitTime <= 0)
                        monster_waitTime = startWaitTime;
                }
            }
        }
    }
    void Patroling()
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
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);
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
                    monster_TimeToRotate -= Time.deltaTime;
                    monster_waitTime -= Time.deltaTime;
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

    public void LookingPlayer(Vector3 player)
    {
        Debug.Log("We're in there.");
        navMeshAgent.SetDestination(player);
        if(Vector3.Distance(transform.position, player) <= 0.5)
        {
            if(monster_waitTime <= 0)
            {
                monster_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(patrolWaypoints[monster_WaypointsIndex].position);
                monster_waitTime = startWaitTime;
                monster_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                monster_waitTime -= Time.deltaTime;
            }
        }
    }

    public void Move(float speed) {

    }

    public void Stop() {

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere (transform.position, viewRadius);
        float rayRange = viewRadius;
        float halfFOV = viewAngle / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
        Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
        Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
    }

    // Monster ability to sense player and the terrain
    void EnvironmentView()
    {
        int j = 0;
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for(int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position,dirToPlayer, dstToPlayer, obstacleMask))
                {
                    monster_PlayerInRange = true;
                    monster_IsPatrol = false;
                    j = i;
                }
                else
                {
                    monster_PlayerInRange = false;
                }
            }
            if(Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                monster_PlayerInRange = false;
            }
        }
        if (monster_PlayerInRange && playerInRange.Length > 0)
        {
            monster_PlayerPosition = playerInRange[j].transform.position;
        } else {
            monster_PlayerInRange = false;
            monster_FoundPlayer = false;
        }
    }
}
