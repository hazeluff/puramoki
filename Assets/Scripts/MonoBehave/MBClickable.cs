using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class MBClickable : MonoBehaviour {

    public abstract void Click();

    void OnMouseUpAsButton() {
        if (!IsPointerOverGameObject()) {
            Click();
        }
    }

    // TODO have MB classes inherit this so that ui check can be done for all "clicks"
    private static bool IsPointerOverGameObject() {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
        const int fingerId = 0;
#else
        const int fingerId = -1;
#endif

        return EventSystem.current == null ? false : EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}
