using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    private static DebugPanel _instance;

    [SerializeField]
    private DebugGrid _debugGridBefore;
    [SerializeField]
    private DebugGrid _debugGridCurrent;


    private void Awake()
    {
        _instance = this;
    }

    public static DebugPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    public void PrintGridBefore(ModelCell[,] cells)
    {
        _debugGridBefore.Print(cells);
    }

    public void PrintGridCurrent(ModelCell[,] cells)
    {
        _debugGridCurrent.Print(cells);
    }
}
