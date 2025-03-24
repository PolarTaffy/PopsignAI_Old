using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    //using TMPro;
    
    public class HoldToSign: MonoBehaviour,IPointerUpHandler
    {
        public bool fingerLifted = false;
        public float lockOutTimeLeft = 1;

        [SerializeField] private Sprite signSprite;
        [SerializeField] private Sprite shootSprite;
        private Image imageComponent;
        // refers to toggle
        [SerializeField] private Slider slider;
    
        public void OnPointerUp(PointerEventData data)
        {
            if(lockOutTimeLeft <= 0f)
            {
                fingerLifted = true;
                lockOutTimeLeft = 1f;
            }
            else if(!GamePlay.Instance.isPopSignAI)
                fingerLifted = true;
        }
        
        void Awake()
        {
            imageComponent = GetComponent<Image>();

            if (slider != null)
            {
                bool isVisible = slider.value > 0;
                gameObject.SetActive(isVisible);

                if (isVisible)
                {
                    imageComponent.sprite = signSprite; // Initialize with the "signSprite"
                }

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
        void Update()
        {
            if(GamePlay.Instance.isPopSignAI)
            {
                if(!fingerLifted && !Input.GetMouseButton(0))
                {
                    lockOutTimeLeft -= Time.deltaTime; 
                }
                if(lockOutTimeLeft <= 0)
                {
                    imageComponent.sprite = shootSprite;
                }
                else
                {
                    imageComponent.sprite = signSprite;
                }   
            }
            else
            {
                imageComponent.sprite = shootSprite;
            }
        }
        public void ToggleShotInput(bool isEnabled)
        {
            //fingerLifted = isEnabled;
        }

        private void OnSliderValueChanged(float value)
        {

            gameObject.SetActive(value > 0);
        }
    }
