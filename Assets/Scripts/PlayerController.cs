using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Movement")]
    [SerializeField] private float _speedNormal;
    [SerializeField] private float _speedMultiplier;

    [Header("Interact")]
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private float _interactionCooldown;
    [SerializeField] private float _interactRadius;

    [Header("Attack")]
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackRange;
    [SerializeField] private AnimationClip _attackClip;

    [Header("Slash")]
    [SerializeField] private Transform slashTransform;
    [SerializeField] private GameObject slashObject;
    [SerializeField] private float spriteAngleOffset = -90f;

    // Data related to Player Logic
    private InputSystem_Actions _inputSystem;
    private GameObject _currentlyHeldItem;

    private Vector3 _moveDirection;
    private float _speed;
    private bool _canAttack;
    private bool _isAttacking;


    void Awake()
    {
        _inputSystem = new InputSystem_Actions();

        _inputSystem.Player.Enable();
        _inputSystem.Player.Interact.performed += OnInteract;  
        _inputSystem.Player.Attack.performed += OnAttack;

        _currentlyHeldItem = null;

        _canAttack = true;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Interact.performed -= OnInteract;
        _inputSystem.Player.Attack.performed -= OnAttack;
        _inputSystem.Player.Disable();
        StopAllCoroutines();
    }

    void Start()
    {
        _speed = _speedNormal;
    }

    void Update()
    {
        _moveDirection = _inputSystem.Player.Move.ReadValue<Vector3>().normalized;
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(
            _rb.position + (Vector2)_moveDirection * _speed * Time.fixedDeltaTime
        );
    }

    // Helper Functions

    private Vector3 GetCurrentMouseWorldPosition()
    {
        Vector3 mouseDirection = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseDirection.z = 0;
        return mouseDirection;
    }

    private void UpdateAnimations()
    {
        if (_isAttacking) return;
        if (_moveDirection.magnitude < 0.1f || Mathf.Abs(_moveDirection.y) == 1f)
        {
            // faces front
            _animator.SetBool("isWalking", false);
        }
        else
        {
            _animator.SetBool("isWalking", true);
            _spriteRenderer.flipX = _moveDirection.x > 0f;
        }
    }

    // User Input

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!this) {
            return;
        }
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, _interactRadius, _interactLayer);

        if (colliderArray.Count() == 0) return;

        Collider2D closestObject = colliderArray[0];
        for (int index = 1; index < colliderArray.Count(); index++)
        {
            if (Vector2.Distance(transform.position, closestObject.transform.position) > Vector2.Distance(transform.position, colliderArray[index].transform.position)){
                closestObject = colliderArray[index];
            }
        }
        
        if (closestObject.CompareTag("Struct"))
        {
            StartCoroutine(ToppleInteraction(closestObject));
            _animator.SetTrigger("isScreaming");
        }
        else if (closestObject.CompareTag("Item"))
        {
            PickupInteraction(closestObject);
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!_canAttack) return;

        if (_currentlyHeldItem)
        {
            StartCoroutine(ThrowAttack());
        }
        else
        {
            StartCoroutine(SlashAttack());
        }
    }

    // Functions that Updates Player Data

    public IEnumerator SpeedIncrease(float duration)
    {
        _speed = _speedNormal * _speedMultiplier;
        yield return new WaitForSeconds(duration);
        _speed = _speedNormal;
    }

    // Interact Actions

    private IEnumerator ToppleInteraction(Collider2D collider)
    {
        collider.transform.parent.GetComponent<EnvironmentScript>().hit((Vector2) this.gameObject.transform.position);
        yield return new WaitForSeconds(_interactionCooldown);
    }

    private void PickupInteraction(Collider2D collider)
    {
        if (_currentlyHeldItem) return;

        _currentlyHeldItem = collider.gameObject;
        collider.gameObject.SetActive(value: false);
    }

    // Attack Actions

    private IEnumerator ThrowAttack()
    {
        _canAttack = false;
        _currentlyHeldItem.transform.position = transform.position;
        _currentlyHeldItem.SetActive(value: true);

        ItemProjectile projectile = _currentlyHeldItem.GetComponent<ItemProjectile>();
        projectile.InstantiateProjectile(GetCurrentMouseWorldPosition());
        projectile.enabled = true;

        _currentlyHeldItem = null;
        yield return new WaitForSeconds(_attackCooldown);

        _canAttack = true;
    }

    private IEnumerator SlashAttack()
    {
        _canAttack = false;
        _isAttacking = true;
        Vector3 mousePosition = GetCurrentMouseWorldPosition();
        Vector3 direction = (mousePosition - transform.position).normalized;
        //Debug.Log(transform.position + mousePosition.normalized * _attackRange);

        UpdateSlash(direction);
        slashObject.SetActive(true);

        AudioManager.Instance.PlayAudio(AudioType.PlayerAttack);
        _animator.SetTrigger("isAttacking");
        _spriteRenderer.flipX = direction.x < 0f;

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + direction * _attackRange, _attackRadius))
        {
            if (collider.CompareTag("NPC"))
            {
                // Deal damage to NPC
                collider.gameObject.GetComponentInChildren<Enemy>().KillEnemy();
            }
            else if (collider.CompareTag("Struct"))
            {
                // Play attack sound
            }
        }

        yield return new WaitForSeconds(_attackClip.length);
        slashObject.SetActive(false);
        _isAttacking = false;

        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }

    private void UpdateSlash(Vector3 direction)
    {
        // Position in front of player (local space)
        slashTransform.localPosition = direction * _attackRange;

        // Rotation to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        slashTransform.localRotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }
}
