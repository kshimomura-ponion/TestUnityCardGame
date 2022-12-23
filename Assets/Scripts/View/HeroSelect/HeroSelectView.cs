using System.Net;
using System.Security.AccessControl;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UniRx;
using MiniUnidux;
using MiniUnidux.SceneTransition;
using MiniUnidux.Util;
using TestUnityCardGame.Presenter.Hero;

namespace TestUnityCardGame.View.HeroSelect
{
    public class HeroSelectView : MonoBehaviour
    {
        public Button battleStartButton;  // button of battle start

        public Transform canvasTransform;  // canvas transform

        [SerializeField] HeroInfoDialog heroInfoDialogPrefab; // Hero Infomantion Dialog Prefab  
        [System.NonSerialized] public HeroInfoDialog heroInfoDialog = null;

        [SerializeField] AlertDialog alertDialogPrefab; // Alert Dialog Prefab  
        [System.NonSerialized] public AlertDialog alertDialog = null;

        // Hero Prefab
        [SerializeField] HeroInfoController heroInfoPrefab;

        [SerializeField] TextMeshProUGUI untilEndOfSelectText;

        private void Awake()
        {
            // ヒーロー情報ダイアログを生成する
            if (heroInfoDialog == null)
            {
                heroInfoDialog = Instantiate(heroInfoDialogPrefab, canvasTransform, false);
            }

            // 警告ダイアログを生成する
            if (alertDialog == null)
            {
                alertDialog = Instantiate(alertDialogPrefab, canvasTransform, false);
            }
        }

        private void Start()
        {
           
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        // いらないオブジェクトの破棄
        void CleanUp()
        {
            if (alertDialog != null)
            {
                Destroy(alertDialog.gameObject);
            }

            if (heroInfoDialog != null)
            {
                Destroy(heroInfoDialog.gameObject);
            }
        }

        public void ShowHeroInfoDialog(int heroID, Player player, Sprite icon, string name, string info)
        {
            heroInfoDialog.Show(heroID, player, icon, name, info);
        }

        public void SetUntilEndOfSelectText(string untilEndOfSelectString)
        {
            untilEndOfSelectText.text = untilEndOfSelectString;
        }

        public HeroInfoController GetHeroInfoPrefab()
        {
            return heroInfoPrefab;
        }
    }
}