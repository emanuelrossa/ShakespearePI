using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class NPCFollow : MonoBehaviour
{
    [Header("Configurações do Seguidor")]
    public Transform playerTarget;
    public float stoppingDistance = 2.5f; 

    [Header("Som de Passos")]
    public AudioClip footstepClip;
    private AudioSource audioSource;

    [Header("Animação")]
    public Animator animator;

    private NavMeshAgent agent;
    private bool isFollowing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        agent.stoppingDistance = stoppingDistance;

        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        if (playerTarget == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTarget = p.transform;
        }
    }

    private void Update()
    {
        if (isFollowing && playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
            HandleMovementState();
        }
        else
        {
            SetIdleState();
        }
    }

    private void HandleMovementState()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;

        if (animator != null)
        {
            animator.SetBool("IsWalking", isMoving);
            animator.SetBool("IsIdle", !isMoving);
        }

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void SetIdleState()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", true);
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }

    public void StopFollowing()
    {
        isFollowing = false;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
        SetIdleState();
    }
}