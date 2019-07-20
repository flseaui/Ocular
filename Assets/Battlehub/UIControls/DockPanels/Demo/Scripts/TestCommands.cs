using UnityEngine;
using UnityEngine.UI;

using Battlehub.UIControls.Dialogs;
using System.Linq;

namespace Battlehub.UIControls.DockPanels
{
    public class TestCommands : MonoBehaviour
    {
        [SerializeField]
        private DialogManager m_dialog = null;

        [SerializeField]
        private Button m_defaultLayout = null;

        [SerializeField]
        private Button m_addButton = null;

        [SerializeField]
        private Button m_deleteButton = null;

        [SerializeField]
        private Button m_showMsgBox = null;

        [SerializeField]
        private Button m_showPopup = null;

        [SerializeField]
        private DockPanel m_dockPanels = null;

        [SerializeField]
        private Sprite m_sprite = null;

        [SerializeField]
        private string m_headerText = null;

        [SerializeField]
        private Transform m_contentPrefab = null;

        [SerializeField]
        private RegionSplitType m_splitType = RegionSplitType.None;

        private int m_counter;

        private void Awake()
        {
            m_defaultLayout.onClick.AddListener(OnDefaultLayout);
            m_addButton.onClick.AddListener(OnAddClick);
            m_deleteButton.onClick.AddListener(OnDeleteClick);
            m_showMsgBox.onClick.AddListener(OnShowMsgBox);
            m_showPopup.onClick.AddListener(OnShowDialog);
        }

        private void Start()
        {
            OnDefaultLayout();
        }

        private void OnDestroy()
        {
            if(m_defaultLayout != null)
            {
                m_defaultLayout.onClick.RemoveListener(OnDefaultLayout);
            }

            if(m_addButton != null)
            {
                m_addButton.onClick.RemoveListener(OnAddClick);
            }
            
            if(m_deleteButton != null)
            {
                m_deleteButton.onClick.RemoveListener(OnDeleteClick);
            }

            if(m_showMsgBox != null)
            {
                m_showMsgBox.onClick.RemoveListener(OnShowMsgBox);
            }

            if(m_showPopup != null)
            {
                m_showPopup.onClick.RemoveListener(OnShowDialog);
            }
        }

        private void SelectRegionIfRequired()
        {
            if (m_dockPanels.SelectedRegion == null || !m_dockPanels.SelectedRegion.CanAdd())
            {
                Region leafRegion = m_dockPanels.GetComponentsInChildren<Region>().Where(r => r.ChildrenPanel.childCount == 0).First();
                leafRegion.IsSelected = true;
            }
        }

        private void OnAddClick()
        {
            SelectRegionIfRequired();

            if (m_dockPanels.SelectedRegion != null)
            {
                m_counter++;

                Transform content = Instantiate(m_contentPrefab);
                m_dockPanels.SelectedRegion.Add(m_sprite, m_headerText + " " + m_counter, content, false, m_splitType);
            }
        }

        private void OnDeleteClick()
        {
            SelectRegionIfRequired();

            if (m_dockPanels.SelectedRegion != null)
            {
                Region region = m_dockPanels.SelectedRegion;
                region.RemoveAt(region.ActiveTabIndex);
            }
        }

        private void OnShowDialog()
        {
            m_counter++;

            Transform content = Instantiate(m_contentPrefab);             
            Dialog dlg = m_dialog.ShowDialog(m_sprite, "Popup Test" + m_counter, content, (sender, okArgs) =>
            {
                Debug.Log("YES");

            }, "Yes", (sender, cancelArgs) =>
            {
                Debug.Log("NO");
            }, "No");

            dlg.IsOkVisible = false;
            dlg.IsCancelVisible = false;
        }

        private void OnShowMsgBox()
        {
            m_dialog.ShowDialog(m_sprite, "Msg Test", "Your message", (sender, okArgs) =>
            {
                Debug.Log("YES");
                OnShowMsgBox();
                okArgs.Cancel = true;

            }, "Yes", (sender, cancelArgs) =>
            {
                Debug.Log("NO");
            }, "No");
        }


        private void OnDefaultLayout()
        {
            Region rootRegion = m_dockPanels.RootRegion;
            rootRegion.Clear();
            foreach (Transform child in m_dockPanels.Free)
            {
                Region region = child.GetComponent<Region>();
                region.Clear();
            }

            LayoutInfo layout = new LayoutInfo(false,
                new LayoutInfo(Instantiate(m_contentPrefab).transform, m_headerText + " " + m_counter++, m_sprite),
                new LayoutInfo(true,
                    new LayoutInfo(Instantiate(m_contentPrefab).transform, m_headerText + " " + m_counter++, m_sprite),
                    new LayoutInfo(Instantiate(m_contentPrefab).transform, m_headerText + " " + m_counter++, m_sprite),
                    0.5f),
                0.75f);

            m_dockPanels.RootRegion.Build(layout);
        }
    }
}
