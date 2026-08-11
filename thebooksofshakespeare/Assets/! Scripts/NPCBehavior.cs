using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

public class NPCBehavior : MonoBehaviour
{
    [Header("Patrulha Aleatória")]
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float timeBetweenPoints = 2.0f;

    [Header("Interação")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField, Range(0f, 180f)] private float detectionAngle = 60f;

    [Header("Passos")]
    [SerializeField] private AudioSource footstepClip;
    [SerializeField] private float baseStepInterval = 0.45f;
    [SerializeField] private float minSpeedForSteps = 0.1f;

    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform eyes;

    [Header("Debug")]
    [SerializeField] private bool drawDebug = false;

    NavMeshAgent agent;
    AudioSource audioSource;
    Animator anim;

    Vector3 startPos;
    float stepTimer = 0f;
    float waitTimer = 0f;

    enum State { Patrol, Chasing }
    State state = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        startPos = transform.position;
        agent.speed = patrolSpeed;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = 0.5f;
        PickNewPatrolPoint();
    }

    void Update()
    {
        bool canSeePlayer = player != null && CheckSeePlayer();

        if (canSeePlayer)
            state = State.Chasing;
        else
            state = State.Patrol;

        switch (state)
        {
            case State.Patrol:
                PatrolLogic();
                break;
            case State.Chasing:
                ChaseLogic();
                break;
        }

        UpdateAnimationParameters();
        HandleFootsteps();
    }

    void PatrolLogic()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= timeBetweenPoints)
            {
                PickNewPatrolPoint();
                waitTimer = 0f;
            }
        }
    }

    void ChaseLogic()
    {
        agent.SetDestination(player.position);
    }

    void PickNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPos;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void UpdateAnimationParameters()
    {
        float currentSpeed = agent.velocity.magnitude;
        bool isMoving = currentSpeed > 0.1f;

        anim.SetBool("IsWalking", isMoving);
        anim.SetBool("IsIdle", !isMoving);
    }

    void HandleFootsteps()
    {
        float speed = agent.velocity.magnitude;

        if (speed > minSpeedForSteps)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                if (!footstepClip.isPlaying)
                {
                    footstepClip.Play();
                }
                stepTimer = baseStepInterval / (speed * 0.5f + 0.5f);
            }
        }
        else
        {
            stepTimer = 0f;
            if (footstepClip.isPlaying)
            {
                footstepClip.Stop();
            }
        }
    }

    bool CheckSeePlayer()
    {
        Vector3 eyePos = (eyes != null) ? eyes.position : transform.position + Vector3.up * 1.5f;
        Vector3 toPlayer = player.position - eyePos;
        float dist = toPlayer.magnitude;

        if (dist > detectionRange) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > detectionAngle) return false;

        RaycastHit hit;
        if (Physics.Raycast(eyePos, toPlayer.normalized, out hit, detectionRange))
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
                return true;
        }
        return false;
    }

}