using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    [field: SerializeField] public Animator SeagullAnimator { get; private set; }
}
