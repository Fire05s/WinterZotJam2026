using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private EnemyState currentState = EnemyState.Patrol;

    private enum EnemyState {
        Idle,
        Patrol,
        Alert
    };

    [Header("Nav Agent")]
    [SerializeField] private NavMeshAgent _agent;

    [Header("Movement")]
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 3.5f;
    [SerializeField] private float _waitTime = 1f;

    [Header("Vision")]
    [SerializeField] private float _viewDistance = 5f;
    [SerializeField] private float _viewAngle = 60f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Transform _fov;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private Transform _exitPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Component References")]
    [SerializeField] private Transform _player;

    private int _patrolIndex;
    private float _waitCounter;
    private Vector2 _lastDirection = Vector2.right; // Facing direction
    private Vector3 _lastAlertedPosition; // Where the alerted enemy will move to

    private void Start()
    {
        //_exitPoint = GameObject.FindGameObjectWithTag("Exit Point").transform;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _waitCounter = _waitTime;
    }

    void Update()
    {
        UpdateFOV();

        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Alert:
                Alert();
                break;
        }
    }

    // STATES
    private void Idle()
    {
        if (_waitCounter <= 0)
        {
            _waitCounter = _waitTime;
            currentState = EnemyState.Patrol;
        }
        else
        {
            _agent.SetDestination(transform.position);
            _waitCounter -= Time.deltaTime;
        }
    }

    private void Patrol()
    {
        Transform target = _patrolPoints[_patrolIndex];
        MoveTowards(target.position, _patrolSpeed);

        // Location reached
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
            currentState = EnemyState.Idle;
        }
    }

    private void Alert()
    {
        MoveTowards(_lastAlertedPosition, _chaseSpeed);

        // Location reached
        if (Vector2.Distance(transform.position, _lastAlertedPosition) < 0.1f)
        {
            currentState = EnemyState.Idle;
        }
    }

    private void Flee()
    {
        MoveTowards(_exitPoint.position, _chaseSpeed);
    }

    // HELPERS
    public void AlertEnemy(Vector2 location)
    {
        _lastAlertedPosition = location;
        currentState = EnemyState.Alert;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        _agent.speed = speed;
        _agent.SetDestination(target);
        _lastDirection = _agent.velocity.normalized;
    }

    private bool CanSeePlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _viewDistance, _playerLayer);
        if (!hit) return false;

        _player = hit.transform;

        Vector2 dirToPlayer = (_player.position - transform.position).normalized;
        float angle = Vector2.Angle(_lastDirection, dirToPlayer);
        if (angle > _viewAngle / 2f) return false;

        RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToPlayer, _viewDistance, _obstacleLayer);

        return ray.collider == null;
    }

    private void UpdateFOV()
    {
        if (_lastDirection.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(_lastDirection.y, _lastDirection.x) * Mathf.Rad2Deg;
        _fov.rotation = Quaternion.Lerp(
            _fov.rotation,
            Quaternion.Euler(0, 0, angle),
            Time.deltaTime * _rotationSpeed
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Vision radius
        Gizmos.DrawWireSphere(transform.position, _viewDistance);

        // Forward direction (lastDirection fallback)
        Vector2 forward = Application.isPlaying
            ? _lastDirection
            : transform.right;

        float halfAngle = _viewAngle * 0.5f;

        Vector2 leftBoundary = Quaternion.Euler(0, 0, halfAngle) * forward;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
            transform.position + (Vector3)(leftBoundary.normalized * _viewDistance));

        Gizmos.DrawLine(transform.position,
            transform.position + (Vector3)(rightBoundary.normalized * _viewDistance));

        // fill cone with lines
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        int segments = 10;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, i / (float)segments);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(dir.normalized * _viewDistance));
        }
    }
}
