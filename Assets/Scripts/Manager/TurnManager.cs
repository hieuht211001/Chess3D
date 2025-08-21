using UnityEngine;
using static GeneralDefine;

public class TurnManager
{
    private TEAM_SIDE currentTurn = TEAM_SIDE.ALLY;

    public TEAM_SIDE GetCurrentTurn() => currentTurn;

    public void SwitchTurn()
    {
        currentTurn = (currentTurn == TEAM_SIDE.ALLY) ? TEAM_SIDE.ENEMY : TEAM_SIDE.ALLY;
    }

    public bool IsTurnOf(TEAM_SIDE team)
    {
        return currentTurn == team;
    }
}
