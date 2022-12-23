using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestUnityCardGame.Presenter.Common
{
    public class CommonSceneSetting : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            // EventSystem シングルトンインスタンスが存在しない場合、
            // EventSystem を動的に生成する
            if (EventSystem.current == null) {
                var instance = new GameObject ("EventSystem");
                EventSystem.current = instance.AddComponent<EventSystem>();
            }
        }
    }
}