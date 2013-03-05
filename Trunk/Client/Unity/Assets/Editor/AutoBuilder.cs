/* 
AutoBuilder.cs
Automatically changes the target platform and creates a build.
 
Installation
Place in an Editor folder.
 
Usage
Go to File > AutoBuilder and select a platform. These methods can also be run from the Unity command line using -executeMethod AutoBuilder.MethodName.
 
License
Copyright (C) 2011 by Thinksquirrel Software, LLC
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
 
public static class AutoBuilder {
 
	static string GetProjectName()
	{
        /*string test = "test";
		string[] s = Application.dataPath.Split('/');
		return s[s.Length - 2];*/
		return PlayerSettings.productName;
	}
 
	static string[] GetScenePaths()
	{
		string[] scenes = new string[EditorBuildSettings.scenes.Length];
 
		for(int i = 0; i < scenes.Length; i++)
		{
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}
 
		return scenes;
	}
 
	[MenuItem("File/AutoBuilder/Windows/32-bit")]
	static void PerformWinBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
		BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName() + ".exe",BuildTarget.StandaloneWindows,BuildOptions.Development);
	}
 
	[MenuItem("File/AutoBuilder/Windows/64-bit")]
	static void PerformWin64Build ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName() + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
	}
 
	[MenuItem("File/AutoBuilder/iOS")]
	static void PerformiOSBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName(), BuildTarget.iPhone, BuildOptions.AllowDebugging & BuildOptions.Development & BuildOptions.ShowBuiltPlayer);
	}
	[MenuItem("File/AutoBuilder/Android")]
	static void PerformAndroidBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName() + ".apk", BuildTarget.Android, BuildOptions.Development);
	}
	[MenuItem("File/AutoBuilder/Web/Standard")]
	static void PerformWebBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebPlayer);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName(), BuildTarget.WebPlayer, BuildOptions.Development);
	}
	[MenuItem("File/AutoBuilder/Web/Streamed")]
	static void PerformWebStreamedBuild ()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WebPlayerStreamed);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Bin/" + GetProjectName(), BuildTarget.WebPlayerStreamed, BuildOptions.None);
	}
}