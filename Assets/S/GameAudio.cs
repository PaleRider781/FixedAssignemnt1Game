using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
        musicSource.clip = backgroundMusic;

        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void PlaySfx(AudioClip clip, Vector3 position, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("One Shot Audio");
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = Mathf.Clamp01(soundVolume * volumeScale);
        source.pitch = pitch;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, clip.length / Mathf.Max(0.01f, pitch));
    }
}