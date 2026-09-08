using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardPatrol : MonoBehaviour
{
    [Header("Pontos de Patrulha")]
    public Transform[] waypoints;
    public float waitTimeAtPoint = 2f;

    [Header("Detecção do Player")]
    public Transform player;
    public float viewDistance = 10f;
    public float viewAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Tempo de Reação e Procura")]
    public float timeToDetect = 1.5f;
    public float searchTime = 3f;

    [Header("Indicador de Alerta")]
    public GameObject _detection;

    [Header("Fontes de Áudio")]
    public AudioSource footstepAudioSource;
    public AudioSource alertAudioSource;

    private Animator animator;
    private string isWalkingParam = "IsWalking";
    private string isIdleParam = "IsIdle";
    private string surpriseTriggerParam = "Surprise";

    [Header("Derrota")]
    public GameObject gameOverCanvas;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool playerDetected = false;

    private float currentDetectionTimer = 0f;
    private bool playedAlertSound = false;

    private bool isSearching = false;
    private float searchTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (_detection != null)
        {
            _detection.SetActive(false);
        }

        if (waypoints.Length > 0)
        {
            MoveToNextWaypoint();
        }
    }

    private void Update()
    {
        if (playerDetected)
        {
            AtualizarEstadoAnimacao(false);
            return;
        }

        CheckForPlayer();
        HandleFootsteps();
        UpdateDetectionUI();

        bool isMoving = agent.velocity.sqrMagnitude > 0.01f && !isWaiting && !agent.isStopped;
        AtualizarEstadoAnimacao(isMoving);

        if (waypoints.Length == 0) return;

        if (!agent.isStopped && !isSearching && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
            }

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
        }
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        bool canSeePlayer = false;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                }
            }
        }

        if (canSeePlayer)
        {
            isSearching = false;
            agent.isStopped = true;

            Vector3 lookDir = dirToPlayer;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }

            if (!playedAlertSound)
            {
                if (alertAudioSource != null)
                {
                    alertAudioSource.Play();
                }

                if (animator != null && !string.IsNullOrEmpty(surpriseTriggerParam))
                {
                    animator.SetTrigger(surpriseTriggerParam);
                }

                playedAlertSound = true;
            }

            currentDetectionTimer += Time.deltaTime;

            if (currentDetectionTimer >= timeToDetect)
            {
                GameOver();
            }
        }
        else
        {
            if (currentDetectionTimer > 0f)
            {
                if (!isSearching)
                {
                    isSearching = true;
                    searchTimer = searchTime;
                    agent.isStopped = true;
                }

                searchTimer -= Time.deltaTime;

                if (searchTimer <= 0f)
                {
                    ForgetPlayer();
                }
            }
        }
    }

    private void UpdateDetectionUI()
    {
        bool shouldShowUI = currentDetectionTimer > 0f || isSearching;

        if (_detection != null)
        {
            _detection.SetActive(shouldShowUI);
        }

    }

    private void ForgetPlayer()
    {
        currentDetectionTimer = 0f;
        playedAlertSound = false;
        isSearching = false;
        agent.isStopped = false;

        if (_detection != null)
        {
            _detection.SetActive(false);
        }
    }

    private void HandleFootsteps()
    {
        if (footstepAudioSource == null) return;

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && !isWaiting && !agent.isStopped;

        if (isMoving)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    private void AtualizarEstadoAnimacao(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool(isWalkingParam, isMoving);
            animator.SetBool(isIdleParam, !isMoving);
        }
    }

    private void GameOver()
    {
        playerDetected = true;
        agent.isStopped = true;

        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }

        if (_detection != null)
        {
            _detection.SetActive(false);
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void MoveToNextWaypoint()
    {
        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}