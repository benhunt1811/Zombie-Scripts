using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{

    [Header("Sliders")]
    public Slider healthSlider;
    public Slider easeSlider;

    [Header("Speed")]
    private float lerpSpeed = 0.01f;

    [Header("Components")]
    private PlayerHealth playerHealthScript;

    void Start()
    {
        playerHealthScript = PlayerScript.Instance.gameObject.GetComponent<PlayerHealth>();
        if (playerHealthScript)
        {
            // Adds update health to OnHealthChange
            // Invokes when player gets hit, so health changes
            playerHealthScript.OnHealthChange += UpdateHealth;
        }
    }

    private void UpdateHealth(int health)
    {
        healthSlider.value = health;
    }

    private void Update()
    {
        if (healthSlider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, healthSlider.value, lerpSpeed);
        }
    }
}
