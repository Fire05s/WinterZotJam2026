using UnityEngine;

public class AlertNPC : MonoBehaviour
{
    private Enemy _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<Enemy>();
    }
    // Trigger collider version of alert sound (constantly check instead of only once)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("NPC")) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.AlertEnemy(transform.position, _owner.IsFleeing());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("NPC")) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.AlertEnemy(transform.position, _owner.IsFleeing());
        }
    }
}
