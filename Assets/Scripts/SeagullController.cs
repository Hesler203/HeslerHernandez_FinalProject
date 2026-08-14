using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SeagullController : MonoBehaviour
{
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
    public SeagullState currentState;
    public enum SeagullState { standby, chasing } // TODO travel

    [Header("Standby")]
    [SerializeField] private float standbySpeed;
    [SerializeField] private float standbyStoppingDistance;
    [SerializeField] private Transform[] standbyTargets;
    private int currentTargetIndex;
    private int nextTargetIndex;

    void Start()
    {
        gameManager = GameManager.Instance;
        seagullAnimator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        navAgent = GetComponentInParent<NavMeshAgent>();

        shadowInitialScale = transform.localScale;

        navAgent.SetDestination(standbyTargets[nextTargetIndex = 0].position);
        currentTargetIndex = nextTargetIndex++;

        currentState = SeagullState.standby;
    }

    void Update()
    {
        AlignSpriteToPlayer();
        BillboardEffect();

        SetShadowPosition();
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
            StandbyFlying();
    }

    private void StandbyFlying()
    {
        navAgent.stoppingDistance = standbyStoppingDistance;
        navAgent.speed = standbySpeed;

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

}
