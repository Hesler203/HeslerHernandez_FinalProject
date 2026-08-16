using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TriggerSeagullDive : MonoBehaviour
{
    private readonly int isDiving = AnimatorManager.IsDivingHash;
    private Animator seagullAnimator;

    void Start()
    {
        seagullAnimator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkyPoint") && !seagullAnimator.GetBool(isDiving))
        {
            seagullAnimator.SetBool(isDiving, true);
            return;
        }
    }
}
