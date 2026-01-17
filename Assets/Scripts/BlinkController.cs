using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkController : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float _blinkReactionTime = 0.3f;
    [SerializeField] private float _passiveCooldown = 5f;
    [SerializeField] private float _passiveBlinkDuration = 0.1f;

    [Header("Blink Indicator")]
    [SerializeField] private GameObject _blinkIndicator;

    [Header("Component References")]
    [SerializeField] private Light2D _visionLight;

    public bool IsBlinking { get; private set; }

    private Animator _blinkAnimator;
    private Coroutine _blinkCoroutine;
    private float _blinkTimer;
    // light radius
    private float _originalOuterRadius;
    private float _originalInnerRadius;

    private void Start()
    {
        // Disables indicator on start
        if (_blinkIndicator.gameObject.activeSelf) _blinkIndicator.gameObject.SetActive(false);
        _blinkAnimator = _blinkIndicator.GetComponent<Animator>();
        _blinkTimer = _passiveCooldown;

        // Cache original light values
        _originalOuterRadius = _visionLight.pointLightOuterRadius;
        _originalInnerRadius = _visionLight.pointLightInnerRadius;
    }

    private void Update()
    {
        if (_blinkTimer <= 0)
        {
            PassiveBlink();
            _blinkTimer = _passiveCooldown;
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

    public void StartBlink(float closeTime, float duration, float openTime, bool isWarned = false)
    {
        if (IsBlinking) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink(closeTime, duration, openTime, isWarned));
    }

    private IEnumerator Blink(float close, float duration, float open, bool warning)
    {
        // Blink warning
        _blinkIndicator.SetActive(true);
        if (warning) yield return new WaitForSeconds(_blinkReactionTime);

        // Start blink
        IsBlinking = true;
        _blinkAnimator.SetTrigger("StartBlink");
        yield return StartCoroutine(
            AnimateLightRadius(_originalOuterRadius, 0f, close)
        );

        yield return new WaitForSeconds(duration);

        // End blink
        _blinkAnimator.SetTrigger("EndBlink");
        yield return StartCoroutine(
            AnimateLightRadius(0f, _originalOuterRadius, open)
        );

        _blinkIndicator.SetActive(false);
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
