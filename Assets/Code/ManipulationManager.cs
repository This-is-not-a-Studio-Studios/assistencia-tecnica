using UnityEngine;

public class ManipulationManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] public Device _debugDevide;
    [SerializeField] private GameObject _sphere;

    [SerializeField] private Part _focusedPart;
    [SerializeField] private GameObject _anchorObject;
    [Header("Stuff")]
    [SerializeField] private GameObject _movePlane;
    
    private float _dragDelayValue = 100; // in ms
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

        this._sphere.transform.localPosition = this._movePlaneHitPoint;
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            this._shouldIncrementDragDelay = true;
            this.TryClickPart(PartClickMode.Focus);
        }

        if (this._isDragging && this._focusedPart.PartState == PartState.Socketed)
        {
            this._focusedPart.TryChangeState(PartInteractionMode.Drag);
            this.FindAndSetAnchorObject();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!this._isDragging) { this.TryClickPart(PartClickMode.Interact); }
            else if (this._focusedPart.PartState == PartState.Loose)
            {
                this._focusedPart.TryChangeState(PartInteractionMode.Drag);
                this.FindAndSetAnchorObject();
            }

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
            Part hitPart = this._debugDevide.FindParentPart(hitInfo.transform.gameObject);

            if (!hitPart) return;

            this._movePlane.transform.position = hitInfo.point;
            this._movePlaneHitPoint = hitInfo.point;

            switch (mode)
            {
                case PartClickMode.Interact:
                    if (hitPart == this._focusedPart) { this._focusedPart.TryChangeState(PartInteractionMode.Click); }
                    return;
                case PartClickMode.Focus:
                    this.FocusPart(hitPart);
                    this.FindAndSetAnchorObject();
                    return;
                default: return;
            }
        }
    }

    private void FocusPart(Part part)
    {
        while (part != null)
        {
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

        while (currentPart.PartState != PartState.Loose && currentPart.PartType != PartType.Chassis)
        {
            currentPart = currentPart.ParentPart;
        }

        this._anchorObject = currentPart.PartModel;
        this._dragOffset = this._anchorObject.transform.position - this._movePlaneHitPoint;
        return;
    }

    private void DragAnchorObject()
    {
        if (this._anchorObject == null || this._focusedPart.PartState == PartState.Socketed || !this._isDragging) return;

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (this._isDragging)
        {
            Vector3 origin = this._focusedPart.PartModel.transform.position;
            Vector3 endpoint = this._focusedPart.PartSocket.transform.position;

            Gizmos.color = this._focusedPart.IsNearSocket() ? Color.green : Color.red;
            Gizmos.DrawLine(origin, endpoint);
        }
    }
#endif
}
