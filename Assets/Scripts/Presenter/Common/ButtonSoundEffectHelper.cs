using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MiniUnidux.Util;
using TestUnityCardGame.Domain.Common;
using TestUnityCardGame.Presenter.Common;

namespace TestUnityCardGame.Presenter.Common {
	public class ButtonSoundEffectHelper : MonoBehaviour
	{
		// sound
		[SerializeField] SE soundEffect;
		private SoundManager soundManager;

		void Start()
		{
			// CommonからSoundManagerを取得する
			soundManager = (SoundManager)new CommonObjectGetUtil().GetCommonObject("SoundManager");
		}
		public void PlaySE(){
			if (soundManager != null) soundManager.PlaySE(soundEffect);
		}

		void OnDestroy()
		{}
	}
}