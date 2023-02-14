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
                    commonInfo.text = "<color=#FF0000>�ϻ�</color>\n��Ȱ��ȭ��";
                    break;
                case AssassinInfo.Bow:
                    commonInfo.text = "<color=#FF0000>�ϻ�\nȰ: 1, 2, 3, 4 -> �ϻ� ����</color>";
                    break;
                case AssassinInfo.Sword:
                    commonInfo.text = "<color=#FF0000>�ϻ�\nĮ: 3, 4, 5, 6 -> �ϻ� ����</color>";
                    break;
                case AssassinInfo.Gun:
                    commonInfo.text = "<color=#FF0000>�ϻ�\nȰ: 1, 2, 4, 5 -> �ϻ� ����</color>";
                    break;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData) { 
        AssassinTooltip.gameObject.SetActive(false);
    }
}
