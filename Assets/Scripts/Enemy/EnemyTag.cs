using System;
using Unity.LEGO.Behaviours;
using UnityEngine;

public class EnemyTag : MonoBehaviour
{
    public Action<SensoryCollider> OnSensorDeactivated;
    public SensoryCollider m_SensoryCollider;

    void OnDestroy()
    {
        OnSensorDeactivated?.Invoke(m_SensoryCollider);
    }
}
