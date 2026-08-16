using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SeagullController : MonoBehaviour
{
    private readonly int inChase = AnimatorManager.InChaseHash;
    private readonly int isClimbing = AnimatorManager.IsClimbingHash;
    private readonly int isDiving = AnimatorManager.IsDivingHash;

    private GameManager gameManager; // TODO
    private Animator seagullAnimator;
    private SpriteRenderer sprite;
    private NavMeshAgent navAgent;

    [Header("Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float shadowRayMaxDistance;
    [SerializeField] private Transform shadowTransform;
    private Vector3 shadowInitialScale;

    [Header("Current State")]
    public static SeagullState currentState;
    public enum SeagullState { standby, chasing } // TODO travel

    [Header("Standby")]
    [SerializeField] private float standbySpeed;
    [SerializeField] private float standbyStoppingDistance;
    [SerializeField] private Transform[] standbyTargets;
    private int currentTargetIndex;
    private int nextTargetIndex;

    [Header("Chase")]
    [SerializeField] private bool isChasing;
    [SerializeField] private float minChaseCooldown;
    [SerializeField] private float maxChaseCooldown;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseStoppingDistance;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerSkyPoint;
    private BoxCollider playerSkyPointCollider;

    [Header("Dive")]
    [SerializeField] private SphereCollider beakCollider;
    [SerializeField] private SphereCollider beakColliderFlipped;
    [SerializeField] private float diveSpeed;

    [Header("Climb")]
    [SerializeField] private float climbSpeed;

    void Start()
    {
        gameManager = GameManager.Instance;
        seagullAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        navAgent = GetComponentInParent<NavMeshAgent>();
        playerSkyPointCollider = playerSkyPoint.GetComponent<BoxCollider>();

        shadowInitialScale = transform.localScale;

        navAgent.SetDestination(standbyTargets[nextTargetIndex = 0].position);
        currentTargetIndex = nextTargetIndex++;

        currentState = SeagullState.standby;
        isChasing = false;
        StartCoroutine(nameof(ChaseCooldownTimer));
    }

    void Update()
    {
        UpdateSeagullState();

        AlignSpriteToPlayer();
        BillboardEffect();

        SetShadowPosition();
    }

    private void UpdateSeagullState()
    {
        if (!isChasing)
        {
            currentState = SeagullState.standby;
        } // TODO other states
        else
        {
            currentState = SeagullState.chasing;
        }
    }

    private void AlignSpriteToPlayer()
    {
        Vector3 playerDirection = navAgent.transform.position - playerTransform.position;
        if (Vector3.Cross(navAgent.transform.up, playerDirection).z < 0f)
        {
            sprite.flipX = false;

            beakCollider.enabled = true;
            beakColliderFlipped.enabled = false;
        }
        else
        {
            sprite.flipX = true;

            beakCollider.enabled = false;
            beakColliderFlipped.enabled = true;
        }
    }

    private void BillboardEffect()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    private void SetShadowPosition()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, shadowRayMaxDistance, LayerMask.GetMask("Ground")))
        {
            shadowTransform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            if (hit.distance < shadowInitialScale.x)
            {
                shadowTransform.localScale *= hit.distance / shadowInitialScale.x;
            }
        }
    }

    void LateUpdate()
    {
        UpdateSeagullBehavior();
    }

    private void UpdateSeagullBehavior()
    {
        if (currentState == SeagullState.chasing)
        {
            playerSkyPointCollider.enabled = true;
            Chase();
        }// TODO other state behavior
        else
        {
            playerSkyPointCollider.enabled = false;
            StandbyFlying();
        }
    }

    private void Chase()
    {
        navAgent.SetDestination(playerTransform.position);
        navAgent.stoppingDistance = chaseStoppingDistance;
        navAgent.speed = chaseSpeed;

        if (seagullAnimator.GetBool(isDiving))
        {
            navAgent.speed = diveSpeed;
        }
    }

    private void StandbyFlying()
    {
        navAgent.stoppingDistance = standbyStoppingDistance;
        navAgent.speed = standbySpeed;

        if (seagullAnimator.GetBool(isClimbing))
        {
            navAgent.speed = climbSpeed;
        }

        if (navAgent.transform.position != standbyTargets[currentTargetIndex].position)
        {
            navAgent.SetDestination(standbyTargets[currentTargetIndex].position);
            return;
        }

        if (nextTargetIndex < standbyTargets.Length)
        {
            navAgent.SetDestination(standbyTargets[nextTargetIndex].position);
            currentTargetIndex = nextTargetIndex++;
            return;
        }
        nextTargetIndex = Random.Range(0, standbyTargets.Length);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerController.CurrentState != PlayerController.PlayerState.caught)
        {
            if (sprite.flipX)
            {
                gameManager.PlayerController.TriggerCapture(beakColliderFlipped.transform);
            }
            else
            {
                gameManager.PlayerController.TriggerCapture(beakCollider.transform);
            }
        }

        if (other.CompareTag("Ground"))
        {
            seagullAnimator.SetBool(isDiving, false);
            seagullAnimator.SetBool(isClimbing, true);
            ReturnToStandBy();
        }
    }

    private void ReturnToStandBy(string climbing = "true")
    {
        if (climbing == "false")
        {
            seagullAnimator.SetBool(isClimbing, false);
            return;
        }

        if (isChasing)
        {
            StartCoroutine(nameof(ChaseCooldownTimer));
        }
    }

    IEnumerator ChaseCooldownTimer()
    {
        seagullAnimator.SetBool(inChase, false);
        isChasing = false;

        yield return new WaitForSeconds(Random.Range(minChaseCooldown, maxChaseCooldown));

        isChasing = true;
        seagullAnimator.SetBool(inChase, true);
        StopCoroutine(nameof(ChaseCooldownTimer));
    }

    private void PlaySound(string soundName)
    {
        gameManager.AudioManager.PlaySound(soundName);
    }

    private void StopSound()
    {
        gameManager.AudioManager.StopSound(AudioManager.SFXType.seagull);
    }
}