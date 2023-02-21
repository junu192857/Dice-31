using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipDice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image diceTooltip;
    public Text diceName;
    public Text diceInformation;
    public Text additionalInfo;
    public Text warning;

    public bool isNormalDiceImage;

    public void OnPointerEnter(PointerEventData eventData) {
        if (GameManager.Inst.gsm.State != GameState.Waiting && GameManager.Inst.gsm.State != GameState.Gameover)
        {
            var normalDice = GameManager.Inst.pm.activatedPlayer.normalDice;
            var specialDice = GameManager.Inst.pm.activatedPlayer.specialDice;
            diceTooltip.gameObject.SetActive(true);
            if (isNormalDiceImage)
            {
                diceName.text = $"{normalDice.koreanDiceName}";
                diceInformation.text = $"{normalDice.diceInformation}";
                additionalInfo.text = "";
                warning.text = "";
                diceTooltip.rectTransform.sizeDelta = new Vector2(480, 180);
            }
            else
            {
                diceName.text = $"{specialDice.koreanDiceName}";
                diceInformation.text = $"{specialDice.diceInformation}";
                if (specialDice is BombDice && GameManager.Inst.pm.bombDiceNum != 0)
                {
                    string post;
                    switch (GameManager.Inst.pm.bombDiceNum) {
                        case 1:
                        case 3:
                        case 6:
                            post = "이";
                            break;
                        case 2:
                        case 4:
                        case 5:
                            post = "가";
                            break;
                        default:
                            post = "";
                            break;
                    }
                    additionalInfo.text = "<color=#FF0000>" + $"{GameManager.Inst.pm.bombDiceNum}" + post + " 나오면 폭발" + "</color>";
                    if (specialDice.available) 
                    {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 260);
                        warning.text = "";
                    }
                    else {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 360);
                        warning.text = "이번 게임에서 이미 사용함";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -300, 0);
                    }
                }
                else if (specialDice is CorruptedDice)
                {
                    switch (GameManager.Inst.pm.corruptStack)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            additionalInfo.text = "<color=#9400D3>" + $"타락 {GameManager.Inst.pm.corruptStack}단계</color>\n" +
                                                  "4단계: 모든 숫자 2배로 적용\n" +
                                                  "5단계: 플레이어 사망";
                            break;
                        case 4:
                            additionalInfo.text = "<color=#9400D3>타락 4단계\n" +
                                                  "모든 숫자 2배로 적용 중\n" +
                                                  "5단계: 플레이어 사망</color>";
                            break;
                        case 5:
                            additionalInfo.text = "<color=#9400D3>타락 5단계\n" +
                                                  "모든 숫자 2배로 적용 중\n";                 
                            break;
                        default:
                            break;
                    }
                    warning.text = "타락 주사위는 반드시 굴려야 함";
                    diceTooltip.rectTransform.sizeDelta = new Vector2(480, 440);
                    warning.rectTransform.anchoredPosition = new Vector3(16, -380, 0);
                }
                else if (specialDice is AssassinDice)
                {
                    additionalInfo.text = "<color=#FF0000>" + "암살 성공 시 다음 사람 사망\n" +
                                          "암살 실패 시 자기자신 사망</color>";

                    if (specialDice.available){
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 310);
                        warning.text = "";
                    }
                    else{
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 390);
                        warning.text = "이번 게임에서 이미 사용함";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -340, 0);
                    }
                }
                else if (specialDice is RevivalDice)
                {
                    additionalInfo.text = "<color=#FF0000>" + "부활 성공 시 죽은 팀원 부활\n" +
                                          "부활 실패 시 자기자신 사망</color>";
                    if (specialDice.available)
                    {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 310);
                        warning.text = "";
                    }
                    else{
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 390);
                        warning.text = "이번 게임에서 이미 사용함";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -340, 0);
                    }
                }
                else if (specialDice is MinusDice) {
                    if (specialDice.available)
                    {
                        additionalInfo.text = "";
                        warning.text = "";
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 130);
                    }
                    else 
                    {
                        additionalInfo.text = "";
                        warning.text = "이번 라운드에서 이미 사용함";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -170, 0);
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 220);
                    }
                }
                else
                {
                    if (specialDice.available){
                        additionalInfo.text = "";
                        warning.text = "";
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 180);
                    }
                    else{
                        additionalInfo.text = "";
                        warning.text = "이번 라운드에서 이미 사용함";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -210, 0);
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 260);
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        diceTooltip.gameObject.SetActive(false);
    }
}
