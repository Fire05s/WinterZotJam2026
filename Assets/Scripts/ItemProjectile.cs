using System;
using UnityEngine;

public class ItemProjectile : MonoBehaviour
{
    [Header("Pojectile Data")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] float _maxDistance;

    private Vector3 _targetPosition;
    private Vector3 _originPosition;
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
        //_projectileSpeed = 10;
        //_distanceTravelled += 3;

        // HORIZONTAL
        square.transform.position = this.gameObject.transform.position; // TODO: REMOVE
        circle.transform.position = _targetPosition; // TODO: REMOVE
        float tempf = 0f;
        if (_originPosition.x >= _targetPosition.x) {
            tempf = _originPosition.x + _targetPosition.x / 2;
        } else {
            tempf = _originPosition.x - _targetPosition.x / 2;
        }
        Vector3 temp = new Vector3(tempf, _originPosition.y, _targetPosition.z); // HORIZONTAL WALL ONLY
        temp = new Vector3(tempf, _originPosition.y, _targetPosition.z); // HORIZONTAL WALL ONLY
        triangle.transform.position = temp; // TODO: REMOVE
        _targetPosition = temp;
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
            if (collider.gameObject.CompareTag("NPC")) {
                collider.gameObject.GetComponentInChildren<Enemy>().KillEnemy();
                Destroy(this.gameObject);
            } else if (collider.gameObject.layer == 6) { // Wall layer
                Ricochet();
            }
        }
    }

    private void OnDrawGizmos() { // TODO: REMOVE ONCE DONE SETTING THEM
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.gameObject.transform.position, soundRadius);
    }
}
