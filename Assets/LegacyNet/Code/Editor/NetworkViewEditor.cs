#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace LegacyNetworking.Editor
{
    [CustomEditor(typeof(NetworkView)), CanEditMultipleObjects]
    public class NetworkViewEditor : UnityEditor.Editor
    {
        private static bool renderData = false;
        private static bool renderObservables = false;
        public override void OnInspectorGUI() {
            var target = (NetworkView)this.target;
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUI.BeginDisabledGroup(true);
            renderData = EditorGUILayout.Foldout(renderData, "Network View Data");
            if (renderData)
                RenderData(target);
            renderObservables = EditorGUILayout.Foldout(renderObservables, "Observed Components");
            if (renderObservables)
                RenderObservables(target);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }

        private void RenderObservables(NetworkView target) {
            GUILayout.Label(target.observables.Count.ToString());
            foreach (var observable in target.observables) {
                if (observable is not MonoBehaviour)
                    continue;
                var comp = observable as MonoBehaviour;
                EditorGUILayout.ObjectField(comp.name, comp, typeof(MonoBehaviour), true);
            }
        }

        private static void RenderData(NetworkView target) {
            EditorGUILayout.IntField("View ID", target.viewId);
            EditorGUILayout.IntField("Owner", target.owner);
            EditorGUILayout.TextField("Instantiate Key", target.instantiateKey != null ? target.instantiateKey.ToString() : "NONE");
            EditorGUILayout.Toggle("Is Instantiated", target.isInstantiated);
            EditorGUILayout.Toggle("Is Mine", target.isMine);
        }
    }
}
#endif