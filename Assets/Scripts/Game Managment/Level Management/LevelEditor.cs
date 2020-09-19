//In Progress
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(LevelCollection))]
public class LevelEditor : Editor
{
    #region Target
    private LevelCollection levelCollection { get; set; }
    #endregion

    #region Options
    private bool isInFirebaseMode { get; set; }
    private bool isInManualMode { get; set; }
    private bool isEnemyLineupListed { get; set; }
    private bool isPowerUpLineupListed { get; set; }
    private bool isBombLineupListed { get; set; }
    private bool isDefaultLevelsListed { get; set; }
    #endregion

    #region GUI Parameters
    private List<Vector2> scroll { get; set; } = new List<Vector2>();
    #endregion

    #region GUI Styles
    GUIStyle bold;
    #endregion

    #region GUI Layout Options
    GUILayoutOption[] minorButtonLayout;
    GUILayoutOption[] majorButtonLayout;
    #endregion

    private void OnEnable()
    {
        levelCollection = (LevelCollection) target;

        bold = BoldStyle();

        Debug.Log("Editing: " + levelCollection.name);

        majorButtonLayout = new GUILayoutOption[] { GUILayout.MinHeight(30), GUILayout.MaxHeight(40), GUILayout.ExpandHeight(true) };
        minorButtonLayout = new GUILayoutOption[] { GUILayout.MinHeight(20), GUILayout.MaxHeight(30), GUILayout.ExpandHeight(true) };
    }

    public override void OnInspectorGUI()
    {
        LevelEditorHeader();

        EditorGUILayout.Space();

        EnvironmentSettings();

        EditorGUILayout.Space();

        ObjectSpawnSettings();

        EditorGUILayout.Space();


        PowerUpLineupHeader();

        ManualPowerUpLineupEditor();

        RandomPowerUpLineupEditor();

        ListPowerUpLineup();

        PowerUpLineupFooter();


        EditorGUILayout.Space();


        BombLineupHeader();

        ManualBombLineupEditor();

        RandomBombLineupEditor();

        ListBombLineup();

        BombLineupFooter();


        EditorGUILayout.Space();


        EnemyLineupHeader();

        ManualEnemyLineupEditor();

        RandomEnemyLineupEditor();

        ListEnemyLineup();

        EnemyLineupFooter();


        EditorGUILayout.Space(25);

        SavedLevelsHeader();

        EditorGUILayout.Space();

        ListSavedLevels();

        //EditorGUILayout.Space(25);

        //ShuffleLevels();

        EditorGUILayout.Space(30);

        SavedLevelsFooter();

        //EditorGUILayout.Space(25);

        //if (GUILayout.Button("Randomize bg index", minorButtonLayout))
        //{
        //    levelCollection.AssignRandomBGIndeces();
        //}
    }

    private GUIStyle BoldStyle()
    {
        GUIStyle bold = new GUIStyle();
        bold.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        bold.fontStyle = FontStyle.Bold;
        return bold;
    }

    private void LevelEditorHeader()
    {
        if (isInManualMode)
        {
            GUIContent randomButton = new GUIContent("Switch to Random Mode", "Change inspector GUI to random generator interface.");

            if (GUILayout.Button(randomButton, minorButtonLayout))
            {
                isInManualMode = false;
            }
        }
        else
        {
            GUIContent manualButton = new GUIContent("Switch to Manual Mode", "Change inspector GUI to manual generator interface.");

            if (GUILayout.Button(manualButton, minorButtonLayout))
            {
                isInManualMode = true;
            }
        }

        if (isInFirebaseMode)
        {
            GUIContent localButton = new GUIContent("Switch to Local Mode", "Change inspector GUI to Local collection editing interface.");

            if (GUILayout.Button(localButton, minorButtonLayout))
            {
                isInFirebaseMode = false;
            }

            GUIContent refreshButton = new GUIContent("Activate Firebase", "Start firebase if it is not ready.");

            if (GUILayout.Button(refreshButton, minorButtonLayout))
            {
                FirebaseUtility.InitializeFirebase(false);
            }

            EditorGUILayout.TextArea(FirebaseUtility.IsFirebaseSafeToUse ? "Firebase is Active" : "Firebase is NOT Active");
        }
        else
        {
            GUIContent firebaseButton = new GUIContent("Switch to Firebase Mode", "Change inspector GUI to Firebase database editing interface.");

            if (GUILayout.Button(firebaseButton, minorButtonLayout))
            {
                isInFirebaseMode = true;
            }
        }
    }

    private void EnvironmentSettings()
    {
        EditorGUILayout.LabelField("Environment Settings", bold);

        GUIContent label = new GUIContent("Background Index", "Set the value to -1 for random index.");

        levelCollection._levelInfo.environmentSettings.backgroundAssetIndex = EditorGUILayout.IntField(label,
            levelCollection._levelInfo.environmentSettings.backgroundAssetIndex);
    }

    private void ObjectSpawnSettings()
    {
        EditorGUILayout.LabelField("Object Spawn Settings", bold);

        levelCollection._levelInfo.objectSpawnSettings.powerUpOrbGenerosity = EditorGUILayout.FloatField("PowerUp Spawn Generosity",
            levelCollection._levelInfo.objectSpawnSettings.powerUpOrbGenerosity);
    }

    private void PowerUpLineupHeader()
    {
        EditorGUILayout.LabelField("Power-Up Lineup", bold);

        EditorGUILayout.IntField("Count", levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Count, bold);
    }

    private void ManualPowerUpLineupEditor()
    {
        if (!isInManualMode) return;

        levelCollection._powerUpOrbSpawnInfo.spawnDelay = EditorGUILayout.FloatField("Delay", levelCollection._powerUpOrbSpawnInfo.spawnDelay);
        levelCollection._powerUpOrbSpawnInfo.powerType = (PowerType) EditorGUILayout.EnumPopup("Type", levelCollection._powerUpOrbSpawnInfo.powerType);
        levelCollection._powerUpOrbSpawnInfo.spawnSide = EditorGUILayout.IntSlider("Spawn Side", levelCollection._powerUpOrbSpawnInfo.spawnSide, 0, 1);

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Add", "Add the current values in the fields as a new entry in the Power-Up Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Add(levelCollection._powerUpOrbSpawnInfo);
        }

        if (GUILayout.Button(new GUIContent("Poll", "Remove the top entry in the Power-Up Lineup and populate the fields with its values.")))
        {
            int count = levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Count;

            if (count > 0)
            {
                levelCollection._powerUpOrbSpawnInfo = levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup[count - 1];

                levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.RemoveAt(count - 1);
            }
        }
    }

    private void RandomPowerUpLineupEditor()
    {
        if (isInManualMode) return;

        levelCollection._powerUpOrbSpawnInfo.spawnSide = EditorGUILayout.IntField("Length", levelCollection._powerUpOrbSpawnInfo.spawnSide);
        levelCollection._powerUpSpawnDuration = EditorGUILayout.FloatField("Duration", levelCollection._powerUpSpawnDuration);
        levelCollection._powerUpOrbSpawnInfo.spawnDelay = EditorGUILayout.FloatField("Duration Error", levelCollection._powerUpOrbSpawnInfo.spawnDelay);

        EditorGUILayout.Space();

        int length = levelCollection._powerUpOrbSpawnInfo.spawnSide;

        if (GUILayout.Button(new GUIContent("Add: " + length, "Generate and add a random Power-Up Lineup to the existing lineup.")))
        {
            levelCollection.AddRandomPowerUpLineup();
        }

        if (GUILayout.Button(new GUIContent("Overwrite: " + length, "Generate and write a new random Power-Up Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Clear();
            levelCollection.AddRandomPowerUpLineup();
        }
    }

    private void ListPowerUpLineup()
    {
        if (isPowerUpLineupListed)
        {
            if (GUILayout.Button(new GUIContent("Unlist", "Hide all entries in the Power-Up Lineup.")))
            {
                isPowerUpLineupListed = false;
            }

            for (int i = 0; i < levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Count; i++)
            {
                PowerUpOrbSpawnInfo info = levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup[i];

                EditorGUILayout.LabelField("Power Up " + (i + 1), bold);

                info.spawnDelay = EditorGUILayout.FloatField("Delay", info.spawnDelay);
                info.powerType = (PowerType)EditorGUILayout.EnumPopup("Type", info.powerType);
                info.spawnSide = EditorGUILayout.IntSlider("Spawn Side", info.spawnSide, 0, 1);

                EditorGUILayout.Space();
            }
        }
        else if (GUILayout.Button(new GUIContent("List", "Show all entries in the Power-Up Lineup.")))
        {
            isPowerUpLineupListed = true;
        }
    }

    private void PowerUpLineupFooter()
    {
        if (GUILayout.Button(new GUIContent("Clear", "Delete all entries in the Power-Up Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.powerUpOrbLineup.Clear();
        }
    }

    private void BombLineupHeader()
    {
        EditorGUILayout.LabelField("Bomb Lineup", bold);

        EditorGUILayout.IntField("Count", levelCollection._levelInfo.objectSpawnSettings.bombLineup.Count, bold);
    }

    private void ManualBombLineupEditor()
    {
        if (!isInManualMode) return;

        levelCollection._bombSpawnInfo.spawnDelay = EditorGUILayout.FloatField("Delay", levelCollection._bombSpawnInfo.spawnDelay);
        levelCollection._bombSpawnInfo.bombSize = EditorGUILayout.Slider("Size", levelCollection._bombSpawnInfo.bombSize, 0.5f, 1.0f);
        levelCollection._bombSpawnInfo.spawnSide = EditorGUILayout.IntSlider("Spawn Side", levelCollection._bombSpawnInfo.spawnSide, 0, 1);

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Add", "Add the current values in the fields as a new entry in the Bomb Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.bombLineup.Add(levelCollection._bombSpawnInfo);
        }

        if (GUILayout.Button(new GUIContent("Poll", "Remove the top entry in the Bomb Lineup and populate the fields with its values.")))
        {
            int count = levelCollection._levelInfo.objectSpawnSettings.bombLineup.Count;

            if (count > 0)
            {
                levelCollection._bombSpawnInfo = levelCollection._levelInfo.objectSpawnSettings.bombLineup[count - 1];

                levelCollection._levelInfo.objectSpawnSettings.bombLineup.RemoveAt(count - 1);
            }
        }
    }

    private void RandomBombLineupEditor()
    {
        if (isInManualMode) return;

        levelCollection._bombSpawnInfo.spawnSide = EditorGUILayout.IntField("Length", levelCollection._bombSpawnInfo.spawnSide);
        levelCollection._bombSpawnInfo.bombSize = EditorGUILayout.FloatField("Duration", levelCollection._bombSpawnInfo.bombSize);
        levelCollection._bombSpawnInfo.spawnDelay = EditorGUILayout.FloatField("Duration Error", levelCollection._bombSpawnInfo.spawnDelay);

        EditorGUILayout.Space();

        int length = levelCollection._bombSpawnInfo.spawnSide;

        if (GUILayout.Button(new GUIContent("Add: " + length, "Generate and add a random Bomb Lineup to the existing lineup.")))
        {
            levelCollection.AddRandomBombLineup();
        }

        if (GUILayout.Button(new GUIContent("Overwrite: " + length, "Generate and write a new random Bomb Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.bombLineup.Clear();
            levelCollection.AddRandomBombLineup();
        }
    }

    private void ListBombLineup()
    {
        if (isBombLineupListed)
        {
            if (GUILayout.Button(new GUIContent("Unlist", "Hide all entries in the Bomb Lineup.")))
            {
                isBombLineupListed = false;
            }

            for (int i = 0; i < levelCollection._levelInfo.objectSpawnSettings.bombLineup.Count; i++)
            {
                BombSpawnInfo info = levelCollection._levelInfo.objectSpawnSettings.bombLineup[i];

                EditorGUILayout.LabelField("Bomb " + (i + 1), bold);

                info.spawnDelay = EditorGUILayout.FloatField("Delay", info.spawnDelay);
                info.bombSize = EditorGUILayout.FloatField("Size", info.bombSize);
                info.spawnSide = EditorGUILayout.IntSlider("Spawn Side", info.spawnSide, 0, 1);

                EditorGUILayout.Space();
            }
        }
        else if (GUILayout.Button(new GUIContent("List", "Show all entries in the Bomb Lineup.")))
        {
            isBombLineupListed = true;
        }
    }

    private void BombLineupFooter()
    {
        if (GUILayout.Button(new GUIContent("Clear", "Delete all entries in the Bomb Lineup.")))
        {
            levelCollection._levelInfo.objectSpawnSettings.bombLineup.Clear();
        }
    }

    private void EnemyLineupHeader()
    {
        EditorGUILayout.LabelField("Enemy Lineup", bold);

        EditorGUILayout.IntField("Count", levelCollection._levelInfo.enemyLineup.Count, bold);
    }

    private void ManualEnemyLineupEditor()
    {
        if (!isInManualMode) return;

        levelCollection._enemySpawnInfo.spawnDelay = EditorGUILayout.FloatField("Delay", levelCollection._enemySpawnInfo.spawnDelay);
        levelCollection._enemySpawnInfo.spawnPositionIndex = EditorGUILayout.
            IntSlider("Spawn Positon Index", levelCollection._enemySpawnInfo.spawnPositionIndex, 1, AsteroidCommander.SPAWN_POSITIONS_COUNT);
        levelCollection._enemySpawnInfo.enemyType = EditorGUILayout.IntSlider("Type", levelCollection._enemySpawnInfo.enemyType, 0, 2);
        levelCollection._enemySpawnInfo.enemySpeed = EditorGUILayout.Slider("Speed", levelCollection._enemySpawnInfo.enemySpeed, 1.0f, 6.5f);
        levelCollection._enemySpawnInfo.enemySize = EditorGUILayout.Slider("Size", levelCollection._enemySpawnInfo.enemySize, 0.5f, 1.0f);

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Add", "Add the current values in the fields as a new entry in the Enemy Lineup.")))
        {
            levelCollection._levelInfo.enemyLineup.Add(levelCollection._enemySpawnInfo);
        }

        if (GUILayout.Button(new GUIContent("Poll", "Remove the top entry in the Enemy Lineup and populate the fields with its values.")))
        {
            if (levelCollection._levelInfo.enemyLineup.Count > 0)
            {
                levelCollection._enemySpawnInfo = levelCollection._levelInfo.enemyLineup[levelCollection._levelInfo.enemyLineup.Count - 1];

                levelCollection._levelInfo.enemyLineup.RemoveAt(levelCollection._levelInfo.enemyLineup.Count - 1);
            }
        }
    }

    private void RandomEnemyLineupEditor()
    {
        if (isInManualMode) return;

        levelCollection._enemySpawnInfo.spawnDelay = EditorGUILayout.IntField("Length", (int)levelCollection._enemySpawnInfo.spawnDelay);
        levelCollection._enemySpawnInfo.enemyType = EditorGUILayout.IntSlider("Difficulty", levelCollection._enemySpawnInfo.enemyType, 0, LevelCollection.MAX_DIFFICULTY);
        levelCollection._enemySpawnInfo.enemySpeed = EditorGUILayout.FloatField("Duration", levelCollection._enemySpawnInfo.enemySpeed);
        levelCollection._enemySpawnInfo.enemySize = EditorGUILayout.FloatField("Duration Error", levelCollection._enemySpawnInfo.enemySize);

        EditorGUILayout.Space();

        int length = (int) levelCollection._enemySpawnInfo.spawnDelay;

        if (GUILayout.Button(new GUIContent("Add: " + length, "Generate and add a random Enemy Lineup to the existing lineup.")))
        {
            levelCollection.AddRandomEnemyLineup();
        }

        if (GUILayout.Button(new GUIContent("Overwrite: " + length, "Generate and write a new random Enemy Lineup.")))
        {
            levelCollection._levelInfo.enemyLineup.Clear();
            levelCollection.AddRandomEnemyLineup();
        }
    }

    private void ListEnemyLineup()
    {
        if (isEnemyLineupListed)
        {
            if (GUILayout.Button(new GUIContent("Unlist", "Hide all entries in the Enemy Lineup.")))
            {
                isEnemyLineupListed = false;
            }

            for (int i = 0; i < levelCollection._levelInfo.enemyLineup.Count; i++)
            {
                AsteroidSpawnInfo info = levelCollection._levelInfo.enemyLineup[i];

                EditorGUILayout.LabelField("Enemy " + (i + 1), bold);

                info.spawnDelay = EditorGUILayout.FloatField("Delay", info.spawnDelay);
                info.spawnPositionIndex = EditorGUILayout.IntSlider("Position Index", info.spawnPositionIndex, 1, AsteroidCommander.SPAWN_POSITIONS_COUNT);
                info.enemyType = EditorGUILayout.IntField("Type", info.enemyType);
                info.enemySpeed = EditorGUILayout.FloatField("Speed", info.enemySpeed);
                info.enemySize = EditorGUILayout.FloatField("Size", info.enemySize);

                EditorGUILayout.Space();
            }
        }
        else if(GUILayout.Button(new GUIContent("List", "Show all entries in the Enemy Lineup.")))
        {
            isEnemyLineupListed = true;
        }
    }

    private void EnemyLineupFooter()
    {
        if (GUILayout.Button(new GUIContent("Clear", "Delete all entries in the Enemy Lineup.")))
        {
            levelCollection._levelInfo.enemyLineup.Clear();
        }
    }

    private void SavedLevelsHeader()
    {
        if (GUILayout.Button(new GUIContent("Save as New Level", "Save all values as a new default level."), majorButtonLayout))
        {
            levelCollection.SaveAsDefaultLevel();
        }
        if (GUILayout.Button(new GUIContent("Overwrite Last Saved Level", "Overwrite all values to the last saved default level."), minorButtonLayout))
        {
            levelCollection.DeleteLevel(levelCollection.Count - 1);
            levelCollection.InsertLevel(levelCollection.Count - 1);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(levelCollection.defaultLevelCollection.Count + " Levels", bold);

        EditorGUILayout.Space(25);
    }

    private void ListSavedLevels()
    {
        if (isDefaultLevelsListed)
        {
            if (GUILayout.Button(new GUIContent("Unlist Saved Levels", "Hide all saved levels."), minorButtonLayout))
            {
                isDefaultLevelsListed = false;
            }

            GUILayoutOption[] JSONLayout = { GUILayout.MinHeight(50), GUILayout.MaxHeight(120), GUILayout.ExpandHeight(true) };

            for (int i = 0; i < levelCollection._levelInfoCollection.Count; i++)
            {
                EditorGUILayout.LabelField("Level " + (i + 1), bold);

                //if (scroll.Count - 1 < i)
                //{
                //    scroll.Add(new Vector2());
                //}

                //scroll[i] = EditorGUILayout.BeginScrollView(scroll[i], JSONLayout);

                //levelCollection.defaultLevelCollection[i] = EditorGUILayout.TextArea(levelCollection.defaultLevelCollection[i]);

                //EditorGUILayout.EndScrollView();

                EditorGUILayout.IntField("Enemies", levelCollection._levelInfoCollection[i].enemyLineup.Count);
                EditorGUILayout.IntField("PowerUps", levelCollection._levelInfoCollection[i].objectSpawnSettings.powerUpOrbLineup.Count);
                EditorGUILayout.IntField("Bombs", levelCollection._levelInfoCollection[i].objectSpawnSettings.bombLineup.Count);

                if (GUILayout.Button(new GUIContent("Delete", "Delete this level and re-assign level indices.")))
                {
                    levelCollection.DeleteLevel(i);
                }
                if (GUILayout.Button(new GUIContent("Insert Above", "Insert a new level here and re-assign level indices.")))
                {
                    levelCollection.InsertLevel(i - 1);
                }
                if (i < levelCollection._levelInfoCollection.Count - 1)
                {
                    if (GUILayout.Button(new GUIContent("Insert Below", "Insert a new level here and re-assign level indices.")))
                    {
                        levelCollection.InsertLevel(i);
                    }
                }

                if (isInFirebaseMode)
                {
                    if (GUILayout.Button(new GUIContent("Publish To Firebase", "Publish this level to the firebase database.")))
                    {
                        FirebaseUtility.WriteToDatabase(LevelManager.DLC_LEVELS_PATH + "/" + i, levelCollection.defaultLevelCollection[i], 
                            () => { Debug.Log("Level Published Successfully!"); });
                    }
                }

                EditorGUILayout.Space();
            }
        }
        else if (GUILayout.Button(new GUIContent("List Saved Levels", "Show all saved levels."), minorButtonLayout))
        {
            isDefaultLevelsListed = true;
        }
        if (GUILayout.Button(new GUIContent("Refresh Saved Levels", "Refresh list of all saved levels."), minorButtonLayout))
        {
            levelCollection.LoadLevelInfoCollection();
        }
    }

    private void SavedLevelsFooter()
    {
        if (isInFirebaseMode)
        {
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Publish Collection To Firebase", "Publish this level collection to the firebase database."), majorButtonLayout))
            {
                Debug.Log("Publishing: " + levelCollection.defaultLevelCollection);

                FirebaseUtility.WriteToDatabase(LevelManager.DLC_LEVELS_PATH, levelCollection.defaultLevelCollection, 
                    () => { Debug.Log("Level Collection Published Successfully!"); });
            }

            EditorGUILayout.Space(100);

            if (GUILayout.Button(new GUIContent("Clear Collection", "Delete all saved levels locally and in the firebase database."), minorButtonLayout))
            {
                FirebaseUtility.WriteToDatabase(LevelManager.DLC_LEVELS_PATH, null, OnClearedFirebaseLevels);
            }
        }
        else
        {
            EditorGUILayout.Space(100);

            if (GUILayout.Button(new GUIContent("Clear Saved Levels", "Delete all saved levels."), minorButtonLayout))
            {
                levelCollection.ClearAll();
            }
        }
    }

    private void ShuffleLevels()
    {
        EditorGUILayout.LabelField("Shuffle Levels", bold);

        GUILayout.BeginHorizontal();

        levelCollection.shuffleStartLevel = EditorGUILayout.IntField("Start Level", levelCollection.shuffleStartLevel);
        levelCollection.shuffleEndLevel = EditorGUILayout.IntField("End Level", levelCollection.shuffleEndLevel);

        GUILayout.EndHorizontal();

        if (GUILayout.Button(new GUIContent("Shuffle"), majorButtonLayout))
        {
            levelCollection.ShuffleCollection();
        }

        if (GUILayout.Button(new GUIContent("Undo Last Shuffle"), minorButtonLayout))
        {
            levelCollection.UndoShuffle();
        }
    }

    private void OnClearedFirebaseLevels()
    {
        levelCollection.ClearAll();
        Debug.Log("Firebase and Local Level Collection Successfully Cleared!");
    }
}
#endif