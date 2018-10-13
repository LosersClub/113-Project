using System;
using UnityEngine;

/// <summary>
/// Base class that implements Singleton pattern for MonoBehavior class.
/// Inherit from this to create a singleton MonoBehavior subclass.
/// </summary>
/// <example>
/// // Define a class that inherits from SingletonMonoBehavior:
/// public class MyClass : SingletonMonoBehavior<MyClass> {
///   // protected constructor prevents external construction of objects,
///   // enforcing use of singleton instance:
///   protected GameControllerScript() {}
///   
///   // Now can implement your code, such as MonoBehavior methods:
///   void Start () {
///     // Do stuff
///   }
///   
///   void myFunc() {
///     // Do stuff...
///   }
///
///   ...
/// }
/// 
/// // Use the newly-defined singleton class, using the Instance property to
/// // access the singleton instance:
/// MyClass.Instance.myFunc();
/// </example>
/// <remarks>
/// Be aware that inheriting the class alone will not prevent a non singleton
/// constructor calls such as `T myT = new T();`.
/// To prevent that, add `protected T () {}` to your singleton class as in the
/// example.
/// 
/// As a note, this is made as MonoBehaviour because Coroutines are needed.
/// 
/// Adapted from class at http://wiki.unity3d.com/index.php/Singleton
/// </remarks>
public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
  private static T _instance;
  
  private static object _lock = new object();

  public static T Instance
  {
    get
    {
      if (applicationIsQuitting) {
        throw new ArgumentException(String.Format(
          "[Singleton] {0} Instance already destroyed on application quit.",
          typeof(T)));
      }
      
      lock(_lock)
      {
        if (_instance == null)
        {
          _instance = (T) FindObjectOfType(typeof(T));
          
          if ( FindObjectsOfType(typeof(T)).Length > 1 )
          {
            throw new ArgumentException(String.Format(
              "[Singleton] More than one {0} Instance! Reopening the scene " +
              "might fix it.",
              typeof(T)));
          }
          
          if (_instance == null)
          {
            GameObject singleton = new GameObject();
            _instance = singleton.AddComponent<T>();
            singleton.name = "(singleton) "+ typeof(T).ToString();
            
            DontDestroyOnLoad(singleton);
            
            Debug.Log("[Singleton] An instance of " + typeof(T) + 
              " is needed in the scene, so '" + singleton +
              "' was created with DontDestroyOnLoad.");
          }
        }
        
        return _instance;
      }
    }
  }
  
  private static bool applicationIsQuitting = false;
  /// <summary>
  /// When Unity quits, it destroys objects in a random order.
  /// In principle, a Singleton is only destroyed when application quits.
  /// If any script calls Instance after it have been destroyed, 
  ///   it will create a buggy ghost object that will stay on the Editor scene
  ///   even after stopping playing the Application. Really bad!
  /// So, this was made to be sure we're not creating that buggy ghost object.
  /// </summary>
  public void OnDestroy () {
    applicationIsQuitting = true;
  }
}
