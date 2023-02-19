using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> SFXList;

    public void DiceCollision(float velocity) {
        audioSource.clip = SFXList[0];
        audioSource.volume = velocity;
        audioSource.Play();
    }
    public void PlayExplosionSound() {
        audioSource.clip = SFXList[1];
        audioSource.Play();
    }
    public void LaserChargeSound() {
        audioSource.clip = SFXList[2];
        audioSource.Play();
    }
    public void LaserShootingSound() {
        audioSource.clip = SFXList[3];
        audioSource.Play();
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
