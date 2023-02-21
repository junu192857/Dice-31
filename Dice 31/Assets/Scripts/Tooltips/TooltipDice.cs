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
                            post = "��";
                            break;
                        case 2:
                        case 4:
                        case 5:
                            post = "��";
                            break;
                        default:
                            post = "";
                            break;
                    }
                    additionalInfo.text = "<color=#FF0000>" + $"{GameManager.Inst.pm.bombDiceNum}" + post + " ������ ����" + "</color>";
                    if (specialDice.available) 
                    {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 260);
                        warning.text = "";
                    }
                    else {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 360);
                        warning.text = "�̹� ���ӿ��� �̹� �����";
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
                            additionalInfo.text = "<color=#9400D3>" + $"Ÿ�� {GameManager.Inst.pm.corruptStack}�ܰ�</color>\n" +
                                                  "4�ܰ�: ��� ���� 2��� ����\n" +
                                                  "5�ܰ�: �÷��̾� ���";
                            break;
                        case 4:
                            additionalInfo.text = "<color=#9400D3>Ÿ�� 4�ܰ�\n" +
                                                  "��� ���� 2��� ���� ��\n" +
                                                  "5�ܰ�: �÷��̾� ���</color>";
                            break;
                        case 5:
                            additionalInfo.text = "<color=#9400D3>Ÿ�� 5�ܰ�\n" +
                                                  "��� ���� 2��� ���� ��\n";                 
                            break;
                        default:
                            break;
                    }
                    warning.text = "Ÿ�� �ֻ����� �ݵ�� ������ ��";
                    diceTooltip.rectTransform.sizeDelta = new Vector2(480, 440);
                    warning.rectTransform.anchoredPosition = new Vector3(16, -380, 0);
                }
                else if (specialDice is AssassinDice)
                {
                    additionalInfo.text = "<color=#FF0000>" + "�ϻ� ���� �� ���� ��� ���\n" +
                                          "�ϻ� ���� �� �ڱ��ڽ� ���</color>";

                    if (specialDice.available){
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 310);
                        warning.text = "";
                    }
                    else{
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 390);
                        warning.text = "�̹� ���ӿ��� �̹� �����";
                        warning.rectTransform.anchoredPosition = new Vector3(16, -340, 0);
                    }
                }
                else if (specialDice is RevivalDice)
                {
                    additionalInfo.text = "<color=#FF0000>" + "��Ȱ ���� �� ���� ���� ��Ȱ\n" +
                                          "��Ȱ ���� �� �ڱ��ڽ� ���</color>";
                    if (specialDice.available)
                    {
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 310);
                        warning.text = "";
                    }
                    else{
                        diceTooltip.rectTransform.sizeDelta = new Vector2(480, 390);
                        warning.text = "�̹� ���ӿ��� �̹� �����";
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
                        warning.text = "�̹� ���忡�� �̹� �����";
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
                        warning.text = "�̹� ���忡�� �̹� �����";
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
