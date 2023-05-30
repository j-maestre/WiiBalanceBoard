using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class RadialSlider : MonoBehaviour
    {
        private const string PREFS_UI_SAVE_NAME = "Radial";

        [Header("OBJECTS")]
        [SerializeField]
        [CanBeNull]
        private Image sliderImage;
        [SerializeField] [CanBeNull] private Transform indicatorPivot;
        [SerializeField] [CanBeNull] private TextMeshProUGUI valueText;

        [Header("SETTINGS")]
        [SerializeField]
        private int sliderID;
        [SerializeField]
        private float maxValue = 100.0f;
        [SerializeField]
        private float currentValue = 50.0f;
        [SerializeField, Range(0, 8)]
        private int decimals;
        [SerializeField]
        private bool isPercent;
        [SerializeField]
        private bool rememberValue;
        [SerializeField]
        private bool enableCurrentValue;
        [SerializeField] [CanBeNull] private UnityEvent onValueChanged;

        [CanBeNull] private GraphicRaycaster graphicRaycaster;
        [CanBeNull] private RectTransform hitRectTransform;
        private bool isPointerDown;
        private float currentAngle;
        private float currentAngleOnPointerDown;
        private float valueDisplayPrecision;

        /// <summary>
        /// Sets
        /// </summary>
        public float SliderAngle
        {
            get { return currentAngle; }
            set { currentAngle = Mathf.Clamp(value, 0.0f, 360.0f); }
        }

        /// <summary>
        /// Slider value with applied display precision, i.e. the number of decimals to show.
        /// </summary>
        public float SliderValue
        {
            get { return (long)(SliderValueRaw * valueDisplayPrecision) / valueDisplayPrecision; }
            set { SliderValueRaw = value; }
        }

        /// <summary>
        /// Raw slider value, i.e. without any display precision applied to its value.
        /// </summary>
        public float SliderValueRaw
        {
            get { return SliderAngle / 360.0f * maxValue; }
            set { SliderAngle = value * 360.0f / maxValue; }
        }

        private void Awake()
        {
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
            if (graphicRaycaster == null)
            {
                Debug.LogWarning("Could not find GraphicRaycaster component in parent of this GameObject: " + name);
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            valueDisplayPrecision = Mathf.Pow(10, decimals);
            LoadState();
            UpdateUI();

            if (rememberValue == false && enableCurrentValue == true)
            {
                SliderAngle = currentValue * 3.6f;
                UpdateUI();
            }
        }

        public void LoadState()
        {
            if (!rememberValue)
            {
                return;
            }

            currentAngle = PlayerPrefs.GetFloat(sliderID + PREFS_UI_SAVE_NAME);
        }

        public void SaveState()
        {
            if (!rememberValue)
            {
                return;
            }

            PlayerPrefs.SetFloat(sliderID + PREFS_UI_SAVE_NAME, currentAngle);
        }

        public void UpdateUI()
        {
            float normalizedAngle = SliderAngle / 360.0f;

            // Rotate indicator (handle / knob)
            indicatorPivot.transform.localEulerAngles = new Vector3(180.0f, 0.0f, SliderAngle);

            // Update slider fill amount
            sliderImage.fillAmount = normalizedAngle;

            // Update slider label
            valueText.text = string.Format("{0}{1}", SliderValue, isPercent ? "%" : "");
        }

        private bool HasValueChanged()
        {
            return SliderAngle != currentAngleOnPointerDown;
        }
    }
}