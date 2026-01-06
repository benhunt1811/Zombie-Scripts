using UnityEngine;

public class EnemyHitboxScript : MonoBehaviour
{
    public int damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageAmount);
            this.gameObject.SetActive(false);
        }
    }
}
