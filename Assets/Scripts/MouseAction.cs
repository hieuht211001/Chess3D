using System;
using UnityEngine;

public class MouseAction : MonoBehaviour
{
    public event Action<Component> OnClick;
    public event Action<Component> OnHoldStart;
    public event Action<Component, Vector2> OnHoldEnd;
    public event Action<Component, Vector2> OnHoldDrag; 

    private bool isHolding = false;
    private bool isDragging = false;
    private float mouseDownTime;
    private float holdThreshold = 0.2f; 

    void OnMouseDown()
    {
        mouseDownTime = Time.time;
        isHolding = true;
    }

    void OnMouseUp()
    {
        float heldTime = Time.time - mouseDownTime;

        if (isDragging)
        {
            isDragging = false;
            OnHoldEnd?.Invoke(Util.GetComponent(this.gameObject),GetMousePosition());
        }
        else if (heldTime < holdThreshold)
        {
            OnClick?.Invoke(Util.GetComponent(this.gameObject));
        }

        isHolding = false;
    }

    private void OnMouseDrag()
    {
        float heldTime = Time.time - mouseDownTime;

        if (heldTime >= holdThreshold && !isDragging)
        {
            isDragging = true;
            OnHoldStart?.Invoke(Util.GetComponent(this.gameObject));
        }

        if (isDragging)
        {
            OnHoldDrag?.Invoke(Util.GetComponent(this.gameObject), GetMousePosition());
        }
    }

    private Vector3 GetMouseWorldPosition(float fixedY = 0f)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }

    private Vector2 GetMousePosition()
    {
        Vector3 mouseWorld = GetMouseWorldPosition(transform.position.y);
        Vector2 dragPos = new Vector2(mouseWorld.x, mouseWorld.z);
        return dragPos;
    }

    void Update()
    {
    }
}
