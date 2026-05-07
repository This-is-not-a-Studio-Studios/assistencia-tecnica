using UnityEngine;

public class ManipulationManager : MonoBehaviour
{
    [SerializeField] private GameObject _focusedPart;

    private bool _isRotatingFocusedPart = false;

    private void Update()
    {
        this.InputHandler();

        this.RotateFocusedPart();
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) { this.TryFocusPart(); }

        if (Input.GetKeyDown(KeyCode.Mouse1)) { this._isRotatingFocusedPart = true; }
        if (Input.GetKeyUp(KeyCode.Mouse1)) { this._isRotatingFocusedPart = false; }
    }

    private void TryFocusPart()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, float.MaxValue, 1 << 31))
        {
            this.FocusPart(hitInfo.collider.gameObject);
        }
    }

    private void FocusPart(GameObject part)
    {
        if (this._focusedPart != part) { this._focusedPart = part; }
    }

    private void RotateFocusedPart()
    {
        if (!this._isRotatingFocusedPart) return;

        Transform device = this._focusedPart.transform.parent;

        device.Rotate(Input.mousePositionDelta.y, 0f, -Input.mousePositionDelta.x, Space.World);
    }
}
