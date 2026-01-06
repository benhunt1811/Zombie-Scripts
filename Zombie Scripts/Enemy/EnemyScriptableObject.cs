using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Enemy", order = 0)]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Configurations")]
    public FieldOfViewConfig FOVConfig;
    public EnemyMove moveConfig;

    [Header("Enemy Info")]
    public GameObject enemyPrefab;
    public float deathTimeAmount;
    private GameObject enemyObject;
    private NavMeshAgent enemyNav;
    private Enemy enemy;

    public GameObject Spawn(MonoBehaviour activeMonoBehaviour, Transform spawnTransform, Transform[] waypoints)
    {
        // Spawning the enemy object
        enemyObject = Instantiate(enemyPrefab, spawnTransform.position, spawnTransform.rotation);

        enemy = enemyObject.GetComponent<Enemy>();

        // Assigning all components to the enemy object
        // Also passing through values
        enemy.timeAdded = deathTimeAmount;
        enemy.SetEnemyObject(this);
        enemy.FOV = FOVConfig;
        enemy.enemyMovement = moveConfig;

        moveConfig = enemyObject.GetComponent<EnemyMove>();
        enemyNav = enemyObject.GetComponent<NavMeshAgent>();
        
        // Resetting the FOV seen player
        FOVConfig.canSeePlayer = false;

        //enemy.StartMovement(activeMonoBehaviour, waypoints);

        // Setting up the waypoints for the enemy move config
        moveConfig.waypoints = waypoints;
        activeMonoBehaviour.StartCoroutine(moveConfig.MoveRoutine(enemyObject.transform, enemyNav));
        moveConfig.UpdateDestination();

        return enemyObject.gameObject;
    }
}