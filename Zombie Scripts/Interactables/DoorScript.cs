using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class DoorScript : Interactable
{
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool isOpen = false;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private Coroutine _currentCoroutine;

    [Header("Sound")]
    private AudioController audioController;
    private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        audioController = AudioController.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator ToggleDoor()
    {
        Quaternion targetRotation = isOpen ? _closedRotation : _openRotation;
        AudioClip audioToPlay = isOpen ? closeSound : openSound;

        if (audioController)
        {
            audioController.PlaySoundInWorldSingle(audioSource, audioToPlay);
        }

        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public override void Interact()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(ToggleDoor());
    }

    public override void ShowInteractable()
    {
        Debug.Log("FOUND");
    }

    public override void HideInteractable()
    {
        Debug.Log("NOT FOUND");
    }
}
