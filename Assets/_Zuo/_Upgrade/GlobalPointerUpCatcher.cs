using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalPointerUpCatcher : MonoBehaviour, IPointerUpHandler
{
    public static System.Action OnGlobalPointerUp;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnGlobalPointerUp?.Invoke();
    }
}
