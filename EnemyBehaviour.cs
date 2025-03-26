using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public float radius;
    
    [Range(0, 360)] 
    
    [SerializeField] private float _angle;
    public float Angle => _angle;
    
    [SerializeField] private GameObject _playerRef;
    public GameObject PlayerRef => _playerRef;

    [SerializeField] private Transform _playerPosition;
    public Transform PlayerPosition => _playerPosition;

    [SerializeField] private LayerMask _targetMask;
    public LayerMask TargetMask => _targetMask;

    [SerializeField] private LayerMask _obstructionMask;
    public LayerMask ObstructionMask => _obstructionMask;

    [SerializeField] private LayerMask _buschMask;
    public LayerMask BuschMask => _buschMask;

    [SerializeField] private bool _canSeePlayer;
    public bool CanSeePlayer => _canSeePlayer;

    [SerializeField] private bool _isAttacking;
    public bool IsAttacking => _isAttacking;
    
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float waitTimeAtPoint = 2f;

    [SerializeField] private Transform _pointA;
    public Transform PointA => _pointA;

    [SerializeField] private Transform _pointB;
    public Transform PointB => _pointB;

    //[SerializeField] private AudioManager _audioManager;
    //public AudioManager AudioManager => _audioManager;

    private NavMeshAgent navAgent;
    private Animator feindAnimator;
    private Transform currentTarget;
    private bool isWaiting;
    private bool isChasing;

    [SerializeField] private EventReference Schritten_T;
    [SerializeField] private float rateTemplar = 1f;
    private FMOD.Studio.EventInstance _schrittenT;
    private float timeSchrittenT;
    
    
    private void Start()
    {
        _schrittenT = RuntimeManager.CreateInstance(Schritten_T);
        _playerRef = GameObject.FindGameObjectWithTag("Player");
        navAgent = GetComponent<NavMeshAgent>();
        feindAnimator = GetComponent<Animator>();

        
        currentTarget = _pointA; 
        navAgent.speed = normalSpeed;
        navAgent.SetDestination(currentTarget.position); 
        feindAnimator.SetBool("Walk_T", true);
        feindAnimator.SetBool("Idle", false);
        feindAnimator.SetBool("Angriff", false);

        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        timeSchrittenT += Time.deltaTime;
        
        if (!navAgent.isStopped && !_isAttacking)
        {
            if (timeSchrittenT >= rateTemplar)
            {
                _schrittenT.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform.position));
                _schrittenT.start();
                timeSchrittenT = 0;
            }
        }
        else
        {
            _schrittenT.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        if (_isAttacking)
        {
            return; 
        }

        if (_canSeePlayer && isChasing)
        {
            ChasePlayer();
        }
        else if (!isChasing)
        {
            Patrol();
        }
        
    }
    
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player Lifes"))
        {
            _isAttacking = true;
            AxeAngriff();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player Lifes"))
        {
            _isAttacking = false;
            ResumeDefaultBehavior();
        }
    }

    private void Patrol()
    {
        if (!isWaiting && navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        navAgent.isStopped = true;

        feindAnimator.SetBool("Idle", true);
        feindAnimator.SetBool("Walk_T", false);
        feindAnimator.SetBool("Angriff", false);

        yield return new WaitForSeconds(waitTimeAtPoint);

        currentTarget = currentTarget == _pointA ? _pointB : _pointA; 
        navAgent.SetDestination(currentTarget.position);
        navAgent.isStopped = false;

        feindAnimator.SetBool("Idle", false);
        feindAnimator.SetBool("Walk_T", true);
        feindAnimator.SetBool("Angriff", false);

        isWaiting = false;
    }

    private void ChasePlayer()
    {
        navAgent.speed = chaseSpeed;
        navAgent.SetDestination(_playerPosition.position);

        feindAnimator.SetBool("Idle", false);
        feindAnimator.SetBool("Walk_T", true);
        feindAnimator.SetBool("Angriff", false);
    }

    private void AxeAngriff()
    {
        navAgent.isStopped = true;
        feindAnimator.SetBool("Idle", false);
        feindAnimator.SetBool("Walk_T", false);
        feindAnimator.SetBool("Angriff", true);
        
    }

    private void ResumeDefaultBehavior()
    {
        navAgent.isStopped = false;
        feindAnimator.SetBool("Angriff", false);

        if (_canSeePlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, _targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstructionMask | _buschMask))
                {
                    if (!isChasing)
                    {
                        isChasing = true;
                    }
                    _canSeePlayer = true;
                    return;
                }
            }
        }
        else if (isChasing)
        {
            isChasing = false;
        }
        _canSeePlayer = false;
    }
}
