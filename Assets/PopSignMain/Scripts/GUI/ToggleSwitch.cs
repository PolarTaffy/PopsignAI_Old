using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, Range(0, 1f)]
    protected float sliderValue;
    public bool CurrentValue { get; private set; }

    private bool _previousValue;
    protected Slider _slider;

    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] private AnimationCurve slideEase =
    AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _animateSliderCoroutine;

    [SerializeField] private UnityEvent onToggleOn;
    [SerializeField] private UnityEvent onToggleOff;

    protected Action transitionEffect;

    [SerializeField] private string playerPrefsKey = "";

    protected virtual void OnValidate()
    {
        // sets up toggle
        SetupToggleComponents();
        // loads previously set settings
        LoadState();
    }

    private void SetupToggleComponents()
    {
        if (_slider != null)
            return;

        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            Debug.Log("No slider found!", this);
                return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
        LoadState();
    }

    protected virtual void Awake()
    {
        // sets up toggle switch state name
        if (string.IsNullOrEmpty(playerPrefsKey))
        {
            playerPrefsKey = $"{gameObject.name}_ToggleSwitchState";
        }
        // loads previously set preferences
        SetupSliderComponent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void Toggle()
    {
        SetStateAndStartAnimation(!CurrentValue);
    }

    private void SetStateAndStartAnimation(bool state)
    {
        SetState(state);

        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }
    
    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = CurrentValue ? 1 : 0;

        float time = 0;
        if (animationDuration > 0)
        {
            while (time < animationDuration)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / animationDuration);
                _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                transitionEffect?.Invoke();
                        
                yield return null;
            }
        }

        _slider.value = endValue;
    }

    // ensures that state is based on player prefs
    private void SetState(bool state)
    {
        _previousValue = CurrentValue;
        CurrentValue = state;

        if (_previousValue != CurrentValue)
        {
            if (CurrentValue)
                onToggleOn?.Invoke();
            else
                onToggleOff?.Invoke();
        }

        SaveState();
    }

    // saves player preferences
    private void SaveState()
    {
        if (!string.IsNullOrEmpty(playerPrefsKey))
        {
            PlayerPrefs.SetInt(playerPrefsKey, CurrentValue ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    // loads player preferences
    public void LoadState()
    {
        if (!string.IsNullOrEmpty(playerPrefsKey) && PlayerPrefs.HasKey(playerPrefsKey))
        {
            int savedValue = PlayerPrefs.GetInt(playerPrefsKey);
            bool isOn = savedValue == 1;

            SetState(isOn);
            _slider.value = isOn ? 1 : 0;
        } else {
            // initializes toggle component so that it toggles are automatically enabled when
            // game first opened
            SetState(true);
            _slider.value = 1;
        }
    }
}
