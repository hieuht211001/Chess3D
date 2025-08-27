using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GeneralDefine;
using static OtherDefine;
using static PiecesDefine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;
using Component = UnityEngine.Component;

public class GameManager : MonoBehaviour
{
    public PiecesUI pieceUI;
    public PlateUI plateUI;
    public GameObject captureLineUI;
    public BoardLogic boardLogic;
    public BoardUI boardUI;
    public Plate plate;
    public Player[] player;
    public ChessRules chessRules;
    public TurnManager turnManager;
    public CanvasUIManager canvasUIManager;
    public CaptureQueue captureQueue;
    public IPlayerController[] playerController;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        plate = new Plate();
        plate.AssignRefInstance(plateUI);
        player = new Player[(int)TEAM_SIDE.MAX];
        captureQueue = new CaptureQueue();
        captureQueue.AssignRefInstance(captureLineUI);
        boardLogic = FindAnyObjectByType<BoardLogic>();
        boardLogic.AssignRefInstance(player);
        boardUI = FindAnyObjectByType<BoardUI>();
        canvasUIManager = FindAnyObjectByType<CanvasUIManager>();
        turnManager = new TurnManager();
        canvasUIManager.AssignRefInstance(turnManager);
        for (int iIndex = 0; iIndex < (int)TEAM_SIDE.MAX; iIndex++)
        {
            GameObject playerObj = new GameObject(((TEAM_SIDE)iIndex).ToString());
            player[iIndex] = playerObj.AddComponent<Player>();
        }
        chessRules = new ChessRules(boardLogic, player);
        InitPlayerMode();
    }

    private void InitPlayerMode()
    {
        playerController = new IPlayerController[(int)TEAM_SIDE.MAX];
        playerController[(int)TEAM_SIDE.ALLY] = new HumanController(TEAM_SIDE.ALLY);
        playerController[(int)TEAM_SIDE.ALLY].InitIndividual();
        playerController[(int)TEAM_SIDE.ENEMY] = new AIPlayer(TEAM_SIDE.ENEMY);
        playerController[(int)TEAM_SIDE.ENEMY].InitIndividual();

        turnManager.OnTurnSwitch += playerController[(int)TEAM_SIDE.ALLY].OnTurnSwitch;
        turnManager.OnTurnSwitch += playerController[(int)TEAM_SIDE.ENEMY].OnTurnSwitch;
    }
}
