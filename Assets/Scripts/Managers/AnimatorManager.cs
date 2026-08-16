using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [field: SerializeField] public Animator PlayerAnimator { get; private set; }
    [field: SerializeField] public Animator SeagullAnimator { get; private set; }

    public static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    public static readonly int IsRollingHash = Animator.StringToHash("isRolling");
    public static readonly int IsFlippedHash = Animator.StringToHash("isFlipped");
    public static readonly int IsCaughtHash = Animator.StringToHash("isCaught");
    public static readonly int IsFallingHash = Animator.StringToHash("isFalling");

    public static readonly int InChaseHash = Animator.StringToHash("inChase");
    public static readonly int IsDivingHash = Animator.StringToHash("isDiving");
    public static readonly int IsClimbingHash = Animator.StringToHash("isClimbing");
}
