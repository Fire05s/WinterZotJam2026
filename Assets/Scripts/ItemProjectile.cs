using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemProjectile : MonoBehaviour {
    [Header("Pojectile Data")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] float _maxDistance;

    private Vector3 _targetPosition;
    private Vector3 _originPosition;
    private float _distanceTravelled;
    private float _targetDistance;
    private bool grounded = true;

    public GameObject enemy;
    void FixedUpdate() {
        gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + (_targetPosition - transform.position).normalized * _projectileSpeed * Time.fixedDeltaTime);
        _distanceTravelled += ((_targetPosition - transform.position).normalized * _projectileSpeed * Time.fixedDeltaTime).magnitude;

        if (_distanceTravelled >= _targetDistance) {
            grounded = true;
            soundWave();
            this.enabled = false;
        }
    }

    public void InstantiateProjectile(Vector3 target) {
        grounded = false;
        _projectileSpeed = 20;
        _distanceTravelled = 0f;
        _targetPosition = target;
        _originPosition = this.transform.position;
        Vector3 temp = _targetPosition - transform.position;
        _targetDistance = temp.magnitude;
        if (temp.magnitude > _maxDistance) {
            _targetPosition = transform.position + temp.normalized * _maxDistance;
            _targetDistance = _maxDistance;
        }
    }

    [SerializeField] GameObject square; // TODO: REMOVE
    [SerializeField] GameObject circle; // TODO: REMOVE
    [SerializeField] GameObject triangle; // TODO: REMOVE
    private void Ricochet() {
        soundWave();
        Vector2 currentPos = transform.position;
        Vector2 targetPos = _targetPosition; // Assuming _targetPosition is Vector2 or castable
        Vector2 currentDirection = (targetPos - currentPos).normalized;

        // Note: Raycast in 2D returns the hit directly, rather than using an 'out' parameter
        RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDirection, 100f);

        if (hit.collider != null) {
            // 3. Reflect works exactly the same way
            Vector2 reflectDir = Vector2.Reflect(currentDirection, hit.normal);

            // 4. Set new target
            _targetPosition = hit.point + (reflectDir * 5.0f);

            // Debug visualization
            Debug.DrawLine(currentPos, hit.point, Color.red, 2f);
            Debug.DrawRay(hit.point, reflectDir * 5f, Color.green, 2f);
        }
    }

    [SerializeField] float soundRadius;
    private void soundWave() { // Eminate out from radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.gameObject.transform.position, soundRadius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.gameObject.CompareTag("NPC") == false) {
                continue;
            }
            hitCollider.gameObject.GetComponentInChildren<Enemy>().AlertEnemy(this.gameObject.transform.position);
        }
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        if (!grounded) {
            if (collider.gameObject.layer == 6) { // Wall layer
                Ricochet();
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

    private void OnDrawGizmos() { // TODO: REMOVE ONCE DONE SETTING THEM
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.gameObject.transform.position, soundRadius);
    }
}
