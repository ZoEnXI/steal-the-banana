using UnityEngine;

public abstract class AbilityBase
{
    public float Cooldown { get; protected set; }

    private float lastUsedTime = float.MinValue;

    public bool IsReady => Time.time >= lastUsedTime + Cooldown;

    public void TryExecute(PlayerRoleHandler context)
    {
        if (!IsReady) return;
        lastUsedTime = Time.time;
        Execute(context);
    }

    protected abstract void Execute(PlayerRoleHandler context);

    public float GetCooldownRemaining() => Mathf.Max(0f, lastUsedTime + Cooldown - Time.time);
}
