using UnityEngine;

public class Part : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PartType _partType;
    [SerializeField] private Part _parentPart;
    [SerializeField] private Part[] _blockers;
    [Header("Debug")]
    [SerializeField] private PartState _partState = PartState.Placed;
    [SerializeField] private GameObject _partSocket;
    [SerializeField] private GameObject _partModel;

    public PartType PartType { get { return this._partType; } }
    public Part ParentPart { get { return this._parentPart; } }
    public PartState PartState { get { return this._partState; } }
    public GameObject PartSocket { get { return this._partSocket; } }
    public GameObject PartModel { get { return this._partModel; } }

    private void Awake()
    {
        this._partSocket = this.transform.GetChild(0).gameObject;
        this._partModel = this.transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        if (this._partType != PartType.Chassis)
        {
            this.transform.parent = this._parentPart.PartModel.transform;
        }
    }

    public bool IsBlocked()
    {
        bool result = false;

        for (int i = 0; i < this._blockers.Length; i++)
        {
            if (this._blockers[i].PartState != PartState.Placed) continue;
            
            result = true;
            break;
        }
        return result;
    }

    public bool IsNearSocket()
    {
        return (this._partModel.transform.position - this._partSocket.transform.position).magnitude <= 1f;
    }

    public bool TryChangeState(PartInteractionMode mode)
    {
        switch (mode)
        {
            case PartInteractionMode.Click:
                if (this._partState == PartState.Placed) { this.ChangeState(PartState.Socketed); break; }
                if (this._partState == PartState.Socketed) { this.ChangeState(PartState.Placed); break; }
                return false;

            case PartInteractionMode.Drag:
                if (this._partState == PartState.Socketed) { this.ChangeState(PartState.Loose); break; }
                if (this._partState == PartState.Loose) { this.ChangeState(PartState.Socketed); break; } // rever
                return false;

            default: return false;
        }
        return true;
    }

    private bool ChangeState(PartState newState)
    {
        switch (newState)
        {
            case PartState.Placed:
                if (this.IsBlocked()) { return false; }
                this._partModel.transform.localPosition = Vector3.zero;
                this._partState = PartState.Placed;
                return true;

            case PartState.Socketed:
                if (this._partState == PartState.Placed)
                {
                    if (this.IsBlocked()) { return false; }
                    this._partModel.transform.localPosition = this._partSocket.transform.localPosition;
                }
                else if (this._partState == PartState.Loose)
                {
                    if (!this.IsNearSocket()) { return false; }
                    this._partModel.transform.parent = this.transform;
                    this._partModel.transform.localPosition = this._partSocket.transform.localPosition;
                    this._partModel.transform.localRotation = this._partSocket.transform.localRotation;
                }

                this._partState = PartState.Socketed;
                return true;

            case PartState.Loose:
                this._partModel.transform.parent = GameObject.FindAnyObjectByType<ManipulationManager>()._debugDevide.transform;
                this._partState = PartState.Loose;
                return true;

            default: return false;
        }
    }
}
