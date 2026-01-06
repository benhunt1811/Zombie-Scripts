using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Singleton:

// Only 1 instance of this across the project, call via instance
public class VignetteControllerScript: MonoBehaviour
{
    // Standard Singleton pattern
    public static VignetteControllerScript Instance { get; private set; }
    public static bool InstanceFound => Instance != null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        else
            Destroy(this);
    }


    [Header("Vignette Objects")]
    public GameObject healVolume;
    public GameObject timeVolume;
    public GameObject hitVolume;

    [Header("Vignettes")]
    private Vignette healVignette;
    private Vignette timeVignette;
    private Vignette hitVignette;

    [Header("Volumes")]
    public Volume healvolume;
    public Volume timevolume;
    public Volume hitvolume;

    [Header("Speed")]
    private float blendSpeed = 2;



    private void Start()
    {
        // Gets all the volumes
        VolumeProfile proflile = healvolume.sharedProfile;
        healvolume.profile.TryGet(out healVignette);

        proflile = healvolume.sharedProfile;
        timevolume.profile.TryGet(out timeVignette);

        proflile = hitvolume.sharedProfile;
        hitvolume.profile.TryGet(out hitVignette);
    }

    // Swtich statement for playing each vignette
    public void StartVignette(VignetteType Type, GameObject obj)
    {
        VignetteType choice;
        choice = Type;

        switch(choice)
        {
            case VignetteType.Time:
                StartCoroutine(PlayVignette(timeVignette, timeVolume));
                break;

            case VignetteType.Hit:
                StartCoroutine(PlayVignette(hitVignette, hitVolume));
                break;

            case VignetteType.Heal:
                StartCoroutine(PlayVignette(healVignette, healVolume));
                break;
        }
    }

    private IEnumerator PlayVignette(Vignette vignette, GameObject obj)
    {
        obj.SetActive(true);

        for (float i = 0f; i <= 1f; i += Time.deltaTime * blendSpeed)
        {

            vignette.intensity.value = Mathf.Lerp(0, 0.5f, i);
            yield return null;
        }

        vignette.intensity.value = 0.5f;

        yield return new WaitForSeconds(1.5f);

        for (float i = 0f; i <= 1f; i += Time.deltaTime * blendSpeed)
        {

            vignette.intensity.value = Mathf.Lerp(0.5f, 0, i);
            yield return null;
        }

        vignette.intensity.value = 0;

        obj.SetActive(false);
    }
}