using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeOpenCloseScript : MonoBehaviour
{

    public GameObject volumeObj;

    public Volume volume;

    private Vignette vignette;

    public bool eyesClosed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VolumeProfile proflile = volume.sharedProfile;
        volume.profile.TryGet(out vignette);


        if (eyesClosed)
        {
            StartCoroutine(OpenEyes());
        }

        else
        {
            StartCoroutine(CloseEyes());
        }
    }

    private IEnumerator CloseEyes()
    {
        volumeObj.SetActive(true);

        for (float i = 0f; i <= 1f; i += (Time.deltaTime * 1.0005f) / 5)
        {

            vignette.intensity.value = Mathf.Lerp(0, 1f, i);
            yield return null;
        }

        vignette.intensity.value = 1f;
    }

    private IEnumerator OpenEyes()
    {
        volumeObj.SetActive(true);

        for (float i = 0f; i <= 1f; i += (Time.deltaTime * 1.0005f))
        {

            vignette.intensity.value = Mathf.Lerp(1, 0, i);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }
}