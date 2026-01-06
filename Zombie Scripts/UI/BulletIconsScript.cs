using UnityEngine;

public class BulletIconsScript : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject bullet;
    public GameObject box;

    [Header("Player")]
    private PlayerScript player;

    private void Start()
    {
        player = PlayerScript.Instance;
    }

    public void UpdateBulletAmount()
    {
        foreach (Transform child in box.transform)
        {
            Destroy(child.gameObject);
        }


        for (var i = 0; i < player.playerGun.activeGun._gunAmmo; i++)
        {
            Instantiate(bullet, box.transform);
        }
    }

    public void RemoveBullet()
    {       
        int index = box.transform.childCount - 1;
        GameObject BulletToDelete = box.transform.GetChild(index).gameObject;
        Destroy(BulletToDelete);
    }
}
