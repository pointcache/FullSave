using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : SavedComponent
{

    [SerializeField, RuntimeSave]
    private float value = 10;
    public float Value => value;

    [RuntimeSave]
    public Health testReference;

    public delegate void OnDamageReceivedDelegate(float damage, float health, GameObject from = null);
    public event OnDamageReceivedDelegate OnDamageReceived;

    private IHealthBeforeDamage[] before;
    private IHealthAfterDamage[] after;

    private void Start()
    {
        before = GetComponents<IHealthBeforeDamage>();
        after = GetComponents<IHealthAfterDamage>();
    }

    public void ApplyDamage(float amount, GameObject from = null)
    {
        float damage = amount;

        foreach (var item in before)
        {
            damage = item.BeforeDamage(damage, from);
        }

        value -= damage;

        foreach (var item in after)
        {
            item.AfterDamage(damage, value, from);
        }

        OnDamageReceived?.Invoke(damage, value, from);
    }
}

public interface IHealthBeforeDamage
{
    float BeforeDamage(float incoming, GameObject from = null);
}

public interface IHealthAfterDamage
{
    void AfterDamage(float damage, float current, GameObject from = null);
}
