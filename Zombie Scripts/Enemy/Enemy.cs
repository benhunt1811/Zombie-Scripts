using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDashTarget
{
    [Header("Components")]
    private EnemyHealth health;
    [HideInInspector]
    public Animator animator;
    private EnemyScriptableObject enemyScriptableObject;
    private NavMeshAgent navAgent;
    [HideInInspector]
    public FieldOfViewConfig FOV;
    public EnemyMove enemyMovement;

    [Header("Values and States")]
    [HideInInspector]
    public float timeAdded;
    private float attackCooldown = 4.5f;
    private float attackDistance = 40;
    private float enemyHuntTime = 5;
    public EnemyState enemyState;
    public EnemyType enemyType;
    private int actionRandomiser;
    private int actionRandomiserMax = 4;

    [Header("Booleans")]
    public bool hasSeenPlayer;
    public bool isStanding;
    public bool isSeenCountdownRunning;
    public bool canAttack = true;

    [Header("Scripts")]
    private PlayerScript player;
    private EnemyAnimatorScript enemyanimator;
    private DissolveController dissolve;

    private void Start()
    {
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
        enemyanimator = GetComponentInChildren<EnemyAnimatorScript>();
        navAgent = GetComponent<NavMeshAgent>();
        dissolve = GetComponentInChildren<DissolveController>();

        enemyState = EnemyState.Patrol;
        health.onDeath += Die;

        player = PlayerScript.Instance;
    }

    // Deallocates events added to
    private void OnDestroy()
    {
        health.onDeath -= Die;
    }

    private void Die(Vector3 position)
    {
        animator.SetTrigger("IsDead");
        // Sets upper body weight to 0
        animator.SetLayerWeight(1, 0);
        navAgent.speed = 0;

        // Begins the dissolve effect
        StartCoroutine(dissolve.Dissolve());

        // Adds time to the players timer
        player.timer.AddTime(timeAdded);
        player.ShowAddedTime(timeAdded);

        player.UpdateKillCount();
    }

    // Used for getting the scriptable object
    // that the enemy originates from
    public void SetEnemyObject(EnemyScriptableObject enemy)
    {
        enemyScriptableObject = enemy;
    }

    private void Update()
    {
        hasSeenPlayer = FOV.FieldOfViewCheck(this.transform);

        if (hasSeenPlayer)
        {
            enemyState = EnemyState.Hunt;

            // Used to get the eating enemy to stop eating
            if (enemyType == EnemyType.Eating)
            {
                animator.SetTrigger("Awake");
            }
        }

        if (enemyState == EnemyState.Hunt)
        {
            navAgent.SetDestination(player.gameObject.transform.position);

            // Gets the distance from this enemy to the player
            // If distance is in attack distance start attacking
            float distance = (this.transform.position - player.transform.position).sqrMagnitude;

            if (distance < attackDistance && isStanding && canAttack && enemyanimator.isScreaming != true)
            {
                actionRandomiser = Random.RandomRange(0, actionRandomiserMax);
                if (actionRandomiser == 0)
                {
                    enemyanimator.Screaming();
                }

                else
                {
                    enemyanimator.Attacking();
                }

                StartCoroutine(AttackCooldown());
            }

            // Both used as timers for seeing the player
            // If the player is not seen it starts a timer
            // Once that timer ends the enemy stops hunting
            if (hasSeenPlayer && isSeenCountdownRunning)
            {
                StopCoroutine(SeenCooldown());
            }

            if (hasSeenPlayer && !isSeenCountdownRunning)
            {
                StartCoroutine(SeenCooldown());
            }
        }
    }

    // Used to reset if enemy hasnt seen player for a set time;
    public IEnumerator SeenCooldown()
    {
        isSeenCountdownRunning = true;

        yield return new WaitForSeconds(enemyHuntTime);

        
        enemyState = EnemyState.Search;
        enemyScriptableObject.moveConfig.UpdateDestination();
        enemyanimator.StopAttacking();

        isSeenCountdownRunning = false;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void OnDashHit()
    {
        enemyanimator.FullStumble();
    }
}