using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private Camera _camera;

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
    
    // Data related to Player Logic
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

    // Helper Functions

    private Vector3 GetCurrentMouseWorldPosition()
    {
        Vector3 mouseDirection = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseDirection.z = 0;
        return mouseDirection;
    }

    // User Input

    public void OnInteract(InputAction.CallbackContext ctx)
    {
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
            ToppleInteraction(closestObject);
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

    private void ToppleInteraction(Collider2D collider)
    {
        collider.transform.parent.GetComponent<EnvironmentScript>().hit((Vector2) this.gameObject.transform.position);
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
        Vector3 mousePosition = GetCurrentMouseWorldPosition();
        Debug.Log(transform.position + mousePosition.normalized * _attackRange);

        AudioManager.Instance.PlayAudio(AudioType.PlayerAttack);

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + mousePosition.normalized * _attackRange, _attackRadius))
        {
            if (collider.CompareTag("NPC"))
            {
                // Deal damage to NPC  
                AudioManager.Instance.PlayAudio(AudioType.NPCDeath);
                collider.gameObject.GetComponent<Enemy>().KillEnemy();
            }
            else if (collider.CompareTag("Struct"))
            {
                // Play attack sound
            }
        }
        yield return new WaitForSeconds(_attackCooldown);
        _canAttack = true;
    }
}
