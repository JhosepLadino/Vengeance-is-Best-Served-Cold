using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BombeKontakt : MonoBehaviour
{
    
    public static event Action OnPotionHit;
    public bool FoeDestruction { get; set; }

    public void Start()
    {
        FoeDestruction = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("A collision with the potion GameObject works");
        if (other.gameObject.CompareTag("Feind"))
        {
            Debug.Log("Feind has been identified");
            FoeDestruction = true;
            OnPotionHit?.Invoke();
        }
        if (other.gameObject.CompareTag("Terrain"))
        {
            OnPotionHit?.Invoke();
        }
    }

   
}
