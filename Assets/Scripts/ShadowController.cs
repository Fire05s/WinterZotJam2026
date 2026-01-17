using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [Header("Darkness View Settings")]
    [SerializeField] private float _initialDarkness = 15f;
    [SerializeField] private float _minDarkness = 8f;
    [SerializeField] private float _maxDarkness = 12f;
    [SerializeField] private float _lerpDuration = 1f;

    [Header("Shadow Material Reference")]
    [SerializeField] private SpriteRenderer _renderer;

    private float _currentDarkness;
    private float _startDarkness;
    private float _targetDarkness;
    private float _lerpTime;
    private bool _forcedSet;

    private void Start()
    {
        _currentDarkness = _initialDarkness;
        SetDarkness(_minDarkness);
    }
    void Update()
    {
        LoopDarkness();
        UpdateDarkness();
    }

    public void SetDarkness(float newTarget)
    {
        _startDarkness = _currentDarkness;
        _targetDarkness = newTarget;
        _lerpTime = 0f;
    }

    private void LoopDarkness()
    {
        if (_currentDarkness == _minDarkness)
        {
            SetDarkness(_maxDarkness);
        }
        else if(_currentDarkness == _maxDarkness)
        {
            SetDarkness(_minDarkness);
        }
    }

    private void UpdateDarkness()
    {
        if (_lerpTime < _lerpDuration)
        {
            _lerpTime += Time.deltaTime;
            float t = _lerpTime / _lerpDuration;

            _currentDarkness = Mathf.Lerp(_startDarkness, _targetDarkness, t);
            _renderer.material.SetFloat("_DarknessStrength", _currentDarkness);
        }
    }
}
