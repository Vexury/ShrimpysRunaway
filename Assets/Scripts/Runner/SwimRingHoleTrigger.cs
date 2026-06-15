using UnityEngine;

public class SwimRingHoleTrigger : MonoBehaviour
{
    private SwimRingObstacle parent;

    public void Init(SwimRingObstacle ring) => parent = ring;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RollerController>() != null)
            parent.NotifyPassedThrough();
    }
}
