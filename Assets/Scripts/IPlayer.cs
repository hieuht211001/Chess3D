using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static GeneralDefine;

public abstract class IPlayerController
{
    public TurnManager turnManager;
    public Player[] player;
    public BoardLogic boardLogic;
    public ChessRules chessRules;
    public Plate plate;
    public PiecesUI piecesUI;
    public CaptureQueue captureQueue;
    protected TEAM_SIDE teamSide;

    public IPlayerController(TEAM_SIDE teamSide)
    {
        AssignRefInstance();
        this.teamSide = teamSide;
    }

    public void AssignRefInstance()
    {
        turnManager = GameManager.Instance.turnManager;
        player = GameManager.Instance.player;
        boardLogic = GameManager.Instance.boardLogic;
        chessRules = GameManager.Instance.chessRules;
        plate = GameManager.Instance.plate;
        piecesUI = GameManager.Instance.pieceUI;
        captureQueue = GameManager.Instance.captureQueue;
    }

    public void InitIndividual()
    {
        player[(int)teamSide].Init(teamSide, boardLogic, piecesUI, captureQueue);
    }
    public virtual void OnTurnSwitch(TEAM_SIDE teamSide) { }
    public virtual void OnClickEvent(Component component) { }
    public virtual void OnHoldStartEvent(Component component) { }
    public virtual void OnHoldDragEvent(Component component, Vector2 position) { }
    public virtual void OnHoldEndEvent(Component component, Vector2 position) { }
}

