[System.Serializable]
public enum ObjectiveCategory
{
    //#region Asteroid Destruction
    //TotalAsteroidDestruction,
    //LevelZeroAsteroidDestruction,
    //LevelOneAsteroidDestruction,
    //LevelTwoAsteroidDestruction,
    //SwordAsteroidDestruction,
    //ShipAsteroidDestruction,
    //ShieldAsteroidDestruction,
    //PowerUpOrbAsteroidDestruction,
    //ExplosionAsteroidDestruction,
    //MissileAsteroidDestruction,
    //SlowMotionAsteroidDestruction,
    //DarkRushAsteroidDestruction,
    //AttractionAsteroidDestruction,
    //#endregion

    //#region Bomb Explosions
    //TotalBombExplosions,
    //SwordBombExplosions,
    //ShipBombExplosions,
    //ShieldBombExplosions,
    //ExplosionBombExplosions,
    //MissileBombExplosions,
    //SlowMotionBombExplosions,
    //DarkRushBombExplosions,
    //AttractionBombExplosions,
    //#endregion

    //#region PowerUp Collection
    //TotalPowerUpCollection,
    //SlowMoPowerUpCollection,
    //ShieldPowerUpCollection,
    //AttractorPowerUpCollection,
    //MissileLauncherPowerUpCollection,
    //DarkRushPowerUpCollection,
    //#endregion

    //#region Non-Violent
    //AsteroidEscapes,
    //BombDisposals,
    //#endregion

    //#region Blissful Failure
    //PlayerDeaths,
    //ShipHits
    //#endregion

    TotalAsteroidDestruction,
    SwordBombExplosions,
    LevelZeroAsteroidDestruction,
    TotalPowerUpCollection,
    ShipAsteroidDestruction,
    MissileLauncherPowerUpCollection,
    TotalBombExplosions,
    SlowMotionAsteroidDestruction,
    AttractorPowerUpCollection,
    LevelOneAsteroidDestruction,
    AttractionBombExplosions,
    ExplosionAsteroidDestruction,
    ExplosionBombExplosions,
    ShieldPowerUpCollection,
    PowerUpOrbAsteroidDestruction,
    ShipHits,
    AttractionAsteroidDestruction,
    SlowMotionBombExplosions,
    LevelTwoAsteroidDestruction,
    PlayerDeaths,
    ShipBombExplosions,
    SwordAsteroidDestruction,
    AsteroidEscapes,
    MissileBombExplosions,
    ShieldAsteroidDestruction,
    SlowMoPowerUpCollection,
    MissileAsteroidDestruction,
    BombDisposals,
}
[System.Serializable]
public enum ObjectiveResetCondition
{
    None,
    OnPlayerDeath,
    OnShipDamage
}
[System.Serializable]
public static class ObjectiveAdapter
{
    public static int GetCorrespondingMetric(ObjectiveCategory objectiveCategory)
    {
        switch (objectiveCategory)
        {
            case ObjectiveCategory.TotalAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.total;

            case ObjectiveCategory.LevelZeroAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.levelZero;

            case ObjectiveCategory.LevelOneAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.levelOne;

            case ObjectiveCategory.LevelTwoAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.levelTwo;

            case ObjectiveCategory.SwordAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.bySword;

            case ObjectiveCategory.ShipAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.byShip;

            case ObjectiveCategory.ShieldAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.byShield;

            case ObjectiveCategory.PowerUpOrbAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.byPowerUpOrb;

            case ObjectiveCategory.ExplosionAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.byExplosion;

            case ObjectiveCategory.MissileAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.byMissile;

            case ObjectiveCategory.SlowMotionAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.duringSlowMotion;

            case ObjectiveCategory.AttractionAsteroidDestruction:
                return Metrics.Instance.asteroidDestruction.duringAttraction;

            case ObjectiveCategory.TotalBombExplosions:
                return Metrics.Instance.bombExplosions.total;

            case ObjectiveCategory.SwordBombExplosions:
                return Metrics.Instance.bombExplosions.bySword;

            case ObjectiveCategory.ShipBombExplosions:
                return Metrics.Instance.bombExplosions.byShip;

            case ObjectiveCategory.ExplosionBombExplosions:
                return Metrics.Instance.bombExplosions.byExplosion;

            case ObjectiveCategory.MissileBombExplosions:
                return Metrics.Instance.bombExplosions.byMissile;

            case ObjectiveCategory.SlowMotionBombExplosions:
                return Metrics.Instance.bombExplosions.duringSlowMotion;

            case ObjectiveCategory.AttractionBombExplosions:
                return Metrics.Instance.bombExplosions.duringAttraction;

            case ObjectiveCategory.TotalPowerUpCollection:
                return Metrics.Instance.powerUpCollection.total;

            case ObjectiveCategory.SlowMoPowerUpCollection:
                return Metrics.Instance.powerUpCollection.slowMo;

            case ObjectiveCategory.ShieldPowerUpCollection:
                return Metrics.Instance.powerUpCollection.shield;

            case ObjectiveCategory.AttractorPowerUpCollection:
                return Metrics.Instance.powerUpCollection.attractor;

            case ObjectiveCategory.MissileLauncherPowerUpCollection:
                return Metrics.Instance.powerUpCollection.missileLauncher;

            case ObjectiveCategory.AsteroidEscapes:
                return Metrics.Instance.nonViolent.enemyEscapes;

            case ObjectiveCategory.BombDisposals:
                return Metrics.Instance.nonViolent.bombDisposal;

            case ObjectiveCategory.PlayerDeaths:
                return Metrics.Instance.others.playerDeaths;

            case ObjectiveCategory.ShipHits:
                return Metrics.Instance.others.shipHits;
        }

        return 0;
    }

    public static int GetCorrespondingMetric(ObjectiveResetCondition resetCondition)
    {
        switch (resetCondition)
        {
            case ObjectiveResetCondition.None:
                break;

            case ObjectiveResetCondition.OnPlayerDeath:
                return Metrics.Instance.others.playerDeaths;

            case ObjectiveResetCondition.OnShipDamage:
                return Metrics.Instance.others.shipHits;
        }

        return 0;
    }

    public static string GetCorrespondingDescription(ObjectiveCategory objectiveCategory, ObjectiveResetCondition resetCondition, int count)
    {
        bool isPlural = count > 1;

        string descriptionPrefix = "";
        string descriptionBody = "";
        string descriptionSuffix = "";

        switch (objectiveCategory)
        {
            case ObjectiveCategory.TotalAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids" : "an Asteroid";
                break;
            case ObjectiveCategory.LevelZeroAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Level Zero Asteroids" : "a Level Zero Asteroid";
                break;
            case ObjectiveCategory.LevelOneAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Level One Asteroids" : "a Level One Asteroid";
                break;
            case ObjectiveCategory.LevelTwoAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Level Two Asteroids" : "a Level Two Asteroid";
                break;
            case ObjectiveCategory.SwordAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids with your Blades" : "an Asteroid with your Blades";
                break;
            case ObjectiveCategory.ShipAsteroidDestruction:
                descriptionPrefix = "Allow";
                descriptionBody = isPlural ? "Asteroids to die on your Ship" : "an Asteroid to die on your Ship";
                break;
            case ObjectiveCategory.ShieldAsteroidDestruction:
                descriptionPrefix = "Allow";
                descriptionBody = isPlural ? "Asteroids to die on your Ship's Shield" : "an Asteroid to die on your Ship's Shield";
                break;
            case ObjectiveCategory.PowerUpOrbAsteroidDestruction:
                descriptionPrefix = "Allow";
                descriptionBody = isPlural ? "Asteroids to die on Power-Up Orbs" : "an Asteroid to die on a Power-Up Orb";
                break;
            case ObjectiveCategory.ExplosionAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids with explosions" : "an Asteroid with an explosion";
                break;
            case ObjectiveCategory.MissileAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids with Missiles" : "an Asteroid with a Missile";
                break;
            case ObjectiveCategory.SlowMotionAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids while in Slow Motion" : "an Asteroid while in Slow Motion";
                break;
            case ObjectiveCategory.AttractionAsteroidDestruction:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Asteroids with Attraction" : "an Asteroid with Attraction";
                break;
            case ObjectiveCategory.TotalBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs" : "a Bomb";
                break;
            case ObjectiveCategory.SwordBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs with your Blades" : "a Bomb with your Blades";
                break;
            case ObjectiveCategory.ShipBombExplosions:
                descriptionPrefix = "Allow";
                descriptionBody = isPlural ? "Bombs to explode on your Ship" : "a Bomb to explode on your Ship";
                break;
            case ObjectiveCategory.ExplosionBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs with explosions" : "a Bomb with an explosion";
                break;
            case ObjectiveCategory.MissileBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs with Missiles" : "a Bomb with a Missile";
                break;
            case ObjectiveCategory.SlowMotionBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs while in Slow Motion" : "a Bomb while in Slow Motion";
                break;
            case ObjectiveCategory.AttractionBombExplosions:
                descriptionPrefix = "Destroy";
                descriptionBody = isPlural ? "Bombs with Attraction" : "a Bomb with Attraction";
                break;
            case ObjectiveCategory.TotalPowerUpCollection:
                descriptionPrefix = "Collect";
                descriptionBody = isPlural ? "Power-Ups" : "a Power-Up";
                break;
            case ObjectiveCategory.SlowMoPowerUpCollection:
                descriptionPrefix = "Collect";
                descriptionBody = isPlural ? "Slow-Mo Power-Ups" : "a Slow-Mo Power-Up";
                break;
            case ObjectiveCategory.ShieldPowerUpCollection:
                descriptionPrefix = "Collect";
                descriptionBody = isPlural ? "Shield Power-Ups" : "a Shield Power-Up";
                break;
            case ObjectiveCategory.AttractorPowerUpCollection:
                descriptionPrefix = "Collect";
                descriptionBody = isPlural ? "Attraction Power-Ups" : "an Attraction Power-Up";
                break;
            case ObjectiveCategory.MissileLauncherPowerUpCollection:
                descriptionPrefix = "Collect";
                descriptionBody = isPlural ? "Missile Launcher Power-Ups" : "a Missile Launcher Power-Up";
                break;
            case ObjectiveCategory.AsteroidEscapes:
                descriptionPrefix = "Allow";
                descriptionBody = isPlural ? "Asteroids to escape" : "an Asteroid to escape";
                break;
            case ObjectiveCategory.BombDisposals:
                descriptionPrefix = "Dispose";
                descriptionBody = isPlural ? "Bombs" : "a Bomb";
                break;
            case ObjectiveCategory.PlayerDeaths:
                descriptionPrefix = "Die";
                descriptionBody = isPlural ? "times" : "once";
                break;
            case ObjectiveCategory.ShipHits:
                descriptionPrefix = "Take damage to your Ship";
                descriptionBody = isPlural ? "times" : "once";
                break;
        }

        switch (resetCondition)
        {
            case ObjectiveResetCondition.None:
                descriptionSuffix = ".";
                break;
            case ObjectiveResetCondition.OnPlayerDeath:
                descriptionSuffix = ", without dying.";
                break;
            case ObjectiveResetCondition.OnShipDamage:
                descriptionSuffix = ", with no Ship damage.";
                break;
        }

        string descriptionCount = isPlural ? count.ToString() : "";

        string description = string.Concat(descriptionPrefix, " ", descriptionCount, " ", descriptionBody, descriptionSuffix);

        return description;
    }
}