using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipBombCorrupt : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image BombCorruptImage;
    public Text BombText;
    public Text CorruptText;
    public void OnPointerEnter(PointerEventData eventData) {
        if (GameManager.Inst.gsm.State != GameState.Waiting && GameManager.Inst.gsm.State != GameState.Gameover) {
            BombCorruptImage.gameObject.SetActive(true);
            if (GameManager.Inst.pm.bombDiceNum == 0)
            {
                BombText.text = $"<color=#FF0000>��ź</color>\n" +
                                 "��Ȱ��ȭ��";
            }
            else {
                string post;
                switch (GameManager.Inst.pm.bombDiceNum)
                {
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
                BombText.text = $"<color=#FF0000>��ź\n" +
                                $"{GameManager.Inst.pm.bombDiceNum}" + post + " ������ ����</color>";
            }
            if (GameManager.Inst.pm.corruptStack == 4)
            {
                CorruptText.text = $"<color=#9400D3>Ÿ�� 4�ܰ�\n" +
                                   $"��� ���� 2��� ���� ��</color>\n" +
                                   $"5�ܰ�: �÷��̾� ���";
            }
            else if (GameManager.Inst.pm.corruptStack == 5) {
                CorruptText.text = $"<color=#9400D3>Ÿ�� {GameManager.Inst.pm.corruptStack}�ܰ�</color>\n" +
                                   $"��� ���� 2��� ���� ��\n";
            }
            else
            {
                CorruptText.text = $"<color=#9400D3>Ÿ�� {GameManager.Inst.pm.corruptStack}�ܰ�</color>\n" +
                                   $"4�ܰ�: ��� ���� 2��� ����\n" +
                                   $"5�ܰ�: �÷��̾� ���";
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BombCorruptImage.gameObject.SetActive(false);
    }
}
