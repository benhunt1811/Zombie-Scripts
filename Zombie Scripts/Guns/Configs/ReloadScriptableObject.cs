using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Reload Config", menuName = "Guns/Reload Config", order = 1)]
public class ReloadScriptableObject : ScriptableObject
{
    [Header("Gun Ammo Values")]
    public int gunAmmo;
    public int gunAmmoReserve;
    public int maxGunClip;

    [Header("Reload Time Amounts")]
    public float reloadTimeLimit;
    public float reloadTime;

    [Header("Reload Rotation Speeds")]
    public float gunRotSpeed;
    public float gunResetSpeed;

    public delegate void OnReload();
    public static event OnReload onReload;

    public (int, int) Reload(int currentAmmo, int currentAmmoReserve)
    {
        int ReloadAmount = maxGunClip - currentAmmo;
        ReloadAmount = (currentAmmoReserve - ReloadAmount) >= 0 ? ReloadAmount : currentAmmoReserve;
        currentAmmo += ReloadAmount;
        currentAmmoReserve -= ReloadAmount;
        

        onReload.Invoke();

        return (currentAmmo, currentAmmoReserve);
    }

    public void UpdateAmmo()
    {
        onReload.Invoke();
    }
}
