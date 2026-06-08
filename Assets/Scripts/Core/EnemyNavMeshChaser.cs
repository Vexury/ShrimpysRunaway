using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshChaser : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float repathThreshold = 0.5f;

    public static int DetectionCount { get; private set; }

    private NavMeshAgent agent;
    private bool isChasing;
    private Vector3 lastDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target == null || !agent.isOnNavMesh) return;

        bool inRange = Vector3.Distance(transform.position, target.position) <= chaseRange;

        if (inRange)
        {
            if (!isChasing)
            {
                agent.isStopped = false;
                isChasing = true;
                DetectionCount++;
            }

            if (Vector3.Distance(target.position, lastDestination) > repathThreshold)
            {
                lastDestination = target.position;
                agent.SetDestination(lastDestination);
            }
        }
        else if (isChasing)
        {
            agent.isStopped = true;
            agent.ResetPath();
            isChasing = false;
        }
    }
}
