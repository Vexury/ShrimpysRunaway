using UnityEngine;

public class RookController : PlayerController
{
    protected override Vector3 ComputeHorizontalMove()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(playerCamera.transform.right,   Vector3.up).normalized;
        Vector3 move = Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y)
            ? camRight   * moveInput.x
            : camForward * moveInput.y;
        return move * CurrentSpeed;
    }
}
