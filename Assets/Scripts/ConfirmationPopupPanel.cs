using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class ConfirmationPopupPanel : MonoBehaviour
{

    [SerializeField] private Text _text;
    [SerializeField] private Text _yesButtonText;
    [SerializeField] private Text _noButtonText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private GameObject _gameView;

    [Inject]
    private GameModel _model;

    void Awake()
    {
        DOTween.Init();
        _gameView.GetComponent<InputDetecter>().OnPopupButtonClick += OnRestartButtonClick;
        _gameView.GetComponent<InputDetecter>().OnNoButtonClick += OnAnswerButtonClick;
        _gameView.GetComponent<InputDetecter>().OnYesButtonClick += OnAnswerButtonClick;
    }

    private void OnAnswerButtonClick(object sender, EventArgs e)
    {
        Disable();
    }

    // Use this for initialization
    void Start () {
        transform.gameObject.SetActive(false);
        Color tempColor = GetComponent<Image>().color;
        tempColor.a = 0f;
        GetComponent<Image>().color = tempColor;
        tempColor = _text.color;
        tempColor.a = 0f;
        _text.color = tempColor;
        _noButtonText.color = tempColor;
        _yesButtonText.color = tempColor;

    }

    /// <summary>
    /// Reacts to a restartButton click and opens a popup
    /// </summary>
    /// <param name="sender">Standard sender parameter</param>
    /// <param name="e">Standard eventArgs parameter</param>
    private void OnRestartButtonClick(object sender, EventArgs e)
    {
        Enable();
        if (_model.State == GameState.GameOver)
        {
            _yesButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// Shows the popup
    /// </summary>
    public void Enable()
    {
        transform.gameObject.SetActive(true);
        _restartButton.enabled = false;
        _text.DOFade(1f, Config.FadeTime);
        _yesButtonText.DOFade(1f, Config.FadeTime);
        _noButtonText.DOFade(1f, Config.FadeTime);
        GetComponent<Image>().DOFade(0.5f, Config.FadeTime);
    }

    /// <summary>
    /// Hides the popup
    /// </summary>
    public void Disable()
    {
        _text.DOFade(0f, Config.FadeTime);
        _restartButton.enabled = false;
        _yesButtonText.DOFade(0f, Config.FadeTime);
        _noButtonText.DOFade(0f, Config.FadeTime);
        GetComponent<Image>().DOFade(0, 1f).onComplete= () => {
            _restartButton.enabled = true;
            transform.gameObject.SetActive(false);
        };
    }

    // Update is called once per frame
    void Update () {
		
	}
}
