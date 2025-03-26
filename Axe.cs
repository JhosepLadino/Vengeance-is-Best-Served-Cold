using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Axe : MonoBehaviour
{

    [SerializeField] private GameObject _life1;
    public GameObject Life1 => _life1;

    [SerializeField] private GameObject _life2;
    public GameObject Life2 => _life2;

    [SerializeField] private GameObject _life3;
    public GameObject Life3 => _life3;

    [SerializeField] private GameObject _character;
    public GameObject Character => _character;

    [SerializeField] private GameObject _gameOverGameObject;
    public GameObject GameOverGameObject => _gameOverGameObject;

    [SerializeField] private GameObject _characterLight;
    public GameObject CharacterLight => _characterLight;
    
    private GlockeSound _glockeSound;
    private int lifeCount = 3;
    private void Start()
    {
        lifeCount = (_life1.activeSelf ? 1 : 0) + (_life2.activeSelf ? 1 : 0) + (_life3.activeSelf ? 1 : 0);
        _glockeSound = FindObjectOfType<GlockeSound>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Lifes"))
        {
            AxeSlashes();
            Debug.Log("REcognized");
        }
    }

    private void AxeSlashes()
    {

        if (lifeCount > 0)
        {
            
            if (lifeCount == 3 && _life3 != null)
            {
                _life3.SetActive(false);
            }
            else if (lifeCount == 2 && _life2 != null)
            {
                _life2.SetActive(false);
            }
            else if (lifeCount == 1 && _life1 != null)
            {
                _life1.SetActive(false);
            }

            lifeCount--;

            if (lifeCount <= 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        _character.SetActive(false); 
        _gameOverGameObject.SetActive(true);
        _characterLight.SetActive(false);
        _glockeSound.vignette.SetFloat("_Vignette", 30f);
    }
}
