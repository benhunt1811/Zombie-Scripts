using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/FieldOfView", order = 1)]
public class FieldOfViewConfig : ScriptableObject
{
    [Header("FOV Values")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    private float FOVDelay = 0.2f;

    [Header("Layers")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [HideInInspector]
    [Header("Booleans")]
    public bool canSeePlayer;

    public delegate void OnSeen();
    public event OnSeen onSeenPlayer;

    public IEnumerator FOVRoutine(Transform enemy)
    {
        WaitForSeconds wait = new WaitForSeconds(FOVDelay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck(enemy);
        }
    }

    // Used to return if the enemy can see the player
    public bool FieldOfViewCheck(Transform enemy)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(enemy.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - enemy.position).normalized;
            if (Vector3.Angle(enemy.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(enemy.position, target.position);

                if (!Physics.Raycast(enemy.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }

                else
                {
                    canSeePlayer = false;
                }
            }

            else
            {
                canSeePlayer = false;
            }
        }

        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }

        return canSeePlayer;
    }
}
