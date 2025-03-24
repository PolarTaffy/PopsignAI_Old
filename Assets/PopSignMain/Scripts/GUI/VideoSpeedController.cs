using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSpeedController : MonoBehaviour
{
     [Header("Components")]
    [SerializeField] private VideoPlayer videoPlayer; // The VideoPlayer component
    [SerializeField] private Slider speedSlider;      // The Slider component

    [Header("Speed Range")]
    [SerializeField] private float minSpeed = 0.5f;   // Minimum playback speed
    [SerializeField] private float maxSpeed = 2.0f;   // Maximum playback speed

    private void Start()
    {
        // Ensure the slider is configured
        if (speedSlider == null || videoPlayer == null)
        {
            Debug.LogError("Slider or VideoPlayer not assigned.");
            return;
        }

        // Set the slider's range and default value
        speedSlider.minValue = minSpeed;
        speedSlider.maxValue = maxSpeed;
        speedSlider.value = 1.0f; // Default playback speed (normal)

        // Add listener to handle slider value changes
        speedSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Update the video playback speed
        if (videoPlayer != null)
        {
            videoPlayer.playbackSpeed = value;
        }
    }
}
