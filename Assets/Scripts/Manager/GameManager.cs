using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GeneralDefine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;
using static PiecesDefine;
using static OtherDefine;

public class GameManager : MonoBehaviour
{
    public PiecesUI pieceUI;
    public PlateUI plateUI;
    public GameObject movePlatePrefab;
    private BoardLogic boardLogic;
    private BoardUI boardUI;
    private Plate plate;
    private PlayerManager[] player;
    private ChessRules chessRules;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    private void Init()
    {
        plate = new Plate(plateUI);
        player = new PlayerManager[(int)TEAM_SIDE.MAX];
        boardLogic = FindAnyObjectByType<BoardLogic>();
        boardLogic.AssignRefInstance(player);
        boardUI = FindAnyObjectByType<BoardUI>();
        for (int iIndex = 0; iIndex < (int)TEAM_SIDE.MAX; iIndex++)
        {
            GameObject playerObj = new GameObject(((TEAM_SIDE)iIndex).ToString());
            player[iIndex] = playerObj.AddComponent<PlayerManager>();
            player[iIndex].Init((TEAM_SIDE)iIndex, boardLogic, pieceUI);
        }
        chessRules = new ChessRules(boardLogic, player);
    }

    private void OnPieceSelected(IPieces piece)
    {
        if (piece == null) return;
        List<CoordXY> legalMoves = piece.GetPossibleMoves();
        List<CoordXY> illegalMoves = new List<CoordXY>();

        player[(int)piece.teamSide].SetSelectPiece(piece);

        (List<CoordXY> legalList, List<CoordXY> illegalList) temp = chessRules.FilterIllegalMoves(piece, legalMoves);
        legalMoves = temp.legalList;
        illegalMoves = temp.illegalList;

        plate.DestroyAllPlates();
        if (legalMoves != null && legalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.LEGAL, legalMoves, piece.GetCurrentPosition());
        }
        if (illegalMoves != null && illegalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.ILLEGAL, illegalMoves, piece.GetCurrentPosition());
        }
    }

    private void OnMovePlateSelected(Plate movePlate)
    {
        if (movePlate == null || movePlate.GetPlateType() == PLATE_TYPE.ILLEGAL) return;
        CoordXY selectedPos = movePlate.GetPos();
        player[(int)TEAM_SIDE.ALLY].MoveSelectedPiece(selectedPos);
        movePlate.DestroyAllPlates();
    }

    public void OnClickEvent(Component component)
    {
        if (component is IPieces) OnPieceSelected((IPieces)component);
        if (component is Plate)  OnMovePlateSelected((Plate)component); 
    }

    DragPieceCommand dragPieceCmd;
    public void OnHoldStartEvent(Component component)
    {
        if (component is IPieces)
        {
            ((IPieces)component).ForceSetPieceHeight(PIECE_LIFT_HEIGHT);
            OnPieceSelected((IPieces)component);
            dragPieceCmd = new DragPieceCommand((IPieces)component);
        }
    }

    public void OnHoldDragEvent(Component component, Vector2 position)
    {
        if (component is IPieces)
        {
            dragPieceCmd.Execute(position);
        }
    }

    public void OnHoldEndEvent(Component component, Vector2 position)
    {
        if (component is IPieces)
        {
            dragPieceCmd.SetUndoRequested();
            Plate selectedPlate = ((IPieces)component).GetTriggeredPlate();
            if (selectedPlate != null 
                && selectedPlate.GetPlateType() == PLATE_TYPE.LEGAL 
                && !selectedPlate.GetPos().IsEqual(((IPieces)component).GetCurrentPosition())) OnMovePlateSelected(selectedPlate);
            else dragPieceCmd.Undo();
        }
    }
}
