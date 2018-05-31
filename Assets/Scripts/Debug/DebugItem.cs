using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugItem : MonoBehaviour
{
    [SerializeField]
    private Text _id;
    [SerializeField]
    private Text _pos;
    [SerializeField]
    private Text _offset;
    [SerializeField]
    private Text _status;

    private Text _currentValue;

    // Use this for initialization
    void Awake () {
        _currentValue = GetComponent<Text>();
        _currentValue.text = "x";
        _id.text = "x";
    }

    public void SetValue(Cell cell)
    {
        _currentValue.text = (cell!=null) ? cell.value.ToString() : "null";
        _id.text = (cell!=null) ? cell.id.ToString() : "null";
        _pos.text = (cell != null) ? cell.x + "/" + cell.y : "#/#";
        _offset.text = (cell != null) ? cell.offsetX + "/" + cell.offsetY : "#/#";
        if (cell != null)
        {
            _status.text = "";
            if (cell.isNew)
            {
                _status.text += "N ";
            }
            if (cell.isReadyToDestroy)
            {
                _status.text += "D ";
            }
            if (cell.isMultiply)
            {
                _status.text += "M ";
            }
        }
        else
        {
            _status.text = "#";
        }
         
    }

}
