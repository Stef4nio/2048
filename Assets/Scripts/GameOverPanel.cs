using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private Text _gameOverText;

    // Use this for initialization
    void Start ()
	{
	    Disable();
	}

    public void OnGameLost()
    {
       Enable();
        _gameOverText.text = "You lost";
    }

    public void Enable()
    {
        transform.gameObject.SetActive(true);
        GetComponent<Image>().DOFade(0.64f, Config.FadeTime);
        _gameOverText.DOFade(1, Config.FadeTime);
    }

    public void Disable()
    {
        transform.gameObject.SetActive(false);
        Color tempColor = GetComponent<Image>().color;
        tempColor.a = 0f;
        GetComponent<Image>().color = tempColor;
        tempColor = _gameOverText.color;
        tempColor.a = 0f;
        _gameOverText.color = tempColor;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
