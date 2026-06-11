using UnityEngine;

public class Device : MonoBehaviour
{
    [SerializeField] private Part[] _parts;

    private void Start()
    {
        this._parts = this.GetComponentsInChildren<Part>();
    }

    public Part FindParentPart(GameObject targetObject)
    {
        for (int i = 0; i < this._parts.Length; i++)
        {
            if (targetObject == this._parts[i].PartModel) return this._parts[i];
        }
        return null;
    }
}