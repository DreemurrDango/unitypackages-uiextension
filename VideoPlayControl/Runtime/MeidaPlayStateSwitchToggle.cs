using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;
using System;

namespace DreemurrStudio.UIExtension.VideoPlayControl
{
    /// <summary>
    /// UI控制视频播放状态开关组件
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class MediaPlayStateSwitchToggle : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("所控制的视频播放的播放器")]
        private MediaPlayer mediaPlayer;

        /// <summary>
        /// 所依赖的 Toggle 组件，其开启状态表示暂停中，关闭状态表示播放中
        /// </summary>
        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
            if(mediaPlayer.Info != null && mediaPlayer.MediaOpened)
            {
                var isPlaying = mediaPlayer.Control.IsPlaying();
                if (_toggle.isOn != !isPlaying) _toggle.isOn = !isPlaying;
            }
        }

        private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode ec)
        {
            if(et == MediaPlayerEvent.EventType.Unpaused) _toggle.SetIsOnWithoutNotify(false);
            else if (et == MediaPlayerEvent.EventType.Paused) _toggle.SetIsOnWithoutNotify(true);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn) mediaPlayer.Pause();
            else mediaPlayer.Play();
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
        }
    }
}
