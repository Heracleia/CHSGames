using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameShape : MonoBehaviour {

    public delegate void ClickAction(GameObject obj);
    public delegate void HoverEnterAction(GameObject obj);
    public delegate void HoverExitAction(GameObject obj);
    public event ClickAction OnClicked;
    public event HoverEnterAction OnHoverEnter;
    public event HoverExitAction OnHoverExit;

    bool MouseHover;

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null && hit.collider.transform == this.transform) {
            if (!MouseHover) {
                MouseHover = true;
                if(OnHoverEnter != null)
                    OnHoverEnter(gameObject);
            }
            if (Input.GetMouseButtonDown(0))
                if(OnClicked != null)
                    OnClicked(gameObject);
        }
        else {
            if (MouseHover) {
                MouseHover = false;
                if(OnHoverExit != null)
                    OnHoverExit(gameObject);
            }
        }
    }
}
