using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BreakableHealthScript : BreakablePickUps
{

    public override void UpdateValues()
    {
        // Calls player heal script to update value
        PlayerScript.Instance.playerHealth.Heal(((int)value));
    }
}