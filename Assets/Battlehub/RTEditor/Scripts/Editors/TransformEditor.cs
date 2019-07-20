using Battlehub.RTCommon;
using Battlehub.Utils;
using UnityEngine;

namespace Battlehub.RTEditor
{
    public class TransformEditor : ComponentEditor
    {
        protected override void InitEditor(PropertyEditor editor, PropertyDescriptor descriptor)
        {
            base.InitEditor(editor, descriptor);

            if(Editor.Tools.LockAxes == null)
            {
                return;
            }

            if(descriptor.ComponentMemberInfo == Strong.PropertyInfo((Transform x) => x.position, "position"))
            {
                Vector3Editor vector3Editor = (Vector3Editor)editor;
                vector3Editor.IsXInteractable = !Editor.Tools.LockAxes.PositionX;
                vector3Editor.IsYInteractable = !Editor.Tools.LockAxes.PositionY;
                vector3Editor.IsZInteractable = !Editor.Tools.LockAxes.PositionZ;
            }

            if (descriptor.ComponentMemberInfo == Strong.PropertyInfo((Transform x) => x.rotation, "rotation"))
            {
                Vector3Editor vector3Editor = (Vector3Editor)editor;
                vector3Editor.IsXInteractable = !Editor.Tools.LockAxes.RotationX;
                vector3Editor.IsYInteractable = !Editor.Tools.LockAxes.RotationY;
                vector3Editor.IsZInteractable = !Editor.Tools.LockAxes.RotationZ;
            }

            if (descriptor.ComponentMemberInfo == Strong.PropertyInfo((Transform x) => x.localScale, "localScale"))
            {
                Vector3Editor vector3Editor = (Vector3Editor)editor;
                vector3Editor.IsXInteractable = !Editor.Tools.LockAxes.ScaleX;
                vector3Editor.IsYInteractable = !Editor.Tools.LockAxes.ScaleY;
                vector3Editor.IsZInteractable = !Editor.Tools.LockAxes.ScaleZ;
            }
        }
    }
}

