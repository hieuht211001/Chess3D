using System;
using System.Collections.Generic;
using UnityEngine;
using static GeneralDefine;

public class CanvasUIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject turnTimerPrefab;
    private List<TurnTimer> turnTimerList;
    private TurnManager turnManager;

    void Start()
    {
        InitTurnTimerPrefabs();
    }

    public void AssignRefInstance(TurnManager turnManager)
    {
        this.turnManager = turnManager;
    }

    void Update()
    {
        foreach (TEAM_SIDE teamSide in Enum.GetValues(typeof(TEAM_SIDE)))
        {
            if (teamSide == TEAM_SIDE.MAX) continue;
            if (turnManager.IsTurnOf(teamSide)) turnTimerList[(int)teamSide].StartTimer();
            else turnTimerList[(int)teamSide].StopTimer();
        }
    }

    private void InitTurnTimerPrefabs()
    {
        turnTimerList = new List<TurnTimer>();
        for (int teamSide = (int)TEAM_SIDE.ALLY; teamSide < (int)TEAM_SIDE.MAX; teamSide++)
        {
            GameObject turnTimer = Instantiate(turnTimerPrefab, canvas.transform);
            turnTimer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, teamSide == (int)TEAM_SIDE.ALLY ? -55 : 55);
            TurnTimer tm = turnTimer.GetComponent<TurnTimer>();
            tm.teamSide = (TEAM_SIDE)teamSide;
            tm.SetTimerData(30f);
            turnTimerList.Add(tm);
        }
    }
}
