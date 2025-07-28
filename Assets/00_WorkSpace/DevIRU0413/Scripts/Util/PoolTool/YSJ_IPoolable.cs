using System;

public interface YSJ_IPoolable
{
    Action OnSpawn { get; set; }
    Action OnDespawn { get; set; }

    void OnSpawned();
    void OnDespawned();
}
