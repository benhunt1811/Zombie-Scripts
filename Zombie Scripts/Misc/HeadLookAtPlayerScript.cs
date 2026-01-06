using UnityEngine;

public class HeadLookAtPlayerScript : MonoBehaviour
{
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerScript.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) transform.LookAt(player.transform.position);
    }
}
