using UnityEngine;

public class ThrowableDetector : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Item")) {
            collision.gameObject.GetComponent<ItemProjectile>().enemy = this.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        collision.gameObject.GetComponent<ItemProjectile>().enemy = null;
    }
}
