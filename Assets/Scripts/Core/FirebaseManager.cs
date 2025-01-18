using UnityEngine;
using MagicWords.Core.Interfaces;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;

namespace MagicWords.Core
{
    public class FirebaseManager : MonoBehaviour, IFirebaseManager
    {
        private FirebaseAuth auth;
        private FirebaseDatabase database;
        private bool isInitialized;

        public bool IsInitialized => isInitialized;

        public event Action<string> OnAuthStateChanged;
        public event Action<string> OnError;

        public void Initialize()
        {
            InitializeFirebaseAsync();
        }

        public void Dispose()
        {
            if (auth != null)
            {
                auth.StateChanged -= AuthStateChanged;
                auth = null;
            }
            database = null;
            isInitialized = false;
        }

        private async void InitializeFirebaseAsync()
        {
            try
            {
                // Check dependencies
                var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebaseServices();
                }
                else
                {
                    Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                    OnError?.Invoke("Firebase initialization failed");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firebase initialization error: {ex.Message}");
                OnError?.Invoke("Firebase initialization error");
            }
        }

        private void InitializeFirebaseServices()
        {
            // Initialize Auth
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;

            // Initialize Database
            database = FirebaseDatabase.DefaultInstance;

#if UNITY_EDITOR
            database.SetPersistenceEnabled(false);
#endif

            isInitialized = true;
            Debug.Log("Firebase services initialized");
        }

        private void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (auth.CurrentUser != null)
            {
                OnAuthStateChanged?.Invoke(auth.CurrentUser.UserId);
            }
            else
            {
                OnAuthStateChanged?.Invoke(null);
            }
        }

        public async void SignIn(string email, string password)
        {
            try
            {
                var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
                Debug.Log($"User signed in successfully: {result.User.UserId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Sign in failed: {ex.Message}");
                OnError?.Invoke("Sign in failed");
            }
        }

        public void SignOut()
        {
            if (auth != null && auth.CurrentUser != null)
            {
                auth.SignOut();
                Debug.Log("User signed out");
            }
        }

        public async void CreateUser(string email, string password, string username)
        {
            try
            {
                var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                await CreateUserProfile(result.User.UserId, username, email);
                Debug.Log($"User created successfully: {result.User.UserId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"User creation failed: {ex.Message}");
                OnError?.Invoke("User creation failed");
            }
        }

        private async Task CreateUserProfile(string userId, string username, string email)
        {
            var userData = new UserProfile
            {
                Username = username,
                Email = email,
                CreatedAt = DateTime.UtcNow.ToString("O"),
                Level = 1,
                Score = 0
            };

            string json = JsonUtility.ToJson(userData);
            await database.RootReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        }

        public string GetCurrentUserId()
        {
            return auth?.CurrentUser?.UserId;
        }

        public DatabaseReference GetDatabaseReference(string path)
        {
            return database?.GetReference(path);
        }

        [Serializable]
        private class UserProfile
        {
            public string Username;
            public string Email;
            public string CreatedAt;
            public int Level;
            public int Score;
        }
    }
}