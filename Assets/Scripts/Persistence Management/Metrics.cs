using System;
[Serializable]
public class Metrics
{
    #region Singleton
    public static Metrics Instance { get; set; } = new Metrics();
    private Metrics() 
    {
        asteroidDestruction = new AsteroidKills();
        bombExplosions = new BombExplosions();
        powerUpCollection = new PowerUpCollection();
        nonViolent = new NonViolent();
        time = new Time();
        others = new Others();
    }
    #endregion

    #region Data
    public AsteroidKills asteroidDestruction { get; }
    public BombExplosions bombExplosions { get; }
    public PowerUpCollection powerUpCollection { get; }
    public NonViolent nonViolent { get; }
    public Time time { get; }
    public Others others { get; }
    #endregion

    #region Action Methods
    public void Wipe()
    {
        Instance = new Metrics();
    }
    #endregion

    #region Logging Methods
    public static void LogAsteroidDeath(AsteroidDeathInfo asteroidDeathInfo, AteroidBase.Type enemyType)
    {
        if(asteroidDeathInfo.killer == AsteroidDeathInfo.Killer.Space)
        {
            Instance.nonViolent.LogEnemyEscape();
        }
        else
        {
            Instance.asteroidDestruction.Log(asteroidDeathInfo, enemyType);
        }
    }

    public static void LogBombExplosion(BombExplosionInfo bombExplosionInfo)
    {
        Instance.bombExplosions.Log(bombExplosionInfo);
    }

    public static void LogBombDisposal()
    {
        Instance.nonViolent.LogBombDisposal();
    }

    public static void LogPlayerDeath()
    {
        Instance.others.LogPlayerDeath();
    }

    public static void LogPowerUpCollection(PowerType powerType)
    {
        Instance.powerUpCollection.Log(powerType);

        FirebaseUtility.RecordCustomEvent("Power Up Collection",
            new Firebase.Analytics.Parameter[]
            {
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemName, powerType.ToString())
            });
    }

    public static void LogPlayTime(uint seconds)
    {
        Instance.time.LogPlayTime(seconds);
    }

    public static void LogGameExit()
    {
        Instance.time.LogDate(DateTime.Now);
    }
    #endregion

    #region Data Structures
    [Serializable]
    public class AsteroidKills
    {
        public int total { get; private set; }
        public int levelZero { get; private set; }
        public int levelOne { get; private set; }
        public int levelTwo { get; private set; }
        public int bySword { get; private set; }
        public int byShip { get; private set; }
        public int byShield { get; private set; }
        public int byPowerUpOrb { get; private set; }
        public int byExplosion { get; private set; }
        public int byMissile { get; private set; }
        public int duringSlowMotion { get; private set; }
        public int duringDarkRush { get; private set; }
        public int duringAttraction { get; private set; }

        public void Log(AsteroidDeathInfo asteroidDeathInfo, AteroidBase.Type type)
        {
            switch (type)
            {
                case AteroidBase.Type.Zero:
                    levelZero++;
                    break;
                case AteroidBase.Type.One:
                    levelOne++;
                    break;
                case AteroidBase.Type.Two:
                    levelTwo++;
                    break;
            }

            switch (asteroidDeathInfo.killer)
            {
                case AsteroidDeathInfo.Killer.Sword:
                    bySword++;
                    break;
                case AsteroidDeathInfo.Killer.Ship:
                    byShip++;
                    break;
                case AsteroidDeathInfo.Killer.Shield:
                    byShield++;
                    break;
                case AsteroidDeathInfo.Killer.PowerUpOrb:
                    byPowerUpOrb++;
                    break;
                case AsteroidDeathInfo.Killer.Explosion:
                    byExplosion++;
                    break;
                case AsteroidDeathInfo.Killer.Projectile:
                    byMissile++;
                    break;
            }

            if (asteroidDeathInfo.deathConditions.duringSlowMotion) duringSlowMotion++;
            if (asteroidDeathInfo.deathConditions.duringAttraction) duringAttraction++;

            total++;
        }
    }

    [Serializable]
    public class BombExplosions
    {
        public int total { get; private set; }
        public int bySword { get; private set; }
        public int byPlayer { get; private set; }
        public int byShip { get; private set; }
        public int byExplosion { get; private set; }
        public int byMissile { get; private set; }
        public int duringSlowMotion { get; private set; }
        public int duringDarkRush { get; private set; }
        public int duringAttraction { get; private set; }

        public void Log(BombExplosionInfo bombExplosionInfo)
        {
            switch (bombExplosionInfo.trigger)
            {
                case BombExplosionInfo.Trigger.Sword:
                    bySword++;
                    break;
                case BombExplosionInfo.Trigger.Player:
                    byPlayer++;
                    break;
                case BombExplosionInfo.Trigger.Ship:
                    byShip++;
                    break;
                case BombExplosionInfo.Trigger.Explosion:
                    byExplosion++;
                    break;
                case BombExplosionInfo.Trigger.Projectile:
                    byMissile++;
                    break;
            }

            if (bombExplosionInfo.explosionConditions.duringSlowMotion) duringSlowMotion++;
            if (bombExplosionInfo.explosionConditions.duringAttraction) duringAttraction++;

            total++;
        }
    }

    [Serializable]
    public class PowerUpCollection
    {
        public int total { get; private set; }
        public int slowMo { get; private set; }
        public int shield { get; private set; }
        public int attractor { get; private set; }
        public int missileLauncher { get; private set; }

        public void Log(PowerType powerType)
        {
            switch (powerType)
            {
                case PowerType.Attractor:
                    attractor++;
                    break;
                case PowerType.MissileLauncher:
                    missileLauncher++;
                    break;
                case PowerType.SlowMo:
                    slowMo++;
                    break;
                case PowerType.Shield:
                    shield++;
                    break;
            }

            total++;
        }
    }

    [Serializable]
    public class NonViolent
    {
        public int enemyEscapes { get; private set; }
        public int bombDisposal { get; private set; }

        public void LogEnemyEscape()
        {
            enemyEscapes++;
        }

        public void LogBombDisposal()
        {
            bombDisposal++;
        }
    }

    [Serializable]
    public class Time
    {
        public float totalSecondsPlayed { get; private set; }

        public DateTime lastTimePlayed { get; private set; }

        public void LogPlayTime(uint seconds)
        {
            totalSecondsPlayed += seconds;
        }

        public void LogDate(DateTime exitTime)
        {
            lastTimePlayed = exitTime;
        }
    }

    [Serializable]
    public class Others
    {
        public int playerDeaths { get; private set; }
        public int shipHits { get; private set; }

        public void LogPlayerDeath()
        {
            playerDeaths++;
        }

        public void LogShipHit()
        {
            shipHits++;
        }
    }
    #endregion
}