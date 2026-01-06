using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isAlive => health > 0;

    private int _health = 10;
    private int maxHealth = 10;

    public VignetteType vignetteType;

    private VignetteControllerScript vignetteController;

    [SerializeField] private AudioClip playerHurtSound;
    private AudioController audioController;

    private GameStateScript gameState;
    private PlayerScript player;

    public int health
    {
        get { return _health; }
        set
        {
            value = Mathf.Clamp(value, 0, maxHealth);
            if (_health == value) return;

            _health = value;

            OnHealthChange?.Invoke(_health);

            if (_health <= 0) OnDeath();
        }
    }

    private void Start()
    {
        vignetteController = VignetteControllerScript.Instance;
        audioController = AudioController.Instance;

        gameState = GameStateScript.Instance;
        player = PlayerScript.Instance;
    }

    private void OnDeath()
    {
        gameState.GameLoss("Better Luck Next Time", player.FormatToMinSec(), player.killCount);
        this.gameObject.SetActive(false);
    }

    public Action<int> OnHealthChange;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (audioController)
        {
            audioController.PlayEffect(playerHurtSound);
        }

        if (vignetteController)
        {
            vignetteController.StartVignette(vignetteType, this.gameObject);
        }
    }       

    public void Heal(int healAmount) => health += healAmount;
}
