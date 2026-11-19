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

```
## Coordinate System: Use these terms to understand what I mean in prompts around front/back/etc when talking about the model. Note that MCP rendering tools may rotate the perspective, so be sure you understand such rotations before making changes to the models based on rendering results.
- **FRONT** = low Y axis direction (negative)
- **BACK** = high Y axis direction (positive)  
- **LEFT** = low X axis direction (negative)
- **RIGHT** = high X axis direction (positive)
- **UP/TOP** = high Z axis direction (positive)
- **DOWN/BOTTOM** = low Z axis direction (negative)

## Prompt for Targeted Modifications to Specific Part Regions
```
When modifying a specific region of a build123d model, be explicit about:

1. WHICH part(s) you're targeting (e.g., "Part 1B lower arm" vs "Part 1A upper clamp")
2. EXACT position in coordinate terms (use FRONT/BACK/LEFT/RIGHT/UP/DOWN + specific Y/Z values when known)
3. WHAT dimensions should stay unchanged to maintain compatibility with other parts

Example: "Add a small Y-notch into the front-top of lower arm (Part 1B) where it meets upper clamp backside (Part 1A). Position at Y=35mm, Z from 0 to +11mm, X-width matching arm (~32mm), Y-depth only ~2mm. Do NOT cut into main clamp block area (-30 to +30mm in Y) or affect spine/riser areas."
```

## Prompt for Understanding Part Separation Logic  
```
Before making changes to part separation cuts, first analyze:

1. How many solids exist after all cuts? (use mcp__build123d-mcp__execute with len(lClampBuilder.part.solids()))
2. What are the bounding box ranges of each resulting solid? (check Part 1A vs Part 1B separately)
3. Which section creates which separation? Map: Section 5A = Z=0 horizontal gap, Section 5B = additional cuts

Ask first: "What does section 5B currently cut - what are its X/Y/Z position and dimensions?" before modifying it."
```

## Prompt for Making Small Targeted Cuts Without Affecting Other Areas  
```
When creating a small notch or cut that should only affect one region:

1. Specify the exact bounding box of your cut in terms of existing parameters (not hardcoded numbers)
2. Use parameter names like arm_halfW, clampHalfW, spineBotZ to make adjustments maintainable
3. Explicitly state what SHOULD NOT be affected: "Leave dorsal spine alone", "Don't chop into clamp block"

Example structure for section 5B cut:
with Locations((X_pos, Y_pos, Z_pos)):
    Box(
        width_X=2*arm_halfW+clampGap,   # Match existing feature widths
        depth_Y=clampGap,                # Small notch depth only  
        height_Z=spineBotZ-0.0,          # Span specific Z range only
        align=(Align.MIN, Align.CENTER, Align.MIN),
        mode=Mode.SUBTRACT,
    )

Verify: "Total solids should remain 2 - Part 1A and Part 1B separated but each still a single solid"
```

## Prompt for Validating Geometry Before Export  
```
Before exporting or submitting a modified model, run these checks in order:

1. mcp__build123d-mcp__validate - confirms watertight manifold, no open edges
2. Measure Part 1A spine width stays at critical dimension (e.g., "spine X-width must be 20mm for riser compatibility")  
3. Check that key parameters unchanged: clampGap, tubeSize, bolt positions
4. Verify part separation still works: len(lClampBuilder.part.solids()) == 2

Only export if all checks pass."
```

## Prompt for Debugging Unexpected Geometry Results  
```
If model builds but looks wrong or has unexpected solids count:

1. First check: "Total solids after cuts: {len(lClampBuilder.part.solids())}" - should be 2 for Part1A+Part1B
2. Render side view to see Z-split at Z=0 is correct
3. Check section cut parameters against what you intended (print them with f-strings)
4. Compare bounding boxes: Part 1A Y-range vs Part 1B Y-range should show proper separation

Common issues:
- Cut box too large → removes more than intended, may fuse parts or create extra solids  
- Wrong alignment mode (Align.MIN vs Align.CENTER) → positions cut incorrectly
- Z-height spans wrong range → affects spine/riser area when shouldn't"
```
