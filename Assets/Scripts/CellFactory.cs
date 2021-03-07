using System.Collections;
using System.Collections.Generic;
using GameLogic2048;
using UnityEngine;

public class CellFactory:ICellFactory<ModelCell>
{

    //TODO:save current ID to playerPrefs
    public int currentID { get; private set; }

    public void Load(int _currentID)
    {
        currentID = _currentID;
    }

    public ModelCell CreateCell(int value,bool doAnimate)
    {
        return new ModelCell(value, currentID++,doAnimate);
    }
}
