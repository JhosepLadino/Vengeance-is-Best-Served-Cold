using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombe : MonoBehaviour
{
    
  [SerializeField] private GameObject _potion;
  public GameObject Potion => _potion;
  
  [SerializeField] private GameObject _mira;
  public GameObject Mira => _mira;
  
  [SerializeField] private Transform _miraInitialPosition;
  public Transform MiraInitialPosition => _miraInitialPosition;
  
  [SerializeField] private Rigidbody _potionRb;
  public Rigidbody PotionRb => _potionRb;
  
  public float flightTime = 2.0f;
  
  [SerializeField] private Transform _potionInitialPosition;
  public Transform PotionInitialPosition => _potionInitialPosition;

  private float spieler;
  private CharacterController spielerController;
  private BoxCollider miraBoxCollider;
  private CharacterController miraCharacterController;
  private Animator characterAnimator;

  public void Start()
  {
    spielerController = GetComponent<CharacterController>();
    _mira.SetActive(false); 
    _potion.SetActive(false);
    _potionRb.useGravity = false;
    _potionRb.isKinematic = true;
    //potionFloating = false;
    miraBoxCollider = _mira.GetComponent<BoxCollider>();
    miraCharacterController = _mira.GetComponent<CharacterController>();
    miraBoxCollider.enabled = false;
    characterAnimator = GetComponent<Animator>();
    //miraInitialPosition = mira.transform.position;

    BombeKontakt.OnPotionHit += ResetPosition;
  }

  private void OnDestroy()
  {
    BombeKontakt.OnPotionHit -= ResetPosition;
  }

  public void Update()
  {
    
    if (spielerController.isGrounded && Input.GetKeyDown(KeyCode.Space))
    {
      Debug.Log("The conditions to invoke the mira were met");
      spielerController.enabled = false;
      miraBoxCollider.enabled = true;
      miraCharacterController.enabled = true;
      _mira.SetActive(true);
      characterAnimator.SetTrigger("WurfVor");
      characterAnimator.SetBool("Ladung", false);
      
      
      // Timer
    }
    if (Input.GetKeyDown(KeyCode.L) && _mira.activeSelf)
    {
      ThrowObject();
    }
    else if (Input.GetKeyUp(KeyCode.Space))
    {
      //StartCoroutine(TimeLapseDeactivation());
      _mira.SetActive(false);
      miraCharacterController.enabled = false;
      miraBoxCollider.enabled = false;
      spielerController.enabled = true;
      _mira.transform.position = _miraInitialPosition.position;
      characterAnimator.SetBool("Ladung", true);
      
    }
  }

  void ThrowObject()
  {
    characterAnimator.SetTrigger("Werfen");
    _potion.transform.rotation = _potion.transform.rotation;
    _potion.transform.position = _potion.transform.position;
    _potion.SetActive(true);
    _potionRb.isKinematic = false;
    Vector3 miraPosition = _mira.transform.position;
    Vector3 potionPosition = _potion.transform.position;
    Vector3 displacement = miraPosition - potionPosition;
    Vector3 velocity = CalculateVelocity(displacement, flightTime);

    _potionRb.useGravity = true;
    _potionRb.velocity = velocity;
    
  }

  Vector3 CalculateVelocity(Vector3 displacement, float time)
  {
    Vector3 velocity = new Vector3();

    velocity.x = displacement.x / time;
    velocity.z = displacement.z / time;
    velocity.y = (displacement.y / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);
    
    return velocity;
  }
  
  void ResetPosition()
  {
    _potion.transform.position = _potionInitialPosition.position;
    _potion.transform.rotation = _potionInitialPosition.rotation;
    _potion.SetActive(false);
    _potionRb.useGravity = false;
    _potionRb.isKinematic = true;
    _potionRb.velocity = Vector3.zero;
  }

  IEnumerator TimeLapseDeactivation()
  {
    yield return new WaitForSecondsRealtime (0f);
    _mira.SetActive(false);
    spielerController.enabled = true;
    _mira.transform.position = _miraInitialPosition.position;
  }
}
