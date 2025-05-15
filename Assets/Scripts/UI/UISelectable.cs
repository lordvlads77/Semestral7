using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace UI
{
    [System.Flags]
    public enum UISelectableExtraClasses
    {
        NONE = 0,
        BlockySlider = 0b0000_0001,
        TextSwitcher = 0b0000_0010,
        TextSwitcherMultiLanguage = 0b000_0100,
        AnyTextSwitcher = TextSwitcher | TextSwitcherMultiLanguage,
    }
    /// <summary>
    /// class for ui elements that are meant to be controlled by game pad
    /// and when you "click" on it something happens (ui elements meant to be controlled by game-pads)
    /// </summary>

    public abstract class UISelectable : MonoBehaviour
    {
        /// <summary>
        /// Keeps track of which optional classes the instance of this type has
        /// </summary>
        public UISelectableExtraClasses extraClasses { get; private set; } = UISelectableExtraClasses.NONE;

        [Header("base attributes")]
        /// <summary>
        /// This control the position, size and scale of the UI elements
        /// </summary>
        public RectTransform RectTransform;
        /// <summary>
        /// What happens when you 'click' on the UISelectable
        /// </summary>
        public UnityEvent assignedEvent;
        /// <summary>
        /// Text that is displayed to the User
        /// </summary>
        public TextMeshProUGUI textMeshProUGUI;

        [Header("Optional extra classes")]
        public BlockySlider blockySlider = null;
        public TextSwitcher textSwitcher = null;
        public TextSwitcherMultiLanguage textSwitcherMultiLanguage = null;

        public void Awake()
        {

            if (blockySlider != null)
            {
                extraClasses |= UISelectableExtraClasses.BlockySlider;
            }

            if (textSwitcher != null)
            {
                extraClasses |= UISelectableExtraClasses.TextSwitcher;
            }

            if (textSwitcherMultiLanguage != null)
            {
                extraClasses |= UISelectableExtraClasses.TextSwitcherMultiLanguage;
            }

        }


        /// <summary>
        /// This controls what happens when an event is called
        /// </summary>
        public virtual void callEvent()
        {
            assignedEvent?.Invoke();
        }

        public void setText(string newText)
        {
            textMeshProUGUI.text = newText;
        }

        public void setAnchor(Transform otherTransform, Vector2 offSet)
        {
            RectTransform.SetParent(otherTransform);
            RectTransform.anchoredPosition = offSet;
        }

        public bool hasExtraClass(UISelectableExtraClasses mask)
        {
            return (extraClasses & mask) > 0;
        }


    }

}
