using UnityEngine;
using System.Collections;

public class FlickerControl : MonoBehaviour
{

    public bool isFlickering = false;
    private float timeDelay;

    public Renderer rendererObj;

    private Light lightObj;

    private float minFlickerValue = 0.1f;
    private float maxFlickerValue = 0.5f;

    private AudioSource audio;

    public int renderIndex = 1;

    private void Start()
    {
        lightObj = GetComponent<Light>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isFlickering == false)
        {
            StartCoroutine(FlickeringLight());
        }
    }

    IEnumerator FlickeringLight()
    {
        isFlickering = true;
        lightObj.enabled = false;
        audio.Pause();
        if (rendererObj != null)
        {
            rendererObj.materials[renderIndex].SetFloat("Emission_Intensity", 0);
        }
        timeDelay = Random.Range(minFlickerValue, maxFlickerValue);
        yield return new WaitForSeconds(timeDelay);
        lightObj.enabled = true;
        audio.Play();
        if (rendererObj != null)
        {
            rendererObj.materials[renderIndex].SetFloat("Emission_Intensity", 1);
        }
        timeDelay = Random.Range(minFlickerValue, maxFlickerValue);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;

    }
}
