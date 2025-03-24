#if UNITY_EDITOR

using System;
using UnityEditor.Build.Reporting;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class PopsignAIBuilder 
{
    private static readonly string[] scenes  = 
            {
                "Assets/PopSignMain/Scenes/opening.unity", 
                "Assets/PopSignMain/Scenes/map.unity", 
                "Assets/PopSignMain/Scenes/game.unity", 
                "Assets/PopSignMain/Scenes/practice.unity", 
                "Assets/PopSignMain/Scenes/settings.unity", 
                "Assets/PopSignMain/Scenes/howtoplay.unity", 
                "Assets/PopSignMain/Scenes/permission.unity", 
                "Assets/PopSignMain/Scenes/popsign_regular.unity", 
            };
    [MenuItem("NanaUtils/1. Build Publishing Game (Android)")]
    public static void BuildGameAndroid ()
    {
        BuildReport report = BuildPipeline.BuildPlayer(
            scenes,
            "Build/Android", 
            BuildTarget.Android, 
            BuildOptions.None
        );

        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build Android succeeded: " + report.summary.totalSize + " bytes");
        }

        if (report.summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
        }
    }

    [MenuItem("NanaUtils/2. Build iOS Workspace (iOS)")]
    public static void BuildGameiOS ()
    {
        BuildReport report = BuildPipeline.BuildPlayer(
            scenes,
            "Build/.iOS-Workspace", 
            BuildTarget.iOS, 
            BuildOptions.None
        );

        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build iOS succeeded: " + report.summary.totalSize + " bytes");
        }

        if (report.summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.Log("Build failed");
        }
    }
    [MenuItem("NanaUtils/3. Build Publishing Archive (iOS)")]
    public static void BuildArchiveiOS() {
        try
        {        
            var buildProcess = new Process();
            buildProcess.EnableRaisingEvents = true;
            buildProcess.StartInfo.FileName = "xcodebuild";
            buildProcess.StartInfo.Arguments = @"
                -project Build/.iOS-Workspace/Unity-iPhone.xcodeproj
                -scheme Unity-iPhone
                -configuration Release
                -sdk iphoneos
                -archivePath Build/.iOS-Workspace/PopsignAI.xcarchive
                clean archive
            ";
            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.RedirectStandardOutput = true;
            // buildProcess.StartInfo.RedirectStandardInput = true;
            buildProcess.StartInfo.RedirectStandardError = true;
            buildProcess.StartInfo.CreateNoWindow = true;
            buildProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    UnityEngine.Debug.Log("Intermediate: " + args.Data);
                }
            };
            buildProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    UnityEngine.Debug.LogError("Error: " + args.Data);
                }
            };
            buildProcess.Start();
            buildProcess.BeginOutputReadLine();
            buildProcess.BeginErrorReadLine();
            buildProcess.WaitForExit();

            UnityEngine.Debug.Log("Successfully Built iOS archive"); // ":\n\nOutput Stream:\n" + buildProcess.StandardOutput.ReadToEnd() + "\nError Stream:\n" + buildProcess.StandardError.ReadToEnd());
        }
        catch( Exception e )
        {
            UnityEngine.Debug.LogError( "Unable build iOS archive: " + e.Message );
        }
    }

    [MenuItem("NanaUtils/4. Export Publishing Package (iOS)")]
    public static void ExportGameiOS() {
        try {
            var buildProcess = new Process();
            buildProcess.EnableRaisingEvents = true;
            buildProcess.StartInfo.FileName = "xcodebuild";
            buildProcess.StartInfo.Arguments = @"
                -exportArchive 
                -archivePath Build/.iOS-Workspace/PopsignAI.xcarchive 
                -exportPath Build/iOS/PopsignAI 
                -exportOptionsPlist Build/.iOS-Workspace/ExportOptions.plist
            ";
            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.StartInfo.RedirectStandardOutput = true;
            // buildProcess.StartInfo.RedirectStandardInput = true;
            buildProcess.StartInfo.RedirectStandardError = true;
            buildProcess.StartInfo.CreateNoWindow = true;
            buildProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    UnityEngine.Debug.Log("Intermediate: " + args.Data);
                }
            };
            buildProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    UnityEngine.Debug.LogError("Error: " + args.Data);
                }
            };
            buildProcess.Start();
            buildProcess.BeginOutputReadLine();
            buildProcess.BeginErrorReadLine();
            buildProcess.WaitForExit();

            UnityEngine.Debug.Log("Successfully Built iOS package");
        }
        catch (Exception e) {
            UnityEngine.Debug.LogError("Unable to export iOS package: " + e.Message);
        }
    }

    [MenuItem("NanaUtils/5. Generate Publishing Manifest (iOS)")]
    public static void GenerateiOSManifest() {
        UnityEngine.Debug.Log(PlayerSettings.applicationIdentifier);
        UnityEngine.Debug.Log(PlayerSettings.bundleVersion);
        // TODO: change based on signdata URL
        // TODO: add in app icon
        // TODO: take in version from build settings
        // TODO: take in app name from build settings
        string mainfest_template = @"
<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>items</key>
    <array>
        <dict>
            <key>assets</key>
            <array>
                <dict>
                    <key>kind</key>
                    <string>software-package</string>
                    <key>url</key>
                    <string>https://signdata.cc.gatech.edu/res/apps/popsignai/ios/test/PopSignAI.ipa</string> <!-- URL to the .ipa -->
                </dict>
                <dict>
                    <key>kind</key>
                    <string>display-image</string>
                    <key>url</key>
                    <string>https://example.com/path-to-your-app/icon.png</string> <!-- Optional: App icon -->
                </dict>
            </array>
            <key>metadata</key>
            <dict>
                <key>bundle-identifier</key>
                <string>{0}</string>
                <key>bundle-version</key>
                <string>{1}</string>
                <key>kind</key>
                <string>software</string>
                <key>title</key>
                <string>{2}</string>
            </dict>
        </dict>
    </array>
</dict>
</plist>
";
        var sr = File.CreateText("Build/iOS/PopSignAI/main.plist");
        sr.WriteLine (mainfest_template, PlayerSettings.applicationIdentifier, PlayerSettings.bundleVersion, PlayerSettings.productName);
        sr.Close();
        UnityEngine.Debug.Log("Created build manifest");
    }
}

#endif