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
  private static T instance;
  private static object _lock = new object();
  private static bool onDestroyed = false;

  public void OnDestroy () {
    onDestroyed = true;
  }

  public static T Instance {
    get {

      // When Unity quits, it destroys objects in a random order. So, during
      // the quit process, a script could request a singleton instance after
      // Unity has destroyed the singleton. Instead of creating a new
      // singleton instance that Unity would not clean up later, we  throw an
      // exception:
      if(onDestroyed) {
        string eStr = String.Format("[Singleton] {0} Instance already " +
                                      "destroyed by OnDestroy", typeof(T));
        throw new ArgumentException(eStr);
      }
      
      lock(_lock) {
        if (instance == null) {
          instance = (T)FindObjectOfType(typeof(T));
          
          if(FindObjectsOfType(typeof(T)).Length > 1) {
            string eStr = String.Format("[Singleton] More than one {0} " +
                                          "Instance", typeof(T));
            throw new ArgumentException(eStr);
          }
          
          if(instance == null) {
            GameObject singleton = new GameObject();
            instance = singleton.AddComponent<T>();
            singleton.name = "(singleton) "+ typeof(T).ToString();
            
            DontDestroyOnLoad(singleton);
            
            Debug.LogFormat("[Singleton] An instance of {0} was requested, " +
                            "so {1} was created", typeof(T), singleton);
          }
        }
        
        return instance;
      }
    }
  }
}
