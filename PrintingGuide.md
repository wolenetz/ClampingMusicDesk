## Clamping Music Desk for OnStage Keyboard Stand
**OrcaSlicer & Prusa CORE One Print Guide**

### ** WIP on version 2.0 right now -- this documentation has not yet been updated **

### Source location

- v1.0 and some more recent work on vNext are hosted on my github:

    - [v1.0 release](https://github.com/wolenetz/ClampingMusicDesk/tree/release/1.0)

    - [vNext (possibly WIP)](https://github.com/wolenetz/ClampingMusicDesk)

### OrcaSlicer Settings

- **Printer:** Prusa CORE One (250 x 220 x 270 mm)

- **Materials:**

    - **PETG:** Part 1A OnStage L Clamp Top Saddle Cap and Spine (recommended), Part 4 springs, Part 5A cam lock yoke

    - **PLA+ / Tough PLA:** All other parts

        - *If you plan to use this in a warm/hot environment, don't use PLA.*

- **Layer Height:** 0.20 mm (or 0.30 mm for speedier prints) (0.4 mm nozzle)

- **Perimeters (Walls):** 8 walls (on upper and lower desk parts, 3-4 walls prints faster)

- **Top / Bottom Shells:** 8 Top / 8 Bottom (on upper and lower desk parts, 1-2 bottom and 3-4 top prints faster)

- **Infill:** 40% Cubic  

- **Brim:** 25 mm outer brim for tall/long parts (1A, 1B, 2A, 2B, 3A/3A-Dual, 3B). Set brim-part distance to 0.0.

- **Infill Rotation:** Rotate the infill to match part rotation when applicable for improved strength.

### Part Orientation & Support

| Part | Face | Rotation | Supports |
|------|------|----------|----------|
| Part 1A | Flat top face | 0° or multiple of 90° | Yes (*On build plate only*) |
| Part 1B | Flat bottom face | 90° | Yes (*On build plate only*) |
| Part 2A | Back face | 0° or multiple of 90° | Yes (*On build plate only*) |
| Part 2B | Back face | 0° or multiple of 90° | Yes (*On build plate only*) |
| Part 3A | Flat bottom face | 140° | Yes (Tree/Organic, *On build plate only*) |
| Part 3B | Flat top edge | 140° | Yes (Tree/Organic, *On build plate only*) |
| Part 4A | Flat side face | Any | Optional |
| Part 4B1 | Flat side face | Any | Yes (*On build plate only*) |
| Part 4B2 | Flat top surface | Any | Optional |
| Part 4C | Flat side face | Any | Optional |
| Part 5A | Flat bottom face | Any | Optional |
| Part 5B | Flat side face | Any | Optional |

### Hardware & Assembly

#### Configuration Options

- **Standard Single-Riser Setup:** Print 1× Part 1A, 1× Part 1B, 1× Part 2A, 1× Part 2B, 1× Part 3A-Single, 1× Part 3B, page clamps, 2× Part 5A, 2× Part 5B.

- **Heavy-Duty Dual-Riser Setup:** Print 2× Part 1A, 2× Part 1B, 2× Part 2A, 2× Part 2B, 1× Part 3A-Dual, 1× Part 3B, page clamps, 2× Part 5A, 2× Part 5B.

- **Low-Profile:** Omit Part 2A.

- **Tall-Profile:** Print more Part 2A.

#### Fasteners & Mounting

- **Clamps & Dorsal Tension Spine:** Secured with 5× M3 × 30 mm screws and 5× M3 hex nuts (10× screws and 10× nuts total for Dual-Riser).

- **Modular Riser Locks:**

    - *Standard Full Height:* Secured with 2× M4 × 30 mm screws and 2× M4 hex nuts

    - *Low-Profile:* Uses 1× M4 × 30 mm screw and 1× M4 hex nut per column

- **Dual-Riser Stand Spacing:** Mount the two Part 1A/1B clamp assemblies spaced **150 mm (approx. 5.9 in) apart center-to-center**.

- **Upper Desk Quick-Release Cam Lock:**

    - **Part 5A (C-Yoke):** Slip over the back of Part 3A's guide block and carefully anchor with **2× M4 (or M3) × 10 mm screws**

    - **Part 5B (Cam Lever):** Insert into C-Yoke center slot, drop an **M3 hex nut** into the right ear pocket (pliers may help snap it into the pocket), and secure with **1× M3 × 20 mm screw**

- **Batwing (Part 4B1/4B2):** Assembled with **1× M3 screw** (30 mm works great) and **1× M3 hex nut**

- **Break-In:** Slide all dovetails back and forth 5–10 times before final mounting.
