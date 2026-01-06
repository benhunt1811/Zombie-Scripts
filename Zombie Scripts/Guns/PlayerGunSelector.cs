using Cinemachine;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField]
    private GunType gunType;
    [SerializeField]
    public Transform gunParent;
    [SerializeField]
    private List<GunScriptableObject> guns;
    public GameObject currentGunObject;
    private PlayerAction playerAction;
    private PlayerScript playerScript;

    [Header("Camera Settings")]
    public Camera camera;
    public CinemachineVirtualCamera virtualCamera;


    [Header("Runtime Filled")]
    public GunScriptableObject activeGun;


    [Header("Input Components")]
    private StarterAssetsInputs inputs;
    private FirstPersonController firstPersonController;

    private void Start()
    {
        inputs = GetComponent<StarterAssetsInputs>();
        firstPersonController = GetComponent<FirstPersonController>();
        playerAction = GetComponent<PlayerAction>();

        GunScriptableObject gun = guns.Find(gun => gun.type == gunType);

        if (gun == null)
        {
            return;
        }

        activeGun = gun;
        
        currentGunObject = gun.Spawn(gunParent, this);
        // Assigns controller and camera
        gun.UpdateControllers(inputs, firstPersonController);
        gun.UpdateCamera(camera, virtualCamera);

        playerAction.AssignReload();

        // Updates UI
        playerScript = PlayerScript.Instance;
        playerScript.ammoText.text = activeGun._gunAmmo + " / " + activeGun._gunAmmoReserve;
        playerScript.bulletIconsScript.UpdateBulletAmount();
        playerScript.playerGun.activeGun.onShoot += playerScript.bulletIconsScript.RemoveBullet;
    }

    private void OnDestroy()
    {
        playerScript.playerGun.activeGun.onShoot -= playerScript.bulletIconsScript.RemoveBullet;
    }
}
