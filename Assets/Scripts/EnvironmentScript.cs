using UnityEngine;

public class EnvironmentScript : MonoBehaviour
{
    [SerializeField] GameObject directionalPointer;
    [SerializeField] GameObject normalObject;
    [SerializeField] GameObject destroyedObject;
    public bool broken = false;
    [SerializeField] bool multiHit;
    public float soundRadius;
    private Vector2 soundOrigin;
    private enum directionEnum {
        Default, // This should only happen on an error
        None,
        Up,
        Down,
        Left,
        Right,
    }
    private directionEnum direction;
    private Vector2 VecDir;

    private CircleCollider2D soundCircle;

    private void Start() {
        // Sets the direction of the object
        if (!directionalPointer) {
            direction = directionEnum.None;
        } else {
            directionalPointer.SetActive(false);
            VecDir = (Vector2) gameObject.transform.position;
            if (VecDir.y >= 0.5f) {
                direction = directionEnum.Up;
            } else if (VecDir.y < -0.5f) {
                direction = directionEnum.Down;
            } else if (VecDir.x < -0.5f) {
                direction = directionEnum.Left;
            } else if (VecDir.x >= 0.5f) {
                direction = directionEnum.Right;
            }
        }

        soundOrigin = new Vector2(destroyedObject.transform.position.x, destroyedObject.transform.position.y);
    }

    private float offsetVal = 0f; // Offset is to prevent side hits, increase further to reduce possible side hits
    public void hit(Vector2 Position) { // Player's position
        if (multiHit) {
            if (!broken) { // First multi-hit
                collapse();
            } else { // Follow-up multi-hit
                soundWave();
            }
        } else {
            if (!broken) {
                if (direction == directionEnum.None) {
                    collapse();
                } else if (direction == directionEnum.Up && Position.y > gameObject.transform.position.y - offsetVal) { // Player positioned below
                    collapse();
                } else if (direction == directionEnum.Down && Position.y < gameObject.transform.position.y + offsetVal) { // Player positioned above
                    collapse();
                } else if (direction == directionEnum.Left && Position.x < gameObject.transform.position.x + offsetVal) { // Player positioned right
                    collapse();
                } else if (direction == directionEnum.Right && Position.x > gameObject.transform.position.x - offsetVal) { // Player positioned left
                    collapse();
                }
            }
        }
    }

    private void collapse() { // Move player to safety if needed
        broken = true;
        // TODO: Move player to safety if needed
        // TODO: Kill NPCs within range
        normalObject.SetActive(false);
        destroyedObject.SetActive(true);
        //Debug.Log("Play Animation"); // TODO: Implement animation
        soundWave();
    }

    private void soundWave() { // Eminate out from radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(soundOrigin, soundRadius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.gameObject == this.gameObject || hitCollider.gameObject.CompareTag("NPC") == false) {
                continue;
            }
            hitCollider.gameObject.GetComponentInChildren<Enemy>().AlertEnemy(soundOrigin);
        }
    }

    private void OnDrawGizmos() { // TODO: REMOVE ONCE DONE SETTING THEM
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(destroyedObject.transform.position, soundRadius);
    }
}
