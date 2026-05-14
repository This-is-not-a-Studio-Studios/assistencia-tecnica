using UnityEngine;

public class ManipulationManager : MonoBehaviour
{
    [SerializeField] private Part _focusedPart;
    [SerializeField] private GameObject _anchorObject;

    private float _dragDelayValue = 200; // in ms
    private float _dragDelayTimer = 0;
    private bool _shouldIncrementDragDelay = false;

    private bool _isDragging = false;
    private bool _isRotating = false;

    private Vector3 _movePlaneHitPoint = Vector3.zero;
    private Vector3 _dragOffset = Vector3.zero;

    private void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.MaxValue, 1 << 30)) { this._movePlaneHitPoint = hitInfo.point; }

        this.InputHandler();
        this.UpdateDelayTimer();

        this.DragAnchorObject();
        this.RotateAnchorObject();
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            this._shouldIncrementDragDelay = true;
            this.TryClickPart(PartClickMode.Focus);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!this._isDragging) { this.TryClickPart(PartClickMode.Interact); }

            this._shouldIncrementDragDelay = false;
            this._dragDelayTimer = 0f;
            this._isDragging = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            this._isRotating = true;
            this.FindAndSetAnchorObject();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1)) { this._isRotating = false; }
    }

    private void TryClickPart(PartClickMode mode)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.MaxValue, 1 << 31))
        {
            Part hitPart = hitInfo.collider.gameObject.GetComponent<Part>();

            switch (mode)
            {
                case PartClickMode.Interact:
                    if (hitPart == this._focusedPart) { this._focusedPart.TryDoTheThing(); }
                    return;
                case PartClickMode.Focus:
                    this.FocusPart(hitPart);
                    this.FindAndSetAnchorObject();
                    this._dragOffset = this._anchorObject.transform.position - this._movePlaneHitPoint;
                    return;
                case PartClickMode.Both:
                    if (hitPart == this._focusedPart) { this._focusedPart.TryDoTheThing(); }
                    else { this.FocusPart(hitPart); }
                    return;
                default: return;
            }
        }
    }

    private void FocusPart(Part part)
    {
        while (part != null)
        {
            if (!part.CanBeFocused)
            {
                part = part.transform.parent.GetComponent<Part>();
                continue;
            }

            this._focusedPart = part;
            return;
        }
    }

    public void TrySetAnchorObject(GameObject anchor)
    {
        if (this._anchorObject != null) return;
        
        this._anchorObject = anchor;
    }

    private void FindAndSetAnchorObject()
    {
        if (this._focusedPart == null) return;

        Part currentPart = this._focusedPart;
        Part parentPart = currentPart;

        while (parentPart != null)
        {
            if (parentPart.IsRemoved || parentPart.IsChassis)
            {
                this._anchorObject = parentPart.gameObject;
                return;
            }

            currentPart = parentPart;
            parentPart = currentPart.transform.parent.GetComponent<Part>();
        }

        this._anchorObject = currentPart.transform.parent.gameObject;
        return;
    }

    private void DragAnchorObject()
    {
        if (this._anchorObject == null || !this._isDragging) return;

        this._anchorObject.transform.position = this._movePlaneHitPoint + this._dragOffset;
    }

    private void RotateAnchorObject()
    {
        if (this._anchorObject == null || !this._isRotating) return;

        this._anchorObject.transform.Rotate(Input.mousePositionDelta.y, 0f, -Input.mousePositionDelta.x, Space.World);
    }

    private void UpdateDelayTimer()
    {
        if (!this._shouldIncrementDragDelay) return;

        this._dragDelayTimer += (Time.deltaTime * 1000);
        if (this._dragDelayTimer >= this._dragDelayValue)
        {
            this._isDragging = true;
            this._shouldIncrementDragDelay = false;
        }
    }
}
