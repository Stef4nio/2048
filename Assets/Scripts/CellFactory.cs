using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CellFactory
{

    //TODO:save current ID to playerPrefs
    public static int currentID { get; private set; }

    public static void Load(int _currentID)
    {
        currentID = _currentID;
    }

    public static ModelCell CreateCell(int value,bool doAnimate)
    {
        return new ModelCell(value, currentID++,doAnimate);
    }
}
