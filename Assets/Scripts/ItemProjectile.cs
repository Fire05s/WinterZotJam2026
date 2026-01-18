using UnityEngine;

public class ItemProjectile : MonoBehaviour {
    [Header("Projectile Data")]
    [SerializeField] private float _projectileSpeed = 20f;
    [SerializeField] float _maxDistance;
    [SerializeField] float soundRadius;

    private Vector3 _targetPosition;
    private float _distanceTravelled;
    private float _targetDistance;
    private bool grounded = true;

    // We keep track of the direction for the reflection math
    private Vector2 _moveDirection;

    public GameObject enemy; // If this is a specific target reference

    void FixedUpdate() {
        // 1. Stop if we are grounded/done
        if (grounded) return;

        // Enemy Check (Preserved from your code)
        if (enemy != null) {
            enemy.GetComponentInChildren<Enemy>().KillEnemy();
            Destroy(this.gameObject);
            return;
        }

        // 2. Move Logic
        // Calculate step size for this frame
        float step = _projectileSpeed * Time.fixedDeltaTime;

        // Move the Rigidbody
        Vector2 currentPos = transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, _targetPosition, step);
        GetComponent<Rigidbody2D>().MovePosition(newPos);

        // 3. Track Distance
        // We calculate distance based on actual movement this frame
        _distanceTravelled += Vector2.Distance(currentPos, newPos);

        // 4. Check if we reached the target
        if (_distanceTravelled >= _targetDistance || Vector2.Distance(transform.position, _targetPosition) < 0.05f) {
            StopProjectile();
        }
    }

    public void InstantiateProjectile(Vector3 target) {
        grounded = false;
        _distanceTravelled = 0f;
        this.enabled = true; // Ensure FixedUpdate runs

        // Setup Distance and Direction
        Vector3 directionVector = target - transform.position;
        _targetDistance = Mathf.Min(directionVector.magnitude, _maxDistance);

        // Calculate the clamped target position based on max distance
        _moveDirection = directionVector.normalized;
        _targetPosition = transform.position + (Vector3)(_moveDirection * _targetDistance);
    }

    private void Ricochet(Collision2D collision) {
        // 1. Get the surface normal from the collision contact point
        Vector2 surfaceNormal = collision.contacts[0].normal;

        // 2. Calculate the reflection vector
        // We use our stored _moveDirection so we reflect the direction we were traveling
        Vector2 newDirection = Vector2.Reflect(_moveDirection, surfaceNormal).normalized;

        // 3. Calculate remaining distance
        // How much further were we supposed to go before hitting the wall?
        float remainingDistance = _targetDistance - _distanceTravelled;

        // 4. Update the state for the new path
        _moveDirection = newDirection;
        _targetPosition = (Vector2)transform.position + (newDirection * remainingDistance);

        // Reset distance travelled because we are starting a "new" leg of the journey
        // but we shorten the target distance to whatever was left.
        _distanceTravelled = 0f;
        _targetDistance = remainingDistance;

        // Visual Debugging
        Debug.DrawRay(collision.contacts[0].point, surfaceNormal, Color.yellow, 1f);
        Debug.DrawRay(transform.position, newDirection * 2f, Color.green, 1f);
    }

    private void StopProjectile() {
        grounded = true;
        SoundWave();
        this.enabled = false;
    }

    private void SoundWave() {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, soundRadius);
        AudioManager.Instance.PlayAudio(AudioType.ItemImpact);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("NPC")) // Simplified check
            {
                var enemyScript = hitCollider.GetComponentInChildren<Enemy>();
                if (enemyScript != null) enemyScript.AlertEnemy(transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!grounded) {
            if (collision.gameObject.CompareTag("NPC")) {
                collision.gameObject.GetComponentInChildren<Enemy>().KillEnemy();
                Destroy(this.gameObject);
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (grounded) return;

        // Check for Wall Layer (Layer 6)
        if (collision.gameObject.layer == 6) {
            Ricochet(collision);
        }
    }
}