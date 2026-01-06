using UnityEngine;

public class AudioController : MonoBehaviour
{

    public static AudioController Instance { get; private set; }
    public static bool InstanceFound => Instance != null;

    public AudioClip music;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        else
            Destroy(this);
    }

    private AudioSource effectAudioSource;
    
    void Start()
    {
        effectAudioSource = GetComponent<AudioSource>();

        PlayEffect(music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundInWorld(AudioSource audio, AudioClip clip)
    {
        if (audio != null && clip != null)
        {
            audio.PlayOneShot(clip);
        }
    }

    public void PlayEffect(AudioClip clip)
    {
        effectAudioSource.PlayOneShot(clip);
    }

    public void PlaySoundInWorldSingle(AudioSource audio, AudioClip clip)
    {
        if (audio != null && clip != null)
        {
            audio.clip = clip;
            audio.Play();
        }
    }
}