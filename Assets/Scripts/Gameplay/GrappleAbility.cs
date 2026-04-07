using UnityEngine;

public class GrappleAbility : AbilityBase
{
    private const float GrappleRange = 10f;
    private const float GrappleForce = 20f;

    public GrappleAbility(float cooldown)
    {
        Cooldown = cooldown;
    }

    protected override void Execute(PlayerRoleHandler context)
    {
        Transform origin = context.transform;

        if (!Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, GrappleRange))
            return;

        if (hit.rigidbody != null)
        {
            Vector3 pullDirection = (origin.position - hit.point).normalized;
            hit.rigidbody.AddForce(pullDirection * GrappleForce, ForceMode.Impulse);
        }
    }
}
