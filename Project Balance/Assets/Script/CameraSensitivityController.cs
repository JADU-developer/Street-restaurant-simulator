using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraSensitivityController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Slider sensitivitySlider;
    public float minSensitivity = 50f;
    public float maxSensitivity = 500f;

    private CinemachinePOV pov;

    void Start()
    {
        if (virtualCamera != null)
            pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSliderValueChanged);
            // Optionally set initial value
            OnSliderValueChanged(sensitivitySlider.value);
        }
    }

    void OnSliderValueChanged(float value)
    {
        if (pov != null)
        {
            float sensitivity = Mathf.Lerp(minSensitivity, maxSensitivity, value);
            pov.m_HorizontalAxis.m_MaxSpeed = sensitivity;
            pov.m_VerticalAxis.m_MaxSpeed = sensitivity;
        }
    }

    void Update()
    {
        
    }
}
