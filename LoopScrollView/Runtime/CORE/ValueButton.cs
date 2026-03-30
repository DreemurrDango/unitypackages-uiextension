using System;
using UnityEngine;
using UnityEngine.UI;

namespace DreemurrStudio.UIExtension.LoopScrollView
{
    /// <summary>
    /// 带值按钮扩展：点击时传出按钮的值（一般用于记录序号值）
    /// </summary>
    public class ValueButton<T> : Button
    {
        ///// <summary>
        ///// 所依赖的按钮组件
        ///// </summary>
        //private Button button;

        [HideInInspector]
        [Tooltip("带值按钮被按下时动作")]
        public event Action<T> OnValueButtonClick;

        /// <summary>
        /// 该按钮的ID值
        /// </summary>
        private T value;
        //public Button GetButton => button ??= GetComponent<Button>();

        /// <summary>
        /// 初始化视频选项按钮
        /// </summary>
        /// <param name="id">按钮的ID值</param>
        /// <param name="sprite">按钮的图片，可置空省略，省略时不更改原图片</param>
        public void Init(T id, Sprite sprite = null)
        {
            this.value = id;
            if(sprite != null)image.sprite = sprite;
            onClick.AddListener(OnValueButtonDown);
        }

        /// <summary>
        /// 带值按钮按下时额外动作
        /// </summary>
        private void OnValueButtonDown()
        {
            OnValueButtonClick?.Invoke(value);
            Debug.Log($"点击了带值按钮{value}");
        }
    }
}