using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshChaser : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRange = 10f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target == null) return;

        if (Vector3.Distance(transform.position, target.position) <= chaseRange)
            agent.SetDestination(target.position);
        else
            agent.ResetPath();
    }
}
