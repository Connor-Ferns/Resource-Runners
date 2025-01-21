using UnityEngine;
using System.Collections;

public class GatheringAgent : MonoBehaviour
{
    public enum ResourceType { Wood, Stone, Gold }

    [Header("Agent Settings")]
    public ResourceType assignedResource;
    public int inventoryCapacity = 10;
    public float resourceCollectionRate = 1f; // Resources gathered per second
    public float moveSpeed = 3f;
    public float gatheringRange = 1f; // Distance from target to start gathering
    public float dropOffTime = 2f; // Time spent delivering resources

    [Header("References")]
    public Transform homeBase;
    public Animator animator;
    public ResourceManager resourceManager;

    private Rigidbody2D rb;
    private int currentInventory = 0;
    private Transform targetPosition;
    private bool isGathering = false;
    private bool isDroppingOff = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindNearestResource();
    }

    private void Update()
    {
        if (isDroppingOff) return; // Prevent other actions during drop-off

        if (targetPosition == null)
        {
            FindNearestResource();
        }
        else
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition.position);

            if (distanceToTarget <= gatheringRange && !isGathering)
            {
                StopMoving();
                animator.SetBool("IsRunning", false); // Stop running animation
                animator.SetBool("IsChopping", true); // Start chopping animation
                StartCoroutine(GatherResource());
            }
            else if (!isGathering && currentInventory < inventoryCapacity)
            {
                MoveTowards(targetPosition.position);
                animator.SetBool("IsRunning", true); // Running animation
                animator.SetBool("IsChopping", false); // Ensure chopping is off
            }
        }

        if (currentInventory >= inventoryCapacity)
        {
            ReturnToBase();
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void FindNearestResource()
    {
        GameObject[] resourceNodes = GameObject.FindGameObjectsWithTag(assignedResource.ToString());
        float nearestDistance = float.MaxValue;

        foreach (var node in resourceNodes)
        {
            float distance = Vector2.Distance(transform.position, node.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                Transform gatheringSpot = node.transform.Find("GatheringSpot");
                targetPosition = gatheringSpot ? gatheringSpot : node.transform;
            }
        }
    }

    private IEnumerator GatherResource()
    {
        isGathering = true;

        while (currentInventory < inventoryCapacity && targetPosition != null)
        {
            yield return new WaitForSeconds(1 / resourceCollectionRate);

            ResourceNode node = targetPosition.GetComponentInParent<ResourceNode>();
            if (node != null && node.Gather(1))
            {
                currentInventory++;
            }
            else
            {
                targetPosition = null;
                animator.SetBool("IsChopping", false); // Stop chopping animation
                FindNearestResource();
                break;
            }
        }

        isGathering = false;
        ReturnToBase();
    }

    private void ReturnToBase()
    {
        Transform gatheringSpot = homeBase.transform.Find("Spot");

        StopMoving();
        MoveTowards(gatheringSpot.position);
        animator.SetBool("IsRunning", true); // Enable running animation
        animator.SetBool("IsChopping", false); // Ensure chopping is off
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HomeBase") && !isDroppingOff)
        {
            StartCoroutine(DropOffResources());
        }
    }

    private IEnumerator DropOffResources()
    {
        isDroppingOff = true;
        StopMoving();

        // Trigger deposit animation (or idle near the home base)
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsChopping", false);
        
        // Simulate drop-off time
        yield return new WaitForSeconds(dropOffTime);

        // Add resources to the ResourceManager
        resourceManager.AddResource(assignedResource.ToString(), currentInventory);

        currentInventory = 0; // Clear inventory after drop-off
        isDroppingOff = false;

        // Resume finding the nearest resource
        FindNearestResource();
    }
}