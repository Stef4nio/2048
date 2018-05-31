using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private Text _gameOverText;
	// Use this for initialization
	void Start ()
	{
	    GetComponent<Image>().enabled = false;
	    _gameOverText.enabled = false;
	}

    public void OnGameLost()
    {
        GetComponent<Image>().enabled = true;
        _gameOverText.enabled = true;
        _gameOverText.text = "Unfortunately, but you lost...";
    }

	// Update is called once per frame
	void Update () {
		
	}
}
