using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 턴 모드를 정합니다.")] ETrunMode eTrunMode;
    [SerializeField] [Tooltip("카드 배분이 매우 빨라집니다.")] bool fastMode;
    [SerializeField] [Tooltip("시작 카드 개수를 정합니다.")] int startCardCount;
    
    [Header("Properties")]
    public bool isLoading; // 게임 끝날시 true일 경우 카드와 인티티 클릭 방지
    public bool myTurn;

    enum ETrunMode { Random, My, Other }
    WaitForSeconds delay05 = new WaitForSeconds(0.5f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    public static Action<bool> onAddCard;
    public static event Action<bool> OnTurnStarted;

    void GameSetup()
    {
        if(fastMode)
            delay05 = new WaitForSeconds(0.05f);

        switch (eTrunMode)
        {
            case ETrunMode.Random:
                myTurn = Random.Range(0, 2) == 0;   
                break;
            case ETrunMode.My:
                myTurn = true;
                break;
            case ETrunMode.Other:
                myTurn = false;
                break;
        }
    }
   
    public IEnumerator StartGameCo()
    {
        GameSetup();

        for(int i = 0; i < startCardCount; i++)
        {
            yield return delay05;
            onAddCard?.Invoke(false);
            yield return delay05;
            onAddCard?.Invoke(true);
        }
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;
        if(myTurn)
            GameManager.Inst.Notification("나의 턴");

        yield return delay07;
        onAddCard?.Invoke(myTurn);
        yield return delay07;
        isLoading = false;
        OnTurnStarted?.Invoke(myTurn);
    }

    public void EndTurn()
    {
        myTurn = !myTurn;
        StartCoroutine(StartTurnCo());
    }
}
