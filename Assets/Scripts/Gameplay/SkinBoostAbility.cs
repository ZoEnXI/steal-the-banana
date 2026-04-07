using System.Collections;
using UnityEngine;

public class SkinBoostAbility : AbilityBase
{
    public SkinBoostAbility()
    {
        Cooldown = GameConstants.BananaAbilityCooldown;
    }

    protected override void Execute(PlayerRoleHandler context)
    {
        context.StartCoroutine(BoostRoutine(context));
    }

    private IEnumerator BoostRoutine(PlayerRoleHandler context)
    {
        context.ApplySpeedMultiplier(GameConstants.BananaAbilitySpeedMultiplier);
        yield return new WaitForSeconds(GameConstants.BananaAbilityDuration);
        context.RemoveSpeedMultiplier();
    }
}
