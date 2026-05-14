using UnityEngine;

public enum PartType
{
    Removable,
    Fixed,
    Chassis,
}

public class Part : MonoBehaviour
{
    [SerializeField] private PartType _partType;
    [SerializeField] private Part[] _blockers;
    [SerializeField] private bool _isRemoved;

    public bool IsChassis { get { return this._partType == PartType.Chassis; } }
    public bool IsRemoved { get { return this._isRemoved; } }
    public bool CanBeFocused
    {
        get
        {
            bool result = true;

            for (int i = 0; i < this._blockers.Length; i++)
            {
                if (this._isRemoved || this._blockers[i]._isRemoved) continue;
                result = false;
                break;
            }

            return result;
        }
    }

    private Transform _parent;
    private Vector3 _anchorPosition;
    private Quaternion _anchorRotation;
    private Vector3 _anchorScale;

    private void Start()
    {
        this._parent = transform.parent;
        this._anchorPosition = this.transform.localPosition;
        this._anchorRotation = this.transform.localRotation;
        this._anchorScale = this.transform.localScale;
    }

    public void TryDoTheThing()
    {
        if (this._partType == PartType.Chassis) return;

        for (int i = 0; i < this._blockers.Length; i++)
        {
            if (this._blockers[i]._isRemoved) continue;
            return;
        }

        this._isRemoved = !this._isRemoved;

        if (this._isRemoved)
        {
            this.transform.parent = null;
            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;
            return;
        }

        this.transform.parent = this._parent;
        this.transform.localPosition = this._anchorPosition;
        this.transform.localRotation = this._anchorRotation;
        this.transform.localScale = this._anchorScale;
    }
}
