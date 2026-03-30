using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DreemurrStudio.UIExtension.LoopScrollView
{
    /// <summary>
    /// 带值开关控制
    /// </summary>
    public class ValueToggle<T> : Toggle
    {
        /// <summary>
        /// 开关按钮ID，由ScientistsScrollViewUI传入
        /// </summary>
        private T value;

        [Tooltip("开关被选择时的事件")]
        public event Action<T> OnToggleSelected;

        /// <summary>
        /// 初始化:设置序号，载入开关组件
        /// </summary>
        /// <param name="id">该开关的值</param>
        /// <param name="belongGroup">所属的开关组</param>
        public void Init(T id, ToggleGroup belongGroup)
        {
            this.value = id;
            group = belongGroup;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnToggleSelected?.Invoke(value);
            Debug.Log($"Toggle{value}被选中");
        }
    }
}