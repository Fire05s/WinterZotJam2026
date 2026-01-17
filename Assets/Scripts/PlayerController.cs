using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private float _speedNormal;
    [SerializeField] private float _speedMultiplier;

    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private float _interactRadius;

    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackRange;
    
    private InputSystem_Actions _inputSystem;
    private GameObject _currentlyHeldItem;

    private Vector3 _moveDirection;
    private float _speed;
    private bool _canAttack;

    void Awake()
    {
        _inputSystem = new InputSystem_Actions();

        _inputSystem.Player.Enable();
        _inputSystem.Player.Interact.performed += OnInteract;  
        _inputSystem.Player.Attack.performed += OnAttack;

        _currentlyHeldItem = null;

        _canAttack = true;
    }

    void Start()
    {
        _speed = _speedNormal;
    }

    void Update()
    {
        _moveDirection = _inputSystem.Player.Move.ReadValue<Vector3>().normalized;

        transform.position = transform.position + _moveDirection * _speed * Time.deltaTime;
    }

    private Vector3 GetCurrentMouseWorldPosition()
    {
        return _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _interactRadius, _interactLayer);
        if (collider) 
        {
            _currentlyHeldItem = collider.gameObject;
            collider.gameObject.SetActive(value: false);
        }

        if (_currentlyHeldItem) Debug.Log(_currentlyHeldItem.name);
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!_canAttack) return;

        if (_currentlyHeldItem)
        {
            ThrowAttack();
        }
        else
        {
            StartCoroutine(SlashAttack());
        }
    }

    public IEnumerator SpeedIncrease(float duration)
    {
        _speed = _speedNormal * _speedMultiplier;
        yield return new WaitForSeconds(duration);
        _speed = _speedNormal;
    }

    private void ThrowAttack()
    {
        
    }

    private IEnumerator SlashAttack()
    {
        _canAttack = false;
        Vector3 mousePosition = GetCurrentMouseWorldPosition();
        Debug.Log(transform.position + mousePosition.normalized * _attackRange);
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + mousePosition.normalized * _attackRange, _attackRadius))
        {
            if (collider.CompareTag("NPC"))
            {
                // Deal damage to NPC  
            }
            else if (collider.CompareTag("Interactable"))
            {
                // Play attack sound
            }
        }
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
}
