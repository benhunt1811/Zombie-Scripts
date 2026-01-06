using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.GlobalIllumination;

public class GunInteratable : Interactable
{
    [Header("Player Scripts")]
    private GameObject player;
    private PlayerGunSelector playerGun;
    private PlayerScript playerScript;

    [Header("Gun Fields")]
    public GunType gun;
    public GunScriptableObject gunScript;

    [Header("Camera Objects")]
    private Camera camera;
    private CinemachineVirtualCamera virtualCamera;

    [Header("Input Fields")]
    private StarterAssetsInputs starterAssets;
    private FirstPersonController firstPersonController;

    [Header("Light Settings")]
    private Light light;
    private float baseLightIntensity = 1;
    private float interactlightIntensity = 15;

    [Header("Animation Settings")]
    public GunPickupAnimationScript gunAnim;
    private float baseGunAnimSpeed = 1;
    private float interactGunAnimSpeed = 3;

    [Header("Dissolve Settings")]
    private DissolveController dissolveController;
    private VisualEffect VFXGraph;

    [Header("Sound")]
    [SerializeField] private AudioClip lookAtSound;
    private AudioController audioController;

    private bool isBeingLookedAt;

    private void Start()
    {
        light = GetComponentInChildren<Light>();
        playerScript = PlayerScript.Instance;
        player = playerScript.gameObject;
        playerGun = player.GetComponent<PlayerGunSelector>();
        firstPersonController = player.GetComponent<FirstPersonController>();
        starterAssets = player.GetComponent<StarterAssetsInputs>();

        camera = playerScript.mainCamera;
        virtualCamera = playerScript.virtualCamera;

        dissolveController = GetComponent<DissolveController>();
        VFXGraph = GetComponent<VisualEffect>();

        audioController = AudioController.Instance;

        isBeingLookedAt = false;

        StartCoroutine(dissolveController.Appear());
    }

    public override void Interact()
    {       
        if (playerGun.currentGunObject != null)
        {
            Destroy(playerGun.currentGunObject);
        }

        SpawnGun();

        light.gameObject.SetActive(false);
        StartCoroutine(dissolveController.Dissolve());
        if (VFXGraph != null)
        {
            VFXGraph.Play();
        }
    }

    public override void HideInteractable()
    {
        if (isBeingLookedAt)
        {
            light.intensity = baseLightIntensity;
            gunAnim.moveTween.timeScale = baseGunAnimSpeed;

            isBeingLookedAt = false;

        }
    }

    public override void ShowInteractable()
    {
        if (isBeingLookedAt == false)
        {
            light.intensity = interactlightIntensity;
            gunAnim.moveTween.timeScale = interactGunAnimSpeed;

            if (audioController && lookAtSound)
            {
                audioController.PlayEffect(lookAtSound);
            }

            isBeingLookedAt = true;
        }
    }


    private void SpawnGun()
    {
        playerGun.activeGun = gunScript;
        playerGun.currentGunObject = gunScript.Spawn(playerGun.gunParent, playerGun);
        playerGun.activeGun.UpdateCamera(camera, virtualCamera);
        playerGun.activeGun.UpdateControllers(starterAssets, firstPersonController);
        playerScript.bulletIconsScript.UpdateBulletAmount();
        playerGun.activeGun.onShoot += playerScript.bulletIconsScript.RemoveBullet;
    }
}
