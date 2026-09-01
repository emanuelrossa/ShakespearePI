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

    [Header("Som de Passos")]
    public AudioClip footstepClip;
    private AudioSource audioSource;

    [Header("Derrota")]
    public GameObject gameOverCanvas;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool playerDetected = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
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
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        CheckForPlayer();
        HandleFootsteps();

        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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

    private void HandleFootsteps()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && !isWaiting;

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    GameOver();
                }
            }
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
        }
    }

    private void MoveToNextWaypoint()
    {
        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
}