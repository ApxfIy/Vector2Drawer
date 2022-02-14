using Apxfly.Editor.Attributes;
using Apxfly.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Apxfly.Editor.CustomDrawers
{
    [CustomPropertyDrawer(typeof(Vector2Selector))]
    public class Vector2SelectorDrawer : PropertyDrawer
    {
        private static bool IsShift => Event.current.shift;
        private static bool IdCtrl => Event.current.control;

        private const float Height = 2 * DefaultHeight;
        private const float DefaultHeight = 18;
        private const float CircleRadius = Height * 0.4f;

        private bool IsInitialized => _id != 0;
        private bool IsVector2 => fieldInfo.FieldType == typeof(Vector2);
        private bool IsVector2Int => fieldInfo.FieldType == typeof(Vector2Int);

        private float _currentScale = 1;
        private int _id;
        private Vector2 _circleCenter;
        private SerializedProperty _property;
        private GUIContent _label;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVector2 && !IsVector2Int)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (position.width < 308)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!IsInitialized)
                Initialize(property, label);

            Handle(position);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void Initialize(SerializedProperty property, GUIContent label)
        {
            _id = GUIUtility.GetControlID(FocusType.Passive);
            _property = property;
            _label = label;
        }

        private void Handle(Rect position)
        {
            switch (CurrentEventType())
            {
                case EventType.MouseDown:
                    OnMouseDown();
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag();
                    break;
                case EventType.MouseUp:
                    OnMouseUp();
                    break;
                case EventType.ScrollWheel:
                    OnScrollWheel();
                    break;
            }

            OnRepaint(position);
        }

        private void OnMouseDown()
        {
            var mousePosition = Event.current.mousePosition;

            if (!IsInsideCircle(_circleCenter, CircleRadius, mousePosition)) return;

            LockFocus();
            OnMouseDrag();
        }

        private void OnMouseDrag()
        {
            var mousePosition = CurrentEvent().mousePosition;

            if (!IsFocusLocked()) return;

            var newPosition = (mousePosition - _circleCenter).normalized * CircleRadius;
            SetValue(new Vector2(newPosition.x, -newPosition.y) * _currentScale);

            CurrentEvent().Use();
        }

        private void OnMouseUp()
        {
            if (!UnlockFocus()) return;

            CurrentEvent().Use();
        }

        private void OnScrollWheel()
        {
            var delta = -Event.current.delta.y;

            var newScale = Mathf.Clamp(_currentScale + delta * GetScrollSensitivity(), 0.01f, 1000f);

            if (Mathf.Approximately(newScale, _currentScale)) return;

            SetValue((GetValue() / _currentScale) * newScale);

            _currentScale = newScale;

            CurrentEvent().Use();
        }

        private void OnRepaint(Rect position)
        {
            var topLeft = new Vector2(position.xMin, position.yMin);
            var centerY = topLeft.y + Height * 0.5f - DefaultHeight * 0.5f;

            _circleCenter = new Vector2(position.xMax - CircleRadius * 2, topLeft.y + Height * 0.5f);

            var rect = new Rect(topLeft.x, centerY, position.width, 18);
            EditorGUI.PropertyField(rect, _property, _label);

            GLUtility.DrawWireFrameCircle(_circleCenter, CircleRadius, 50, Color.grey);

            var currentValue = GetValue();
            GLUtility.DrawLine(_circleCenter,
                _circleCenter + (new Vector2(currentValue.x, -currentValue.y).normalized *
                                 CircleRadius), Color.green);
        }

        private Vector2 GetValue()
        {
            return IsVector2 ? _property.vector2Value : _property.vector2IntValue;
        }

        private void SetValue(Vector2 newValue)
        {
            if (IsVector2)
                _property.vector2Value = newValue;
            else
                _property.vector2IntValue = Vector2Int.RoundToInt(newValue);
        }

        private EventType CurrentEventType()
        {
            return Event.current.GetTypeForControl(_id);
        }

        private static Event CurrentEvent()
        {
            return Event.current;
        }

        private bool IsFocusLocked()
        {
            return GUIUtility.hotControl == _id;
        }

        private void LockFocus()
        {
            EditorGUI.FocusTextInControl(null);
            GUIUtility.hotControl = _id;
        }

        private bool UnlockFocus()
        {
            var isLocked = GUIUtility.hotControl == _id;

            if (!isLocked) return false;

            GUIUtility.hotControl = 0;
            return true;
        }

        private static float GetScrollSensitivity()
        {
            const float defaultSensitivity = 0.4f;

            if (IsShift)
                return defaultSensitivity * 5f;

            if (IdCtrl)
                return defaultSensitivity * 0.2f;

            return defaultSensitivity;
        }

        private static bool IsInsideCircle(Vector2 center, float radius, Vector2 position)
        {
            return (position - center).sqrMagnitude < radius * radius;
        }
    }
}
