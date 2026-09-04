using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaSystemsLab.Editor
{
    public static class ArenaProjectValidator
    {
        private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";
        private const string ProjectVersionPrefix = "m_EditorVersion: ";
        private const string MenuPath = "Tools/Arena Systems Lab/Validate Project";

        [MenuItem(MenuPath)]
        private static void ValidateFromMenu()
        {
            Log(ValidateCurrentProject());
        }

        public static void ValidateFromCommandLine()
        {
            bool success = Log(ValidateCurrentProject());
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        public static List<string> ValidateCurrentProject()
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            InputActionMap playerMap = inputActions == null ? null : inputActions.FindActionMap("Player", false);
            bool hasEnabledBuildScene = false;

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) != null)
                {
                    hasEnabledBuildScene = true;
                    break;
                }
            }

            return ValidateSnapshot(
                ReadExpectedEditorVersion(),
                Application.unityVersion,
                hasEnabledBuildScene,
                inputActions != null,
                playerMap != null && playerMap.FindAction("Move", false) != null,
                playerMap != null && playerMap.FindAction("Attack", false) != null);
        }

        public static List<string> ValidateSnapshot(
            string expectedEditorVersion,
            string currentEditorVersion,
            bool hasEnabledBuildScene,
            bool hasInputActions,
            bool hasMoveAction,
            bool hasAttackAction)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(expectedEditorVersion))
            {
                errors.Add("ProjectSettings/ProjectVersion.txt does not declare an Editor version.");
            }
            else if (!string.Equals(expectedEditorVersion, currentEditorVersion, StringComparison.Ordinal))
            {
                errors.Add($"Editor version mismatch: expected {expectedEditorVersion}, current {currentEditorVersion}.");
            }

            if (!hasEnabledBuildScene)
            {
                errors.Add("Build Settings requires at least one enabled Scene asset.");
            }

            if (!hasInputActions)
            {
                errors.Add($"Input Actions asset is missing at {InputActionsPath}.");
            }
            else
            {
                if (!hasMoveAction)
                {
                    errors.Add("Input Actions requires Player/Move.");
                }

                if (!hasAttackAction)
                {
                    errors.Add("Input Actions requires Player/Attack.");
                }
            }

            return errors;
        }

        private static bool Log(List<string> errors)
        {
            if (errors.Count == 0)
            {
                Debug.Log("Arena Systems Lab validation passed.");
                return true;
            }

            for (int i = 0; i < errors.Count; i++)
            {
                Debug.LogError($"Arena Systems Lab validation failed: {errors[i]}");
            }

            return false;
        }

        private static string ReadExpectedEditorVersion()
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "ProjectSettings", "ProjectVersion.txt"));
            if (!File.Exists(path))
            {
                return null;
            }

            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith(ProjectVersionPrefix, StringComparison.Ordinal))
                {
                    return line.Substring(ProjectVersionPrefix.Length).Trim();
                }
            }

            return null;
        }
    }
}
