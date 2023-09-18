using System;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemySettingRound1", menuName = "Enemy Setting")]
public class EnemySettings : ScriptableObject
{
    [SerializeField]
    private EnemyData[] m_EnemyData;

    public EnemyData Get(int index)
    {
        if (index < 0 || index >= m_EnemyData.Length)
        {
            throw new IndexOutOfRangeException();
        }
        return m_EnemyData[index];
    }

    public int Count()
    {
        return m_EnemyData.Length;
    }
}