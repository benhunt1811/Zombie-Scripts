using DG.Tweening;
using LibreFracture;
using UnityEngine;

public class BreakableWallScript : Breakable
{
    [Header("Objects")]
    public GameObject Parent;
    public Fracture FracturedObject;
    public GameObject cube;

    [Header("Components")]
    public MeshRenderer Mesh;
    public BoxCollider Box;

    private AudioSource audioSource;
    private AudioController audioController;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioController = AudioController.Instance; 
    }

    public override void Break()
    {
        if (audioController)
        {
            audioController.PlaySoundInWorld(audioSource, audioSource.clip);
        }

        Parent.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<MeshCollider>().enabled = false;
        Box.enabled = false;
        cube.SetActive(true);
        cube.GetComponent<FractureForceScript>().Force();
    }
}
