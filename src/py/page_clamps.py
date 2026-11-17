import math
from build123d import *

# =========================================================================
# PART 4: Modular Page Clamps (3 Variants for the Upper Back Dovetail Rail)
# =========================================================================

# Parameters (matching FeatureScript defaults in mm)
tol = 0.3
m3Clear = 3.4
m3Nut = 5.6

nutCircumradius = m3Nut / (2.0 * math.cos(math.radians(30.0)))


def makeRailCutout(xStart: float, width: float, tolVal: float) -> Part:
    """Helper function to create the Upper Dovetail Rail cutout solid."""
    # Matches Part 3's extUpperRail (Y = 0 to 14mm, Z = 140 to 168mm) expanded by tolVal
    railPts = [
        (-1.0, 144.0 - tolVal),          # u1
        (6.0 - tolVal, 144.0 - tolVal),  # u2
        (6.0 - tolVal, 140.0 - tolVal),  # u3
        (14.0 + tolVal, 144.0 - tolVal), # u4
        (14.0 + tolVal, 164.0 + tolVal), # u5
        (6.0 - tolVal, 168.0 + tolVal),  # u6
        (6.0 - tolVal, 164.0 + tolVal),  # u7
        (-1.0, 164.0 + tolVal),          # u8
    ]

    with BuildPart() as railCutBuilder:
        # Sketch plane: origin=(xStart - 1, 0, 0), normal=(1, 0, 0), x_dir=(0, 1, 0) -> local Y is (0, 0, 1)
        rPlane = Plane(origin=(xStart - 1.0, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
        with BuildSketch(rPlane) as rSketch:
            Polygon(*railPts, align=None)
        extrude(rSketch.sketch, amount=width + 2.0)

    return railCutBuilder.part


# =====================================================================
# VARIANT A: Thin Sheet "Swan-Neck" Leaf Spring (X = 660mm, Width = 16mm)
# =====================================================================
xA = 660.0
wA = 16.0

aPts = [
    (0.0, 135.0),           # 1: Carriage bottom-front (flush with Y=0 backplate)
    (20.0, 135.0),          # 2: Carriage bottom-back
    (20.0, 189.0),          # 3: Carriage top-back
    (-12.0, 189.0),         # 4: Arch forward over the 6mm desk plate
    (-5.5, 148.0),          # 5: Leaf spring pre-load contact point (presses against Y=-6mm face)
    (-14.0, 136.0),         # 6: Ski-tip flare bottom-front
    (-12.0, 135.0),         # 7: Ski-tip bottom edge
    (-3.5, 148.0),          # 8: Inside of contact knee (2mm spring thickness)
    (-9.5, 185.0),          # 9: Inside of top arch
    (-6.0 - tol, 185.0),    # 10: Clears front of desk top edge (Y=-6mm)
    (-6.0 - tol, 180.0 + tol),  # 11: Rests on top of desk (Z=180mm)
    (0.0, 180.0 + tol),     # 12: Back of desk top edge (Y=0mm)
]

with BuildPart() as part4aBuilder:
    planeA = Plane(origin=(xA, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(planeA) as skA:
        Polygon(*aPts, align=None)
    extrude(skA.sketch, amount=wA)

    # Subtract Upper Dovetail Rail cutout
    add(makeRailCutout(xA, wA, tol), mode=Mode.SUBTRACT)

part4a = part4aBuilder.part
part4a.label = "Part 4A-Thin Sheet Swan Neck Leaf Spring - Page Clamp"


# =====================================================================
# VARIANT B1: Medium Book Flip-UP Circular Knuckle Hinge Base (X = 695mm)
# =====================================================================
xB1 = 695.0
wB1 = 24.0
pivY = 7.0
pivZ = 191.0

bPts = [
    (0.0, 135.0),
    (20.0, 135.0),
    (20.0, 183.0),
    (-6.0, 183.0),
    (-6.0, 180.0 + tol),
    (0.0, 180.0 + tol),
]

with BuildPart() as part4b1Builder:
    # 1. Base Rail Carriage (Z = 135mm to 183mm)
    planeB1 = Plane(origin=(xB1, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(planeB1) as skB1:
        Polygon(*bPts, align=None)
    extrude(skB1.sketch, amount=wB1)

    # 2. Circular Center Hinge Turret on Top of B1 (14mm wide, 7mm radius drum + lower pedestal)
    with Locations((xB1 + 5.0, 0.0, 182.0)):
        Box(
            14.0,
            14.0,
            pivZ - 182.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    with Locations(Plane(origin=(xB1 + 5.0, pivY, pivZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=7.0,
            height=14.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.ADD,
        )

    # 3. Cut Upper Dovetail Rail & Horizontal M3 Hinge Hole
    add(makeRailCutout(xB1, wB1, tol), mode=Mode.SUBTRACT)

    with Locations(Plane(origin=(xB1, pivY, pivZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=m3Clear / 2.0,
            height=wB1,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

part4b1 = part4b1Builder.part
part4b1.label = "Part 4B1-Medium Book Flip Up Circular Knuckle Hinge - Page Clamp"


# =====================================================================
# VARIANT B2: Circular-Knuckle Clevis Batwing Finger (Offset to X = 735mm)
# =====================================================================
xB2 = 735.0
clevisGap = 14.0 + (2.0 * tol)  # 14.6mm inner gap
earW = 4.5                      # 4.5mm thick left and right hinge ears
halfGap = clevisGap / 2.0       # 7.3mm
earRad = 6.5                    # 6.5mm radius

with BuildPart() as part4b2Builder:
    # 1. Left & Right Circular Hinge Drums (Coaxial with pivY = 7mm, pivZ = 191mm)
    with Locations(Plane(origin=(xB2 - halfGap - earW, pivY, pivZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=earRad,
            height=earW,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.ADD,
        )

    with Locations(Plane(origin=(xB2 + halfGap, pivY, pivZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=earRad,
            height=earW,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.ADD,
        )

    # 2. Tangent Upper Ear Webs (Y = -1mm to pivY, Z = pivZ to 198mm)
    with Locations((xB2 - halfGap - earW, -1.0, pivZ)):
        Box(
            earW,
            pivY - (-1.0),
            198.0 - pivZ,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    with Locations((xB2 + halfGap, -1.0, pivZ)):
        Box(
            earW,
            pivY - (-1.0),
            198.0 - pivZ,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 3. Forward Reach Bridge (X = xB2 - halfGap - earW to xB2 + halfGap + earW, Y = -30 to -1, Z = 192 to 198)
    with Locations((xB2 - halfGap - earW, -30.0, 192.0)):
        Box(
            2.0 * (halfGap + earW),
            29.0,
            6.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 4. Downward Page-Holding Batwing Paddle (X = xB2 - 25 to xB2 + 25, Y = -30 to -25, Z = 162 to 198)
    with Locations((xB2 - 25.0, -30.0, 162.0)):
        Box(
            50.0,
            5.0,
            36.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 5. Horizontal M3 Hinge Hole + Captured Hex Nut Trap in Right Ear
    with Locations(Plane(origin=(xB2 - 15.0, pivY, pivZ), z_dir=(1, 0, 0))):
        Cylinder(
            radius=m3Clear / 2.0,
            height=30.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # In FeatureScript: plane normal=(1,0,0), x_dir=(0,1,0) -> local Y is (0,0,1).
    # First vertex at (0, R) aligns vertices along global Z; extrude direction=(-1,0,0) cuts 2.5mm inward (-X).
    b2NutPlane = Plane(origin=(xB2 + halfGap + earW, pivY, pivZ), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(b2NutPlane) as b2NutSk:
        RegularPolygon(
            radius=nutCircumradius,
            side_count=6,
            major_radius=True,
            rotation=90.0,
            align=None,
        )
    extrude(b2NutSk.sketch, amount=-2.5, mode=Mode.SUBTRACT)

part4b2 = part4b2Builder.part
part4b2.label = "Part 4B2-Medium Book Circular Knuckle Clevis Batwing Finger - Page Clamp"


# =====================================================================
# VARIANT C: Thick & Short/Tall Book Deep-Reach S-Spring (X = 775mm)
# =====================================================================
xC = 775.0
wC = 22.0

cPts = [
    (0.0, 135.0),           # 1: Carriage bottom-front
    (20.0, 135.0),          # 2: Carriage bottom-back
    (20.0, 212.0),          # 3: High-arch spring back (rises 32mm above desk)
    (-50.0, 212.0),         # 4: Deep 44mm throat reach forward over thick spines
    (-44.0, 125.0),         # 5: Plunges down to Z=125mm
    (-53.0, 115.0),         # 6: Forward easy-entry ski-tip flare
    (-50.2, 114.0),         # 7: Ski-tip bottom edge
    (-41.0, 125.0),         # 8: Inside of lower knee (3mm thick spring wall)
    (-46.8, 208.8),         # 9: Inside of front top corner (3.2mm spring wall)
    (16.8, 208.8),          # 10: Inside of rear top corner
    (16.8, 184.0),          # 11: Flex relief step behind carriage
    (-6.0 - tol, 184.0),    # 12: Clears front of desk top edge
    (-6.0 - tol, 180.0 + tol),  # 13: Rests on top of desk
    (0.0, 180.0 + tol),     # 14: Back of desk top edge
]

with BuildPart() as part4cBuilder:
    planeC = Plane(origin=(xC, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(planeC) as skC:
        Polygon(*cPts, align=None)
    extrude(skC.sketch, amount=wC)

    # Subtract Upper Dovetail Rail cutout
    add(makeRailCutout(xC, wC, tol), mode=Mode.SUBTRACT)

part4c = part4cBuilder.part
part4c.label = "Part 4C-Thick Book Deep Reach S Spring - Page Clamp"


# =========================================================================
# Assembly & STL/STEP Export
# =========================================================================
pageClampsAssembly = Compound(
    label="OnStage Page Clamps (Part 4A, 4B1, 4B2, 4C)",
    children=[part4a, part4b1, part4b2, part4c],
)

if __name__ == "__main__":
    export_step(pageClampsAssembly, "part4_pageClamps.step")
    export_stl(part4a, "part4a_swanNeckLeafSpring.stl")
    export_stl(part4b1, "part4b1_knuckleHingeBase.stl")
    export_stl(part4b2, "part4b2_clevisBatwingFinger.stl")
    export_stl(part4c, "part4c_deepReachSSpring.stl")
