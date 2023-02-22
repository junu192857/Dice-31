using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> SFXList;
    
    // 0 = mute, 1 = full volume
    public float SFXVolume = 1;
    public float BGMVolume = 1;

    public void PlayExplosionSound() {
        audioSource.clip = SFXList[1];
        audioSource.volume = SFXVolume;
        audioSource.Play();
    }
    public void LaserChargeSound() {
        audioSource.clip = SFXList[2];
        audioSource.volume = SFXVolume;
        audioSource.Play();
    }
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
