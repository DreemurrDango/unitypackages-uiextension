using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//using DreemurrStudio.AudioSystem;
using System.Collections;
using System;

namespace DreemurrStudio.UIExtension.LoopScrollView
{
    /// <summary>
    /// 循环选择滚动视窗控制
    /// 实现如下UI效果：进入中间缩放区域的按钮项放大、循环滚动、点击索引按钮时跳转到指定的按钮项
    /// </summary>
    public class LoopSelectScrollView : MonoBehaviour
    {
        [Tooltip("所控制的滚动视窗组件")]
        public ScrollRect scrollRect;
        [Tooltip("索引按钮组")]
        public HorizontalLayoutGroup buttonGroup;
        [Tooltip("下方的快速单选框组")]
        public ToggleGroup toggleGroup;
        [Tooltip("当选择内容时的事件回调")]
        public UnityEvent<int> onIndexContentSelect;
        [Tooltip("当当前居中的内容改变时的事件回调，参数为当前居中的按钮索引值")]
        public UnityEvent<int> onCurrentIndexChanged;

        [Header("自动化生成")]
        [Tooltip("是否自动生成选项UI实例，设置后将根据图片列表的数量自动生成对应的索引按钮与序号转跳开关\n 若不设置，需要手动生成实例并保证索引按钮、索引按钮的数量与GameManager的数据一致")]
        public bool createInstance = false;
        [SerializeField]
        [Tooltip("是否在开始时自动初始化")]
        private bool initOnStart = true;
        [SerializeField]
        [Tooltip("内容选择按钮的图片集合")]
        private List<Sprite> buttonSprites;

        [Header("互动配置")]
        [SerializeField]
        [Tooltip("带值选择按钮预制体")]
        private IDButton buttonPrefab;
        //[SerializeField]
        //[Tooltip("按下按钮时的要播放的音效名")]
        //private string buttonSFXName;
        [SerializeField]
        [Tooltip("索引转跳按钮预制体")]
        private IDToggle togglePrefab;
        //[SerializeField]
        //[Tooltip("按下转跳缩影开关时要播放的音效名")]
        //private string toggleSFXName;

        [Header("聚焦缩放")]
        [Tooltip("开始进行缩放的区域矩形，将此项设为空将不会发生缩放")]
        public RectTransform scaleCheckArea;
        [Tooltip("未处于缩放区域时的正常缩放比例")]
        public float normalScale = 0.75f;
        [Tooltip("到达中心点时的最大缩放比例")]
        public float maxScale = 1f;
        [Tooltip("是否启用动态间距调整功能，启用后靠近中心时按钮间距会变大\n注意，此功能会在组内元素过多时出现明显的抖动现象，建议在元素数量大于20个时关闭")]
        public bool dynamicSpacing = true;

        /// <summary>
        /// 视窗的矩形变换组件
        /// </summary>
        private RectTransform viewRT;
        /// <summary>
        /// 视窗左下角坐标
        /// </summary>
        private Vector2 viewRTMin;
        /// <summary>
        /// 视窗右上角坐标
        /// </summary>
        private Vector2 viewRTMax;
        /// <summary>
        /// 下方的单选节点组列表
        /// </summary>
        private List<IDToggle> _toggles;
        /// <summary>
        /// 内容选择按钮列表
        /// </summary>
        private List<IDButton> _buttons;
        /// <summary>
        /// 运行中生成的内容选择按钮组实例，用于实现循环
        /// </summary>
        private HorizontalLayoutGroup buttonGroup1;
        /// <summary>
        /// 每个按钮UI的大小
        /// </summary>
        private Vector2 buttonSize;
        /// <summary>
        /// 按钮与中心位置的距离小于该值时开始
        /// </summary>
        private float scaleDistance;
        /// <summary>
        /// 缩放区域的中心位置
        /// </summary>
        private Vector3 centerPos;
        /// <summary>
        /// 展示的人物数量
        /// </summary>
        private int num;
        /// <summary>
        /// 当前所选的ID
        /// </summary>
        private int currentIndex;
        /// <summary>
        /// 两个选择选择按钮组的矩形形变
        /// </summary>
        private RectTransform[] rts;
        /// <summary>
        /// 当前是否正在索引转跳中
        /// </summary>
        private bool inMoving;
        /// <summary>
        /// 默认的组间隔
        /// </summary>
        private float defaultSpacing;

        public List<Sprite> ButtonSprites { get => buttonSprites; set => buttonSprites = value; }
        /// <summary>
        /// 获取当前居中的按钮索引
        /// </summary>
        public int CurrentIndex => currentIndex % num;

        public void Start()
        {
            if (initOnStart) Init(buttonSprites, buttonSprites.Count / 2);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public void Init(List<Sprite> portraitSprites,int defaultIndex)
        {
            StartCoroutine(InitCoroutine(portraitSprites, defaultIndex));
            IEnumerator InitCoroutine(List<Sprite> portraitSprites, int defaultIndex)
            {
                _toggles = new List<IDToggle>();
                _buttons = new List<IDButton>();
                //生成实例或读入已有的UI实例
                if (createInstance && portraitSprites != null && portraitSprites.Count > 0)
                {
                    num = portraitSprites.Count;
                    //如果需要自动生成实例，则清空原有的按钮列表
                    foreach (var b in buttonGroup.GetComponentsInChildren<IDButton>()) Destroy(b.gameObject);
                    foreach (var t in toggleGroup.GetComponentsInChildren<IDToggle>()) Destroy(t.gameObject);
                    yield return new WaitForSeconds(0);
                    //生成索引按钮实例
                    for (int i = 0; i < num; i++)
                    {
                        var b = Instantiate(buttonPrefab.gameObject, buttonGroup.transform)
                        .GetComponent<IDButton>();
                        _buttons.Add(b);
                        var t = Instantiate(togglePrefab.gameObject, toggleGroup.transform)
                           .GetComponent<IDToggle>();
                        _toggles.Add(t);
                    }
                    buttonSprites = new List<Sprite>(portraitSprites);
                }
                else
                {
                    _buttons = new(buttonGroup.GetComponentsInChildren<IDButton>());
                    _toggles = new(toggleGroup.GetComponentsInChildren<IDToggle>());
                    if (_buttons.Count != _toggles.Count) throw new System.Exception("索引按钮与图片按钮数量不匹配");
                    num = _buttons.Count;
                }
                //初始化UI实例数据
                for (int i = 0; i < num; i++)
                {
                    _buttons[i].Init(i, buttonSprites.Count > i ? buttonSprites[i] : null);
                    _buttons[i].OnValueButtonClick += OnContentButtonClick;
                    _toggles[i].Init(i, toggleGroup);
                    _toggles[i].OnToggleSelected += OnIndexToggleClick;
                }
                //读取数据，主要是一些预计算数据
                rts = new RectTransform[2];
                rts[0] = buttonGroup.transform as RectTransform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rts[0]);
                yield return 0;
                scaleDistance = scaleCheckArea.rect.size.x / 2;
                viewRT = scrollRect.viewport;
                viewRTMin = (Vector2)viewRT.position - viewRT.rect.size / 2;
                viewRTMax = (Vector2)viewRT.position + viewRT.rect.size / 2;
                centerPos = viewRT.position;
                buttonSize = (_buttons[0].transform as RectTransform).sizeDelta;
                defaultSpacing = buttonGroup.spacing;
                //订阅事件
                scrollRect.onValueChanged.AddListener((delta) => UpdateShow(scrollRect.velocity.x));
                //用于循环视图内容的备用索引按钮组
                buttonGroup1 = Instantiate(buttonGroup.gameObject, buttonGroup.transform.parent)
                        .GetComponent<HorizontalLayoutGroup>();
                foreach (Transform pt in buttonGroup1.transform) Destroy(pt.gameObject);
                yield return 0;
                for (int i = 0; i < num; i++)
                {
                    var b = Instantiate(buttonPrefab.gameObject, buttonGroup1.transform)
                    .GetComponent<IDButton>();
                    b.Init(num + i, buttonSprites.Count > i ? buttonSprites[i] : null);
                    b.OnValueButtonClick += OnContentButtonClick;
                    _buttons.Add(b);
                }
                rts[1] = buttonGroup1.transform as RectTransform;
                yield return 0;
                //进行一次更新
                UpdateShow(1f);
                //移动到默认索引位置
                MoveToIndex(defaultIndex);
            }
        }

        /// <summary>
        /// 根据输入的X轴移动方向更新显示
        /// </summary>
        /// <param name="xVelocity">X轴运动方向</param>
        private void UpdateShow(float xVelocity = 0f)
        {
            //获取当前处于正中的内容按钮，并缩放靠近中心的按钮
            var index = -1;
            var minD = scaleDistance;
            var minT = 0f;
            for (var i = 0; i < _buttons.Count; i++)
            {
                var d = Mathf.Abs(_buttons[i].transform.position.x - centerPos.x);
                var t = Mathf.Clamp01(d / scaleDistance);
                if (d < minD)
                {
                    minD = d;
                    index = i;
                    minT = t;
                }
                if (scaleCheckArea != null)
                    _buttons[i].transform.localScale = Mathf.Lerp(maxScale, normalScale, t) * Vector3.one;
            }
            //获取当前处于正中的内容按钮，打开对应的索引按钮
            if (index >= 0 && index % num != currentIndex)
            {
                _toggles[index % num].isOn = true;
                if(currentIndex != index)onCurrentIndexChanged?.Invoke(index % num);
                currentIndex = index;
            }

            // 【关键修复】：禁用了动态间距调整代码
            // 在长列表中，动态改变 Spacing 会导致位置剧烈震荡（抽搐）。
            // 如果您必须要有间距缩放效果，建议减小列表长度或使用更高级的自定义布局算法
            //开启缩放功能时，按钮布局组间距动态调整
            if (dynamicSpacing)
            {
                var spacing = defaultSpacing;
                if (scaleCheckArea != null)
                {
                    spacing = defaultSpacing + 0.2f * defaultSpacing * (1 - minT);
                    buttonGroup.spacing = buttonGroup1.spacing = spacing;
                    buttonGroup1.padding.right = buttonGroup.padding.right = (int)spacing / 2;
                    buttonGroup1.padding.left = buttonGroup.padding.left = (int)spacing / 2;
                }
            }            

            //计算布局组最终的大小
            //虽然禁用了spacing变化，但若有其他布局变动仍需刷新，或者为了保险起见保留
            LayoutRebuilder.ForceRebuildLayoutImmediate(rts[0]);
            //循环效果控制
            LayoutRebuilder.ForceRebuildLayoutImmediate(rts[1]);
            
            var xOffset = new Vector3(rts[0].rect.size.x / 2 + rts[1].rect.size.x / 2, 0);
            var mainRT = index < num ? rts[0] : rts[1];
            var otherRT = index < num ? rts[1] : rts[0];

            if (mainRT.position.x > centerPos.x) 
                otherRT.position = mainRT.position - xOffset;            
            else otherRT.position = mainRT.position + xOffset;
        }

        #region 索引按钮转跳功能
        [ContextMenu("移动到下一个")]
        public void MoveToNext() => MoveToIndex((currentIndex + 1) % num);
        [ContextMenu("移动到上一个")]
        public void MoveToPrev() => MoveToIndex((currentIndex + num - 1) % num);

        /// <summary>
        /// 移动到指定的索引位置
        /// </summary>
        /// <param name="index"></param>
        public void MoveToIndex(int index)
        {
            if (inMoving) return;
            var ci = currentIndex % num;
            //判断左移还是右移
            var right = (index + num - ci) % num;
            var left = (ci + num - index) % num;
            int delta = left < right ? -left : right;
            int aimIndex = (currentIndex + delta + num * 2) % (num * 2);
            //计算移动的距离并开始移动
            //TODO: 优化移动效果
            float delatX = _buttons[currentIndex].transform.position.x - centerPos.x +
                delta * (defaultSpacing * 1.2f + buttonSize.x * normalScale);
            inMoving = true;
            scrollRect.inertia = false;
            scrollRect.content.DOMoveX(scrollRect.content.position.x - delatX, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { inMoving = false; scrollRect.inertia = true; });
        }

        /// <summary>
        /// 按下转跳点按钮时触发
        /// </summary>
        /// <param name="id">被点击的转跳点的ID</param>
        private void OnIndexToggleClick(int id)
        {
            //if (!string.IsNullOrEmpty(toggleSFXName))
            //    SFXManager.Instance.PlayOverlaySFX(toggleSFXName);
            MoveToIndex(id);
        }

        /// <summary>
        /// 按下内容选择按钮时触发
        /// </summary>
        /// <param name="id">按钮的ID值</param>
        private void OnContentButtonClick(int id)
        {
            //if (!string.IsNullOrEmpty(buttonSFXName))
            //    SFXManager.Instance.PlayOverlaySFX(buttonSFXName);
            onIndexContentSelect?.Invoke(id % num);
        }

        #endregion
    }
}