#!/bin/bash

UNITY_PATH="/Applications/Unity/Hub/Editor/6000.0.26f1/Unity.app/Contents/MacOS/Unity"

echo "Building Project"
"$UNITY_PATH" \
                -projectPath "../.." \
                -executeMethod PopsignAIBuilder.BuildGameAndroid \
                -logFile build.log


APK_PATH="../../Build/Android/*.apk"

if [ -z "$APK_PATH" ]; then
    echo "we ain't find it"
    exit 1
fi


echo "Looking for device..."
adb devices

adb wait-for-device
echo "Found device!"

adb install -r -d "$APK_PATH"
echo "Install finished."

# -d directs command to the only connected usb device