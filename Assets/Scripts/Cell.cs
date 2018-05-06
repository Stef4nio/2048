using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int value = 0;
    public int offset = 0;
    public int x = 0;
    public int y = 0;
    public bool isNew = false;
    public bool isMultiply = false;

    public Cell(int val)
    {
        isNew = true;
        value = val;
    }

}
