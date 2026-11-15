# OnStage Sheet Music Stand for Roland Fantom-G8
**OrcaSlicer & Prusa CORE One Print Guide**

---

## 1. Global OrcaSlicer Settings
- **Printer:** Prusa CORE One (250 x 220 x 270 mm)
- **Materials:** 
  - **PETG:** Part 1A OnStage L Clamp Top Saddle Cap and Spine (recommended), Part 4 springs, Part 5 detent pins
  - **PLA+ / Tough PLA:** Part 1B OnStage L Clamp Lower Cradle and Arm, Parts 2A/2B modular riser stem, Parts 3A/3A-Dual/3B desks, Parts 4B1/4B2 batwing assembly
    - **If you plan to use this in a warm/hot environment, don't use PLA, which will not perform well in those conditions and structure might fail easily.**
- **Layer Height:** 0.20 mm (0.4 mm nozzle)
- **Perimeters (Walls):** **8 walls**
- **Top / Bottom Shells:** 8 Top / 8 Bottom
- **Infill:** 40% Gyroid or Cubic
- **Brim:** 5 mm outer brim for tall/long parts (1A, 1B, 2A, 2B, 3A/3A-Dual, 3B)
- **Draft prints might work, 0.3mm all PLA with similar settings otherwise.** Really just the springs might benefit the most from printing with PETG.
    - UDPATE: I printed:
      - Part 1A OnStage L Clamp Top Saddle Cap and Spine: 0.3mm layer, Transparent Red PETG 8 walls, 8 top, 8 bottom, 40% Cubic, auto brim, upside-down, rotated on Z to parallel the front of print bed. Post-processed using plyers and burr removal tool.
      - Part 1B OnStage L Clamp Lower Cradle and Arm, Parts 2A/2B modular riser stem: 0.3mm layer, Overture black Easy PLA 8 walls, 8 top, 8 bottom, 40% cubic, auto brim, risers laid flat, lower riser and lower clamp rotated on Z to parallel the front of print bed, upper riser rotated on Z perpendicular to the front of print bed. Post-processed using plyers, burr removal tool, and on male joint parts I sanded with fine-grit until able to join securely without too much force. Lower clamp portion started peeling from bed on clamp side but didn't severely impact function or aesthetic. Assembled to rigid and surprisingly heavy structure. 
      - Part 3A-Single Single Riser Lower Music Desk: 0.3mm layer, Eryone Silk blue/green PLA, 4 walls, 5 top, 1 bottom, 15% cubic, rotated on Z to 40 degrees. Used Orcaslicer Prusa Generic Silk PLA preset (slower extrusion, 0.6mm Z-hop). It peeled from both sides of long dimension, making the underside slightly curved, but the middle stayed attached.
      - Part 4 springs and Part 5 detent pins: 0.2mm layer, Transparent Red PETG 8 walls, 8 top, 8 bottom, 40% cubic, auto brim, all flat against bed on largest sides.
      - Part 4B1/4B2 batwing assembly TBD
      - Part 3B Upper Sliding Music Desk TBD
      - Note: Result might be better for angled parts if I had rotated the infill too.

---

## 2. Part Orientation & Support

- **Part 1A (OnStage L Clamp Top Saddle Cap and Spine):** 
  - Face: Flat top face (`Z = +23.25 mm`) flipped 180° upside-down
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only*)
- **Part 1B (OnStage L Clamp Lower Cradle and Arm):** 
  - Face: Flat bottom face (`Z = -35 mm`)
  - Rotation: **90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — bridges the side-entry M4 nut slots cleanly)
- **Part 2A (Lower Riser Stem Modular Extension):** 
  - Face: Back face (`Y = +15 mm`) lying flat on the bed for maximum longitudinal layer strength
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — supports the 5 mm step under the 20×20 mm top peg and the internal socket roof)
- **Part 2B (Upper Dovetail Riser Head):** 
  - Face: Back face (`Y = +15 mm`, where the column back and dovetail back are coplanar and flush on the bed)
  - Rotation: **0° or multiple of 90°** (Fits straight, parallel to bed edges)
  - Supports: Yes (*On build plate only* — for the bottom socket cutout)
- **Part 3A (Single Riser Lower Music Desk) OR Part 3A-Dual (Dual Riser Lower Music Desk):** 
  - Face: Flat bottom face (`Z = 0`)
  - Rotation: **0° or 180°** (Fits straight, parallel to bed edges)
  - Supports: Yes (Tree/Organic, *On build plate only*)
- **Part 3B (Upper Sliding Music Desk):** 
  - Face: Flat top edge flipped 180° upside-down (`Z = 190 mm`)
  - Rotation: **0° or 180°** (Fits straight, parallel to bed edges)
  - Supports: Yes (Tree/Organic, *On build plate only*)
- **Part 4A (Thin Sheet Swan Neck Leaf Spring - Page Clamp):** Flat side face; **No supports**.
- **Part 4B1 (Medium Book Flip Up Circular Knuckle Hinge - Page Clamp):** Flat side face; Supports *on build plate only*.
- **Part 4B2 (Medium Book Circular Knuckle Clevis Batwing Finger - Page Clamp):** Flat top surface flipped 180° upside-down (`Z = 198 mm`); **No supports**.
- **Part 4C (Thick Book Deep Reach S Spring - Page Clamp):** Flat side face; **No supports**.
- **Part 5 (Detent Pin):** Flat bottom face (`Z = -1.2 mm`); **No supports**.

---

## 3. Hardware & Assembly

### Configuration Options
- **Standard Single-Riser Setup:** Print **1×** Part 1A OnStage L Clamp Top Saddle Cap and Spine, **1×** Part 1B OnStage L Clamp Lower Cradle and Arm, **1×** Part 2A Lower Riser Stem Modular Extension, **1×** Part 2B Upper Dovetail Riser Head, and **1×** Part 3A-Single Single Riser Lower Music Desk (plus 1× Part 3B Upper Sliding Music Desk, page clamps, and 2× detent pins).
- **Heavy-Duty Dual-Riser Setup:** Print **2×** Part 1A OnStage L Clamp Top Saddle Cap and Spine, **2×** Part 1B OnStage L Clamp Lower Cradle and Arm, **2×** Part 2A Lower Riser Stem Modular Extension, **2×** Part 2B Upper Dovetail Riser Head, and **1×** Part 3A-Dual Dual Riser Lower Music Desk (plus 1× Part 3B Upper Sliding Music Desk, page clamps, and 2× detent pins).
- **Height Flexibility:** 
  - *Full Height (223 mm column):* Stack **Part 1B $\rightarrow$ Part 2A $\rightarrow$ Part 2B**.
  - *Low-Profile (108 mm column):* Omit Part 2A and mount **Part 2B directly onto Part 1B's peg**.
  - *Extra-Tall (338 mm column):* Stack **two Part 2A extensions** beneath Part 2B.

### Fasteners & Mounting
- **Clamps & Dorsal Tension Spine (Per Part 1A/1B Assembly):** 
  - Secured with **5× M4 × 30 mm** socket head cap screws and **5× M4 hex nuts** (10× screws and 10× nuts total for the Dual-Riser setup).
    - UDPATE: For these, I found that M3 nuts and bolts fit better (especially the nuts)
  - Slide the 5× M4 nuts sideways into the horizontal side-entry slots on Part 1B.
    - UDPATE: For these, I found that M3 nuts and bolts fit better (especially the nuts)
  - Tighten the **4× corner clamp bolts** around the stand's horizontal beam first, then tighten the **5th rear dorsal spine bolt** (20 mm in front of the riser seat) to pre-tension the upper spine and lock out cantilever sag.
    - UDPATE: For these, I found that M3 nuts and bolts fit better (especially the nuts)
- **Modular Riser Locks (Per Riser Column):** 
  - *Standard Full Height (1B + 2A + 2B):* Secured with **2× M4 × 30 mm** socket head cap screws and **2× M4 hex nuts** along the Y-axis—one locking Part 2A to Part 1B's peg, and a second locking Part 2B to Part 2A's peg (4× screws and 4× nuts total for the Dual-Riser setup).
  - *Low-Profile (1B + 2B only):* Uses **1× M4 × 30 mm** screw and **1× M4 hex nut** per column.
- **Dual-Riser Stand Spacing (Part 3A-Dual Only):** 
  - Mount the two Part 1A/1B clamp assemblies onto the stand's horizontal bar spaced **150 mm (approx. 5.9 in) apart center-to-center** (leaving a **60 mm gap** between the inner side walls of the two 90 mm-wide clamp blocks) so both Part 2B dovetails slide cleanly into the left and right receivers on Part 3A-Dual.
- **Batwing (Part 4B1/4B2):** Assembled with **1× M3 screw** and **1× M3 hex nut**.
- **Break-In:** Slide all dovetails back and forth 5–10 times before final mounting.