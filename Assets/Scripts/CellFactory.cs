using System.Collections;
using System.Collections.Generic;
using GameLogic2048;
using UnityEngine;

public class CellFactory:ICellFactory<ModelCell>
{

    //TODO:save current ID to playerPrefs
    public int currentID { get; private set; }

    /// <summary>
    /// Loads currentId of the last cell from playerPrefs
    /// </summary>
    /// <param name="_currentID">currentID from previous session</param>
    public void Load(int _currentID)
    {
        currentID = _currentID;
    }

    /// <summary>
    /// Creates and returns a new cell
    /// </summary>
    /// <param name="value">Value of a new cell</param>
    /// <param name="doAnimate">Does a cell need to have animated pop-up</param>
    /// <returns>A new cell</returns>
    public ModelCell CreateCell(int value,bool doAnimate)
    {
        return new ModelCell(value, currentID++,doAnimate);
    }
}
