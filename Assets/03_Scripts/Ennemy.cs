using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ennemy : MonoBehaviour
{
    public List<Transform> waypoints;
    private NavMeshAgent navAgent;
    [SerializeField] private Breakable breakable;

    private int currentWaypointIndex = 0;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private float distanceToHitPlayer = 0f;

    private float timer = 0f;

    private enum State
    {
        Moving,
        LookingLeft,
        LookingRight,
        Chasing
    }

    private State currentState = State.Moving;

    private Quaternion originalRotation;
    private Quaternion leftRotation;
    private Quaternion rightRotation;

    [Header("Player Detection")]
    public Transform player;
    public float viewAngle = 60f;
    public float viewDistance = 10f;
    public LayerMask obstructionMask;
    private bool isChasing = false;
    public Light ambientLight;
    public AudioClip alertSound;
    public float updatePlayerPositionEvery = 5f;
    private float lastPlayerPositionUpdate = 0f;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (waypoints.Count == 0 || navAgent == null) return;
        navAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public void ChasePlayerNow()
    {
        if (isChasing) return;

        isChasing = true;
        navAgent.isStopped = false;
        currentState = State.Chasing;
        ambientLight.color = Color.red;
        AudioSource.PlayClipAtPoint(alertSound, transform.position);
    }

    private void Update()
    {
        if (player != null && CanSeePlayer() && !isChasing)
            ChasePlayerNow();

        if (isChasing)
        {
            ChasePlayer();
            return;
        }

        switch (currentState)
        {
            case State.Moving:
                if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    navAgent.isStopped = true;
                    timer = 0f;
                    currentState = State.LookingLeft;

                    originalRotation = transform.rotation;
                    leftRotation = Quaternion.Euler(0f, -90f, 0f) * originalRotation;
                    rightRotation = Quaternion.Euler(0f, 90f, 0f) * originalRotation;
                }
                break;

            case State.LookingLeft:
                RotateTowards(leftRotation);
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    timer = 0f;
                    currentState = State.LookingRight;
                }
                break;

            case State.LookingRight:
                RotateTowards(rightRotation);
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    timer = 0f;
                    GoToNextWaypoint();
                }
                break;
        }
    }

    private void ChasePlayer()
    {
        if (Time.time - lastPlayerPositionUpdate > updatePlayerPositionEvery)
        {
            lastPlayerPositionUpdate = Time.time;
            navAgent.SetDestination(player.position);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= distanceToHitPlayer)
        {
            if (!Player.Instance.CanAttackPlayer()) return;
            if (IsWallBetween(player, distanceToHitPlayer)) return;

            breakable.Knockback();
            Player.Instance.TakeDamage();
        }
    }

    private bool IsWallBetween(Transform target, float distance)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + directionToTarget * distance, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up, directionToTarget, out RaycastHit hit, distance, obstructionMask))
        {
            Debug.DrawLine(transform.position + Vector3.up, hit.point, Color.green);
            return true;
        }
        return false;
    }

    private void RotateTowards(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }

    private void GoToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            if (loop) currentWaypointIndex = 0;
            else return;
        }

        navAgent.isStopped = false;
        navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = State.Moving;
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < viewAngle / 2f
            && Vector3.Distance(transform.position, player.position) <= viewDistance)
        {
            Debug.DrawLine(transform.position + Vector3.up, player.position, Color.green);
            return !IsWallBetween(player, viewDistance + 1f);
        }
        else Debug.DrawLine(transform.position + Vector3.up, player.position, Color.red);

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
        Gizmos.DrawLine(transform.position + leftBoundary * viewDistance, transform.position + rightBoundary * viewDistance);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Ennemy))]
public class EnnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Ennemy ennemy = (Ennemy)target;

        if (GUILayout.Button("Chase Player Now"))
        {
            ennemy.ChasePlayerNow();
        }
    }
}
#endif