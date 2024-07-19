using System;
using Fusion;
using UnityEngine;

namespace VisibilityControlPlugin;

public static class NetworkingPatch
{
    private static NetworkPrefabId _id;
    
    public static void Hook()
    {
        RegisterNetworkingPrefab();
        On.GameManager.Spawned += (orig, self) =>
        {
            orig(self);
            self.Runner.Spawn(_id);
        };
    }

    private static void RegisterNetworkingPrefab()
    {
        On.GameManager.Start += (orig, self) =>
        {
            orig(self);
            var prefab = new GameObject("VisibilityControlNetworkPrefab");
            var networkObject = prefab.AddComponent<NetworkObject>();
            networkObject.NetworkedBehaviours = [prefab.AddComponent<VisibilityController>()];
            networkObject.NetworkGuid = NetworkObjectGuid.Parse(Guid.NewGuid().ToString());
            var source = new NetworkPrefabSourceStatic { PrefabReference = networkObject };
            NetworkProjectConfig.Global.PrefabTable.TryAdd(networkObject.NetworkGuid, source, out _id);
        };
    }
}