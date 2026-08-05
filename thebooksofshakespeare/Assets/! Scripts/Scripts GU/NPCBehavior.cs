using UnityEngine;
using UnityEngine.AI;

// Adicionamos o NavMeshAgent como requisito
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

public class NPCBehavior : MonoBehaviour
{
    [Header("Patrulha Aleatória")]
    public float patrolRadius = 8f;
    public float patrolSpeed = 2f;
    public float timeBetweenPoints = 2.0f; // Tempo de espera ao chegar no ponto

    [Header("Interação")]
    public float detectionRange = 12f;
    [Range(0f, 180f)]
    public float detectionAngle = 60f;
    public float chaseSpeed = 4.5f;

    [Header("Passos")]
    public AudioClip footstepClip;
    public float baseStepInterval = 0.45f;
    public float minSpeedForSteps = 0.1f;

    [Header("Referências")]
    public Transform player;
    public Transform eyes;

    [Header("Debug")]
    public bool drawDebug = false;

    // Componentes
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

        // Configurações iniciais do Agent
        agent.speed = patrolSpeed;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f; // Velocidade de rotação
        agent.stoppingDistance = 0.5f;

        PickNewPatrolPoint();

        if (player == null) Debug.LogWarning("NPCBehavior: Arraste o Player!");
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

        // Se o agente chegar perto do destino
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
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void PickNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPos;

        NavMeshHit hit;
        // Tenta encontrar o ponto válido mais próximo no NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void UpdateAnimationParameters()
    {
        // Usamos a velocidade real do NavMeshAgent para animar
        float currentSpeed = agent.velocity.magnitude;
        bool isMoving = currentSpeed > 0.1f;

        anim.SetBool("IsRunning", isMoving);
        anim.SetBool("isParado", !isMoving);
    }

    void HandleFootsteps()
    {
        float speed = agent.velocity.magnitude;

        if (speed > minSpeedForSteps)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                audioSource.PlayOneShot(footstepClip);
                stepTimer = baseStepInterval / (speed * 0.5f + 0.5f);
            }
        }
        else
        {
            stepTimer = 0f;
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

    void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos, patrolRadius);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(agent.destination, 0.5f);
        }
    }
}