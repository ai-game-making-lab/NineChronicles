#!/bin/bash
set -ex

if [[ "$#" != "1" ]]; then
  {
    echo "error: too few arguments"
    echo "usage: $0 BUILD-TARGET"
  } > /dev/stderr
  exit 1
fi

build_target="$1"

case "$build_target" in
  macOS|MacOS|OSX|StandaloneOSX)
    build_method="BuildStandaloneOSX"
    ;;
  Windows|StandaloneWindows|StandaloneWindows64)
    build_method="BuildStandaloneWindows"
    ;;
  Linux|StandaloneLinux|StandaloneLinux64)
    build_method="BuildStandaloneLinux64"
    ;;
  Android)
    build_method="BuildAndroid"
    ;;
  iOS|IOS)
    build_method="BuildiOS"
    ;;
  *)
    build_method="Build$build_target"
    ;;
esac

# shellcheck disable=SC1090
source "$(dirname "$0")/_common.sh"

title "Unity license"
install_license

title "Build binary"
/opt/unity/Editor/Unity \
  -quit \
  -batchmode \
  -nographics \
  -logFile \
  -username "$UNITY_EMAIL" \
  -password "$UNITY_PASSWORD" \
  -serial "$UNITY_SERIAL" \
  -projectPath nekoyume \
  -executeMethod "NekoyumeEditor.Builder.$build_method"
