# Clamping Music Desk for OnStage Keyboard Stand
**OrcaSlicer & Prusa CORE One Print Guide**

---

## 1. Global OrcaSlicer Settings
**Printer:** Prusa CORE One (250 x 220 x 270 mm)

**Materials:** 
  - **PETG:** Part 1A OnStage L Clamp Top Saddle Cap and Spine (recommended), Part 4 springs
  - **PLA+ / Tough PLA:** All other parts
    - **If you plan to use this in a warm/hot environment, don't use PLA, which will not perform well in those conditions and structure might fail easily.**

**Layer Height:**
  - 0.20 mm (or 0.30 mm for speedier prints) (0.4 mm nozzle)

**Perimeters (Walls):**
  - **8 walls** (on upper and lower desk parts, 3-4 walls prints faster and doesn't really impact result)

**Top / Bottom Shells:**
  - 8 Top / 8 Bottom, though on upper and lower desk parts, 1-2 bottom and 3-4 top prints faster and doesn't really impact results

**Infill:**
  - 40% Cubic
  - Rotate the infill to match part rotation, if any.

**Brim:**
  - 25 mm outer brim for tall/long parts (1A, 1B, 2A, 2B, 3A/3A-Dual, 3B). Set brim-part distance to 0.0. This helps prevent PLA detaching from bed on long narrow parts.

---

## 2. Part Orientation & Support

**Part 1A (OnStage L Clamp Top Saddle Cap and Spine):** 
  - Face: Flat top face (`Z = +23.25 mm`) flipped 180° upside-down
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only*)

**Part 1B (OnStage L Clamp Lower Cradle and Arm):** 
  - Face: Flat bottom face (`Z = -35 mm`)
  - Rotation: **90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — bridges the side-entry nut slots cleanly)

**Part 2A (Lower Riser Stem Modular Extension):** 
  - Face: Back face (`Y = +15 mm`) lying flat on the bed for maximum longitudinal layer strength
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — supports the 5 mm step under the 20×20 mm top peg and the internal socket roof)

**Part 2B (Upper Dovetail Riser Head):** 
  - Face: Back face (`Y = +15 mm`, where the column back and dovetail back are coplanar and flush on the bed)
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — for the bottom socket cutout)

**Part 3A (Single Riser Lower Music Desk) OR Part 3A-Dual (Dual Riser Lower Music Desk):** 
  - Face: Flat bottom face (`Z = 0`)
  - Rotation: **140°** (Angled for more room for lots of brim)
  - Supports: Yes (Tree/Organic, *On build plate only*)

**Part 3B (Upper Sliding Music Desk):** 
  - Face: Flat top edge flipped 180° upside-down (`Z = 190 mm`) with 0.0 mm brim-object gap, mouse ears on extreme left/right ends, and aux fan disabled for the first 10 mm to prevent edge curling.
  - Rotation: **140°** (Angled for more room for lots of brim)
  - Supports: Yes (Tree/Organic, *On build plate only*)

**Part 4A (Thin Sheet Swan Neck Leaf Spring - Page Clamp):**
  - Flat side face; **No supports**.

**Part 4B1 (Medium Book Flip Up Circular Knuckle Hinge - Page Clamp):**
  - Flat side face; Supports *on build plate only*.

**Part 4B2 (Medium Book Circular Knuckle Clevis Batwing Finger - Page Clamp):**
  - Flat top surface flipped 180° upside-down (`Z = 198 mm`); **No supports**.

**Part 4C (Thick Book Deep Reach S Spring - Page Clamp):**
  - Flat side face; **No supports**.

**Part 5A (Rear Cam-Lock Yoke):**
  - Flat bottom face.

**Part 5B (Eccentric Cam Lever):**
  - Flat side face

---

## 3. Hardware & Assembly

### Configuration Options
**Standard Single-Riser Setup:** Print **1×** Part 1A OnStage L Clamp Top Saddle Cap and Spine, **1×** Part 1B OnStage L Clamp Lower Cradle and Arm, **1×** Part 2A Lower Riser Stem Modular Extension, **1×** Part 2B Upper Dovetail Riser Head, **1×** Part 3A-Single Single Riser Lower Music Desk, **1×** Part 3B Upper Sliding Music Desk, page clamps, **2×** Part 5A Rear Cam-Lock Yoke, and **2×** Part 5B Eccentric Cam Lever.

**Heavy-Duty Dual-Riser Setup:** Print **2×** Part 1A OnStage L Clamp Top Saddle Cap and Spine, **2×** Part 1B OnStage L Clamp Lower Cradle and Arm, **2×** Part 2A Lower Riser Stem Modular Extension, **2×** Part 2B Upper Dovetail Riser Head, **1×** Part 3A-Dual Dual Riser Lower Music Desk, **1×** Part 3B Upper Sliding Music Desk, page clamps, **4×** Part 5A Rear Cam-Lock Yoke, and **4×** Part 5B Eccentric Cam Lever.

**Height Flexibility:** 
  - *Full Height (223 mm column):* Stack **Part 1B $\rightarrow$ Part 2A $\rightarrow$ Part 2B**.
  - *Low-Profile (108 mm column):* Omit Part 2A and mount **Part 2B directly onto Part 1B's peg**.
  - *Extra-Tall (338 mm column):* Stack **two Part 2A extensions** beneath Part 2B.

### Fasteners & Mounting
**Clamps & Dorsal Tension Spine (Per Part 1A/1B Assembly):** 
  - Secured with **5× M3 × 30 mm** socket head cap screws and **5× M3 hex nuts** (10× screws and 10× nuts total for the Dual-Riser setup).
  - Slide the 5× M3 nuts sideways into the horizontal side-entry slots on Part 1B.
  - Tighten the **4× corner clamp bolts** around the stand's horizontal beam first, then tighten the **5th rear dorsal spine bolt** (20 mm in front of the riser seat) to pre-tension the upper spine and lock out cantilever sag.

**Modular Riser Locks (Per Riser Column):** 
  - *Standard Full Height (1B + 2A + 2B):* Secured with **2× M4 × 30 mm** socket head cap screws and **2× M4 hex nuts** along the Y-axis—one locking Part 2A to Part 1B's peg, and a second locking Part 2B to Part 2A's peg (4× screws and 4× nuts total for the Dual-Riser setup).
  - *Low-Profile (1B + 2B only):* Uses **1× M4 × 30 mm** screw and **1× M4 hex nut** per column.

**Dual-Riser Stand Spacing (Part 3A-Dual Only):** 
  - Mount the two Part 1A/1B clamp assemblies onto the stand's horizontal bar spaced **150 mm (approx. 5.9 in) apart center-to-center** (leaving a **60 mm gap** between the inner side walls of the two 90 mm-wide clamp blocks) so both Part 2B dovetails slide cleanly into the left and right receivers on Part 3A-Dual.

**Upper Desk Quick-Release Cam Lock (Per Riser / 2 Sets per Desk):**
  - **Part 5A (C-Yoke):** Slip over the back of Part 3A's guide block at `Z = 160 mm` and anchor using **2× M3 × 10 mm screws** threaded into the side holes (stopping flush in the wall without obstructing the internal dovetail channel).
  - **Part 5B (Cam Lever):** Insert into the C-Yoke center slot with the V-tooth facing the runner. Drop an **M3 hex nut** into the right ear pocket, and secure with **1× M3 × 20 mm screw** acting as the pivot axle.
  - **Operation:** Flip the lever up ($90^\circ$) to slide Part 3B freely; flip the lever down to lock the cam's V-tooth into Part 3B's rear micro-grooves and wedge the dovetail securely with zero wall-bending stress.

**Batwing (Part 4B1/4B2):** Assembled with **1× M3 screw** and **1× M3 hex nut**.

**Break-In:** Slide all dovetails back and forth 5–10 times before final mounting.