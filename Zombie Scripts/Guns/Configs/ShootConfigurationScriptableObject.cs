using FirstGearGames.SmoothCameraShaker;
using StarterAssets;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]

public class ShootConfigurationScriptableObject : ScriptableObject
{
    public LayerMask hitMask;
    public float fireRate = 0.25f;
    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;
    public float zoomFOV;
    public int bulletPower;
    public BulletSpreadType spreadType = BulletSpreadType.Simple;

    [Header("Spread Settings")]
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 zoomSpread = new Vector3(0.1f, 0.1f, 0.1f);
    public float walkSpreadMultiplier;
    public float runSpreadMultiplier;
    public float midAirMultiplier;

    [Header("Texture-Based Spread")]
    [Range(0.001f, 5f)]
    public float spreadMultiplier = 0.1f;
    public Texture2D spreadTexture;
    public ShootType shootType = ShootType.FromGun;

    public ShakeData shootShakeData;

    [Header("Bullet Hit Effects")]
    public GameObject bulletHole;
    public ParticleSystem hitEffect;

    [Header("Time settings")]
    public float bodyHitTimerAdd;
    public float headHitTimerAdd;




    // For simple spread
    public Vector3 GetSpread(bool isZoomedIn, StarterAssetsInputs inputs, FirstPersonController controller, float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;
        Vector3 SpreadValues = Vector3.zero;


        // Used to increase or decrease spread based on movement
        if (spreadType == BulletSpreadType.Simple)
        {
            if (isZoomedIn)
            {
                SpreadValues = zoomSpread;
            }

            else
            {
                SpreadValues = this.spread;
            }

            if ((inputs.move.x != 0 || inputs.move.y != 0) && inputs.sprint == false)
            {
                SpreadValues *= walkSpreadMultiplier;
            }

            if (inputs.sprint)
            {
                SpreadValues *= runSpreadMultiplier;
            }

            if (controller.Grounded != true)
            {
                SpreadValues *= midAirMultiplier;
            }

                // Used to smoothly move between the values
                spread = Vector3.Lerp(
                    Vector3.zero,
                    new Vector3(
                        Random.Range(-SpreadValues.x, SpreadValues.x),
                        Random.Range(-SpreadValues.y, SpreadValues.y),
                        Random.Range(-SpreadValues.z, SpreadValues.z)
                    ),
                    Mathf.Clamp01(shootTime / maxSpreadTime)

                );
        }
        else if (spreadType == BulletSpreadType.TextureBased)
        {
            Vector3 direction = GetTextureDirection(shootTime);

            spread = direction * spreadMultiplier;
        }

        return spread;
    }

    private Vector3 GetTextureDirection(float ShootTime)
    {
        Vector2 halfSize = new Vector2(spreadTexture.width / 2f, spreadTexture.height / 2f);

        int halfSquareExtents = Mathf.CeilToInt(
            Mathf.Lerp(
                0.01f,
                halfSize.x,
                Mathf.Clamp01(ShootTime / maxSpreadTime)
            )
        );

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColours = spreadTexture.GetPixels(
            minX,
            minY,
            halfSquareExtents * 2,
            halfSquareExtents * 2

        );

        float[] coloursAsGrey = System.Array.ConvertAll(sampleColours, (Color) => Color.grayscale);
        float totalGreyValue = coloursAsGrey.Sum();

        float grey = Random.Range(0, totalGreyValue);
        int i = 0;
        for (; i < coloursAsGrey.Length; i++)
        {
            grey -= coloursAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new Vector2(x, y);
        Vector2 direction = (targetPosition - halfSize) / halfSize.x; 

        return direction;
    }
}
