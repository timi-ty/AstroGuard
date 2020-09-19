using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public static class FirebaseUtility
{
    #region App
    private static FirebaseApp App { get; set; }
    public static bool IsFirebaseSafeToUse { get; set; }
    #endregion

    #region Auth
    public static FirebaseUser CurrentUser { get; set; }
    private static FirebaseAuth Auth { get; set; }
    private static string FacebookAccessToken { get; set; }
    #endregion

    #region Storage
    private static FirebaseStorage Storage { get; set; }
    private static string CurrentUserStoragePath { get; set; }
    private static GameData OnlineGameData { get; set; }
    private static bool IsSyncing { get; set; }
    #endregion

    #region Database
    private static FirebaseDatabase Database { get; set; }
    private static List<DBQuery> ActiveQueries { get; set; } = new List<DBQuery>();
    #endregion

    #region Events
    private static event Action<FirebaseUser> AuthChanged;
    private static event Action DatabaseReady;
    #endregion

    #region Setup
    public static void InitializeFirebase(bool inPlayMode)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;

                Auth = FirebaseAuth.DefaultInstance;

                Storage = FirebaseStorage.DefaultInstance;

                App.SetEditorDatabaseUrl("https://astroguard-eb5e7.firebaseio.com/");

                Database = FirebaseDatabase.DefaultInstance;

                FCMClient.Initialize();

                IsFirebaseSafeToUse = true;

                if(inPlayMode)
                {
                    Auth.StateChanged += Auth_StateChanged;
                }

                if(Database != null)
                {
                    DatabaseReady?.Invoke();
                }

                SignIn();

                Debug.Log("Firebase Started.");
            }
            else
            {
                Debug.LogError(string.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                IsFirebaseSafeToUse = false;
            }
        });
    }
    #endregion

    #region Authentication
    public static void AuthneticateWithFacebook(string accessToken)
    {
        FacebookAccessToken = accessToken;

        if (!IsFirebaseSafeToUse)
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "Ooops...",
                messageText = "Sorry, we couldn't log you in.",
                positiveActionText = "Retry",
                negativeActionText = "Cancel"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, SignIn, () => { });
            return;
        }

        SignIn();
    }

    private static void SignIn()
    {
        if (FacebookAccessToken == null)
        {
            Debug.Log("Access token is not ready. Sign in will be called when it is.");

            return;
        }
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Firebase is not ready. Sign in will be called when it is.");
            return;
        }

        Credential credential = FacebookAuthProvider.GetCredential(FacebookAccessToken);

        Auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");

                AlertSignInFailure();

                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);

                AlertSignInFailure();

                return;
            }

            RewardNewUser();

            AuthChanged?.Invoke(CurrentUser);

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                CurrentUser.DisplayName, CurrentUser.UserId);
        });
    }

    private static void RewardNewUser()
    {
        if (CurrentUser.Metadata.CreationTimestamp == CurrentUser.Metadata.LastSignInTimestamp)
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "SIGN IN REWARD!",
                messageText = "Congratulations! You just got rewarded 1000 Astro Gold for signing in. " +
                "Makesure to sync your data to claim your reward.",
                positiveActionText = "Sync Data",
                negativeActionText = "Dismiss",
                isCelebratory = true
            };

            UIManager.QueueAlertDialog(alertMessageInfo, SyncGameData, () => { });

            PlayerStats.RewardAstroGold(1000);
        }
    }

    private static void AlertSignInFailure()
    {
        AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
        {
            titleText = "Ooops...",
            messageText = "Sorry, we couldn't log you in.",
            positiveActionText = "Retry",
            negativeActionText = "Cancel"
        };

        UIManager.QueueAlertDialog(alertMessageInfo, SignIn, () => { });
    }

    public static void AddOnAuthListener(Action<FirebaseUser> onAuthChanged)
    {
        AuthChanged += onAuthChanged;
        
        onAuthChanged?.Invoke(CurrentUser);
    }

    private static void Auth_StateChanged(object sender, EventArgs e)
    {
        CurrentUser = Auth.CurrentUser;

        CurrentUserStoragePath = "users/" + CurrentUser?.UserId;

        AuthChanged?.Invoke(CurrentUser);

        if (CurrentUser != null)
        {
            Debug.Log("Logged In As: " + CurrentUser.DisplayName);
            RecordAuthEvent();
            SyncGameData();
        }
    }

    public static void SignOut()
    {
        Auth.SignOut();
    }
    #endregion

    #region Game Data Operations
    public static async void SyncGameData()
    {
        if (!IsFirebaseSafeToUse || IsSyncing)
        {
            Debug.Log("Game sync aborted because: isFirebaseNotSafeToUse = " + !IsFirebaseSafeToUse + " or isSyncing " + IsSyncing);
            IsSyncing = false;
            return;
        }

        Debug.Log("Game Data Syncing...");

        IsSyncing = true;

        StorageReference storageReference = Storage.GetReference(CurrentUserStoragePath + "/SaveFile.bin");

        Task<byte[]> downloadTask = storageReference.GetBytesAsync(1024 * 1024 * 8);

        bool objectNotFound = false;

        try
        {
            await downloadTask;
        }
        catch(StorageException downloadException)
        {
            objectNotFound = downloadException.ErrorCode == StorageException.ErrorObjectNotFound;

            Debug.Log("Couldn't download save data because " + downloadException);

            if (!objectNotFound)
            {
                Debug.Log("Game sync failed. Aborting...");
                IsSyncing = false;
                return;
            }
        }

        OnlineGameData = null;

        if (downloadTask.Exception is null)
        {
            OnlineGameData = ByteDataHelper.BytesToGameData(downloadTask.Result);
        }

        if (GameDataManager.IsCompatibleWith(OnlineGameData))
        {
            GameDataManager.GameData = GameDataManager.MoreRecent(OnlineGameData);

            UploadGameData();
        }
        else if (downloadTask.IsCompleted || objectNotFound)
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "SYNC GAME DATA",
                messageText = "Hey, the backup associated with this account is incompatible with the local save. What do you want to do?",
                positiveActionText = "Keep Local Save",
                negativeActionText = "Keep Online Backup"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, KeepLocalSave, KeepOnlineBackup);
        }
        else
        {
            IsSyncing = false;
        }
    }

    private static void KeepLocalSave()
    {
        if (GameDataManager.IsUploadable())
        {
            UploadGameData();
        }
        else
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "SYNC GAME DATA",
                messageText = "Hey, this local save belongs to another user. You would have to clear it first.",
                positiveActionText = "Wipe Local Save",
                negativeActionText = "Cancel"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, UploadNewGameData, () => { });
        }
    }

    private static void KeepOnlineBackup()
    {
        if(OnlineGameData != null)
        {
            OnlineGameData.Sign();

            GameDataManager.GameData = OnlineGameData;

            IsSyncing = false;
        }
        else
        {
            AlertMessageInfo alertMessageInfo = new AlertMessageInfo()
            {
                titleText = "SYNC GAME DATA",
                messageText = "Hey, it looks like you have no online backup. Would you like to start a new game? (Local save will be lost.)",
                positiveActionText = "Start New Game",
                negativeActionText = "Cancel"
            };

            UIManager.QueueAlertDialog(alertMessageInfo, UploadNewGameData, () => { });
        }
    }

    private static async void UploadGameData()
    {
        GameData gameData = GameDataManager.GameData;

        gameData.Sign();

        StorageReference storageReference = Storage.GetReference(CurrentUserStoragePath + "/SaveFile.bin");

        Task uploadTask = storageReference.PutBytesAsync(ByteDataHelper.GameDataToBytes(gameData));

        try
        {
            await uploadTask;
        }
        catch(StorageException uploadException)
        {
            Debug.Log("Couldn't upload save data because " + uploadException);

            Debug.Log("Game sync failed. Aborting...");
            IsSyncing = false;
            return;
        }

        if (uploadTask.Exception is null)
        {
            PlayerStats.ClaimRewardedAstroGold();

            Debug.Log("Successfully backed up saved data to firebase.");
        }
        else
        {
            Debug.Log("Failed to upload because " + uploadTask.Exception);
        }

        IsSyncing = false;
    }

    private static void UploadNewGameData()
    {
        GameDataManager.GameData.Reset();

        UploadGameData();
    }
    #endregion

    #region Realtime Database Operations
    public static void AddDatabaseReadyListener(Action onDatabaseReady)
    {
        DatabaseReady += onDatabaseReady;

        if(Database != null)
        {
            onDatabaseReady.Invoke();
        }
    }
    public static DatabaseReference GetNodeReferenceFromPath(string path)
    {
        string[] childNodes = path.Split('/', '\\');

        DatabaseReference databaseReference = Database.RootReference;

        DatabaseReference nodeReference = databaseReference;

        foreach (string childNode in childNodes)
        {
            nodeReference = nodeReference.Child(childNode);
        }

        return nodeReference;
    }

    public static async void UploadJsonToDatabase(string path, string json)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Upload query to " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        Task uploadTask = nodeReference.SetRawJsonValueAsync(json);

        try
        {
            await uploadTask;
        }
        catch(DatabaseException e)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + e);
        }

        if (uploadTask.IsFaulted)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + uploadTask.Exception);
        }
        else
        {
            Debug.Log("Success! Upload to " + path + " succeeded.");
        }
    }

    public static async void WriteToDatabase(string path, object value, Action onSuccessListener)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Upload query to " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        Task uploadTask = nodeReference.SetValueAsync(value);

        try
        {
            await uploadTask;
        }
        catch (DatabaseException e)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + e);
            return;
        }

        if (uploadTask.IsFaulted)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + uploadTask.Exception);
        }
        else if(uploadTask.IsCompleted)
        {
            onSuccessListener?.Invoke();
            Debug.Log("Success! Upload to " + path + " succeeded.");
        }
    }

    public static async void PushToDatabase(string path, object value, Action onSuccessListener)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Upload query to " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path).Push();

        Task uploadTask = nodeReference.SetValueAsync(value);

        try
        {
            await uploadTask;
        }
        catch (DatabaseException e)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + e);
            return;
        }

        if (uploadTask.IsFaulted)
        {
            Debug.Log("Upload to " + path + " couldn't be completed because " + uploadTask.Exception);
        }
        else if(uploadTask.IsCompleted)
        {
            onSuccessListener?.Invoke();
            Debug.Log("Success! Upload to " + path + " succeeded.");
        }
    }

    public static async void ReadFromDatabase(string path, Action<object> onSuccessListener)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Download query from " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        Task<DataSnapshot> downloadTask = nodeReference.GetValueAsync();

        try
        {
            await downloadTask;
        }
        catch (DatabaseException e)
        {
            Debug.Log("Download from " + path + " couldn't be completed because " + e);
            return;
        }

        if (downloadTask.IsFaulted)
        {
            Debug.Log("Download from " + path + " couldn't be completed because " + downloadTask.Exception);
        }
        else if(downloadTask.IsCompleted)
        {
            DataSnapshot dataSnapshot = downloadTask.Result;

            if(dataSnapshot?.Value != null)
            {
                onSuccessListener?.Invoke(dataSnapshot.Value);

                Debug.Log("Success! Download from " + path + " succeeded.");
            }
        }
    }

    public static void QueryFromDatabase(string path, Action<List<object>> onQuerySucceeded)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Query from " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        EventHandler<ValueChangedEventArgs> valueChanged = (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError("Query from " + path + " failed because " + args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot?.Children != null)
            {
                List<object> children = new List<object>();

                foreach (DataSnapshot child in args.Snapshot.Children)
                {
                    children.Add(child?.Value);
                }

                onQuerySucceeded?.Invoke(children);

                Debug.Log("Succesfully queried from " + path + " due to value change.");
            }
        };

        Query nodeQuery = nodeReference.OrderByKey();

        nodeQuery.ValueChanged += valueChanged;

        ActiveQueries.Add(new DBQuery(nodeQuery, valueChanged));
    }

    public static void QueryFromDatabase(string path, string orderByChild, int limitToLast, Action<List<object>> onQuerySucceeded)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Query from " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        EventHandler<ValueChangedEventArgs> valueChanged = (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot?.Children != null)
            {
                List<object> children = new List<object>();

                foreach (DataSnapshot child in args.Snapshot.Children)
                {
                    children.Add(child?.Value);
                }

                onQuerySucceeded?.Invoke(children);

                Debug.Log("Succesfully queried from " + path + " due to value change.");
            }
        };

        Query nodeQuery = nodeReference.OrderByChild(orderByChild).LimitToLast(limitToLast);

        nodeQuery.ValueChanged += valueChanged;

        ActiveQueries.Add(new DBQuery(nodeQuery, valueChanged));
    }

    public static void QueryFromDatabase(string path, string orderByChild, int startAt, int endAt, Action<List<object>> onQuerySucceeded)
    {
        if (!IsFirebaseSafeToUse)
        {
            Debug.Log("Query from " + path + " aborted because Firebase is not ready.");
            return;
        }

        DatabaseReference nodeReference = GetNodeReferenceFromPath(path);

        EventHandler<ValueChangedEventArgs> valueChanged = (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot?.Children != null)
            {
                List<object> children = new List<object>();

                foreach (DataSnapshot child in args.Snapshot.Children)
                {
                    children.Add(child?.Value);
                }

                onQuerySucceeded?.Invoke(children);

                Debug.Log("Succesfully queried from " + path + " due to value change.");
            }
        };

        Query nodeQuery = nodeReference.OrderByChild(orderByChild).StartAt(startAt).EndAt(endAt);

        nodeQuery.ValueChanged += valueChanged;

        ActiveQueries.Add(new DBQuery(nodeQuery, valueChanged));
    }
    #endregion

    #region Analytics
    public static void RecordAuthEvent()
    {
        if (CurrentUser.Metadata.CreationTimestamp == CurrentUser.Metadata.LastSignInTimestamp)
        {
            RecordSignUpEvent(CurrentUser.ProviderId);
            Debug.Log("Signed Up With: " + CurrentUser.ProviderId);
        }
        else
        {
            RecordLoginEvent(CurrentUser.ProviderId);
            Debug.Log("Signed In With: " + CurrentUser.ProviderId);
        }
    }

    public static void RecordLoginEvent(string loginMethod)
    {
        FirebaseAnalytics.LogEvent(
          FirebaseAnalytics.EventLogin, 
          new Parameter[] 
          { 
              new Parameter(FirebaseAnalytics.ParameterMethod, loginMethod),
          }
        );

        Analytics.LogSignIn();
    }

    public static void RecordSignUpEvent(string signUpMethod)
    {
        FirebaseAnalytics.LogEvent(
          FirebaseAnalytics.EventSignUp,
          new Parameter[]
          {
              new Parameter(FirebaseAnalytics.ParameterMethod, signUpMethod),
          }
        );

        Analytics.LogSignUp();
    }

    public static void RecordCustomEvent(string eventName, params Parameter[] parameters)
    {
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }
    #endregion

    #region Cleanup
    public static void Cleanup()
    {
        foreach(DBQuery dBQuery in ActiveQueries)
        {
            dBQuery.databaseQuery.ValueChanged -= dBQuery.eventHandler;
        }
    }
    #endregion
}

public class DBQuery
{
    public Query databaseQuery;
    public EventHandler<ValueChangedEventArgs> eventHandler;

    public DBQuery(Query databaseQuery, EventHandler<ValueChangedEventArgs> eventHandler)
    {
        this.databaseQuery = databaseQuery;
        this.eventHandler = eventHandler;
    }
}