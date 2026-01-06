using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Header("Time Values")]
    public float startTime;
    public float timeLeft;

    [Header("UI Elements")]
    public TMP_Text timerText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLeft = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            
            if (timeLeft > 60)
            {
                FormatToMinSec();
            }

            else
            {
                timerText.text = timeLeft.ToString("0.00");
            }                
        }

        else
        {
            // Timer runs out stuff happens here
        }
    }

    void FormatToMinSec()
    {
        float mins = Mathf.FloorToInt(timeLeft / 60);
        float secs = Mathf.FloorToInt(timeLeft % 60);

        timerText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }

    public void AddTime(float time)
    {
        timeLeft += time;
    }
}
