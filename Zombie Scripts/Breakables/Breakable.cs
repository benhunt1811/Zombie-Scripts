using UnityEngine;

public abstract class Breakable : MonoBehaviour, IDashTarget
{
    // Function for when an object breaks, updating given values
    public abstract void Break();

    public void OnDashHit()
    {
        Break();
    }
}
