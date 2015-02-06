using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InGameGUIButton))]
public class InGameGUIButtonEditor : Editor
{

   private SerializedProperty          m_operatedObjectProp;

   public void OnEnable () 
   {
		m_operatedObjectProp = serializedObject.FindProperty ("m_operatedObject");
	}

   public override void OnInspectorGUI()
   {
      InGameGUIButton button = (InGameGUIButton)target;

      // display the "operated object" property
      EditorGUILayout.PropertyField( m_operatedObjectProp, new GUIContent( "Operated object" ) );

      // list all available methods on the operated object
      if ( button.m_operatedObject != null )
      {
         MethodInfo[] methods = button.m_operatedObject.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
         string[] options = new string[methods.Length + 1];

         int idx = 1;
         foreach ( MethodInfo method in methods )
         {
            if ( method.GetParameters().Length == 0 && !method.IsConstructor )
            {
               options[idx++] = method.Name;
            }
         }

         int currentlySelectedOption = button.GetMethodIdx( options );
         if ( currentlySelectedOption < 0 )
         {
            currentlySelectedOption = 0;
         }
         int selectedOption = EditorGUILayout.Popup( "method", currentlySelectedOption, options );
         button.m_methodName = options[selectedOption];
         
      }

      // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties ();
      
   }
}