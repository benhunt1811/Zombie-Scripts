using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorScript : MonoBehaviour
{
    [Header("Components")]
    private NavMeshAgent navAgent;
    private Animator animator;
    private Enemy enemy;
    public GameObject hitbox;

    private float blendSpeed = 10;

    private bool isAttacking;
    private bool isFullStumbling;
    public bool isScreaming;

    [Header("Sound")]
    private AudioController audioController;
    private AudioSource audioSource;
    [SerializeField] private AudioClip stumbleAudio;
    [SerializeField] private AudioClip screamAudio;
    [SerializeField] private AudioClip attackAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navAgent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemy = GetComponentInParent<Enemy>();

        audioSource = GetComponentInParent<AudioSource>();
        audioController = AudioController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();      
    }

    // Used to tie the enemys speed with the blend animator
    private void UpdateSpeed()
    {
        // Used to get enemys velocity
        Vector3 currentVelocity = navAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);
        float speed = localVelocity.z;

        animator.SetFloat("Speed", speed);
    }

    public void StartHitbox()
    {
        hitbox.SetActive(true);
    }

    public void HideHitbox()
    {
        hitbox.SetActive(false);
        isAttacking = false;
    }

    public void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }

    public void Attacking()
    {
        if (isAttacking != true)
        {
            if (audioController && attackAudio)
            {
                audioController.PlaySoundInWorldSingle(audioSource, attackAudio);
            }

            animator.SetTrigger("Attack");
            isAttacking = true;
        }   
    }

    public void Screaming()
    {
        if (isAttacking != true)
        {

            if (audioController && screamAudio)
            {
                audioController.PlaySoundInWorldSingle(audioSource, screamAudio);
            }

            animator.SetTrigger("Scream");
            isScreaming = true;
        }
    }

    public void StopAttacking()
    {
        isAttacking = false;
    }

    public void StopScreaming()
    {
        isScreaming = false;
    }

    public void Stumble()
    {

        if (enemy.isStanding)
        {
            animator.SetTrigger("Stumble");
            PlayStumbleNoise();

            if (isAttacking)
            {
                isAttacking = false;
                hitbox.SetActive(false);
            }
        }
    }

    public void FullStumble()
    {
        if (isFullStumbling != true && enemy.isStanding)
        {
            animator.SetTrigger("FullStumble");
            animator.SetBool("IsFullStumbling", true);
            isFullStumbling = true;
            navAgent.speed = 0;
            PlayStumbleNoise();

            if (isAttacking)
            {
                isAttacking = false;
                hitbox.SetActive(false);
            }
        }
    }

    public void FullStumbleReset()
    {
        StartCoroutine(ResetWalk());
        animator.SetBool("IsFullStumbling", false);
        isFullStumbling = false;
        isAttacking = false;        
    }

    public void SetSpeed()
    {
        navAgent.speed = 0.5f;
        enemy.isStanding = true;
    }

    private IEnumerator ResetWalk()
    {
        float duration = 1f; 

        float normalizedTime = 0;
        while (normalizedTime <= duration / 2)
        {
            navAgent.speed = normalizedTime;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }

    private void PlayStumbleNoise()
    {
        if (audioController && stumbleAudio)
        {
            audioController.PlaySoundInWorldSingle(audioSource, stumbleAudio);
        }
    }

}
