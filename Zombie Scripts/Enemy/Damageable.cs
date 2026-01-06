using UnityEngine;

public interface Damageable
{
    [Header("Heal Values")]
    public int currentHealth { get;}
    public int maxHealth { get;}

    public delegate void TakeDamageEvent(int Damage);
    public event TakeDamageEvent onTakeDamage;

    public delegate void DeathEvent(Vector3 Position);
    public event DeathEvent onDeath;

    public void TakeDamage(int Damage);

    public void StumbleCheck(int Power);
}
