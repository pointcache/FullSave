using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenNoHealth : MonoBehaviour, IHealthAfterDamage
{

    public void AfterDamage(float damage, float current, GameObject from = null)
    {
        if (current <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }

}
