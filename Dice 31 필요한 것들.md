## 발견된 버그
* 없음
***
## 추가할 것들
* 4인 게임 만들기
* 게임 후 통계 추가?
  * 별 의미 없긴 할듯.. 하는거라고는 주사위 굴리는거밖에 없는 게임이라

* Dice 31 Delux Mode
  * 베스킨라빈스 게임 방식이 아닌, 롤체 비슷한 게임 방식
***

## 새로운 Special Dice 아이디어

### 초록 주사위


### 빨간 주사위
1. Poison Dice
***    

# 게임 로직
## 1. Initiate()
> 게임이 시작할 때 모든 것을 초기화하는 부분이다.
* matchCount, winCount를 0으로 초기화한다.
* 플레이어 이름을 할당한다.
## 2. ResetMatch()
> 매치가 시작될 때마다 필요한 것을 초기화한다. 매치에서 승리하면 winCount가 오른다.
### 가장 먼저 GameOver() 함수를 작동시켜 게임이 오버되었는지 확인한다.
* GameOver()함수는 특정 팀의 winCount가 기준값을 넘어서면 True를 반환
* GameOver 시, GameState가 GameOver로 바뀌고 엔딩 씬을 로드함
### GameOver() = false인 경우, 또다시 필요한 값을 초기화한다.
```cs
roundCount = 0; // round를 초기화한다.

turnIndex = -1;
turnDirection = 1; // 순서를 지칭하는 인덱스 관련 값 초기화

bombDiceNum = 0;
onMyOwnDiceNum = 0;
assassinInfo = AssassinInfo.None;
corruptStack = 0; //값의 저장이 필요한 주사위들의 값 초기화.
```
**<U>주사위 관련 값들을 PlayManager에서 다루는 것이 맞는가? 각각의 주사위 클래스에서 static 변수를 놓고 하는 것이 좋지 않을까?</U>**
```cs
dicesToRoll.Clear(); // ActivatedPlayer가 굴릴 주사위들의 목록
previousDices.Clear(); // 이전 플레이어가 굴린 주사위의 목록. EffectAfterNextPlayerRoll()이 있는 주사위가 있을 때 중요해짐.

matchCount++; // 몇 번째 매치인지 표시. UI때문에 중요함

foreach (var player in playerInfos)
{
    player.Revive();
    player.unDead = false;
}
//플레이어들의 상태 초기화. 살아 있으며 언데드가 아닌 상태여야 함
//언데드는 기본적으로 살아 있는 판정임. 부활 주사위로 부활시킬 수 없음

allAlive = true; //allAlive = true이면 부활 주사위를 이용할 수 없다.
```
**<U>allAlive 변수 이대로 괜찮은가?</U>**  
allAlive는, 새로운 플레이어가 턴을 시작할 때 "그 플레이어의 Special Dice가 RevivalDice" 라면 특정 팀의 플레이어가 전부 살아있는지 판단하고 그렇다면 True, 아니라면 False로 할당된다.  
  
allAlive의 사용처 1: UIManager에서, 플레이어가 부활 주사위를 사용할 수 없도록 막는 곳에 사용된다.  
allAlive의 사용처 2: 부활 주사위에 마우스를 가져다댔을 때 툴팁을 보여주는 데 사용한다.  
  
**<U>개인적으로는, 부활 주사위의 턴의 시작 시점에만 update되는 allAlive를 없애고, 팀이 살아있는지 판단하는 함수를 만들어서 필요한 곳에 사용하는 것이 좋을 것 같다.</U>**
* 나중에 부활 주사위 말고도 팀이 살아있는지 판단해야 하는 경우가 생길 수도 있기 때문이다.
```cs
GameManager.Inst.um.ResetUI(); //UI를 초기화하는 부분이다.
```
ResetUI() 함수에는 특별히 고쳐야 할 게 없는 것 같다.
```cs
AssignDices(); //플레이어마다 랜덤한 특수 주사위를 할당하는 부분이다.
```
**<U>TODO: 구스구스덕의 직업 선택처럼, 반드시 등장하는 주사위, 등장할 수도 있는 주사위, 안 쓰는 주사위를 게임 시작 전에 플레이어가 설정할 수 있게 만들고 싶다.</U>**
```cs
ResetRound(); //라운드는 플레이어 사망의 원인이 생길 때마다 바뀐다
```
## 3.ResetRound()
> 라운드가 바뀔 때마다 필요한 변수들을 초기화한다.

**<U>코드에서 ResetRound가 ResetMatch보다 위에 있는거 너무 불편하다.</U>**
```cs
if (MatchOver())
{
    ResetMatch();
}
```
가장 먼저, MatchOver()인지 확인해서 맞다면 매치를 새로 시작한다.
* MatchOver()에서는 레드팀 또는 블루팀이 전원 사망했는지 확인해서 맞다면 상대팀의 winCount를 1점 올린다. 만약 동시에 사망했다면 Draw 판정이 되어 점수가 오르지 않지만 어쨌거나 매치는 리셋된다.
* **<U>Draw 시 둘 다 점수를 안 주기(현행) vs 둘 다 점수를 주기</U>**
  * Draw여도 점수를 받으면 기분이 좋지 않을까? ~~프세카 랭겜 쌍AP처럼~~
  * 만약 둘 다 점수를 준다면 maxWinCount를 늘려야 한다. 그렇지 않는다면 동시에 승리 조건을 만족했을 때, 코드에서 조건을 먼저 만족하는 팀이 승리하는 부조리가 일어날 것. 또는 승리를 판단하는 코드를 고치는 방법도 있다.
```cs
else
{
    curCount = 0;
    maxCount = 31; //게이지 바를 초기화하는 부분

    pendingRoundEnd = false; //해당 턴이 종료되면 라운드가 바뀌어야 한다는 것을 알려주는 변수인 pendingRoundEnd 변수를 초기화하는 부분이다.

    roundCount++; //UI를 위해 몇 라운드인지 알려준다.

    deadInfo.Clear(); //플레이어가 사망할 때 
    revivalInfo = -1;
    isNewUnDead = false;
    GameManager.Inst.um.GaugeBarCoroutine(curCount, maxCount);
            
    GameManager.Inst.gsm.WaitForPlayerTurn();
}

foreach (var player in playerInfos)
{
    if (player.specialDice.color == DiceColor.Green)
    {
        player.specialDice.EnableDice();
    }
}
```