using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SLS.StateMachineH
{
    public static class EditorUtilities
    {
        public static void DrawScriptClicker<T>(Object target, Rect position) where T : MonoBehaviour
        {
            GUI.enabled = false;
            EditorGUI.ObjectField(position, "Script", MonoScript.FromMonoBehaviour((T)target), typeof(T), false);
            GUI.enabled = true;
        }
        /*
        public static void DrawStateNodeHeader(Rect position, State target, SerializedObject serializedObject)
        {
            
            Rect scriptRect = new(position.x, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            Rect machineRect = new(position.x + EditorGUIUtility.singleLineHeight, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            Rect warningRect = new(position.x + (EditorGUIUtility.singleLineHeight * 2), position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            Rect behaviorsRect = new(position.x + (EditorGUIUtility.singleLineHeight * 3f), position.y, position.xMax - warningRect.xMax, EditorGUIUtility.singleLineHeight);

            var stateChild = target as State;
            StateMachine machine = target is StateMachine 
                ? target as StateMachine 
                : (target as State).Machine;


            // Draw MonoBehavior icon in scriptRect  
            Texture monoIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
            GUI.DrawTexture(scriptRect, monoIcon, ScaleMode.ScaleToFit);

            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseDown && (scriptRect.Contains(Event.current.mousePosition)))
                EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour((MonoBehaviour)target));


            // Draw State Machine icon in machineRect  
            Texture machineIcon = EditorGUIUtility.GetIconForObject(machine);
            GUI.DrawTexture(machineRect, machineIcon, ScaleMode.ScaleToFit);

            machineRect.LinkToObject(machine);

            // Draw a red box in the spacerRect if Machine's StatesSetup is false  
            if (stateChild != null && stateChild.Machine != null && !stateChild.Machine.StatesSetup)
            {
                GUIContent warning = new();
                warning.text = "!!!";
                warning.tooltip = "This State Machine's Tree either has not been set up or has been labeled Dirty!";
                GUI.color = Color.red;
                GUI.Label(warningRect, warning);
                GUI.color = Color.white;

                warningRect.LinkToObject(machine);
            }



            // Draw (X behaviors) label showing the number of behaviors attached to the node  
            int behaviorCount = target.Behaviors?.Length ?? 0;
            GUI.Label(behaviorsRect, $"({behaviorCount} Behaviors)");
        }

        public static void DrawStateGroupEditor(Rect position, State target, SerializedObject serializedObject)
        {
            if (!target.HasChildren) return;

            GUIContent childrenLabel = new($"({target.ChildCount} Children)");
            Rect childrenRect = new(position.x, position.y, GUI.skin.label.CalcSize(childrenLabel).x, position.height);

            Rect addStateButtonRect = new(position.xMax - (position.height * 2), position.y, position.height, position.height);
            Rect addGroupButtonRect = new(position.xMax - position.height, position.y, position.height, position.height);

            // Draw "(X Children)" label  
            GUI.Label(childrenRect, childrenLabel);

            // Draw current child label if game is playing  
            if (Application.isPlaying && target.CurrentChild != null)
            {
                Rect currentChildRect = new(childrenRect.xMax, position.y, addStateButtonRect.xMin - childrenRect.xMax, position.height);
                currentChildRect.LinkToObject(target.CurrentChild as Object);
                GUIContent current = new();
                current.image = EditorGUIUtility.GetIconForObject(target.CurrentChild as Object);
                current.text = (target.CurrentChild as MonoBehaviour).gameObject.name;
                current.tooltip = "The currently active child node of this Machine/Group.";
                GUI.Label(currentChildRect, current, EditorStyles.linkLabel);
            }

            GUIContent addStateButtonIcon = new("", EditorGUIUtility.IconContent(stateIconPath).image, "Add a new State under this Machine/Group");
            GUIContent addGroupButtonIcon = new("", EditorGUIUtility.IconContent(stateGroupIconPath).image, "Add a new Group under this Machine/Group");

            if (GUI.Button(addStateButtonRect, addStateButtonIcon)) target.AddChildNode();

            // Draw "ADD" label above the buttons
            Vector2 addTextSize = GUI.skin.label.CalcSize(new("ADD"));
            Rect addLabelRect = new(position.xMax - position.height - (addTextSize.x/2), addStateButtonRect.yMin - addTextSize.y-5, position.height * 2, position.height);
            GUI.Label(addLabelRect, "ADD", EditorStyles.boldLabel);
        }
        */
        public static string stateIconPath = "Assets/Plugins/HierarchyStateMachine/Editor/Icons/State.png";
        public static string stateGroupIconPath = "Assets/Plugins/HierarchyStateMachine/Editor/Icons/StateGroup.png";
        public static string stateMachineIconPath = "Assets/Plugins/HierarchyStateMachine/Editor/Icons/StateMachine.png";

        public static void LinkToObject(this Rect rect, Object target)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.PingObject(target as Object);
                Selection.activeObject = target as Object;
            }
        }

        public static void DrawStateNode(ref Rect position, State target, SerializedObject serializedObject, bool showMachineIcon = true)
        {
            if((target as State).Machine == null)
            {
                EditorGUI.LabelField(position, "This State is not connected to a StateMachine.");
                return;
            }


            float iconSize = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            // Script Icon  
            Rect scriptRect = new(position.x, position.y, iconSize, iconSize);
            Texture monoIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
            GUI.DrawTexture(scriptRect, monoIcon, ScaleMode.ScaleToFit);
            EditorGUIUtility.AddCursorRect(scriptRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseDown && scriptRect.Contains(Event.current.mousePosition))
                EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour((MonoBehaviour)target));

            // Machine Icon  
            Rect machineRect = new(scriptRect.xMax + spacing, position.y, iconSize, iconSize);
            if (showMachineIcon)
            {
                StateMachine machine = target is StateMachine ? target as StateMachine : (target as State)?.Machine;
                if (machine != null)
                {
                    Texture machineIcon = EditorGUIUtility.GetIconForObject(machine);
                    GUI.DrawTexture(machineRect, machineIcon, ScaleMode.ScaleToFit);
                    machineRect.LinkToObject(machine);
                }
            }


            bool showWarningIcon = !((State)target).Machine.StatesSetup;
            // Warning Icon  
            Rect warningRect = new(machineRect.xMax + spacing, position.y, iconSize, iconSize);
            if (showWarningIcon && target is State stateChild && stateChild.Machine != null && !stateChild.Machine.StatesSetup)
            {
                GUIContent warning = new("!!!", "This State Machine's Tree either has not been set up or has been labeled Dirty!");
                GUI.color = Color.red;
                GUI.Label(warningRect, warning);
                GUI.color = Color.white;
                warningRect.LinkToObject(stateChild.Machine);
            }

            // (X Children) Text  
            Rect childrenRect = new(warningRect.xMax + spacing, position.y, GUI.skin.label.CalcSize(new GUIContent($"({target.ChildCount} Children)")).x, iconSize);
            GUI.Label(childrenRect, $"({target.ChildCount} Children)");

            // (X Behaviors) Text  
            Rect behaviorsRect = new(childrenRect.xMax + spacing, position.y, GUI.skin.label.CalcSize(new GUIContent($"({target.Behaviors?.Length ?? 0} Behaviors)")).x, iconSize);
            GUI.Label(behaviorsRect, $"({target.Behaviors?.Length ?? 0} Behaviors)");

            // Add Child Button
            float buttonWidth = EditorGUIUtility.singleLineHeight * 1.5f;
            Rect addChildButtonRect = new(position.xMax - buttonWidth, position.y, buttonWidth, iconSize);
            GUI.color = new(.341f, .706f, 1.141f);
            if (GUI.Button(addChildButtonRect, "+"))
            {
                State newNode = target.AddChildNode();
                RenameAfterCreate(newNode.gameObject);
            }
            GUI.color = Color.white;
            position.y += position.yMax;
        }

        async static void RenameAfterCreate(GameObject select)
        {
            Selection.activeObject = select;
            await Task.Delay(20);
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            EditorApplication.ExecuteMenuItem("Edit/Rename");
        }


        public static void DrawActiveStateMachineDetails(ref Rect position, State target)
        {
            float iconSize = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;

            // Active Status Icon  
            Rect statusRect = new(position.x, position.y, GUI.skin.label.CalcSize(new("[OFF]")).x, iconSize);
            GUIContent statusContent = new(target.Active ? "[ON]" : "[OFF]", target.Active ? "StateMachine is Active" : "StateMachine is Inactive");
            GUI.color = target.Active ? Color.green : Color.red;
            GUI.Label(statusRect, statusContent);
            GUI.color = Color.white;

            // Active Child Details  
            Rect childRect = new(statusRect.xMax + spacing, position.y, position.width - statusRect.width - spacing, iconSize);
            if (target.HasChildren && target.CurrentChild != null)
            {
                GUIContent markerContent = new("Active: ");
                childRect.width = GUI.skin.label.CalcSize(markerContent).x;
                GUI.Label(childRect, markerContent);

                childRect.x = childRect.xMax;
                GUIContent directContent = new(target.CurrentChild.name, "Click to focus on the active child.");
                childRect.width = GUI.skin.label.CalcSize(directContent).x;

                childRect.LinkToObject(target.CurrentChild);
                GUI.Label(childRect, directContent, EditorStyles.linkLabel);

                if (target is StateMachine Machine && Machine.CurrentChild != null)
                {
                    childRect.x = childRect.xMax;
                    GUIContent arrowContent = new($" {new string('-', Machine.CurrentState.Layer)}> ");
                    childRect.width = GUI.skin.label.CalcSize(arrowContent).x;
                    GUI.Label(childRect, arrowContent);

                    childRect.x = childRect.xMax;
                    GUIContent finalContent = new(Machine.CurrentState.name, "Click to focus on the active state.");
                    childRect.width = GUI.skin.label.CalcSize(finalContent).x;

                    childRect.LinkToObject(Machine.CurrentState);
                    GUI.Label(childRect, finalContent, EditorStyles.linkLabel);
                }
                    
                
            }
            position.y += position.yMax + 12;
        }
    } 
    [CustomEditor(typeof(State), false)]
    public class StateChildEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Rect position = EditorGUILayout.GetControlRect();
            float startY = position.y;

            EditorUtilities.DrawStateNode(ref position, (State)target, serializedObject, true);

            if (Application.isPlaying) EditorUtilities.DrawActiveStateMachineDetails(ref position, (State)target);

            GUILayout.Space(position.yMax - startY); 
        }
    }

    [CustomEditor(typeof(StateMachine), true)]
    public class StateMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var stateMachine = (StateMachine)target;

            Rect position = EditorGUILayout.GetControlRect();
            float startY = position.y;

            EditorUtilities.DrawStateNode(ref position, (State)target, serializedObject, false);

            if (Application.isPlaying)
                EditorUtilities.DrawActiveStateMachineDetails(ref position, (State)target);
            else
            {
                position.height += 15;

                GUI.color = stateMachine.StatesSetup ? new(.9f, 1.1f, .9f) : new(1, .7f, .7f);
                if (GUI.Button(position, stateMachine.StatesSetup ? "State Machine Tree Built!" : "State Machine Tree needs Building!"))
                {
                    if (stateMachine.StatesSetup)
                    {
                        bool answer = EditorUtility.DisplayDialog("Setup", "This State Machine has already been setup, do you still want to setup again?", "Yes", "No");
                        if (!answer) return;
                    }

                    stateMachine.Setup(stateMachine, stateMachine, -1);

                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(stateMachine.gameObject.scene);

                    EditorUtility.DisplayDialog("Setup Complete", "This State Machine has been setup, be sure to save changes to the prefab.", "Nice.");
                }
                GUI.color = Color.white;
                position.y = position.yMax;
            }

            GUILayout.Space(position.yMax - startY + 5);

            if(target.GetType() != typeof(StateMachine))
            {
                // Display additional fields unique to derived classes  
                SerializedProperty property = serializedObject.GetIterator();
                property.NextVisible(true);

                // Skip the first 9 properties (Script, Name, Layer, Active, CurrentChild, Type, HasChildren, Behaviors, ChildCount)
                for (int i = 0; i < 8; i++)
                    if (!property.NextVisible(false)) 
                        break;

                while (property.NextVisible(false))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(property, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(target);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}

