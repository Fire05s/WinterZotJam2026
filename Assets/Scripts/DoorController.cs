using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Animation References")]
    [SerializeField] private AnimationClip _doorOpenClip;

    [SerializeField] private Animator _animator;
    private bool _isOpen = false;
    private bool _isOpening = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (_isOpening || _isOpen) return;
        StartCoroutine(DoorOpening());
    }

    private IEnumerator DoorOpening()
    {
        _isOpening = true;
        _animator.SetTrigger("Open");
        yield return new WaitForSeconds(_doorOpenClip.length);
        _isOpen = true;
        _isOpening = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isOpen) return;

        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.PlayerWin();
        }
        else if (collision.gameObject.tag == "NPC")
        {
            Destroy(collision.gameObject);
            GameManager.Instance.PlayerLose();
        }
    }
}
