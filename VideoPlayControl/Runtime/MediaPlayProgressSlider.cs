using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System;
using UnityEngine.EventSystems;

namespace DreemurrStudio.UIExtension.VideoPlayControl
{
    [RequireComponent(typeof(Slider))]
    public class MediaPlayProgressSlider : MonoBehaviour,IBeginDragHandler,IEndDragHandler
    {
        [SerializeField]
        [Tooltip("所控制的视频播放的播放器")]
        private MediaPlayer mediaPlayer;
        [SerializeField]
        [Tooltip("显示当前播放时间点的文本")]
        private TMP_Text progressTimeText;
        [SerializeField]
        [Tooltip("时间显示格式化字符串，参数0-3分别为<当前进度分钟,当前进度秒钟,总时长分钟,总时长秒钟>")]
        private string timeFormatString = "{0:D2}:{1:D2} / {2:D2}:{3:D2}";
        [SerializeField]
        [Tooltip("拖动进度条时是否暂停播放")]
        private bool pauseOnSeek = true;

        /// <summary>
        /// 显示当前播放进度的滑动条
        /// </summary>
        private Slider progressSlider;
        /// <summary>
        /// 当前视频的完整时间
        /// </summary>
        private float _fullTime;
        /// <summary>
        /// 当前是否正在拖动进度条
        /// </summary>
        private bool isDragging = false;
        /// <summary>
        /// 完整时间的TimeSpan表示
        /// </summary>
        private TimeSpan _fullTimeSpan;

        /// <summary>
        /// 当前视频的完整时间，单位为秒
        /// </summary>
        public float FullTime
        {
            get => _fullTime;
            private set
            {
                _fullTime = value;
                _fullTimeSpan = TimeSpan.FromSeconds(_fullTime);
            }
        }

        private void Awake()
        {
            progressSlider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            progressSlider.onValueChanged.AddListener(OnProgressSliderValueChanged);
            mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        }


        private void OnDisable()
        {
            progressSlider.onValueChanged.RemoveListener(OnProgressSliderValueChanged);
            mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
        }

        private void Update()
        {
            if (mediaPlayer.Info == null || !mediaPlayer.MediaOpened)
            {
                UpdateShow(0f);
                return;
            }
            if (!isDragging) UpdateShow((float)mediaPlayer.Control.GetCurrentTime());
        }

        /// <summary>
        /// 进度条数值变化时的回调
        /// </summary>
        /// <param name="value">进度条的当前值，范围是0到1</param>
        private void OnProgressSliderValueChanged(float value)
        {
            if(mediaPlayer.Info == null || !mediaPlayer.MediaOpened)
            {
                UpdateShow(0f);
                return;
            }
            float seekTime = value * FullTime;
            mediaPlayer.Control.Seek(seekTime);
            TimeSpan timeSpan = TimeSpan.FromSeconds(seekTime);
            if(progressTimeText != null) progressTimeText.text = string.Format(timeFormatString, timeSpan.Minutes, timeSpan.Seconds, _fullTimeSpan.Minutes, _fullTimeSpan.Seconds);
        }

        private void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode ec)
        {
            if (et == MediaPlayerEvent.EventType.FirstFrameReady)
            {
                FullTime = (float)mp.Info.GetDuration();
                UpdateShow(0f);
            }
        }

        /// <summary>
        /// 更新播放进度显示
        /// </summary>
        /// <param name="currentTime">当前播放时间，单位为秒</param>
        private void UpdateShow(float currentTime)
        {
            if (FullTime > 0 && currentTime > 0)
            {
                progressSlider.SetValueWithoutNotify(currentTime / FullTime);
                TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
                if (progressTimeText != null) progressTimeText.text = string.Format(timeFormatString, timeSpan.Minutes, timeSpan.Seconds, _fullTimeSpan.Minutes, _fullTimeSpan.Seconds);
            }
            else
            {
                progressSlider.SetValueWithoutNotify(0f);
                if (progressTimeText != null) progressTimeText.text = string.Format(timeFormatString, 0, 0,_fullTimeSpan.Minutes, _fullTimeSpan.Seconds);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            if (pauseOnSeek && mediaPlayer.Control.IsPlaying()) 
                mediaPlayer.Pause();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            if (pauseOnSeek && !mediaPlayer.Control.IsPlaying())
                mediaPlayer.Play();
        }
    }
}
