using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.Scripts;
using DG;
using DG.Tweening;

public class CellView : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField]
    private Text _IdText;

    private Cell _cellData;

    public Cell CellData
    {
        get { return _cellData; }
    }

    public void SetCellData(Cell cellData)
    {
        _cellData = cellData;

        UpdateText();
        SetPosition(new Vector3(Config.PaddingX + _cellData.x * (Width + Config.CellViewSpacing),
            -Config.PaddingX-_cellData.y * (Height + Config.CellViewSpacing)));

    }

    private RectTransform _rectTransform;


    public int Width
    {
        get
        {
            GetRectTransform();
            return (int)_rectTransform.rect.width;
        }
    }
    public int Height
    {
        get
        {
            GetRectTransform();
            return (int)_rectTransform.rect.height;
        }
    }

    private void SetText(string text)
    {
        _text.text = text;
    }

    public void SetPosition(Vector2 pos)
    {
        GetRectTransform();
        _rectTransform.localPosition = pos;
    }

    internal void Kill()
    {
        Destroy(gameObject);
    }

    public void UpdateText()
    {
        SetText(_cellData.value.ToString());
        _IdText.text = _cellData.id.ToString();
    }


    private void GetRectTransform()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    public void Move(Action onComplete)
    {
        
        var prevCellData = GameModel.GetPreviousCellById(_cellData.id);
        Debug.Log("MOVE: " + prevCellData.ToString() + " ==> " +  _cellData.ToString());

        transform.DOLocalMove(new Vector3((prevCellData.x+_cellData.offsetX) * (Width + Config.CellViewSpacing) + Config.PaddingX,
            -(prevCellData.y+_cellData.offsetY) * (Height + Config.CellViewSpacing) - Config.PaddingX), Config.MovingTime).onComplete = () =>
        {
            _cellData.offsetX = 0;
            _cellData.offsetY = 0;
            if (onComplete != null)
            {
                onComplete();
            }
        };


        if (_cellData.isMultiply)
        {
            UpdateText();
            _cellData.isMultiply = false;
        }
    }

}
