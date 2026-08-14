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

    void Start()
    {
        gameManager = GameManager.Instance;
        sprite = GetComponent<SpriteRenderer>();
        navAgent = GetComponentInParent<NavMeshAgent>();
    }

    void Update()
    {
        AlignSpriteToPlayer();
        BillboardEffect();
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

