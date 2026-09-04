using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ArenaSystemsLab.Editor
{
    public static class ArenaWindowsBuilder
    {
        public const string OutputRelativePath = "Builds/Windows/ArenaSystemsLab.exe";

        [MenuItem("Tools/Arena Systems Lab/Build Windows Development")]
        private static void BuildFromMenu()
        {
            BuildReport report = BuildWindows();
            EditorUtility.RevealInFinder(report.summary.outputPath);
        }

        public static void BuildWindowsFromCommandLine()
        {
            bool success = false;
            try
            {
                BuildWindows();
                success = true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static BuildReport BuildWindows()
        {
            var validationErrors = ArenaProjectValidator.ValidateCurrentProject();
            if (validationErrors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, validationErrors));
            }

            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone) != ScriptingImplementation.Mono2x)
            {
                throw new InvalidOperationException("Windows Development build requires the existing Standalone Mono scripting backend.");
            }

            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string outputPath = Path.Combine(projectRoot, OutputRelativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            var options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Windows build failed: {report.summary.result}, errors: {report.summary.totalErrors}.");
            }

            Debug.Log($"Windows Development build succeeded: {report.summary.outputPath}");
            return report;
        }
    }
}
