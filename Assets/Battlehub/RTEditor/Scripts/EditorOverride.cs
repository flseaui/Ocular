using Battlehub.RTCommon;
using System;
using System.Collections;
using UnityEngine;

namespace Battlehub.RTEditor
{
    public class EditorOverride : MonoBehaviour
    {
        private IRTEState m_rteState;
        private IRuntimeEditor m_editor;

        protected virtual void Awake()
        {
            m_rteState = IOC.Resolve<IRTEState>();
            if (m_rteState != null)
            {
                if (m_rteState.IsCreated)
                {
                    OnEditorExist();
                }
                else
                {
                    m_rteState.Created += OnEditorCreated;
                }
            }
            else
            {
                OnEditorExist();
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_rteState != null)
            {
                m_rteState.Created -= OnEditorCreated;
            }
        }

        protected virtual void OnEditorExist()
        {
            m_editor = IOC.Resolve<IRuntimeEditor>();
            if (m_editor.IsOpened)
            {
                OnEditorOpened();
            }
            else
            {
                m_editor.IsOpenedChanged += OnIsOpenedChanged;
            }
        }

        private void OnIsOpenedChanged()
        {
            m_editor.IsOpenedChanged -= OnIsOpenedChanged;
            if(m_editor.IsOpened)
            {
                OnEditorOpened();
            }
        }

        protected virtual void OnEditorCreated(object obj)
        {
            OnEditorExist();
        }

        protected virtual void OnEditorOpened()
        {

        }

        protected void RunNextFrame(Action action)
        {
            StartCoroutine(CoWaitForEndOfFrame(action));
        }

        private IEnumerator CoWaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }
    }
}

