using Game;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Objects
{
    /// <summary>
    /// A walkable object housing a button, can target multiple blocks and change their color on press.
    /// </summary>
    public class ButtonWalkable : Walkable, IController
    {
        [SerializeField] private Button _buttonModel;

        [SerializeField] public GlassesController.OcularState Color;

        private Vector3 _initialPosition;

        private List<GameObject> _lines;

        /// <summary>
        /// The blocks that will be modified when the button is pressed.
        /// </summary>
        public List<Colorable> TargetBlocks;

        /// <summary>
        /// The state of the <c>Button</c> (true - pressed, false - unpressed).
        /// </summary>
        public bool State
        {
            get => _buttonModel.State;
            set => _buttonModel.State = value;
        }

        protected override void OnAwake()
        {
            _lines = new List<GameObject>();
        }

        private void Start()
        {
            if (_buttonModel == null)
                transform.Find("ButtonModel").gameObject.AddComponent<Button>();

            _initialPosition = _buttonModel.transform.localPosition;

            // register with all target blocks
            TargetBlocks.ForEach(t => t.RegisterController(this));


            _buttonModel.OnStateChanged += () =>
            {
                Debug.Log($"OH YEAH DA STATE IS {_buttonModel.State}");
                if (_buttonModel.State)
                {
                    // Press button
                    _buttonModel.transform.localPosition = _initialPosition - new Vector3(0, .1f, 0);
                    TargetBlocks.ForEach(t =>
                    {
                        t.OcularState = Color;
                        t.UpdateState();
                        Debug.Log("YES");
                    });
                }
                else
                {
                    // Release button
                    _buttonModel.transform.localPosition = _initialPosition;
                    TargetBlocks.ForEach(t =>
                    {
                        t.OcularState = GlassesController.OcularState.Null;
                        t.UpdateState();
                    });
                }
            };
        }

        private void Update()
        {
            if (Enabled)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    TargetBlocks.ForEach(t =>
                    {
                        var y = _buttonModel.GetComponent<MeshFilter>().sharedMesh.bounds.max.y;
                        if (t.Outlined)
                            DrawLine(new Vector3(_buttonModel.transform.position.x, y, _buttonModel.transform.position.z), t.transform.position,
                                Colorable.StateToColor(t.State == Colorable.BlockState.Visible ? Color : t.OcularState));
                    });
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                foreach (var line in _lines)
                {
                    Destroy(line.gameObject);
                }
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f)
        {
            var myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            var lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.SetColors(color, color);
            lr.SetWidth(0.05f, 0.05f);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            _lines.Add(myLine);
        }
    }
}
