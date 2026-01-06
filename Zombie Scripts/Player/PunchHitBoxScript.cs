using UnityEngine;

public class PunchHitBoxScript : MonoBehaviour
{
    private float PushBackPower = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Enemy")
        {
            other.GetComponent<Rigidbody>().AddForce(this.transform.forward * PushBackPower, ForceMode.Impulse);
        }
    }
}
