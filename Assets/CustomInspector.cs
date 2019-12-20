/*
using UnityEngine;
using UnityEditor;
//using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(BoolArray))]
public class CustomInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        Rect newposition = position;
        newposition.y += 18f;
        SerializedProperty data = property.FindPropertyRelative("column");
        //data.rows[0][]
        for(int j=data.arraySize-1;j>=0;j--)
        {
                SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
                newposition.height = 18f;
                //if(row.arraySize != 7)
                    //row.arraySize = 7;
                newposition.width = position.width/ 10;
                for(int i=0;i<row.arraySize; i++)
                {
                    EditorGUI.PropertyField(newposition,row.GetArrayElementAtIndex(i),GUIContent.none);
                    newposition.x += newposition.width;
                }
 
                newposition.x = position.x;
                newposition.y += 18f;
        }
        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty data = property.FindPropertyRelative("column");
        return 18f * data.arraySize+18;
    }
}
*/

