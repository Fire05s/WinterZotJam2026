using System;
using UnityEngine;

public class ItemProjectile : MonoBehaviour
{
    [Header("Pojectile Data")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] float _maxDistance;

    private Vector3 _targetPosition;
    private float _distanceTillLanded = 0.1f;

    void Update()
    {
        transform.position += (_targetPosition - transform.position).normalized * _projectileSpeed * Time.deltaTime;

        if (Vector2.Distance(_targetPosition, transform.position) < _distanceTillLanded) {
            this.enabled = false;
        }
    }

    public void InstantiateProjectile(Vector3 target)
    {
        _targetPosition = target;
        Vector3 temp = _targetPosition - transform.position;
        Debug.Log(temp.magnitude);
        if (temp.magnitude > _maxDistance) {
            _targetPosition = transform.position + temp.normalized * _maxDistance;
        }
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        // TODO: If it is a struct and has pass through then don't bounce back
        if (collider.gameObject.CompareTag("NPC")) {
            collider.gameObject.GetComponent<Enemy>().KillEnemy();
            // TODO: Destroy throwable
        }
        Debug.Log(collider.gameObject.name);
    }
}
