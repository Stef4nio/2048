using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.Scripts;
using DG;
using DG.Tweening;
using Zenject;

[RequireComponent(typeof(Image))]
public class CellView : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField]
    private Text _IdText;
    private ModelCell _cellData;
    private Sequence _multiplication;
    

    /// <summary>
    /// Returns an inner modelCell
    /// </summary>
    public ModelCell CellData
    {
        get { return _cellData; }
    }

    /// <summary>
    /// Sets a new inner modelCell
    /// </summary>
    /// <param name="cellData">New modelCell</param>
    public void SetCellData(ModelCell cellData)
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

    private void SetTextColor(Color color)
    {
        _text.color = color;
    }

    public void SetPosition(Vector2 pos)
    {
        GetRectTransform();
        _rectTransform.localPosition = pos;
    }

    /// <summary>
    /// Destroys the cell
    /// </summary>
    internal void Kill()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the text on the scene, according to changes in gameModel
    /// </summary>
    public void UpdateText()
    {
        SetText(_cellData.value.ToString());
        if (_cellData.value > 4)
        {
            SetTextColor(Color.white);
        }
        else
        {
            Color myColor;
            ColorUtility.TryParseHtmlString("#5A5A5AFF", out myColor);
            SetTextColor(myColor);
        }
        _IdText.text = _cellData.id.ToString();
    }


    private void GetRectTransform()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Sets a movement animation for the cell
    /// </summary>
    /// <param name="onComplete">A delegate, that allows to add some actions after an animation is finished</param>
    /// <param name="previousState">A previous state of modelCell, to compute an offset</param>
    public void Move(Action onComplete, ModelCell previousState)
    {
        //var previousState = _model.GetPreviousCellById(_cellData.id);
        Debug.Log("MOVE: " + previousState.ToString() + " ==> " +  _cellData.ToString());

        transform.DOLocalMove(new Vector3((previousState.x+_cellData.offsetX) * (Width + Config.CellViewSpacing) + Config.PaddingX,
            -(previousState.y+_cellData.offsetY) * (Height + Config.CellViewSpacing) - Config.PaddingX), Config.MovingTime).onComplete = () =>
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

    /// <summary>
    /// Starts a spawn animation
    /// </summary>
    public void AnimateSpawn()
    {
        transform.localScale = new Vector3(0.1f, 0.1f);
        transform.DOScale(new Vector3(1, 1), Config.SpawningTime);
    }

    /// <summary>
    /// Starts a multiplication animation
    /// </summary>
    public void AnimateMultiplication()
    {
        _multiplication.Append(transform.DOScale(new Vector3(1*Config.MultiplyAnimationScaleMultiplier, 1 * Config.MultiplyAnimationScaleMultiplier), Config.MultiplicationTime));
        _multiplication.AppendInterval(Config.MultiplicationTime);
        _multiplication.Append(transform.DOScale(new Vector3(1, 1), Config.MultiplicationTime));
        _multiplication.Play();
    }



}
