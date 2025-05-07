using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using UnityEngine.Events;
    using TMPro;
    /// <summary>
    /// class for ui elements that are meant to be controlled by game pad
    /// and when you "click" on it something happens (ui elements meant to be controlled by game-pads)
    /// </summary>
    public abstract class UISelectable 
    {
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


        /// <summary>
        /// This controls what happens when an event is called
        /// </summary>
        public virtual void callEvent()
        {
            assignedEvent?.Invoke();
        }


    }

}
