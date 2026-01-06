using UnityEngine;

public class EnemyHealth : MonoBehaviour, Damageable
{
    [SerializeField]
    private int _maxHealth = 100;

    [SerializeField]
    private int _health;

    private Enemy enemy;
    private EnemyAnimatorScript animator;

    [Header("Health Values")]
    public int maxHealth { get => _maxHealth; private set => _maxHealth = value; }
    public int currentHealth { get => _health; private set => _health = value; }

    public event Damageable.TakeDamageEvent onTakeDamage;
    public event Damageable.DeathEvent onDeath;

    [Header("Sound")]
    private AudioController audioController;
    private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        enemy = GetComponent<Enemy>();
        animator = GetComponentInChildren<EnemyAnimatorScript>();
        audioController = AudioController.Instance;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage, 0, currentHealth);
        currentHealth -= damageTaken;

        if (damageTaken != 0)
        {
            onTakeDamage?.Invoke(damageTaken);
            enemy.enemyState = EnemyState.Hunt;

            if (enemy.enemyType == EnemyType.Eating && enemy.isStanding != true)
            {
                enemy.animator.SetTrigger("Awake");
            }

            if (!enemy.isSeenCountdownRunning)
            {
                StartCoroutine(enemy.SeenCooldown());
            }
        }

        if (currentHealth == 0 && damageTaken != 0)
        {
            if (audioController)
            {
                audioController.PlaySoundInWorldSingle(audioSource, deathSound);
            }
            onDeath?.Invoke(transform.position);
        }
    }

    public void StumbleCheck(int power)
    {
        // Power is passed in as a value
        // The lower the power the higher chance of a stagger
        int rand = Random.Range(0, power);

        if (rand == 1)
        {
            rand = Random.Range(0, 1);
            if (rand == 1)
            {
                animator.Stumble();
            }

            if (rand == 0)
            {
                animator.FullStumble();
            }
        }
    }
}
