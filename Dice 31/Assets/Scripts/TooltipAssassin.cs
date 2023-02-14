using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipAssassin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image AssassinTooltip;
    public Text commonInfo;
    public void OnPointerEnter(PointerEventData eventData) { 
        if (GameManager.Inst.gsm.State != GameState.Waiting && GameManager.Inst.gsm.State != GameState.Gameover) {
            AssassinTooltip.gameObject.SetActive(true);
            switch (GameManager.Inst.pm.assassinInfo) {
                case AssassinInfo.None:
                    commonInfo.text = "<color=#FF0000>암살</color>\n비활성화됨";
                    break;
                case AssassinInfo.Bow:
                    commonInfo.text = "<color=#FF0000>암살\n활: 1, 2, 3, 4 -> 암살 성공</color>";
                    break;
                case AssassinInfo.Sword:
                    commonInfo.text = "<color=#FF0000>암살\n칼: 3, 4, 5, 6 -> 암살 성공</color>";
                    break;
                case AssassinInfo.Gun:
                    commonInfo.text = "<color=#FF0000>암살\n활: 1, 2, 4, 5 -> 암살 성공</color>";
                    break;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData) { 
        AssassinTooltip.gameObject.SetActive(false);
    }
}
