using UnityEngine;

public class RightHandAnim : MonoBehaviour
{
    private Animator _anim;
    [SerializeField] private float _gripVal;
    [SerializeField] private float _triggerVal;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimGrip();
        AnimTrigger();
    }

    private void AnimGrip()
    {
        _gripVal = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        _anim.SetFloat("Grip", _gripVal);
    }

    private void AnimTrigger()
    {
        _triggerVal = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        _anim.SetFloat("Trigger", _triggerVal);
    }
}
