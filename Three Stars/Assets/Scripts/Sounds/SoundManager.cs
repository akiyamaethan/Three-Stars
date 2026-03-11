using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music Playlist")]
    [SerializeField] private AudioClip[] backgroundTracks;
    [SerializeField] private bool playMusicOnStart = true;

    [Header("Shop Ambience")]
    [SerializeField] private AudioClip combinedAmbienceLong;
    [SerializeField] private float musicVolume = 0.35f;
    [SerializeField] private float ambienceVolume = 0.25f;
    [SerializeField] private float fadeDuration = 1.25f;

    [Header("SFX Settings")]
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.6f;
    [SerializeField] private float startupSfxBlockTime = 0.25f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip clickRankSuit;
    [SerializeField] private AudioClip editedBowlTossing;
    [SerializeField] private AudioClip editedEthanCutting;
    [SerializeField] private AudioClip editedShortSizzle;
    [SerializeField] private AudioClip editedBurgerDown;

    private int currentTrackIndex = 0;
    private Coroutine musicRoutine;
    private Coroutine ambienceRoutine;
    private bool isPlaylistActive = true;
    private bool allowSfxPlayback = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("[SoundManager] Duplicate SoundManager found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[SoundManager] Awake complete. Singleton assigned.");
    }

    private void Start()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("[SoundManager] Music source is not assigned.");
        }
        else
        {
            musicSource.playOnAwake = false;
            musicSource.loop = false;
            musicSource.volume = musicVolume;
        }

        if (ambienceSource == null)
        {
            Debug.LogWarning("[SoundManager] Ambience source is not assigned.");
        }
        else
        {
            ambienceSource.playOnAwake = false;
            ambienceSource.loop = true;
            ambienceSource.volume = 0f;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] SFX source is not assigned.");
        }
        else
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.volume = 1f; // keep source at full, scale via PlayOneShot master
        }

        if (playMusicOnStart)
        {
            isPlaylistActive = true;
            StartMusicPlaylist();
        }
        else
        {
            isPlaylistActive = false;
            Debug.Log("[SoundManager] playMusicOnStart is false. Playlist idle.");
        }

        StartCoroutine(EnableSfxAfterDelay());
    }

    private IEnumerator EnableSfxAfterDelay()
    {
        allowSfxPlayback = false;
        yield return new WaitForSeconds(startupSfxBlockTime);
        allowSfxPlayback = true;
        Debug.Log("[SoundManager] SFX playback enabled after startup delay.");
    }

    private void Update()
    {
        if (musicSource == null || backgroundTracks == null || backgroundTracks.Length == 0)
        {
            return;
        }

        if (isPlaylistActive && !musicSource.isPlaying)
        {
            Debug.Log("[SoundManager] Music source stopped. Advancing playlist.");
            PlayNextTrack();
        }
    }

    public void StartMusicPlaylist()
    {
        if (backgroundTracks == null || backgroundTracks.Length == 0)
        {
            Debug.LogWarning("[SoundManager] No background tracks assigned.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogWarning("[SoundManager] Cannot start playlist. Music source is null.");
            return;
        }

        currentTrackIndex = Mathf.Clamp(currentTrackIndex, 0, backgroundTracks.Length - 1);
        isPlaylistActive = true;
        PlayTrack(currentTrackIndex);
    }

    public void PlayTrack(int trackIndex)
    {
        if (backgroundTracks == null || backgroundTracks.Length == 0)
        {
            Debug.LogWarning("[SoundManager] Cannot play track. Background track list is empty.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogWarning("[SoundManager] Cannot play track. Music source is null.");
            return;
        }

        if (trackIndex < 0 || trackIndex >= backgroundTracks.Length)
        {
            Debug.LogWarning($"[SoundManager] Track index {trackIndex} is out of range.");
            return;
        }

        currentTrackIndex = trackIndex;
        musicSource.clip = backgroundTracks[currentTrackIndex];
        musicSource.loop = false;
        musicSource.volume = musicVolume;
        musicSource.Play();

        Debug.Log($"[SoundManager] Playing music track: {musicSource.clip.name}");
    }

    public void PlayNextTrack()
    {
        if (backgroundTracks == null || backgroundTracks.Length == 0)
        {
            Debug.LogWarning("[SoundManager] Cannot play next track. Background track list is empty.");
            return;
        }

        currentTrackIndex++;

        if (currentTrackIndex >= backgroundTracks.Length)
        {
            currentTrackIndex = 0;
        }

        PlayTrack(currentTrackIndex);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!allowSfxPlayback)
        {
            Debug.Log("[SoundManager] Blocked SFX during startup.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] Tried to play a null SFX clip.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] Tried to play SFX, but SFX source is null.");
            return;
        }

        float finalVolume = Mathf.Clamp01(volume * sfxVolume);
        Debug.Log($"[SoundManager] Playing SFX: {clip.name} at volume {finalVolume}");
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void PlayRankSuitClick()
    {
        PlaySFX(clickRankSuit, 1f);
    }

    public void PlayDiscard()
{
    if (editedBowlTossing == null && editedEthanCutting == null)
    {
        Debug.LogWarning("[SoundManager] No discard clips assigned.");
        return;
    }

    AudioClip chosen = null;

    if (editedBowlTossing != null && editedEthanCutting != null)
    {
        int roll = Random.Range(0, 2); // 0 or 1
        chosen = (roll == 0) ? editedBowlTossing : editedEthanCutting;
        Debug.Log($"[SoundManager] Discard roll: {roll}, chosen clip: {chosen.name}");
    }
    else if (editedBowlTossing != null)
    {
        chosen = editedBowlTossing;
        Debug.Log("[SoundManager] Only EditedBowlTossing assigned for discard.");
    }
    else
    {
        chosen = editedEthanCutting;
        Debug.Log("[SoundManager] Only EditedEthanCutting assigned for discard.");
    }

    PlaySFX(chosen, 1f);
}

    public void PlayHand()
    {
        PlaySFX(editedShortSizzle, 1f);
    }

    public void PlayNextShift()
    {
        PlaySFX(editedBurgerDown, 1f);
    }

    public void FadeToShopAmbience()
    {
        if (combinedAmbienceLong == null)
        {
            Debug.LogWarning("[SoundManager] Cannot fade to shop ambience. combinedAmbienceLong is null.");
            return;
        }

        if (ambienceSource == null || musicSource == null)
        {
            Debug.LogWarning("[SoundManager] Cannot fade to shop ambience. One or more audio sources are null.");
            return;
        }

        Debug.Log("[SoundManager] Fading to shop ambience.");
        isPlaylistActive = false;

        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
        }

        if (ambienceRoutine != null)
        {
            StopCoroutine(ambienceRoutine);
        }

        ambienceSource.clip = combinedAmbienceLong;
        ambienceSource.loop = true;

        if (!ambienceSource.isPlaying)
        {
            ambienceSource.Play();
        }

        musicRoutine = StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, 0f, fadeDuration, true));
        ambienceRoutine = StartCoroutine(FadeAudioSource(ambienceSource, ambienceSource.volume, ambienceVolume, fadeDuration));
    }

    public void FadeBackToMusic()
    {
        if (musicSource == null || ambienceSource == null)
        {
            Debug.LogWarning("[SoundManager] Cannot fade back to music. One or more audio sources are null.");
            return;
        }

        if (backgroundTracks == null || backgroundTracks.Length == 0)
        {
            Debug.LogWarning("[SoundManager] Cannot fade back to music. No background tracks assigned.");
            return;
        }

        Debug.Log("[SoundManager] Fading back to music.");
        isPlaylistActive = true;

        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
        }

        if (ambienceRoutine != null)
        {
            StopCoroutine(ambienceRoutine);
        }

        if (musicSource.clip == null)
        {
            PlayTrack(currentTrackIndex);
            musicSource.volume = 0f;
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
            musicSource.volume = 0f;
        }

        musicRoutine = StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, musicVolume, fadeDuration));
        ambienceRoutine = StartCoroutine(FadeAudioSource(ambienceSource, ambienceSource.volume, 0f, fadeDuration, true));
    }

    private IEnumerator FadeAudioSource(AudioSource source, float startVolume, float targetVolume, float duration, bool stopWhenDone = false)
    {
        if (source == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            source.volume = targetVolume;

            if (stopWhenDone && Mathf.Approximately(targetVolume, 0f))
            {
                source.Stop();
            }

            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            source.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        source.volume = targetVolume;

        if (stopWhenDone && Mathf.Approximately(targetVolume, 0f))
        {
            source.Stop();
        }
    }
}