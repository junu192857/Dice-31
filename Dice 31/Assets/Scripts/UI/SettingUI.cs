using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Button settingReturnButton;
    public Button settingExitButton;
    public Slider settingBgmVolume;
    public Slider settingSfxVolume;

    public float bgmVolume;
    public float sfxVolume;

    void Start()
    {

        //bgmVolume = GameManager.gm.bgmVolume;
        //sfxVolume = GameManager.gm.sfxVolume;

        transform.GetChild(0).transform.GetChild(5).GetComponent<Slider>().value = bgmVolume;
        transform.GetChild(0).transform.GetChild(7).GetComponent<Slider>().value = sfxVolume;
    }

    void Update()
    {
        /*
        if (GameManager.mm == null || !GameManager.mm.IsReady) return;
        if (GameManager.mm.IsTimeActivated && GameManager.mm.RemainingTime > 0f && !GameManager.mm.HasCleared)
        {
            pauseSkipButton.interactable = true;
        }
        else if (GameManager.mm.IsTimeActivated && (GameManager.mm.RemainingTime <= 0f || GameManager.mm.HasCleared))
        {
            pauseSkipButton.interactable = false;
        }
        */

    }
}
