using Cinemachine;
using StarterAssets;
using UnityEngine;

public class HeadBobScript : MonoBehaviour
{
    [Header("Components")]
    public CinemachineVirtualCamera playerCamera;
    private StarterAssetsInputs starterInputs;
    private FirstPersonController firstPersonController;
    private CinemachineBasicMultiChannelPerlin cinemachineNoise;

    [Header("Movement Frequencies")]
    private float standingFreq = 0.2f;
    private float runningFreq = 2.5f;
    private float walkingFreq = 1f;
    private float midAirFreq = 0.1f;

    [Header("Aim Settings")]
    private bool isAiming => (Input.GetMouseButton(1));
    private Vector3 aimProfile = new Vector3(0.01f, 0, 0.01f);

    [Header("Default Values")]
    private Vector3 defaultProfile;
    private float defaultAmplitude;
    private float defaultFrequency;

    private void Start()
    {
        starterInputs = GetComponent<StarterAssetsInputs>();
        firstPersonController = GetComponent<FirstPersonController>();
        cinemachineNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        defaultProfile = cinemachineNoise.m_PivotOffset;
        defaultFrequency = cinemachineNoise.m_FrequencyGain;
        defaultAmplitude = cinemachineNoise.m_AmplitudeGain;
    }

    void Update()
    {
        CheckAiming();
        CheckMovement();
    }

    private void CheckMovement()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (starterInputs.sprint != true && inputMagnitude > 0 && firstPersonController.Grounded == true)
        {
            if (!isAiming)
            {
                cinemachineNoise.m_FrequencyGain = walkingFreq;
            }
        }

        if (starterInputs.sprint && inputMagnitude > 0.5 && firstPersonController.Grounded == true)
        {
            if (!isAiming)
            {
                cinemachineNoise.m_FrequencyGain = runningFreq;
            }
        }

        if (inputMagnitude == 0 && firstPersonController.Grounded == true)
        {
            if (!isAiming)
            {
                cinemachineNoise.m_FrequencyGain = standingFreq;
            }
        }

        if (firstPersonController.Grounded != true)
        {
            cinemachineNoise.m_FrequencyGain = midAirFreq;
        }
    }

    private void CheckAiming()
    {
        if (isAiming)
        {
            cinemachineNoise.m_PivotOffset = aimProfile;
            cinemachineNoise.m_AmplitudeGain = 0.1f;
            cinemachineNoise.m_FrequencyGain = 0.1f;
        }

        else
        {
            cinemachineNoise.m_PivotOffset = defaultProfile;
            cinemachineNoise.m_AmplitudeGain = defaultAmplitude;
            cinemachineNoise.m_FrequencyGain = defaultFrequency;
        }
    }
}
