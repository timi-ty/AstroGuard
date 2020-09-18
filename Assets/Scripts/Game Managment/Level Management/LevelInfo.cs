using System.Collections.Generic;

[System.Serializable]
public struct LevelInfo
{
    public EnvironmentSettings environmentSettings;
    public ObjectSpawnSettings objectSpawnSettings;
    public List<AsteroidSpawnInfo> enemyLineup;
}