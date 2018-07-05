using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public class DebugOnlyAttribute : PropertyAttribute 
{
    public DebugOnlyAttribute()
    {
    }

}