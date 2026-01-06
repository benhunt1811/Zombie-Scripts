using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfigScriptableObject : ScriptableObject
{
    [Header("Damage Curve")]
    public MinMaxCurve DamageCurve;

    [Header("Headshot Value")]
    public float headShotMultiplier;

    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    // Uses the curve along with distance to calculate damage
    public int GetDamage(bool wasHeadShot, float Distance = 0)
    {
        int damage = Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));

        if (wasHeadShot)
        {
            damage *= (int)headShotMultiplier;
        }
        return damage;
    }
}