using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent enemyMove;

    [HideInInspector]
    [Header("Target Location")]
    public  Vector3 target;

    [Header("Waypoint Info")]
    public Transform[] waypoints;
    public float waypointDistance;
    private int waypointIndex;

    [Header("Move Info")]
    private float MoveDelay = 0.2f;

    private void Start()
    {
        enemyMove = GetComponent<NavMeshAgent>();
    }

    // Updates enemies new location once it reaches its current one
    public void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        enemyMove.SetDestination(target);
    }

    private void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    public IEnumerator MoveRoutine(Transform enemy, NavMeshAgent enemyNav)
    {
        enemyMove = enemyNav;
        WaitForSeconds wait = new WaitForSeconds(MoveDelay);

        while (true)
        {
            yield return wait;
            DistanceCheck(enemy);
        }
    }

    // Used to check distance to location
    private void DistanceCheck(Transform enemy)
    {
        if (Vector3.Distance(enemy.transform.position, target) < waypointDistance)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }
    }

    public void MoveToPlayer(Transform playerLocation)
    {
        enemyMove.SetDestination(playerLocation.position);
    }
}
