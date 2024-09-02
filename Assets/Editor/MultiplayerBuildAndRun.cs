using UnityEditor;
using UnityEngine;

public class MultiplayerBuildAndRun
{
    [MenuItem("Tools/Run Multiplayer/Win64/1 Players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }
    
    [MenuItem("Tools/Run Multiplayer/Win64/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }

    [MenuItem("Tools/Run Multiplayer/Win64/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }

    [MenuItem("Tools/Run Multiplayer/Win64/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 1; i <= playerCount; i++)
        {
            //BuildPipeline.BuildPlayer(GetScenePaths(),
            //    "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
            //    BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);

            // 빌드 경로 설정
            string path = "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe";

            // 빌드 옵션 설정: Development 빌드와 함께 실행 시 창모드로
            BuildPipeline.BuildPlayer(GetScenePaths(),
                path,
                BuildTarget.StandaloneWindows64,
                BuildOptions.Development | BuildOptions.ShowBuiltPlayer);

            // 창모드로 실행되도록 명령줄 인수 추가
            System.Diagnostics.Process.Start(path, "-screen-fullscreen 0 -screen-width 800 -screen-height 600");
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}