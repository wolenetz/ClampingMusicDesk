import math
from build123d import *

# =========================================================================
# PART 3A: Lower Stationary Music Desk (Unified Side Profile & Tapered Spine)
# =========================================================================

# Parameters (matching FeatureScript defaults in mm / degrees)
tol = 0.3
deskAngle = 15.0
deskAngleRad = math.radians(deskAngle)
m3Clear = 3.4

slideTol = tol + 0.15

cx = 230.0
cy = 0.0

deskW = 240.0
halfW = deskW / 2.0               # 120.0mm
lowerH = 180.0                    # Height of Part 3A lower plate
plateT = 6.0                      # 6.0mm rigid main backplate
shelfT = 8.0                      # 8.0mm bottom shelf
shelfD = 48.0                     # 48.0mm inside book-resting depth
lipT = 5.0                        # 5.0mm front retention lip thickness
lipH = 8.0                        # 8.0mm upward front retention lip

yFrontEdge = cy - plateT - shelfD - lipT  # -59.0mm
railXOffsets = [-75.0, 75.0]

# 2 Parameters: Continuous Profiled Book Shelf + Front Lip + 4mm Inside Root Chamfer
lipTopZ = shelfT + lipH           # 16.0mm
rLip = 2.4                        # Smooth 2.4mm front-top bullnose radius on the 5mm lip
arcOff = rLip * (1.0 - math.cos(math.radians(45.0)))

s1 = (-plateT, 0.0)
s2 = (yFrontEdge, 0.0)
s3 = (yFrontEdge, lipTopZ - rLip)
sM = (yFrontEdge + arcOff, lipTopZ - arcOff)
s4 = (yFrontEdge + rLip, lipTopZ)
s5 = (yFrontEdge + lipT, lipTopZ)
s6 = (yFrontEdge + lipT, shelfT)
s7 = (-plateT - 4.0, shelfT)
s8 = (-plateT, shelfT + 4.0)

# 6 & 7 Parameters: Guide Housings & Center Angled Riser Receiver Block
guideHalfW = 16.0
guideDepth = 14.0

recHalfW = 32.0
recHeight = 95.0
baseDepth = 26.0 + (recHeight * math.sin(deskAngleRad))

wedgePts = [
    (0.0, 0.0),                                                                 # w1
    (baseDepth, 0.0),                                                           # w2
    (baseDepth - (recHeight * math.sin(deskAngleRad)), recHeight * math.cos(deskAngleRad)),  # w3
    (0.0, recHeight * math.cos(deskAngleRad)),                                  # w4
]

# 8 Parameters: Angled Riser Female Dovetail & Dual Vertical Slider Female Dovetails
uSlide = Vector(0.0, -math.sin(deskAngleRad), math.cos(deskAngleRad))
cornerOrigin = Vector(cx, baseDepth, 0.0) - (uSlide * 8.0)

dOuter = 2.0
dInner = -18.0 - tol
halfNeck = 13.0 + tol
halfWide = 19.0 + tol

doveCutPts = [
    (-halfNeck, dOuter),
    (halfNeck, dOuter),
    (halfWide, dInner),
    (-halfWide, dInner),
]

sYOut = guideDepth + 1.0
sYIn = 5.0 - slideTol
sHNeck = 6.0 + slideTol
sHWide = 9.0 + slideTol

with BuildPart() as part3aBuilder:
    # 1. Lower Vertical Backing Plate (Y = -6mm to 0mm, Z = 0 to 180mm)
    with Locations((cx - halfW, cy - plateT, 0.0)):
        Box(
            deskW,
            plateT,
            lowerH,
            align=(Align.MIN, Align.MIN, Align.MIN),
        )

    # 2. Continuous Profiled Book Shelf + Front Lip + 4mm Inside Root Chamfer
    shelfPlane = Plane(origin=(cx - halfW, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(shelfPlane) as shelfSketch:
        with BuildLine():
            Line(s1, s2)
            Line(s2, s3)
            ThreePointArc(s3, sM, s4)
            Line(s4, s5)
            Line(s5, s6)
            Line(s6, s7)
            Line(s7, s8)
            Line(s8, s1)
        make_face()
    extrude(shelfSketch.sketch, amount=deskW, mode=Mode.ADD)

    # 3. Full-Width Lower Rear Torsion Spine (Y = 0 to +6mm, Z = 0 to 50mm)
    with Locations((cx - halfW, cy, 0.0)):
        Box(
            deskW,
            6.0,
            50.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 4 & 5. Round the Outer Vertical Edges of the Unified Shell!
    fillet(part3aBuilder.edges().filter_by(Axis.Z), radius=4.5)

    # 6. Two Vertical Female Dovetail Guide Housings (X = +/-75mm, Z = 50mm to 180mm)
    for rx in railXOffsets:
        with Locations((cx + rx - guideHalfW, cy, 50.0)):
            Box(
                2.0 * guideHalfW,
                guideDepth,
                lowerH - 50.0,
                align=(Align.MIN, Align.MIN, Align.MIN),
                mode=Mode.ADD,
            )

    # 7. Center Angled Heavy-Duty Riser Receiver Block (64mm wide x 95mm tall housing)
    wedgePlane = Plane(origin=(cx - recHalfW, cy, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(wedgePlane) as wedgeSketch:
        Polygon(*wedgePts, align=None)
    extrude(wedgeSketch.sketch, amount=recHalfW * 2.0, mode=Mode.ADD)

    # 8A. Cut Center Angled Riser Female Dovetail
    doveCutPlane = Plane(origin=cornerOrigin, x_dir=(1, 0, 0), z_dir=uSlide)
    with BuildSketch(doveCutPlane) as doveCutSketch:
        Polygon(*doveCutPts, align=None)
    extrude(doveCutSketch.sketch, amount=79.0, mode=Mode.SUBTRACT)

    # 8B. Cut Dual Vertical Slider Female Dovetails (Z = 50mm to 182mm)
    femSlidePlane = Plane(origin=(cx, cy, 50.0), x_dir=(1, 0, 0), z_dir=(0, 0, 1))
    with BuildSketch(femSlidePlane) as femSlideSk:
        for rx in railXOffsets:
            femPts = [
                (rx - sHNeck, sYOut),
                (rx + sHNeck, sYOut),
                (rx + sHWide, sYIn),
                (rx - sHWide, sYIn),
            ]
            Polygon(*femPts, align=None)
    extrude(femSlideSk.sketch, amount=132.0, mode=Mode.SUBTRACT)

    # 8C. Cut Side Detent Pin Holes (Y = cy + 9.5mm, Z = 160mm)
    for rx in railXOffsets:
        with Locations(Plane(origin=(cx + rx - guideHalfW - 2.0, cy + 9.5, 160.0), z_dir=(1, 0, 0))):
            Cylinder(
                radius=m3Clear / 2.0,
                height=2.0 * (guideHalfW + 2.0),
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.SUBTRACT,
            )

part3a = part3aBuilder.part
part3a.label = "Part 3A-Single - Single Riser Lower Music Desk"

if __name__ == "__main__":
    export_step(part3a, "part3a_lowerStationaryMusicDesk.step")
    export_stl(part3a, "part3a_lowerStationaryMusicDesk.stl")
