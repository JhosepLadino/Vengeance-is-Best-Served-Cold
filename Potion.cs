using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
        
    public BombeKontakt bombeKontakt;
    public TouchOfDeath touchOfDeath;
    
    [SerializeField] private GameObject _explotion;
    public GameObject Explotion => _explotion;
    
    [SerializeField] private GameObject _enemy;
    public GameObject Enemy => _enemy;
    
    [SerializeField] private GameObject _potionReady;
    public GameObject PotionReady => _potionReady;
    
    [SerializeField] private GameObject _charge1;
    public GameObject Charge1 => _charge1;
    
    [SerializeField] private GameObject _charge2;
    public GameObject Charge2 => _charge2;
    

    private void Update()
    {
        _explotion.transform.position = _enemy.transform.position;
        if (bombeKontakt.FoeDestruction && _potionReady.activeSelf )
        {
            StartCoroutine(ExplotionActivation());
        }
        else
        {
            StopCoroutine(ExplotionActivation());
        }
    }
    
    IEnumerator ExplotionActivation()
    {
        Debug.Log("The enemy was destroyed");
        _explotion.SetActive(true);
        _enemy.SetActive(false);
        yield return new WaitForSecondsRealtime(3/2);
        _explotion.SetActive(false);
        bombeKontakt.FoeDestruction = false;
        _potionReady.SetActive(false);
        _charge1.SetActive(false);
        _charge2.SetActive(false);
        touchOfDeath.Charges = 0;


    }
}
