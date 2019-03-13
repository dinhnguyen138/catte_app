using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseHandler : MonoBehaviour
{
    protected Firebase.Auth.FirebaseAuth auth;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    // Start is called before the first frame update
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth.IdTokenChanged -= IdTokenChanged;
    }

    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
    }

    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
    }
}
