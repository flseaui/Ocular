using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTEditor
{
    public class ConsoleLogEntry
    {
        public LogType LogType;
        public string Condition;
        public string StackTrace;

        public ConsoleLogEntry(LogType logType, string condition, string stackTrace)
        {
            LogType = logType;
            Condition = condition;
            StackTrace = stackTrace;
        }
    }

    public delegate void RuntimeConsoleEventHandler<T>(RuntimeConsole console, T arg);

    public interface IRuntimeConsole
    {
        event RuntimeConsoleEventHandler<ConsoleLogEntry> MessageAdded;
        event RuntimeConsoleEventHandler<ConsoleLogEntry[]> MessagesRemoved;

        bool Store
        {
            get;
            set;
        }

        int MaxItems
        {
            get;
        }

        IEnumerable<ConsoleLogEntry> Log
        {
            get;
        }

        int InfoCount
        {
            get;
        }

        int WarningsCount
        {
            get;
        }

        int ErrorsCount
        {
            get;
        }

        void Clear();
        
    }


    public class RuntimeConsole : MonoBehaviour, IRuntimeConsole
    {
        public event RuntimeConsoleEventHandler<ConsoleLogEntry> MessageAdded;
        public event RuntimeConsoleEventHandler<ConsoleLogEntry[]> MessagesRemoved;

        [SerializeField]
        private bool m_store = false;

        public bool Store
        {
            get { return m_store; }
            set { m_store = value; }
        }

        [SerializeField]
        private int m_maxItems = 300;
        public int MaxItems
        {
            get { return m_maxItems; }
        }

        [SerializeField]
        private int m_clearThreshold = 600;

        private Queue<ConsoleLogEntry> m_log;
        public IEnumerable<ConsoleLogEntry> Log
        {
            get { return m_log; }
        }

        private int m_infoCount;
        public int InfoCount
        {
            get { return m_infoCount; }
        }

        private int m_warningsCount;
        public int WarningsCount
        {
            get { return m_warningsCount; }
        }

        private int m_errorsCount;
        public int ErrorsCount
        {
            get { return m_errorsCount; }
        }

        private void Awake()
        {
            m_log = new Queue<ConsoleLogEntry>();
            if(m_clearThreshold <= m_maxItems)
            {
                m_clearThreshold = m_maxItems + 50;
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private void UpdateCounters(LogType type, int delta)
        {
            switch(type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    m_errorsCount += delta;
                    break;
                case LogType.Warning:
                    m_warningsCount += delta;
                    break;
                case LogType.Log:
                    m_infoCount += delta;
                    break;
            }
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            ConsoleLogEntry logEntry = null;
            if(MessageAdded != null || m_store)
            {
                logEntry = new ConsoleLogEntry(type, condition, stackTrace);
                
                m_log.Enqueue(logEntry);
                UpdateCounters(type, 1);
                if(m_log.Count > m_clearThreshold)
                {
                    ConsoleLogEntry[] removedItems = new ConsoleLogEntry[m_clearThreshold - m_maxItems];
                    for(int i = 0; i < removedItems.Length; ++i)
                    {
                        ConsoleLogEntry removedLogEntry = m_log.Dequeue();
                        removedItems[i] = removedLogEntry;
                        UpdateCounters(removedLogEntry.LogType, -1);
                    }


                    if(MessagesRemoved != null)
                    {
                        MessagesRemoved(this, removedItems);
                    }
                }
            }

            if(MessageAdded != null)
            {
                MessageAdded(this, logEntry);
            }
        }

        public void Clear()
        {
            m_infoCount = 0;
            m_warningsCount = 0;
            m_errorsCount = 0;

            ConsoleLogEntry[] logEntries = m_log.ToArray();

            m_log.Clear();

            if(MessagesRemoved != null)
            {
                MessagesRemoved(this, logEntries);
            }
        }
    }
}



