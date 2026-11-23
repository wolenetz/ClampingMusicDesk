import math
from build123d import *

# =========================================================================
# PART 5A & 5B: Rear C-Yoke Bracket & Eccentric Quick-Release Cam Lever
# Hardware per side: 2x M3x10mm (side anchors) + 1x M3x20mm & 1x M3 nut (cam axle)
# =========================================================================

# Parameters (matching FeatureScript defaults in mm)
m3Clear = 3.4
m3Nut = 5.6

cx = 835.0
cy = 0.0
yokeH = 14.0

# Updated for central wide dovetail engagement:
# Inner span should be upper desk's guideCentralHalfW + some clearance
inHalfW = 40.5   # 81mm inner span - just wider than central dovetail neck (~80mm)
outHalfW = 46.0  # 92mm outer arms for structural rigidity when wedging cam lock tight

# The following should allow the cam to fit snugly in the slot and be fastened with m3x30mm and inset nut:
earOutX = 15.0   # 30mm face-to-face. nut will be inset
camThick = 19.0  # Should be no larger than upper desk's grooveHalfWidth (26.95mm) minus some tolerance
slotHalfX = camThick / 2 + 0.15 # Cam lever slot in which the cam should fit snugly

holeZ = 8.5

# =========================================================================
# PART 5A: Rear C-Yoke Bracket
# =========================================================================
with BuildPart() as part5aBuilder:
    # 1. Extrude left and right C-yoke halves
    with BuildSketch(Plane(origin=(cx, cy, 0), x_dir=(1, 0, 0), z_dir=(0, 0, 1))) as yokeSk:
        leftHalfPts = [
            (-outHalfW, 4.0),
            (-inHalfW, 4.0),
            (-inHalfW, 14.2),
            (-slotHalfX, 14.2),
            (-slotHalfX, 25.0),
            (-earOutX, 25.0),
            (-earOutX, 17.3),
            (-outHalfW, 17.3),
        ]
        Polygon(*leftHalfPts, align=None)
    extrude(amount=yokeH)
    mirror(about=Plane.YZ.offset(cx))

    # 2. Union the 3.0mm base bridge (X: cx - earOutX to cx + earOutX, Y: cy + 14.2 to cy + 25.0, Z: 0 to 3.0)
    with Locations((cx - earOutX, cy + 14.2, 0.0)):
        Box(
            2 * earOutX,
            25.0 - 14.2,
            3.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 3. Cut the 2x M3x10mm side trunnion holes at Y = 9.5mm, Z = 8.5mm
    with Locations(Plane(origin=(cx, cy + 9.5, holeZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=m3Clear / 2,
            height=outHalfW * 2 + 1.0,
            align=(Align.CENTER, Align.CENTER, Align.CENTER),
            mode=Mode.SUBTRACT,
        )

    # 4. Cut the 1x M3x30mm rear cam axle hole at Y = 20.0mm, Z = 8.5mm
    with Locations(Plane(origin=(cx, cy + 20.0, holeZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=m3Clear / 2,
            height=earOutX * 2,
            align=(Align.CENTER, Align.CENTER, Align.CENTER),
            mode=Mode.SUBTRACT,
        )

    # 5. Cut captured M3 Hex Nut Pocket on Right Rear Ear (X = +6.5mm to +10.0mm)
    nutCircumradius = m3Nut / (2 * math.cos(math.radians(30)))
    nutPlane = Plane(origin=(cx + earOutX - 1.5, cy + 20.0, holeZ), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(nutPlane) as nutSk:
        RegularPolygon(
            radius=nutCircumradius,
            side_count=6,
            major_radius=True,
            rotation=90.0,
            align=None,
        )
    extrude(nutSk.sketch, amount=5.5, mode=Mode.SUBTRACT) # extrude increased to ease assembling

part5a = part5aBuilder.part
part5a.label = "Part 5A - Rear Cam-Lock Yoke"


# =========================================================================
# PART 5B: Eccentric Quick-Release Cam Lever
# =========================================================================
lcx = cx + outHalfW + 45.0
lcy = cy + 15.0

camPts = [
    (-4.00, 0.00),   # c1: Unlocked face resting against runner (R = 4.00mm -> 2.0mm gap)
    (-5.80, -2.00),  # c2: Smooth eccentric lead-in ramp
    (-6.80, -0.75),  # c3: Lower wedging flat (6.80mm reach)
    (-7.55, 0.00),   # c4: Tooth #1 tip (7.55mm reach)
    (-6.80, 0.75),   # c5: Inter-tooth flat
    (-6.80, 1.75),   # c6: Tooth #2 base
    (-7.55, 2.50),   # c7: Tooth #2 tip (matches 2.5mm groove pitch)
    (-6.80, 3.25),   # c8: Upper wedging flat
    (-5.00, 4.80),   # c9: Top shoulder
    (9.50, 4.80),    # c10: Steps out past Part 5A's rear bridge edge
    (9.50, -26.0),   # c11: Outer back edge of thumb handle
    (5.50, -26.0),   # c12: Inner front edge of thumb handle
    (5.50, -4.50),   # c13: Clears Part 5A's bridge corner
    (0.00, -4.00),   # c14: Bottom sweep radius
]

with BuildPart() as part5bBuilder:
    # 1. Extrude cam lever profile (Z = 0 to 9.8mm)
    with BuildSketch(Plane(origin=(lcx, lcy, 0), x_dir=(1, 0, 0), z_dir=(0, 0, 1))) as camSk:
        Polygon(*camPts, align=None)
    extrude(amount=camThick)

    # 2. Cut M3 axle pivot hole through cam center (lcx, lcy)
    with Locations((lcx, lcy, -1.0)):
        Cylinder(
            radius=m3Clear / 2,
            height=camThick + 2.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

part5b = part5bBuilder.part
part5b.label = "Part 5B - Eccentric Cam Lever"


# =========================================================================
# Assembly & STL/STEP Export
# =========================================================================
camLockAssembly = Compound(
    label="OnStage Cam Lock (Part 5A & 5B)",
    children=[part5a, part5b],
)

if __name__ == "__main__":
    export_step(camLockAssembly, "part5a_5b_camLock.step")
    export_stl(part5a, "part5a_rearCYoke.stl")
    export_stl(part5b, "part5b_eccentricCamLever.stl")
