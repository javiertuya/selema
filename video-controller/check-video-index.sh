#!/bin/bash
# KEEP THIS FILE WITH LINUX LINE ENDINGS
#
# Checks that every video referenced in the selema video index (video-index.log) is a valid recorded video:
# the file exists, it is not empty, it is a complete mp4 file (top level boxes span the whole file and include
# the moov box, that is written when the recording is finalized). Also checks that the index has no
# duplicated entries (a video saved twice means that the second save may have overwritten the first one).
# Intended to run in CI after the tests, as it covers the videos of drivers managed at the class level,
# that can't be checked from the tests.
# The index is only appended: when running locally, clean the report folder before the tests, or the videos of
# previous runs will be reported as duplicated.
#
# Usage: check-video-index.sh <selema report folder> [<wait seconds>]
# - wait seconds: maximum time to wait for the videos to become valid, for browser services that write
#   the videos asynchronously after the session ends (e.g. dynamic grid). Default: no wait

REPORT_DIR="$1"
WAIT="${2:-0}"
MIN_VIDEO_SIZE=10000

INDEX="$REPORT_DIR/video-index.log"
ERRORS=0
DEADLINE=$((SECONDS + WAIT))

report() {
  echo "::error::$1"
  ERRORS=$((ERRORS+1))
}

# Reads the big endian unsigned integer of the given number of bytes at the given offset of a file
read_uint() {
  local value=0 byte
  for byte in $(od -An -tu1 -j "$2" -N "$3" "$1"); do
    value=$(( (value << 8) | byte ))
  done
  echo "$value"
}

# Checks a video file, if not valid, prints the reason and returns non zero
check_video() {
  local file="$1" size offset box_size box_type has_moov=false
  if [ ! -f "$file" ]; then
    echo "Video file not found: $file"
    return 1
  fi
  size=$(stat -c '%s' "$file")
  if [ "$size" -lt "$MIN_VIDEO_SIZE" ]; then
    echo "Video file has $size bytes, expected at least $MIN_VIDEO_SIZE: $file"
    return 1
  fi
  if [ "$(dd if="$file" bs=1 skip=4 count=4 2>/dev/null)" != "ftyp" ]; then
    echo "Video file does not have the mp4 signature: $file"
    return 1
  fi
  # Walk the top level boxes: each one starts with its size (4 bytes) and type (4 bytes)
  offset=0
  while [ "$offset" -lt "$size" ]; do
    box_size=$(read_uint "$file" "$offset" 4)
    box_type=$(dd if="$file" bs=1 skip=$((offset + 4)) count=4 2>/dev/null)
    if [ "$box_size" -eq 1 ]; then # 64 bit size follows the type
      box_size=$(read_uint "$file" $((offset + 8)) 8)
    elif [ "$box_size" -eq 0 ]; then # box extends to the end of file
      box_size=$((size - offset))
    fi
    if [ "$box_size" -lt 8 ]; then
      echo "Video file has an invalid mp4 box at offset $offset: $file"
      return 1
    fi
    [ "$box_type" == "moov" ] && has_moov=true
    offset=$((offset + box_size))
  done
  if [ "$offset" -ne "$size" ]; then
    echo "Video file is truncated (mp4 boxes span $offset bytes, file has $size): $file"
    return 1
  fi
  if [ "$has_moov" != "true" ]; then
    echo "Video file is not finalized (no moov box): $file"
    return 1
  fi
  echo "OK ($size bytes): $file"
}

if [ ! -s "$INDEX" ]; then
  report "Video index not found or empty: $INDEX"
else
  DUPLICATES=$(grep -v '^$' "$INDEX" | sort | uniq -d)
  for dup in $DUPLICATES; do
    report "Video saved more than once in the index: $dup"
  done

  while IFS= read -r entry; do
    file="$REPORT_DIR/$entry"
    # Retry until the video is valid or the wait time is exhausted (all videos share the same deadline)
    until result=$(check_video "$file"); do
      [ "$SECONDS" -ge "$DEADLINE" ] && break
      sleep 2
    done
    if [[ "$result" == OK* ]]; then
      echo "$result"
    else
      report "$result"
    fi
  done < <(grep -v '^$' "$INDEX" | sort -u)
fi

if [ "$ERRORS" -gt 0 ]; then
  echo "Found $ERRORS problems in the recorded videos"
  exit 1
fi
exit 0
