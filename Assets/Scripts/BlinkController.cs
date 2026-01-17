using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkController : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private float _blinkDuration = 1f;
    [SerializeField] private float _blinkCooldown = 10f;
    [SerializeField] private float _blinkCloseTime = 0.15f;
    [SerializeField] private float _blinkOpenTime = 0.2f;

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
        _blinkTimer = _blinkCooldown;

        // Cache original light values
        _originalOuterRadius = _visionLight.pointLightOuterRadius;
        _originalInnerRadius = _visionLight.pointLightInnerRadius;
    }

    private void Update()
    {
        if (_blinkTimer <= 0)
        {
            StartBlink();
            _blinkTimer = _blinkCooldown;
        }
        else
        {
            _blinkTimer -= Time.deltaTime;
        }
    }

    public void StartBlink()
    {
        if (IsBlinking) return;
        if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        // Start blink
        IsBlinking = true;
        _blinkIndicator.SetActive(true);
        yield return StartCoroutine(
            AnimateLightRadius(_originalOuterRadius, 0f, _blinkCloseTime)
        );

        yield return new WaitForSeconds(_blinkDuration);

        // End blink
        _blinkAnimator.SetTrigger("EndBlink");
        yield return StartCoroutine(
            AnimateLightRadius(0f, _originalOuterRadius, _blinkOpenTime)
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
