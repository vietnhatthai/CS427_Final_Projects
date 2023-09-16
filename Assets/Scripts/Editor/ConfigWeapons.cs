using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponsData", menuName = "Weapons Data", order = 1)]
public class ConfigWeapons : ScriptableObject 
{
    public WeaponData[] weapons;

    public WeaponData GetWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            throw new IndexOutOfRangeException();
        }
        return weapons[index];
    }

    public int Count()
    {
        return weapons.Length;
    }
}
