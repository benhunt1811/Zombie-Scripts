using UnityEngine;

[CreateAssetMenu(fileName = "Gun Effects Config", menuName = "Guns/Gun Effects Config", order = 4)]
public class GunEffectsScriptableObject : ScriptableObject
{    
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public GameObject bulletHole;
    public GameObject bodyshotText;
    public GameObject headshotText;
}
