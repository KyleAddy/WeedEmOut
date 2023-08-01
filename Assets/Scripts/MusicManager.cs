using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public static AudioSource audioSource;

    [SerializeField]
    FloatVariable musicVolumeVar;

    [SerializeField]
    private AudioClip ambientMusic;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance = this;
        }

        audioSource = GetComponent<AudioSource>();

        UpdateVolume();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RepeatAudioClip());
    }

    public void PlayClip(Enumes.MusicClips _clip)
    {
        switch (_clip)
        {
            case Enumes.MusicClips.ambientMusic:
                audioSource.PlayOneShot(ambientMusic);
                break;
        }
    }

    public void UpdateVolume()
    {
        audioSource.volume = musicVolumeVar.GetValue();
    }

    private IEnumerator RepeatAudioClip()
    {

        // Play the audio clip again
        PlayClip(Enumes.MusicClips.ambientMusic);

        yield return new WaitForSeconds(ambientMusic.length - 1f);


        // Repeat the coroutine to create a loop
        StartCoroutine(RepeatAudioClip());
    }
}
