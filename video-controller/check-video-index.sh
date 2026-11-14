#!/bin/bash
# KEEP THIS FILE WITH LINUX LINE ENDINGS
#
# Checks that every video referenced in the selema video index (video-index.log) is a valid recorded video:
# the file exists, it is not empty or truncated and it has the mp4 signature. Also checks that the index has no
# duplicated entries (a video saved twice means that the second save may have overwritten the first one).
# Intended to run in CI after the tests, as it covers the videos of drivers managed at the class level,
# that can't be checked from the tests.
# The index is only appended: when running locally, clean the report folder before the tests, or the videos of
# previous runs will be reported as duplicated.
#
# Usage: check-video-index.sh <selema report folder> [<video folder prefix> [--warn-only]]
# - video folder prefix: subfolder (relative to the report folder) where the videos are actually stored,
#   if it differs from the path written in the index (e.g. dynamic grid stores videos under assets)
# - --warn-only: reports problems as GitHub warnings, but does not fail

REPORT_DIR="$1"
PREFIX="$2"
WARN_ONLY="$3"
MIN_VIDEO_SIZE=10000

INDEX="$REPORT_DIR/video-index.log"
ERRORS=0

report() {
  if [ "$WARN_ONLY" == "--warn-only" ]; then
    echo "::warning::$1"
  else
    echo "::error::$1"
  fi
  ERRORS=$((ERRORS+1))
}

if [ ! -s "$INDEX" ]; then
  report "Video index not found or empty: $INDEX"
else
  DUPLICATES=$(grep -v '^$' "$INDEX" | sort | uniq -d)
  for dup in $DUPLICATES; do
    report "Video saved more than once in the index: $dup"
  done

  while IFS= read -r entry; do
    file="$REPORT_DIR/${PREFIX:+$PREFIX/}$entry"
    if [ -n "$PREFIX" ] && [ ! -f "$REPORT_DIR/$entry" ]; then
      report "Video link in the selema log does not resolve (video is under '$PREFIX'): $entry"
    fi
    if [ ! -f "$file" ]; then
      report "Video file not found: $file"
      continue
    fi
    size=$(stat -c '%s' "$file")
    if [ "$size" -lt "$MIN_VIDEO_SIZE" ]; then
      report "Video file has $size bytes, expected at least $MIN_VIDEO_SIZE: $file"
      continue
    fi
    signature=$(dd if="$file" bs=1 skip=4 count=4 2>/dev/null)
    if [ "$signature" != "ftyp" ]; then
      report "Video file does not have the mp4 signature: $file"
      continue
    fi
    echo "OK ($size bytes): $file"
  done < <(grep -v '^$' "$INDEX" | sort -u)
fi

if [ "$ERRORS" -gt 0 ] && [ "$WARN_ONLY" != "--warn-only" ]; then
  echo "Found $ERRORS problems in the recorded videos"
  exit 1
fi
exit 0
