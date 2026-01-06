using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DashSliderEaseScript : MonoBehaviour
{

    public Slider DashSlider;
    public Slider EaseSlider;

    private float LerpSpeed = 0.01f;



    // Update is called once per frame
    void Update()
    {
        if (DashSlider.value != EaseSlider.value)
        {
            EaseSlider.value = Mathf.Lerp(EaseSlider.value, DashSlider.value, LerpSpeed);
        }
    }
}
