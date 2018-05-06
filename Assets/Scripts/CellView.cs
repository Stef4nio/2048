using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CellView : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public Cell Cell;

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

    public void ChangeText()
    {
        SetText(Cell.value.ToString());
    }


    private void GetRectTransform()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

}
