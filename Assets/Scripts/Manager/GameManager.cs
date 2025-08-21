using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GeneralDefine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;
using static PiecesDefine;
using static OtherDefine;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public PiecesUI pieceUI;
    public PlateUI plateUI;
    private BoardLogic boardLogic;
    private BoardUI boardUI;
    private Plate plate;
    private PlayerManager[] player;
    private ChessRules chessRules;
    private TurnManager turnManager;
    private CanvasUIManager canvasUIManager;
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
        canvasUIManager = FindAnyObjectByType<CanvasUIManager>();
        turnManager = new TurnManager();
        canvasUIManager.AssignRefInstance(turnManager);
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
        List<CastlePosPair> castleMoves = new List<CastlePosPair>();

        player[(int)piece.teamSide].SetSelectPiece(piece);

        (List<CoordXY> legalList, List<CoordXY> illegalList) temp = chessRules.FilterIllegalMoves(piece, legalMoves);
        legalMoves = temp.legalList;
        illegalMoves = temp.illegalList;

        if (piece.pieceType == PIECE_TYPE.KING)
        {
            if (chessRules.IsAbleToCastle(piece.teamSide))
            {
                castleMoves.AddRange(chessRules.GetCastlePosPair(piece.teamSide).pieceKing);
                legalMoves.Remove(piece.GetCurrentPosition());
            }
        }

        plate.DestroyAllPlates();
        if (legalMoves != null && legalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.LEGAL, legalMoves, piece.GetCurrentPosition());
        }
        if (illegalMoves != null && illegalMoves.Count > 0)
        {
            plate.ShowPlateAt(PLATE_TYPE.ILLEGAL, illegalMoves, piece.GetCurrentPosition());
        }
        if (castleMoves != null && castleMoves.Count > 0)
        {
            List<CoordXY> castleCoord = new List<CoordXY>();

            foreach (CastlePosPair pair in castleMoves)
            {
                castleCoord.Add(pair.castlePos);
            }
            plate.ShowPlateAt(PLATE_TYPE.SPECIAL, castleCoord, piece.GetCurrentPosition());
        }
    }

    private void OnMovePlateSelected(Plate movePlate)
    {
        if (movePlate == null || movePlate.GetPlateType() == PLATE_TYPE.ILLEGAL) return;
        if (movePlate.GetPlateType() == PLATE_TYPE.LEGAL)
        {
            CoordXY selectedPos = movePlate.GetPos();
            player[(int)turnManager.GetCurrentTurn()].MoveSelectedPiece(selectedPos);
        }
        else if (movePlate.GetPlateType() == PLATE_TYPE.SPECIAL)
        {
            CoordXY selectedPos = movePlate.GetPos();
            var castleData = chessRules.GetMoveCastlePieceBySelectedPlate(TEAM_SIDE.ALLY, selectedPos);
            player[(int)turnManager.GetCurrentTurn()].MoveCastle
                (castleData.kingData.pieceKing, castleData.rookData.pieceRook, 
                castleData.kingData.castleKingPos, castleData.rookData.castleRookPos);
        }
        movePlate.DestroyAllPlates();
        turnManager.SwitchTurn();
    }

    public void OnClickEvent(Component component)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            OnPieceSelected((IPieces)component);
        }
        if (component is Plate) OnMovePlateSelected((Plate)component); 
    }

    DragPieceCommand dragPieceCmd;
    public void OnHoldStartEvent(Component component)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            ((IPieces)component).ForceSetPieceHeight(PIECE_LIFT_HEIGHT);
            OnPieceSelected((IPieces)component);
            dragPieceCmd = new DragPieceCommand((IPieces)component);
        }
    }

    public void OnHoldDragEvent(Component component, Vector2 position)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            dragPieceCmd.Execute(position);
        }
    }

    public void OnHoldEndEvent(Component component, Vector2 position)
    {
        if (component is IPieces && turnManager.IsTurnOf(((IPieces)component).teamSide))
        {
            Plate selectedPlate = ((IPieces)component).GetTriggeredPlate();
            if (selectedPlate != null 
                && selectedPlate.GetPlateType() != PLATE_TYPE.ILLEGAL 
                && !selectedPlate.GetPos().IsEqual(((IPieces)component).GetCurrentPosition())) OnMovePlateSelected(selectedPlate);
            else dragPieceCmd.Undo();
        }
    }
}
