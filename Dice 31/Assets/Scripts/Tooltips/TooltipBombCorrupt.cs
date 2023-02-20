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
                BombText.text = $"<color=#FF0000>폭탄</color>\n" +
                                 "비활성화됨";
            }
            else {
                string post;
                switch (GameManager.Inst.pm.bombDiceNum)
                {
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
                BombText.text = $"<color=#FF0000>폭탄\n" +
                                $"{GameManager.Inst.pm.bombDiceNum}" + post + " 나오면 폭발</color>";
            }
            if (GameManager.Inst.pm.corruptStack == 4)
            {
                CorruptText.text = $"<color=#9400D3>타락 4단계\n" +
                                   $"모든 숫자 2배로 적용 중</color>\n" +
                                   $"5단계: 플레이어 사망";
            }
            else if (GameManager.Inst.pm.corruptStack == 5) {
                CorruptText.text = $"<color=#9400D3>타락 {GameManager.Inst.pm.corruptStack}단계</color>\n" +
                                   $"모든 숫자 2배로 적용 중\n";
            }
            else
            {
                CorruptText.text = $"<color=#9400D3>타락 {GameManager.Inst.pm.corruptStack}단계</color>\n" +
                                   $"4단계: 모든 숫자 2배로 적용\n" +
                                   $"5단계: 플레이어 사망";
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BombCorruptImage.gameObject.SetActive(false);
    }
}
