using Cinemachine;
using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;
    [SerializeField]
    private PlayerScript Player;

    private ReloadScriptableObject reloadConfig;

    private float ReloadTimer = 0;
    private float MaxReloadTimer = 2;
    private bool IsReloading;

    private float _reloadTime;
    private float _gunRotSpeed;
    private float _gunResetSpeed;

    public CinemachineVirtualCamera VirtualCamera;
    public GameObject GunHolder;

    public Vector3 GunReloadPosition;
    public Vector3 GunReloadRotation;

    public GameObject SniperVignette;
    public GameObject sniperCrosshair;

    public GameObject crosshair;

    public bool isShootDown;

    private void Start()
    {
        Player.ammoUpdateText.DOFade(0, 0);
    }

    public void AssignReload()
    {
        reloadConfig = GunSelector.activeGun.reloadConfig;

        _reloadTime = reloadConfig.reloadTime;
        _gunRotSpeed = reloadConfig.gunRotSpeed;
        _gunResetSpeed = reloadConfig.gunResetSpeed;
    }

    public void ReloadInput()
    {
        if (IsReloading == false && GunSelector.activeGun.isReloading == false && GunSelector.activeGun._gunAmmo != GunSelector.activeGun._maxGunClip && GunSelector.activeGun._gunAmmoReserve > 0 && Player.isMeleeing == false)
        {
            StartCoroutine(Reload());
        }
    }

    public void AimInput()
    {
        GunSelector.activeGun.AimDownSights(GunSelector.activeGun.zoomFOV);
        VirtualCamera.m_Lens.FieldOfView = GunSelector.activeGun.zoomFOV;

        if (crosshair)
        {
            crosshair.SetActive(false);
        }

        if (GunSelector.activeGun.type == GunType.Sniper)
        {
            SniperVignette.SetActive(true);
            sniperCrosshair.SetActive(true);
        }
    }

    public void AimInputUp()
    {
        GunSelector.activeGun.ResetAimDownSights();
        VirtualCamera.m_Lens.FieldOfView = 40;

        if (crosshair)
        {
            crosshair.SetActive(true);
        }

        if (GunSelector.activeGun.type == GunType.Sniper)
        {
            SniperVignette.SetActive(false);
            sniperCrosshair.SetActive(false);
        }
    }

    public void ShootInput(bool _isShootDown)
    {  
        isShootDown = _isShootDown;
    }

    private void Update()
    {
        if (isShootDown)
        {
            if (GunSelector.activeGun != null && Player.isMeleeing == false)
                GunSelector.activeGun.Tick(Mouse.current.leftButton.isPressed);
                GunSelector.activeGun.Tick(Gamepad.current.rightTrigger.isPressed);
        }
    }

    private IEnumerator Reload()
    {
        GunSelector.activeGun.isReloading = true;
        IsReloading = true;

        GunHolder.transform.DOLocalMove(GunReloadPosition, _gunRotSpeed);
        GunHolder.transform.DOLocalRotate(GunReloadRotation, _gunRotSpeed);

        float CurrentAmmo = GunSelector.activeGun._gunAmmo;       


        yield return new WaitForSeconds(_reloadTime);

        GunSelector.activeGun.Reload();

        GunHolder.transform.DOLocalRotate(new Vector3(0, 0, 0), _gunResetSpeed);
        GunHolder.transform.DOLocalMove(new Vector3(0, 0, 0), _gunResetSpeed).OnComplete(() => IsReloading = false);

        float NewAmmo = GunSelector.activeGun._gunAmmo;

        float AmmoUpdate = NewAmmo - CurrentAmmo;
        Player.ammoUpdateText.text = AmmoUpdate.ToString();
        Player.ammoUpdateText.DOFade(1, 0.5f).OnComplete(() => Player.ammoUpdateText.DOFade(0, 0.5f));
        Player.bulletIconsScript.UpdateBulletAmount();
    }
}
