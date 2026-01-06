using UnityEngine;

public abstract class BreakablePickUps : Breakable
{
    [Header("Objects")]
    public GameObject parent;

    [Header("Values")]
    public float value;
    public VignetteType vignetteType;

    [Header("Sound")]
    public AudioClip breakSound;

    private VignetteControllerScript vignetteController;
    private AudioController audioController;


    private void Start()
    {
        vignetteController = VignetteControllerScript.Instance;
        audioController = AudioController.Instance;  
    }

    // Called when player dashes and collides with object
    public override void Break()
    {        
        if (vignetteController)
        {
            vignetteController.StartVignette(vignetteType, this.gameObject);
        }

        if (audioController)
        {
            audioController.PlayEffect(breakSound);
        }

        UpdateValues();

        parent.SetActive(false);
    }

    public abstract void UpdateValues();
}
