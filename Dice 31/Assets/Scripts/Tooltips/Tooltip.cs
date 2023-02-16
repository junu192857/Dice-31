using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject player;
    public Player playerInfo;

    public Image playerTooltip;
    public Text PlayerName;
    public Image NormalDiceImage;
    public Image SpecialDiceImage;
    public Text PlayerState;
    public void OnPointerEnter(PointerEventData eventData) {
        if (GameManager.Inst.gsm.State != GameState.Waiting && GameManager.Inst.gsm.State != GameState.Gameover)
        {
            playerTooltip.gameObject.SetActive(true);
            playerTooltip.rectTransform.anchoredPosition = new Vector3(-440, 123 - 80 * playerInfo.playerIndex, 0);
            PlayerName.text = $"{playerInfo.playerName}";
            NormalDiceImage.sprite = GameManager.Inst.um.Dice2D[0];
            SpecialDiceImage.sprite = GameManager.Inst.um.Dice2D[playerInfo.specialDice.diceIndex];
            if (playerInfo.alive)
            {
                PlayerState.text = "������";
            }
            else
            {
                switch (playerInfo.deadCause)
                {
                    case DeadCause.Number:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 ���� �ʰ��� ���� ���";
                        break;
                    case DeadCause.Assassin:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 �ϻ�� ���� ���";
                        break;
                    case DeadCause.AssassinFail:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 �ϻ� ���з� ���� ���";
                        break;
                    case DeadCause.Bomb:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 ��ź���� ���� ���";
                        break;
                    case DeadCause.RevivalFail:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 ��Ȱ ���з� ���� ���";
                        break;
                    case DeadCause.Corrupted:
                        PlayerState.text = $"{playerInfo.deadRound}���忡 Ÿ���� �����Ǿ� ���";
                        break;
                    default:
                        PlayerState.text = "";
                        break;
                }
            }
        }

    }
    public void OnPointerExit(PointerEventData eventData) {
        playerTooltip.gameObject.SetActive(false);
    }

    private void Awake()
    {
        playerInfo = player.GetComponent<Player>();
    }
}
