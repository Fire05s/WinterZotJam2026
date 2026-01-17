using System;
using UnityEngine;

public class ItemProjectile : MonoBehaviour
{
    [Header("Pojectile Data")]
    [SerializeField] private float _projectileSpeed;

    private Vector3 _targetPosition;
    private float _distanceTillLanded = 0.1f;

    void Update()
    {
        MoveToPosition();
    }

    void MoveToPosition()
    {
        transform.position += (_targetPosition - transform.position).normalized * _projectileSpeed * Time.deltaTime;
        

        if (Vector2.Distance(_targetPosition, transform.position) < _distanceTillLanded)
        {
            this.enabled = false;
        }
    }

    public void InstantiateProjectile(Vector3 target)
    {
        _targetPosition = target;
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        
    }
}
