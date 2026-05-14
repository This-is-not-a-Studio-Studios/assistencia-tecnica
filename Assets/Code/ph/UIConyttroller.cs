using UnityEngine;

public class UIConyttroller : MonoBehaviour
{
    [SerializeField] private GameObject _clipboard;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { this._clipboard.SetActive(!this._clipboard.activeSelf); }
    }
}
