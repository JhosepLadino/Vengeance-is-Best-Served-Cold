using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOfDeath : MonoBehaviour
{
    [SerializeField] private GameObject _templarModel;
    public GameObject TemplarModel => _templarModel;

    [SerializeField] private GameObject _charge1;
    public GameObject Charge1 => _charge1;

    [SerializeField] private GameObject _charge2;
    public GameObject Charge2 => _charge2;

    [SerializeField] private GameObject _potionReady;
    public GameObject PotionReady => _potionReady;

    [SerializeField] private float _freezedTime;
    public float FreezedTime => _freezedTime;

    [SerializeField] private int _charges; 
    public int Charges { get; set; }
  
    
    private Rigidbody _templarRG;
    private Animator _templarAnimator;
    private Renderer _templarRenderer;
    private bool _isFreezed;

    private void Start()
    {
        _isFreezed = false;
        
        _templarRenderer = _templarModel.GetComponent<Renderer>();
        _templarRenderer.material = new Material(_templarRenderer.material);
        
        _templarRG = gameObject.GetComponent<Rigidbody>();
        _templarAnimator = gameObject.GetComponent<Animator>();

        _charge1.SetActive(false);
        _charge2.SetActive(false);
        _potionReady.SetActive(false);
    }
    
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("The TriggerStay with the templar worked");
        
        if(other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.B) && !_isFreezed && !_potionReady.activeSelf) 
        {
            _isFreezed = true;
            ChargeActivation();
        }
       
    }

    private void ChargeActivation()
    {
        _charges++;
        switch (_charges)
        {
            case 1:
                _charge1.SetActive(true);
                break;
            case 2:
                _charge2.SetActive(true);
                break;
            case 3:
                _potionReady.SetActive(true);
                break;
            default:
                Debug.Log("The charges have been activated");
                break;
        }

        StartCoroutine(EnemyFreezed());
    }
    
    // Is better to tap into the enemies scripts. Theres not enough range of access to influence each enemy independently 
    private IEnumerator EnemyFreezed()
    {
        _templarRenderer.material.SetColor("_BaseColor", Color.cyan);
        _templarAnimator.enabled = false;
        _templarRG.constraints = RigidbodyConstraints.FreezeAll;
        
        yield return new WaitForSecondsRealtime(_freezedTime);
        
        Debug.Log("Coroutine of the normalisation of the color is working");
        _templarRenderer.material.SetColor("_BaseColor", Color.white);
        _templarAnimator.enabled = true;
        _templarRG.constraints = RigidbodyConstraints.None;
        _isFreezed = false;
        
    }
}
