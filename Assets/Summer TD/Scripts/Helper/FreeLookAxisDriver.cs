using UnityEngine;
using Cinemachine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    // source: https://forum.unity.com/threads/cm-2-3-4-recenter-to-target-heading-stuck-after-first-cycle.691306/#post-4631089
    [RequireComponent(typeof(CinemachineFreeLook)), DisallowMultipleComponent]
    public class FreeLookAxisDriver : MonoBehaviour
    {
        public CinemachineInputAxisDriver _xAxis;
        public CinemachineInputAxisDriver _yAxis;

        private CinemachineFreeLook _freeLook;

        private void Awake()
        {
            _freeLook = GetComponent<CinemachineFreeLook>();
            _freeLook.m_XAxis.m_MaxSpeed = _freeLook.m_XAxis.m_AccelTime = _freeLook.m_XAxis.m_DecelTime = 0;
            _freeLook.m_XAxis.m_InputAxisName = string.Empty;
            _freeLook.m_YAxis.m_MaxSpeed = _freeLook.m_YAxis.m_AccelTime = _freeLook.m_YAxis.m_DecelTime = 0;
            _freeLook.m_YAxis.m_InputAxisName = string.Empty;
        }

        private void OnValidate()
        {
            _xAxis.Validate();
            _yAxis.Validate();
        }

        private void Reset()
        {
            _xAxis = new CinemachineInputAxisDriver
            {
                multiplier = -10f,
                accelTime = 0.1f,
                decelTime = 0.1f,
                name = "Mouse X",
            };
            _yAxis = new CinemachineInputAxisDriver
            {
                multiplier = 0.1f,
                accelTime = 0.1f,
                decelTime = 0.1f,
                name = "Mouse Y",
            };
        }

        private void Update()
        {
            bool changed = _yAxis.Update(Time.deltaTime, ref _freeLook.m_YAxis);
            changed |= _xAxis.Update(Time.deltaTime, ref _freeLook.m_XAxis);
            if (changed)
            {
                _freeLook.m_RecenterToTargetHeading.CancelRecentering();
                _freeLook.m_YAxisRecentering.CancelRecentering();
            }
        }
    }
}
