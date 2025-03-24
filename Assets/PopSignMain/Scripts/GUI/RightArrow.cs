using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightArrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject line;
    private bool pressed;
    [SerializeField] private float scaler = 0.1f;
    [SerializeField] private Slider slider;

    void Awake() {
        if (slider != null)
        {
            gameObject.SetActive(slider.value > 0);
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void Start()
    {
        if (slider != null)
        {
            OnSliderValueChanged(slider.value);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        line.GetComponent<DrawLine>().arrowDown = true;
        pressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        line.GetComponent<DrawLine>().arrowDown = false;
        pressed = false;
    }

    void Update()
    {
        if(pressed)
        {
            MoveRight();

        }
    }

    public void MoveRight()
    {
        if(line.GetComponent<DrawLine>().dir.y < -3.5f && (line.GetComponent<DrawLine>().dir.x >= -2.5 && line.GetComponent<DrawLine>().dir.x <= 2.5))
            scaler = 0.03f;
        else
            scaler = 0.1f;
        Vector3 dir = line.GetComponent<DrawLine>().dir + (Vector3.right * scaler);
        line.GetComponent<DrawLine>().dir = dir;
    }

    private void OnSliderValueChanged(float value)
    {
        gameObject.SetActive(value > 0);
    }
}
