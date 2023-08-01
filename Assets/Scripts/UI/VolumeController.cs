using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{

    [SerializeField]
    Slider musicSlider;

    [SerializeField]
    Slider fxSlider;

    [SerializeField]
    FloatVariable musicVolumeVar;

    [SerializeField]
    FloatVariable fXVolumeVar;

    private void Awake()
    {
        fxSlider.value = fXVolumeVar.GetValue();
        musicSlider.value = musicVolumeVar.GetValue();
    }

    // Start is called before the first frame update
    void Start()
    {
        //fxSlider.value = fXVolumeVar.GetValue();
        //musicSlider.value = musicVolumeVar.GetValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVolumes()
    {
        fXVolumeVar.SetValue(fxSlider.value);
        musicVolumeVar.SetValue(musicSlider.value);
    }
}
