using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Vector3 defaultScale;

    void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = defaultScale * 0.92f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }
}
