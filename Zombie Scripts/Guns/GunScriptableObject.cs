using Cinemachine;
using DG.Tweening;
using FirstGearGames.SmoothCameraShaker;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    [Header("Gun Settings")]
    public GunType type;
    public string gunName;
    public GameObject modelPrefab;
    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    [Header("Spawn Settings")]
    public Vector3 spawnPoint;
    public Vector3 aimDownSightsPoint;
    public Vector3 spawnRotation;

    [Header("Config Settings")]
    public DamageConfigScriptableObject damageConfig;
    public ShootConfigurationScriptableObject shootConfig;
    public TrailConfigScriptableObject trailConfig;
    public ReloadScriptableObject reloadConfig;
    public GunEffectsScriptableObject gunEffectsConfig;
    public GunSoundEffectsConfig gunSoundEffectsConfig;

    [HideInInspector]
    [Header("Gun Ammo Values")]
    public int _gunAmmo;
    public int _gunAmmoReserve;
    public int _maxGunClip;

    [Header("Shoot Info")]
    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool lastFrameWantedToShoot;
    public bool isReloading;
    private float gunSpreadDampener = 20;

    [Header("Effects")]
    private ParticleSystem shootSystem;
    private ObjectPool<TrailRenderer> trailPool;
    private ParticleSystem hitEffect;
    private GameObject bulletHole;
    private float impactOffset = 0.01f;
    private float impactDeathTime = 3;

    [Header("Camera")]
    private Camera activeCamera;
    private CinemachineVirtualCamera virtualCamera;

    [Header("Texts")]
    private GameObject bodyshotText;
    private GameObject headshotText;

    [Header("Zoom")]
    private float defaultFOV = 40;
    public float zoomFOV;
    public bool isZoomedIn;
    private float zoomInTime = 0.2f;

    [Header("Player Movement and Inputs")]
    private StarterAssetsInputs _inputs;
    private FirstPersonController _firstPersonController;

    public delegate void OnShoot();
    public event OnShoot onShoot;

    public delegate void OnEnemyHit(float Time);
    public static event OnEnemyHit onEnemyHit;

    [Header("Sounds")]
    private GunAudioManager gunAudioManager;
    private AudioController audioController;
    


    // For spawning in the gun
    public GameObject Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.activeMonoBehaviour = ActiveMonoBehaviour;

        // in editor these will not be properly reset, in build it's fine
        lastShootTime = 0;
        stopShootingTime = 0;
        initialClickTime = 0;
        isReloading = false;

        // Setting all ammo values 
        _gunAmmo = reloadConfig.gunAmmo;
        _gunAmmoReserve = reloadConfig.gunAmmoReserve;
        _maxGunClip = reloadConfig.maxGunClip;

        // Assigning the effects
        hitEffect = gunEffectsConfig.hitEffect;
        bulletHole = gunEffectsConfig.bulletHole;
        bodyshotText = gunEffectsConfig.bodyshotText;
        headshotText = gunEffectsConfig.headshotText;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        // Event for updating UI for ammo
        reloadConfig.UpdateAmmo();
       

        // Spawns gun and sets location
        model = Instantiate(modelPrefab);
        model.transform.SetParent(Parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        // Gets the muzzle flash attached to the gun model
        shootSystem = model.GetComponentInChildren<ParticleSystem>();
        gunAudioManager = model.GetComponentInChildren<GunAudioManager>();

        return model.gameObject;
    }

    public void UpdateCamera(Camera ActiveCamera, CinemachineVirtualCamera VirtualCamera)
    {
        this.activeCamera = ActiveCamera;
        this.virtualCamera = VirtualCamera;
    }

    public void UpdateControllers(StarterAssetsInputs inputs, FirstPersonController controller)
    {
        _inputs = inputs;
        _firstPersonController = controller;
    }

    public void Shoot()
    {
        // For getting the bullet spread when player stops and shoots again
        if (Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime && _gunAmmo > 0 && isReloading == false)
        {
            float lastDuration = Mathf.Clamp(
                (stopShootingTime - initialClickTime),
                0,
                shootConfig.maxSpreadTime
            );
            float lerpTime = (shootConfig.recoilRecoverySpeed - (Time.time - stopShootingTime))
                / shootConfig.recoilRecoverySpeed;

            initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        // Creates camera shake and plays the shoot particle effect
        if (Time.time > shootConfig.fireRate + lastShootTime && _gunAmmo > 0 && isReloading == false)
        {
            lastShootTime = Time.time;
            shootSystem.Play();

            audioController = AudioController.Instance;

            int random = Random.Range(0, gunSoundEffectsConfig.shootClips.Length);            
            audioController.PlaySoundInWorld(gunAudioManager.shootAudioSource, gunSoundEffectsConfig.shootClips[random]);

            CameraShakerHandler.Shake(shootConfig.shootShakeData);

            // Gets spread amount and shoot direction 
            Vector3 spreadAmount = shootConfig.GetSpread(isZoomedIn, _inputs, _firstPersonController, Time.time - initialClickTime);
            model.transform.localEulerAngles += spreadAmount;
            Vector3 shootDirection = Vector3.zero;

            // Used for getting shoot type from the gun
            if (shootConfig.shootType == ShootType.FromGun)
            {
                shootDirection = shootSystem.transform.forward;
            }

            // Used for getting the shoot type from the players camera
            else
            {
                shootDirection = activeCamera.transform.forward
                    + activeCamera.transform.TransformDirection(spreadAmount / gunSpreadDampener);
            }

            // Shooting raycast for the gun
            if (Physics.Raycast(
                    activeCamera.transform.position,
                    shootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    shootConfig.hitMask
                ))
            // Shoots trail for bullet
            {
                activeMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        shootSystem.transform.position,
                        hit.point,
                        hit
                    )                   
                );
                Instantiate(hitEffect, hit.point, Quaternion.identity);

            }
            else
            {
                activeMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        shootSystem.transform.position,
                        shootSystem.transform.position + (shootDirection * trailConfig.missDistance),
                        new RaycastHit()
                    )
                );
            }

            // Updates ammo ammounts
            _gunAmmo -= 1;
            onShoot.Invoke();
            reloadConfig.UpdateAmmo(); 
        }
    }

    public void Tick(bool WantsToShoot)
    {
        // Sets gun models rotation for bullet spread
        model.transform.localRotation = Quaternion.Lerp(
            model.transform.localRotation,
            Quaternion.Euler(spawnRotation),
            Time.deltaTime * shootConfig.recoilRecoverySpeed
        );

        if (WantsToShoot)
        {
            lastFrameWantedToShoot = true;
            Shoot();         
        }

        if (!WantsToShoot && lastFrameWantedToShoot)
        {
            stopShootingTime = Time.time;
            lastFrameWantedToShoot = false;
        }
    }

    public Vector3 GetRaycastOrigin()
    {
        Vector3 origin = shootSystem.transform.position;
        return origin;
    }

    public Vector3 GetGunForward()
    {
        return model.transform.forward;
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; // avoid position carry-over from last frame if reused

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        Debug.Log(StartPoint);
        Debug.Log(EndPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if (Hit.collider != null)
        {
            Debug.Log(Hit.collider.transform.parent);
            if (Hit.collider.transform.root.TryGetComponent<Damageable>(out Damageable damagable))
            {
                bool wasHeadShot;
                if (Hit.collider.tag == "Head")
                {
                    onEnemyHit.Invoke(shootConfig.headHitTimerAdd);
                    DisplayHitText(Hit, shootConfig.headHitTimerAdd, headshotText);
                    wasHeadShot = true;
                }

                else
                {
                    onEnemyHit.Invoke(shootConfig.bodyHitTimerAdd);
                    DisplayHitText(Hit, shootConfig.bodyHitTimerAdd, bodyshotText);
                    wasHeadShot = false;
                }
                damagable.TakeDamage(damageConfig.GetDamage(wasHeadShot, distance));
                damagable.StumbleCheck(shootConfig.bulletPower);
            }

            else 
            {
                GameObject impact = Instantiate(bulletHole, Hit.point, Quaternion.LookRotation(Hit.normal));
                Vector3 forwardVector = impact.transform.forward;
                impact.transform.Translate(forwardVector * impactOffset, Space.World);

                Destroy(impact, impactDeathTime);
            }
        }


        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private void DisplayHitText(RaycastHit Hit, float TimeAdded, GameObject Text)
    {
        Text.GetComponent<TextMeshPro>().text = "+ " + TimeAdded.ToString();
        Quaternion textRot = Quaternion.LookRotation(Hit.normal);
        Quaternion textOffsetRot = Quaternion.Euler(0, 180, 0);
        textRot *= textOffsetRot;
        GameObject HitText = Instantiate(Text, Hit.point, textRot);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.colour;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public void Reload()
    {
        // Calls the reload function from the reload config
        // Results are then passed into ammo values here
        (_gunAmmo, _gunAmmoReserve) = reloadConfig.Reload(_gunAmmo, _gunAmmoReserve);
        isReloading = false;
        reloadConfig.UpdateAmmo();

        audioController = AudioController.Instance;
        audioController.PlayEffect(gunSoundEffectsConfig.reloadSound);
    }

    public void AimDownSights(float FOV)
    {
        model.transform.DOLocalMove(aimDownSightsPoint, zoomInTime);
        isZoomedIn = true;


    }

    public void ResetAimDownSights()
    {
        model.transform.DOLocalMove(spawnPoint, zoomInTime);       
        isZoomedIn = false;
    }
}