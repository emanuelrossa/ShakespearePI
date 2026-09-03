using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
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

    [Header("Tempo de Reação")]
    public float timeToDetect = 1.5f;
    public float detectionCooldownSpeed = 1f;

    [Header("Sons")]
    public AudioClip footstepClip;
    public AudioClip alertClip;
    private AudioSource audioSource;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private string isWalkingParam = "IsWalking";
    [SerializeField] private string isIdleParam = "IsIdle";

    [Header("Derrota")]
    public GameObject gameOverCanvas;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool playerDetected = false;

    private float currentDetectionTimer = 0f;
    private bool playedAlertSound = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
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

        bool isMoving = agent.velocity.sqrMagnitude > 0.01f && !isWaiting && !agent.isStopped;
        AtualizarEstadoAnimacao(isMoving);

        if (waypoints.Length == 0) return;

        if (!agent.isStopped && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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
            agent.isStopped = true;

            Vector3 lookDir = dirToPlayer;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }

            if (!playedAlertSound && alertClip != null)
            {
                audioSource.PlayOneShot(alertClip);
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
            currentDetectionTimer -= Time.deltaTime * detectionCooldownSpeed;
            currentDetectionTimer = Mathf.Max(0f, currentDetectionTimer);

            if (currentDetectionTimer == 0f)
            {
                playedAlertSound = false;
                agent.isStopped = false;
            }
        }
    }

    private void HandleFootsteps()
    {
        if (playedAlertSound && audioSource.isPlaying && audioSource.clip != footstepClip)
            return;

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && !isWaiting && !agent.isStopped;

        if (isMoving)
        {
            if (footstepClip != null && (!audioSource.isPlaying || audioSource.clip != footstepClip))
            {
                audioSource.clip = footstepClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else if (!isMoving && audioSource.isPlaying && audioSource.clip == footstepClip)
        {
            audioSource.Stop();
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

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
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