using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSA.InputUI
{
    /// <summary>
    /// Left-side MOBA virtual stick.
    /// </summary>
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform root;
        [SerializeField] RectTransform knob;
        [SerializeField] float maxRadius = 70f;

        public Vector2 Value { get; private set; }

        public void Bind(RectTransform area, RectTransform handle, float radius)
        {
            root = area;
            knob = handle;
            maxRadius = radius;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            if (root == null)
                return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                root, eventData.position, eventData.pressEventCamera, out var local);
            Vector2 clamped = Vector2.ClampMagnitude(local, maxRadius);
            if (knob != null)
                knob.anchoredPosition = clamped;
            Value = maxRadius > 0.01f ? clamped / maxRadius : Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (knob != null)
                knob.anchoredPosition = Vector2.zero;
        }
    }
}
