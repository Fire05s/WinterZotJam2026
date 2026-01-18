using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkController : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float _passiveCooldown = 5f;
    [SerializeField] private float _cooldownVariability = 1f;
    [SerializeField] private float _passiveBlinkDuration = 0.1f;

    [Header("Component References")]
    [SerializeField] private Light2D _visionLight;

    public bool IsBlinking { get; private set; }

    private Coroutine _blinkCoroutine;
    private float _blinkTimer;
    // light radius
    private float _originalOuterRadius;
    private float _originalInnerRadius;

    private void Start()
    {
        _blinkTimer = Random.Range(_passiveCooldown - _cooldownVariability, _passiveCooldown + _cooldownVariability); ;

        // Cache original light values
        _originalOuterRadius = _visionLight.pointLightOuterRadius;
        _originalInnerRadius = _visionLight.pointLightInnerRadius;
    }

    private void Update()
    {
        if (_blinkTimer <= 0)
        {
            PassiveBlink();
            _blinkTimer = Random.Range(_passiveCooldown - _cooldownVariability, _passiveCooldown + _cooldownVariability);
        }
        else
        {
            _blinkTimer -= Time.deltaTime;
        }
    }

    private void PassiveBlink()
    {
        if (IsBlinking) return;
        StartCoroutine(DoPassiveBlink());
    }

    private IEnumerator DoPassiveBlink()
    {
        // Purely visual
        _visionLight.pointLightOuterRadius = 0f;
        _visionLight.pointLightInnerRadius = 0f;
        yield return new WaitForSeconds(_passiveBlinkDuration);
        _visionLight.pointLightOuterRadius = _originalOuterRadius;
        _visionLight.pointLightInnerRadius = _originalInnerRadius;
    }

    public void StartBlink(float closeTime, float duration, float openTime)
    {
        if (IsBlinking) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink(closeTime, duration, openTime));
    }

    private IEnumerator Blink(float close, float duration, float open)
    {
        // Start blink
        IsBlinking = true;
        yield return StartCoroutine(
            AnimateLightRadius(_originalOuterRadius, 0f, close)
        );

        yield return new WaitForSeconds(duration);

        // End blink
        yield return StartCoroutine(
            AnimateLightRadius(0f, _originalOuterRadius, open)
        );
        IsBlinking = false;
    }
    private IEnumerator AnimateLightRadius(float fromOuter, float toOuter, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            // Applies lerp to light radius expanding
            _visionLight.pointLightOuterRadius = Mathf.Lerp(fromOuter, toOuter, lerp);

            // makes inner radius expand slower than outer light radius
            _visionLight.pointLightInnerRadius = Mathf.Lerp(_originalInnerRadius, _originalInnerRadius * (toOuter / _originalOuterRadius), lerp);

            yield return null;
        }

        _visionLight.pointLightOuterRadius = toOuter;
        _visionLight.pointLightInnerRadius = _originalInnerRadius * (toOuter / _originalOuterRadius);
    }
}
