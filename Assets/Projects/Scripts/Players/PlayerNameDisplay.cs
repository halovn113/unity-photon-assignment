using Fusion;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : NetworkBehaviour
{
    public TextMeshPro textName;
    public Transform lookAtCamera;
    [Networked, OnChangedRender(nameof(NameChanged))]
    public NetworkString<_32> PlayerName { get; set; }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            string savedName = PlayerPrefs.GetString(PlayerPrefsKeys.PlayerName, StaticString.DefaultName);
            RPC_SetName(savedName);
        }
        else
        {
            UpdateDisplayName();
        }

        if (Camera.main != null)
            lookAtCamera = Camera.main.transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (lookAtCamera == null) return;

        transform.rotation = Quaternion.LookRotation(transform.position - lookAtCamera.position);

        float distance = Vector3.Distance(transform.position, lookAtCamera.position);
        float scale = Mathf.Clamp(1 / distance, 0.8f, 1.5f);
        transform.localScale = Vector3.one * scale;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 32)
            return;

        PlayerName = name;
        RPC_UpdateNameToAll(name);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateNameToAll(string name)
    {
        PlayerName = name;
        UpdateDisplayName();
    }

    private void UpdateDisplayName()
    {
        if (textName != null && !string.IsNullOrEmpty(PlayerName.ToString()))
        {
            textName.text = PlayerName.ToString();
        }
    }

    void NameChanged()
    {
        UpdateDisplayName();
    }

    public override void Render()
    {
        if (lookAtCamera == null) return;

        transform.rotation = Quaternion.LookRotation(transform.position - lookAtCamera.position);

        float distance = Vector3.Distance(transform.position, lookAtCamera.position);
        float scale = Mathf.Clamp(1 / distance, 0.8f, 1.5f);
        transform.localScale = Vector3.one * scale;
    }
}
