using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniUnidux.Util {
    public class CommonObjectGetUtil
    {
        public MonoBehaviour GetCommonObject(String typeName) {
            Scene scene = SceneManager.GetSceneByName("Common");
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                MonoBehaviour[] monoBehaviours = rootGameObject.GetComponents<MonoBehaviour> ();
                foreach (var monoBehaviour in monoBehaviours){
                    if (monoBehaviour.GetType().Name == typeName)
                        return monoBehaviour;
                }
            }
            return null;
        }
    }
}