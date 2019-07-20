using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.UIControls.DockPanels
{
    public class PointerEventArgs
    {
        public PointerEventData EventData
        {
            get;
            private set;
        }

        public PointerEventArgs(PointerEventData eventData)
        {
            EventData = eventData;
        }
    }

    public delegate void TabEventArgs(Tab sender);
    public delegate void TabEventArgs<T>(Tab sender, T args);

    public class Tab : MonoBehaviour, IBeginDragHandler, IDragHandler, IInitializePotentialDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event TabEventArgs<bool> Toggle;
        public event TabEventArgs<PointerEventData> PointerDown;
        public event TabEventArgs<PointerEventData> PointerUp;
        public event TabEventArgs<PointerEventData> InitializePotentialDrag;
        public event TabEventArgs<PointerEventData> BeginDrag;
        public event TabEventArgs<PointerEventData> Drag;
        public event TabEventArgs<PointerEventData> EndDrag;
        public event TabEventArgs Close;

        private DockPanel m_root;

        [SerializeField]
        private TabPreview m_tabPreviewPrefab = null;
        private TabPreview m_tabPreview;

        [SerializeField]
        private CanvasGroup m_canvasGroup = null;

        [SerializeField]
        private Image m_img = null;

        [SerializeField]
        private TextMeshProUGUI m_text = null;

        [SerializeField]
        private Toggle m_toggle = null;

        [SerializeField]
        private Button m_closeButton = null;

        private RectTransform m_rt;

        public Sprite Icon
        {
            get { return m_img.sprite; }
            set
            {
                m_img.sprite = value;
                m_img.gameObject.SetActive(value != null);
            }
        }

        public string Text
        {
            get { return m_text.text; }
            set { m_text.text = value; }
        }

        public ToggleGroup ToggleGroup
        {
            get { return m_toggle.group; }
            set
            {
                if(m_toggle.group)
                {
                    m_toggle.group.UnregisterToggle(m_toggle);
                }

                m_toggle.group = value;
            }
        }

     
        public bool IsOn
        {
            get { return m_toggle.isOn; }
            set { m_toggle.isOn = value; }
        }

        public int Index
        {
            get { return transform.GetSiblingIndex(); }
            set { transform.SetSiblingIndex(value); }
        }

        public bool IsCloseButtonVisible
        {
            get
            {
                if(m_closeButton != null)
                {
                    return m_closeButton.gameObject.activeSelf;
                }
                return false;
            }
            set
            {
                if(m_closeButton != null)
                {
                    m_closeButton.gameObject.SetActive(value);
                }
            }
        }

        public Vector3 PreviewPosition
        {
            get { return m_tabPreview.transform.position; }
            set
            {
                m_tabPreview.transform.position = value;
                Vector3 localPosition = m_tabPreview.transform.localPosition;
                localPosition.z = 0;
                m_tabPreview.transform.localPosition = localPosition;
            }
        }

        public bool IsPreviewContentActive
        {
            get { return m_tabPreview.IsContentActive; }
            set { m_tabPreview.IsContentActive = value; }
        }

        public Vector2 PreviewHeaderSize
        {
            get { return m_tabPreview.HeaderSize; }
        }

        public Vector2 PreviewContentSize
        {
            set
            {
                m_tabPreview.MaxWidth = m_rt.rect.width; 
                m_tabPreview.Size = value;
            }
        }

        private void Awake()
        {
            m_rt = (RectTransform)transform;
            m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
            if(m_closeButton != null)
            {
                m_closeButton.onClick.AddListener(OnCloseButtonClick);
            }
        }

        private void Start()
        {
            m_root = GetComponentInParent<DockPanel>();
        }

        private void OnDestroy()
        {
            if(m_toggle != null)
            {
                m_toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }

            if (m_closeButton != null)
            {
                m_closeButton.onClick.RemoveListener(OnCloseButtonClick);
            }
        }

        private void OnToggleValueChanged(bool value)
        {
            if(Toggle != null)
            {
                Toggle(this, value);
            }
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            if(InitializePotentialDrag != null)
            {
                InitializePotentialDrag(this, eventData);
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            m_tabPreview = Instantiate(m_tabPreviewPrefab, m_root.Preview);

            RectTransform previewTransform = (RectTransform)m_tabPreview.transform;
            RectTransform rt = (RectTransform)transform;
            PreviewPosition = rt.position;
            previewTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rt.rect.width);
            previewTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rt.rect.height);
            
            m_tabPreview.Icon = Icon;
            m_tabPreview.Text = Text;
            m_tabPreview.IsCloseButtonVisible = IsCloseButtonVisible;

            m_canvasGroup.alpha = 0;

            if (BeginDrag != null)
            {
                BeginDrag(this, eventData);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if(Drag != null)
            {
                Drag(this, eventData);
            }
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {          
            m_canvasGroup.alpha = 1;

            if (EndDrag != null)
            {
                EndDrag(this, eventData);
            }

            Destroy(m_tabPreview.gameObject);
            m_tabPreview = null;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {            
            m_toggle.isOn = true;

            if(PointerDown != null)
            {
                PointerDown(this, eventData);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if(PointerUp != null)
            {
                PointerUp(this, eventData);
            }
        }

        private void OnCloseButtonClick()
        {
            if(Close != null)
            {
                Close(this);
            }
        }

    }
}
