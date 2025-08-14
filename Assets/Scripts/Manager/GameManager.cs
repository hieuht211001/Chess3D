using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static EnumDefines;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

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

    // Update is called once per frame
    void Update()
    {
        if (GetSelectedPiece() is IPieces piece) { OnPieceSelected(piece); }
        else if (GetSelectedPiece() is Plate movePlate) { OnMovePlateSelected(movePlate); }
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
        if (movePlate == null) return;
        CoordXY selectedPos = movePlate.GetPos();
        player[(int)TEAM_SIDE.ALLY].MoveSelectedPiece(selectedPos);
        movePlate.DestroyAllMovePlates();
    }

    private object GetSelectedPiece()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Camera currentCamera = null;
            foreach (var cam in Camera.allCameras)
            {
                if (cam.isActiveAndEnabled && cam.pixelRect.Contains(mousePos))
                {
                    currentCamera = cam;
                    break;
                }
            }

            Ray ray = currentCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject selectedObject = hit.collider.gameObject;
                Renderer rend = selectedObject.GetComponent<Renderer>();
                if (rend != null && selectedObject.tag == TAG.PIECES.ToString())
                {
                    GameObject hitObject = hit.collider.gameObject;
                    IPieces pieceComp = hitObject.GetComponent<IPieces>();
                    return pieceComp;
                }
                else if (rend != null && selectedObject.tag == TAG.MOVE_PLATE.ToString())
                {
                    GameObject hitObject = hit.collider.gameObject;
                    Plate movePlate = hitObject.GetComponent<Plate>();
                    return movePlate;
                }
            }
        }
        return null;
    }
}
