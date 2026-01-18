using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] private Animator _anim;

    public void TriggerInspection()
    {
        _anim.SetTrigger("question");
    }

    public void TriggerFlee()
    {
        _anim.SetTrigger("exclaimation");
    }
}
