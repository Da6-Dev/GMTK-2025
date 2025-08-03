using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("VolumeAudio", 1f);
        AudioListener.volume = sliderValue;
    }

    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("VolumeAudio", sliderValue);
        AudioListener.volume = slider.value;
    }

   
}