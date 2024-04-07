//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Runtime.InteropServices.ComTypes;

//[CustomEditor(typeof(DeckSO))]
//public class DeckEditor : Editor
//{
//    private SerializedProperty suitsProperty;
//    private DeckSO deck;

//    private void OnEnable()
//    {
//        deck = (DeckSO)target;
//        suitsProperty = serializedObject.FindProperty("suits");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        EditorGUILayout.LabelField("Suits", EditorStyles.boldLabel);

//        for (int i = 0; i < suitsProperty.arraySize; i++)
//        {
//            SerializedProperty suitProperty = suitsProperty.GetArrayElementAtIndex(i);
//            SerializedProperty suitNameProperty = suitProperty.FindPropertyRelative("suitName");
//            SerializedProperty cardsProperty = suitProperty.FindPropertyRelative("cards");

//            EditorGUILayout.BeginVertical(GUI.skin.box);
//            EditorGUILayout.PropertyField(suitNameProperty);
//            EditorGUI.indentLevel++;

//            for (int j = 0; j < cardsProperty.arraySize; j++)
//            {
//                SerializedProperty cardProperty = cardsProperty.GetArrayElementAtIndex(j);
//                SerializedProperty rankProperty = cardProperty.FindPropertyRelative("Rank");

//                EditorGUILayout.PropertyField(rankProperty);
//                // Add additional fields for other card data properties as needed
//            }

//            EditorGUI.indentLevel--;
//            EditorGUILayout.EndVertical();
//        }

//        if (GUILayout.Button("Add Suit"))
//        {
//            deck.suits.Add(new Suit());
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
