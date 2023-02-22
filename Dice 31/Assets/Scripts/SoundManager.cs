using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> SFXList;
    public List<AudioClip> BGMList;
    
    // 0 = mute, 1 = full volume
    public float SFXVolume = 1;
    public float BGMVolume = 1;

    public void LaserShootingSound() {
        audioSource.clip = SFXList[3];
        audioSource.volume = SFXVolume;
        audioSource.Play();
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
