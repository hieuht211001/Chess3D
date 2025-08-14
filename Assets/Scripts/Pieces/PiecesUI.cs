using System;
using UnityEngine;
using static EnumDefines;

[CreateAssetMenu(fileName = "PieceUI", menuName = "Scriptable Objects/PieceUI")]
public class PiecesUI : ScriptableObject
{
    [Serializable]
    private class PieceMaterial
    {
        public Material material;
        public PIECE_MATERIAL name;
    }

    [SerializeField] private PieceMaterial[] pieceMaterialArray;
    public Material GetPieceMaterial(PIECE_MATERIAL teamColor)
    {
        try
        {
            foreach (var pieceInfo in pieceMaterialArray)
            {
                if (pieceInfo.name == teamColor)
                    return pieceInfo.material;
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }

    [Serializable]
    private class PieceType
    {
        public GameObject pieceObject;
        public PIECE_TYPE type;
    }

    [SerializeField] private PieceType[] pieceTypeArray;
    public GameObject GetPieceObject(PIECE_TYPE pieceType)
    {
        try
        {
            foreach (var pieceInfo in pieceTypeArray)
            {
                if (pieceInfo.type == pieceType)
                    return pieceInfo.pieceObject;
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
}
