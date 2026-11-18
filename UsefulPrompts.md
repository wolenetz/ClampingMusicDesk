### Useful prompt for claude validation of build123d model file using local MCP server:
```
Use the following steps to analyze
model_file.py
Use the b123d-modeling skill and build123d-mcp tools directly (no Bash python, no subagents).
  The CAD scripts are in /workspace/ClampingMusicDesk/src/py/.
  1. First, call mcp__build123d-mcp__execute with:
     import os, sys; sys.path.insert(0, '/workspace/ClampingMusicDesk/src/py'); os.chdir('/workspace/ClampingMusicDesk/src/py')
     so sibling imports inside src/py/ resolve cleanly.
  2. Read file being analyzed /workspace/ClampingMusicDesk/src/py/ using the Read tool.
  3. Execute its code via mcp__build123d-mcp__execute, appending show(<part_var>, name="l_clamp") at the end so the shape is registered in the session. (Skip or comment out any top-level export_stl / export_step lines during validation).
  4. Call mcp__build123d-mcp__validate, mcp__build123d-mcp__measure, and mcp__build123d-mcp__render_view and report the results.
```
