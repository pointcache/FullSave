using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDefinition : SavedScriptable
{
    public enum MuzzleMode
    {
        single,
        all,
        sequential,
        random
    }

    [System.Serializable]
    public struct Muzzle
    {
        public Vector2 pos;
        public float angle;
    }

    public MuzzleMode muzzlemode = MuzzleMode.single;
    public Muzzle[] muzzles = new Muzzle[1];
    public GameObject bulletPrefab;

}
