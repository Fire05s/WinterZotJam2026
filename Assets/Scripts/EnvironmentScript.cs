using UnityEngine;

public class EnvironmentScript : MonoBehaviour
{
    [SerializeField] GameObject directionalPointer;
    [SerializeField] GameObject normalObject;
    [SerializeField] GameObject destroyedObject;
    public bool broken = false;
    public float soundRadius;
    public GameObject SoundCircle;
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

    public Transform PositionTest; // FOR TESTING ONLY, TODO: REMOVE

    private void Start() {
        if (!directionalPointer) {
            direction = directionEnum.None;
        } else {
            directionalPointer.SetActive(false);
            VecDir = new Vector2(gameObject.transform.up.x, gameObject.transform.up.y);
            if (VecDir.y > 0.5f) {
                direction = directionEnum.Up;
            } else if (VecDir.y < -0.5f) {
                direction = directionEnum.Down;
            } else if (VecDir.x < -0.5f) {
                direction = directionEnum.Left;
            } else if (VecDir.x > 0.5f) {
                direction = directionEnum.Right;
            }
        }
        hit(new Vector2(PositionTest.position.x, PositionTest.position.y)); // FOR TESTING ONLY, TODO: REMOVE
    }

    private float offsetVal = 1f; // Offset is to prevent side hits, increase further to reduce possible side hits
    public void hit(Vector2 Position) { // Player's position
        if (!broken) {
            if (direction == directionEnum.None) {
                Debug.Log("Break!");
                collapse();
            } else if (direction == directionEnum.Up && Position.y < gameObject.transform.position.y - offsetVal) { // Player positioned below
                Debug.Log("Hit up!");
                collapse();
            } else if (direction == directionEnum.Down && Position.y > gameObject.transform.position.y + offsetVal) { // Player positioned above
                Debug.Log("Hit down!");
                collapse();
            } else if (direction == directionEnum.Left && Position.x > gameObject.transform.position.x + offsetVal) { // Player positioned right
                Debug.Log("Hit left!");
                collapse();
            } else if (direction == directionEnum.Right && Position.x < gameObject.transform.position.x - offsetVal) { // Player positioned left
                Debug.Log("Hit right!");
                collapse();
            }
        }
    }

    private void collapse() { // Move player to safety if needed
        broken = true;
        normalObject.SetActive(false);
        destroyedObject.SetActive(true);
        Debug.Log("Play Animation");
        soundWave();
    }

    private void soundWave() { // Eminate out from radius
        Debug.Log("Sound wave distraction");
        SoundCircle.transform.localScale = new Vector3(soundRadius, soundRadius, 1f);
        // Detect everything in range (Make condition for NPC)
    }
}
