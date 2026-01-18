using System;
using UnityEngine;

public class ItemProjectile : MonoBehaviour
{
    [Header("Pojectile Data")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] float _maxDistance;

    private Vector3 _targetPosition;
    private float _distanceTravelled;
    private float _targetDistance;
    private bool grounded = true;

    void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody2D>().MovePosition(transform.position + (_targetPosition - transform.position).normalized * _projectileSpeed * Time.fixedDeltaTime);
        _distanceTravelled += ((_targetPosition - transform.position).normalized * _projectileSpeed * Time.fixedDeltaTime).magnitude;

        if (_distanceTravelled >= _targetDistance) {
            grounded = true;
            soundWave();
            this.enabled = false;
        }
    }

    public void InstantiateProjectile(Vector3 target)
    {
        grounded = false;
        _distanceTravelled = 0f;
        _targetPosition = target;
        Vector3 temp = _targetPosition - transform.position;
        _targetDistance = temp.magnitude;
        if (temp.magnitude > _maxDistance) {
            _targetPosition = transform.position + temp.normalized * _maxDistance;
            _targetDistance = _maxDistance;
        }
    }

    [SerializeField] GameObject player;
    private void Ricochet() {
        soundWave();
        _targetDistance -= _distanceTravelled;
        _distanceTravelled = 0f;
        _targetPosition = transform.position; // DETERMINE IF WALL IS HORIZONTAL OR VERTICAL, IF HORIZONTAL, MAKE NEW VEC2 WITH SAME Y AS PLAYER, BUT SAME X
        // IF VERTICAL, SAME X AS PLAYER, BUT SAME Y AS CURRENT PATH

        _targetPosition = new Vector3(this.gameObject.transform.position.x + _distanceTravelled, player.transform.position.y, _targetPosition.z); // HORIZONTAL
        // FIX X
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
                Debug.Log("Hit a wall");
                Ricochet();
            } else if (collider.gameObject.CompareTag("NPC")) {
                collider.gameObject.GetComponentInChildren<Enemy>().KillEnemy();
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDrawGizmos() { // TODO: REMOVE ONCE DONE SETTING THEM
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.gameObject.transform.position, soundRadius);
    }
}
