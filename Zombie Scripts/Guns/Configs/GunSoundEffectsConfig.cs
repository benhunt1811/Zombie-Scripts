using UnityEngine;


[CreateAssetMenu(fileName = "Gun Sound Effects Config", menuName = "Guns/Gun Sound Effects Config", order = 5)]
public class GunSoundEffectsConfig : ScriptableObject
{
    public AudioClip[] shootClips;

    public AudioClip reloadSound;
    public AudioSource gunPickUpSound;
}
