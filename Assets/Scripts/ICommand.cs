using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static GeneralDefine;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class CommandManager
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }
}

public class MoveCommand : ICommand
{
    private IPieces targetPiece;
    private IPieces capturedPiece;
    private CoordXY originCoord, destCoord;
    private BoardLogic boardGrid;
    public MoveCommand(BoardLogic board, IPieces targetPiece, CoordXY destCoord)
    {
        this.boardGrid = board;
        this.targetPiece = targetPiece;
        this.originCoord = targetPiece.GetCurrentPosition();
        this.destCoord = destCoord;
    }

    public void Execute()
    {
        capturedPiece = boardGrid.GetPieceAt(this.destCoord);
        if (capturedPiece != null) capturedPiece.MoveToCaptureQueue();
        targetPiece.MoveToWithLift(destCoord);
    }

    public void Undo()
    {
        targetPiece.MoveToWithLift(originCoord);
    }
}

public class SimulateCommand : ICommand
{
    private IPieces targetPiece;
    private CoordXY originCoord, destCoord;
    public SimulateCommand(IPieces targetPiece, CoordXY destCoord)
    {
        this.targetPiece = targetPiece;
        this.originCoord = targetPiece.GetCurrentPosition();
        this.destCoord = destCoord;
    }

    public void Execute()
    {
        targetPiece.ForceSimulatePieceCoord(destCoord);
    }

    public void Undo()
    {
        targetPiece.ForceSimulatePieceCoord(originCoord);
    }
}

public class DragPieceCommand : ICommand
{
    private IPieces targetPiece;
    private CoordXY originCoord;
    private Vector2 dragPos;
    private bool isUndoRequested;
    public DragPieceCommand(IPieces targetPiece)
    {
        this.isUndoRequested = false;
        this.targetPiece = targetPiece;
        this.originCoord = targetPiece.GetCurrentPosition();
    }

    public void Execute(Vector2 dragPos)
    {
        if (isUndoRequested) return;
        if (this.dragPos.Equals(dragPos)) return;
        this.dragPos = dragPos;
        targetPiece.ForceSetPiecePos(dragPos);
    }

    public void Execute() { }

    public void Undo()
    {
        targetPiece.ForceSetPiecePos(originCoord);
    }

    public void SetUndoRequested() => isUndoRequested = true;
}

