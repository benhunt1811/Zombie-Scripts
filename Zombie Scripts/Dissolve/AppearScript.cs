using UnityEngine;
using UnityEngine.VFX;

public class AppearScript : MonoBehaviour
{
    private DissolveController Dissolve;
    private VisualEffect VFXGraph;

    [SerializeField] private AudioClip appearSound;
    private AudioController audioController;


    void Start()
    {
        Dissolve = GetComponentInChildren<DissolveController>();
        VFXGraph = GetComponentInChildren<VisualEffect>();

        StartCoroutine(Dissolve.Appear());
        VFXGraph.Play();

        audioController = AudioController.Instance;
        audioController.PlayEffect(appearSound);
    }
}