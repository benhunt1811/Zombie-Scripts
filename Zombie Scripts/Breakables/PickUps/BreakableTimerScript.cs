using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;

public class BreakableTimerScript : BreakablePickUps
{

    public override void UpdateValues()
    {
        // Updates timer values
        PlayerScript.Instance.timer.AddTime(value);
        PlayerScript.Instance.ShowAddedTime(value);
    }
}
