using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SpriteRenderer))]
public class SeagullController : MonoBehaviour
{
    private GameManager gameManager; // TODO
    private SpriteRenderer sprite;
    private NavMeshAgent navAgent;

    [Header("Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float shadowRayMaxDistance;
    [SerializeField] private Transform shadowTransform;
    private Vector3 shadowInitialScale;

    void Start()
    {
        gameManager = GameManager.Instance;
        sprite = GetComponent<SpriteRenderer>();
        navAgent = GetComponentInParent<NavMeshAgent>();

        shadowInitialScale = transform.localScale;

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
}
