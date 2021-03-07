using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGrid : MonoBehaviour
{
    [SerializeField]
    private DebugItem[] _items;


    public void Print(ModelCell[,] cells)
    {
        
        for (var j = 0; j < cells.GetLength(0); j++)
        {
            for (var i = 0; i < cells.GetLength(1); i++)
            {
                _items[j * 4 + i].SetValue(cells[j, i]);
            }
        }
    }

}
