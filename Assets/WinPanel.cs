using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WinPanel : MonoBehaviour {

    [SerializeField] private Text _winText;


    // Use this for initialization
    void Awake () {
		Disable();
        
    }
	
	// Update is called once per frame
	public void OnWin () {
		Enable();
	}


    public void Enable()
    {
        transform.gameObject.SetActive(true);
        GetComponent<Image>().DOFade(0.64f, Config.FadeTime);
        _winText.DOFade(1, Config.FadeTime);
    }

    public void Disable()
    {
        GetComponent<Image>().DOFade(0f, Config.FadeTime);
        _winText.DOFade(0, Config.FadeTime);
        transform.gameObject.SetActive(false);
    }
}
