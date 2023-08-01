using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public static AudioSource audioSource;

    [SerializeField]
    FloatVariable fxVolumeVar;


    [SerializeField]
    private AudioClip pop;

    [SerializeField]
    private AudioClip beep;

    [SerializeField]
    private AudioClip spray;

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

    }

    public void PlayClip(Enumes.AudioClips _clip)
    {
        switch (_clip)
        {
            case Enumes.AudioClips.beep:
                audioSource.PlayOneShot(beep);
                break;

            case Enumes.AudioClips.pop:
                audioSource.PlayOneShot(pop);
                break;

            case Enumes.AudioClips.spray:
                audioSource.PlayOneShot(spray);
                break;
        }
    }

    public void UpdateVolume()
    {
        audioSource.volume = fxVolumeVar.GetValue();
    }

}
