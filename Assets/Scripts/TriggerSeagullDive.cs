using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TriggerSeagullDive : MonoBehaviour
{
    private static readonly int DiveHash = Animator.StringToHash("dive");
    private Animator seagullAnimator;

    void Start()
    {
        seagullAnimator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkyPoint"))
        {
            seagullAnimator.SetBool(DiveHash, true);
        }
    }
}
