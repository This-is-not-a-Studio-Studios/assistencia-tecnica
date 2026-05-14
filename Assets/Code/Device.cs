using UnityEngine;

public class Device : MonoBehaviour
{
    [SerializeField] private Part[] _parts;

    private void Start()
    {
        this._parts = this.GetComponentsInChildren<Part>();
        for (int i = 0; i < this._parts.Length; i++)
        {
            if (!this._parts[i].IsChassis) continue;
            GameObject.FindFirstObjectByType<ManipulationManager>().TrySetAnchorObject(this._parts[i].gameObject);
            break;
        }
    }
}