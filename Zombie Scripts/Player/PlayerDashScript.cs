using FirstGearGames.SmoothCameraShaker;
using StarterAssets;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PlayerDashScript : MonoBehaviour
{
    [Header("Core Dash Values")]
    [SerializeField] private float dashSpeed = 0.3f;
    [SerializeField] private float dashDuration = 0.5f;


    [Header("Dash Reset Values")]
    [SerializeField] private int maxDashes = 3;
    [SerializeField] private float dashRecoverTime = 3;


    [Header("Dash Effects")]
    [SerializeField] private GameObject dashEffect;
    [SerializeField] private ShakeData dashShake;


    [Header("Damage")]
    [SerializeField] private Vector3 hitBoxCenter;
    [SerializeField] private Vector3 hitBoxSize;


    [Header("UI Elements")]
    [SerializeField] private Slider dashSlider;

    private CharacterController characterController;
    private StarterAssetsInputs _input;

    [Header("Sound")]
    [SerializeField] private AudioClip dashSound;
    private AudioController audioController;


    private int m_remainingDashes = 3;
    public int RemainingDashes
    {
        get { return m_remainingDashes; }
        set
        {
            value = Mathf.Clamp(value, 0, maxDashes);

            if(m_remainingDashes == value) return;

            m_remainingDashes = value;
            dashSlider.value = value;
        }

    }

    private bool m_isDashing;
    public bool IsDashing
    {
        get { return m_isDashing;}
        set
        {
            m_isDashing = value;
            dashEffect.SetActive(value);
        }
    }

    private float dashEnd, dashRecoveryTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();

        RemainingDashes = maxDashes;

        audioController = AudioController.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (IsDashing)
        {
            ApplyDashForces();
            CheckForTargets();

            // Stop here
            if (Time.time > dashEnd)
                IsDashing = false;
        }


        // Used for refilling dash amount
        if (RemainingDashes < maxDashes)
            DashRecovery();
    }

    public void DashInput()
    {
        // If player is not moving at all dont dash
        if (_input.move.x == 0 && _input.move.y == 0 || RemainingDashes < 1) return;

        // Changes all settings to begin dashing
        // Also applies effects
        BeginDash();

        if (dashSound && audioController)
        {
            audioController.PlayEffect(dashSound);
        }
    }

    private void DashRecovery()
    {
        dashRecoveryTime += Time.deltaTime;
        if (dashRecoveryTime >= dashRecoverTime)
        {
            RemainingDashes++;
            dashRecoveryTime = 0;
        }
    }
    

    private void ApplyDashForces()
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

        Vector3 moveForce = inputDirection * dashSpeed;
        characterController.Move(moveForce);
    }

    private void BeginDash()
    {
        RemainingDashes--;
        CameraShakerHandler.Shake(dashShake);
        IsDashing = true;

        dashEnd = Time.time + dashDuration;
    }

    private void CheckForTargets()
    {
       Collider[] colliders = Physics.OverlapBox(transform.TransformPoint(hitBoxCenter), hitBoxSize);

        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out IDashTarget dashTarget)) continue;  
            dashTarget.OnDashHit();
        }

        //foreach(IDashTarget target in colliders.Select(x => x.GetComponent<IDashTarget>()))  // Uses Linq to get colliders in a neater way but technically less efficient
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Translates gizmo position into local space
        Gizmos.matrix = transform.localToWorldMatrix;  
        Gizmos.DrawWireCube(hitBoxCenter, hitBoxSize);
    }
}

public interface IDashTarget
{
    public void OnDashHit();
}
