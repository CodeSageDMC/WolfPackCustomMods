//TODO ADD COMMENTS
public class EcoShowOnlyAttribute : UnityEngine.PropertyAttribute
{
#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(EcoShowOnlyAttribute))]
    public class PropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) { return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true); }

        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
        {
            var saveEnabled = UnityEngine.GUI.enabled;
            UnityEngine.GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            UnityEngine.GUI.enabled = saveEnabled;
        }
    }
#endif // #if UNITY_EDITOR
}
