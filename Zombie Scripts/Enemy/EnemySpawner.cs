using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public EnemyScriptableObject enemy;

    public Transform[] waypoints;

    void Start()
    {
        enemy.Spawn(this, this.transform, waypoints);
    }
}
