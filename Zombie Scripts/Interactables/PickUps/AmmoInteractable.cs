using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.VFX;

public class AmmoInteractable : Interactable
{
    [Header("Gun Settings")]
    public GunType gun;
    public int ammoAmount;

    [Header("Rotations")]
    public GameObject lid;
    private Vector3 lidRotate = new Vector3(-120, 0, 0);
    private float rotateDuration = 1.5f;

    [Header("Dissolve Objects")]
    public DissolveController dissolveBox;
    public DissolveController dissolveLid;
    private VisualEffect VFX;

    [Header("Light Settings")]
    public Light light;
    private float baseLightIntensity = 5;
    private float interactlightIntensity = 15;

    [Header("Player")]
    private PlayerScript player;

    [Header("Sound")]
    [SerializeField] private AudioClip lookAtSound;
    [SerializeField] private AudioClip openAmmoSound;
    [SerializeField] private AudioClip dissolveSound;
    private AudioController audioController;
    private AudioSource audioSource;

    private bool isLookingAt;

    private void Start()
    {
        player = PlayerScript.Instance;
        audioController = AudioController.Instance;
        audioSource = GetComponent<AudioSource>();

        isLookingAt = false;
    }


    public override void Interact()
    {
        if (player.playerGun.activeGun.type == gun)
        {
            player.playerGun.activeGun._gunAmmoReserve += ammoAmount;
            player.playerGun.activeGun.reloadConfig.UpdateAmmo();

            lid.transform.DOLocalRotate(new Vector3 (-120, 0, 0), rotateDuration).OnComplete(() => StartDissolve());

            if (audioController)
            {
                audioController.PlaySoundInWorld(audioSource, openAmmoSound);
            }
        }
    }

    public override void HideInteractable()
    {
        if (isLookingAt)
        {
            light.intensity = baseLightIntensity;
        }

        isLookingAt = false;
    }

    public override void ShowInteractable()
    {   
        if (isLookingAt == false)
        {
            light.intensity = interactlightIntensity;

            if (audioController)
            {
                audioController.PlayEffect(lookAtSound);
            }

            isLookingAt = true;
        }
    }

    private void StartDissolve()
    {
        StartCoroutine(dissolveBox.Dissolve());
        StartCoroutine(dissolveLid.Dissolve());

        if (audioController)
        {
            audioController.PlaySoundInWorld(audioSource, dissolveSound);
        }
    }
}
