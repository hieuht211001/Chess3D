using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static GeneralDefine;
using static PiecesDefine;

public class PlayerManager : MonoBehaviour
{
    private TEAM_SIDE teamSide;
    private CommandManager commandManager;
    private BoardLogic boardLogic;
    private List<IPieces> pieceList;
    private PiecesUI pieceUI;
    private IPieces selectedPiece;

    public void Init(TEAM_SIDE teamSide, BoardLogic board, PiecesUI pieceUI)
    {
        this.teamSide = teamSide;
        commandManager = new CommandManager();
        this.boardLogic = board;
        this.pieceUI = pieceUI;
        this.pieceList = new List<IPieces>();
        DrawStartingBoardNPieces();
        AssignRefInstance();
    }
    public List<IPieces> GetPieceList() => this.pieceList;
    public IPieces GetIPiecesRef() => this.selectedPiece;
    public void SetSelectPiece(IPieces piece) => selectedPiece = piece;
    private void AssignRefInstance()
    {
        foreach (IPieces piece in pieceList)
        { piece.AssignRefIntance(boardLogic); }
    }

    public void DrawStartingBoardNPieces()
    {
        for (int i = 0; i < 8; i++)
        {
            if (teamSide == TEAM_SIDE.ALLY) CreatePiece<PiecePawn>(TEAM_SIDE.ALLY, PIECE_TYPE.PAWN, (START_PIECE_LAYOUT)i, (COORD_X)i, COORD_Y._2);
            else if (teamSide == TEAM_SIDE.ENEMY) CreatePiece<PiecePawn>(TEAM_SIDE.ENEMY, PIECE_TYPE.PAWN, (START_PIECE_LAYOUT)i, (COORD_X)i, COORD_Y._7);
        }

        if (teamSide == TEAM_SIDE.ALLY) CreateMainPieces(TEAM_SIDE.ALLY, COORD_Y._1);
        else if (teamSide == TEAM_SIDE.ENEMY) CreateMainPieces(TEAM_SIDE.ENEMY, COORD_Y._8);
    }

    private void CreateMainPieces(TEAM_SIDE side, COORD_Y row)
    {
        CreatePiece<PieceRook>(side, PIECE_TYPE.ROOK, START_PIECE_LAYOUT.ROOK_1, COORD_X.A, row);
        CreatePiece<PieceKnight>(side, PIECE_TYPE.KNIGHT, START_PIECE_LAYOUT.KNIGHT_1, COORD_X.B, row);
        CreatePiece<PieceBishop>(side, PIECE_TYPE.BISHOP, START_PIECE_LAYOUT.BISHOP_1, COORD_X.C, row);
        CreatePiece<PieceQueen>(side, PIECE_TYPE.QUEEN, START_PIECE_LAYOUT.QUEEN, COORD_X.D, row);
        CreatePiece<PieceKing>(side, PIECE_TYPE.KING, START_PIECE_LAYOUT.KING, COORD_X.E, row);
        CreatePiece<PieceBishop>(side, PIECE_TYPE.BISHOP, START_PIECE_LAYOUT.BISHOP_2, COORD_X.F, row);
        CreatePiece<PieceKnight>(side, PIECE_TYPE.KNIGHT, START_PIECE_LAYOUT.KNIGHT_2, COORD_X.G, row);
        CreatePiece<PieceRook>(side, PIECE_TYPE.ROOK, START_PIECE_LAYOUT.ROOK_2, COORD_X.H, row);
    }


    public void CreatePiece<T>(TEAM_SIDE teamSide, PIECE_TYPE pieceType, START_PIECE_LAYOUT layoutPos, COORD_X x, COORD_Y y) where T : IPieces
    {
        GameObject gameObject = Instantiate(pieceUI.GetPieceObject(pieceType), new Vector3(0, 0, 0), Quaternion.identity);
        T piece = gameObject.AddComponent<T>();
        piece.Init(this.pieceUI, teamSide, pieceType, new CoordXY(x, y));
        pieceList.Add(piece);
    }

    public void MoveSelectedPiece(CoordXY newPos)
    {
        if (!selectedPiece.IsCoordInPossiblePosList(newPos)) return;
        CoordXY oldPos = selectedPiece.GetCurrentPosition();
        var moveCommand = new MoveCommand(this.boardLogic, selectedPiece, newPos);
        commandManager.ExecuteCommand(moveCommand);
    }


    public void UndoMove()
    {
        commandManager.Undo();
    }

    public void RedoMove()
    {
        commandManager.Redo();
    }

    public List<IPieces> GetAssignedPieces(PIECE_TYPE type)
    {
        List<IPieces> returnList = new List<IPieces> ();
        foreach (IPieces piece in pieceList)
        {
            if (piece.pieceType == type) returnList.Add(piece);
        }
        return returnList;
    }
}
