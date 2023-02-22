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

    public IEnumerator CorruptedBGMFadeIn() {
        float runTime = 0f;
        while (runTime < 2f) {
            runTime += Time.deltaTime;
            audioSource.volume = BGMVolume * (1 - runTime / 3f);
            yield return null;
        }
        audioSource.clip = BGMList[2];
        audioSource.Play();
        audioSource.loop = true;
        runTime = 0f;
        while (runTime < 2f) {
            runTime += Time.deltaTime;
            audioSource.volume = BGMVolume * (runTime / 2f);
            yield return null;
        }
    }
    public IEnumerator CorruptedBGMFadeOut()
    {
        float runTime = 0f;
        while (runTime < 2f)
        {
            runTime += Time.deltaTime;
            audioSource.volume = BGMVolume * (1 - runTime / 2f);
            yield return null;
        }
        audioSource.clip = BGMList[1];
        runTime = 0f;
        audioSource.Play();
        audioSource.loop = true;
        while (runTime < 2f)
        {
            runTime += Time.deltaTime;
            audioSource.volume = BGMVolume * (runTime / 2f);
            yield return null;
        }
    }
    public void TurnOffBGM() {
        audioSource.volume = 0f;
    }
    public void UpdateBGMVolume() {
        audioSource.volume = BGMVolume;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
