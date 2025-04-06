using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    public CinemachineCamera followCamera;
    [SerializeField] FusionInput inputProvider;
    [SerializeField] private PlayerSpawner playerSpawner;

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (shutdownReason == ShutdownReason.GameClosed || shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
        {
            ServiceLocator.Get<UIMenu>().BackToMenu(StaticString.GameClosed);
        }
        else
        {
            ServiceLocator.Get<UIMenu>().BackToMenu(StaticString.Disconnected + " , Error Code: " + shutdownReason.ToString());
        }

        ShutdownRunner();
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        ServiceLocator.Get<UIMenu>().BackToMenu(StaticString.Disconnected);
        ShutdownRunner();
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        ServiceLocator.Get<UIMenu>().BackToMenu(StaticString.ConnectFailed);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()

        });

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        playerSpawner.OnPlayerJoined(runner, player);
        ServiceLocator.Get<UIMenu>().HideLoadingLayer();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        playerSpawner.OnPlayerLeft(runner, player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(inputProvider.GetNetworkInput());
    }

    public async void ShutdownRunner()
    {
        if (_runner != null)
        {
            playerSpawner.CleanupAllPlayers(_runner);

            await _runner.Shutdown();

            Destroy(_runner);
            _runner = null;
        }
    }
}