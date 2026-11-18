import math
from build123d import *

# =========================================================================
# PART 1: Horizontal-Split L-Clamp (Dorsal Bolt Raised +4mm for Max Part 1A Strength)
# =========================================================================

# Parameters (matching FeatureScript defaults in mm)
tubeSize = 30.5
tubeRad = 3.5
clampGap = 2.0
m4Clear = 4.4
m4Nut = 7.4
m4Head = 7.6
pegSize = 20.0
backReach = 155.0

halfTube = tubeSize / 2.0                        # 15.25mm
topZ = halfTube + 8.0                            # +23.25mm
botZ = -35.0                                     # -35.00mm

# Peg center is at Y = 15.25 + 155 + 12.5 = 182.75mm
pegCenterY = halfTube + backReach + 12.5         # 182.75mm
armExtension = 10.0
armEndY = halfTube + backReach + 25.0 + armExtension  # 205.25mm
seatFrontY = pegCenterY - 15.0                   # 167.75mm (Front edge of 40x30mm riser footprint)
spineBoltY = seatFrontY - 20.0                   # 147.75mm (20mm in front of riser footprint)

nutCircumradius = m4Nut / (2.0 * math.cos(math.radians(30.0)))

def edgeAtPoint(partObj: Part, ptCoords: tuple[float, float, float], tolVal: float = 1e-2) -> Edge:
    """Finds the single edge on partObj passing through ptCoords (replicates qContainsPoint)."""
    targetVertex = Vertex(*ptCoords)
    for edgeItem in partObj.edges():
        if edgeItem.distance_to(targetVertex) < tolVal:
            return edgeItem
    raise ValueError(f"No edge found near point {ptCoords}")

# 1. Solid 90mm Wide Clamp Block (Filleted on Z-parallel edges while pristine)
with BuildPart() as clampBlockBuilder:
    with Locations((-45.0, -30.0, botZ)):
        Box(90.0, 60.0, topZ - botZ, align=(Align.MIN, Align.MIN, Align.MIN))
    fillet(clampBlockBuilder.edges().filter_by(Axis.Z), radius=4.0)
clampBlock = clampBlockBuilder.part


# 2. Profiled Extension Arm (Part 1B) with Swept Curved Bottom Keel
curveStartY = armEndY - 45.0
a1 = (20.0, botZ)
a2 = (curveStartY, botZ)
a3 = (armEndY, 10.0)
a4 = (20.0, 10.0)
aMid = (curveStartY + 31.0, botZ + 13.0)

with BuildPart() as armBuilder:
    armPlane = Plane(origin=(-25.0, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(armPlane) as armProfileSk:
        with BuildLine():
            Line(a1, a2)
            ThreePointArc(a2, aMid, a3)
            Line(a3, a4)
            Line(a4, a1)
        make_face()
    extrude(armProfileSk.sketch, amount=50.0)

    # 2A. Fillet Arm Longitudinal Edges, Curved Keel, and Rear Top Edge (radius = 5mm)
    armTargetPts = [
        (-25.0, 50.0, 10.0),       # topLongEdges left
        (25.0, 50.0, 10.0),        # topLongEdges right
        (-25.0, 50.0, botZ),       # botFlatEdges left
        (25.0, 50.0, botZ),        # botFlatEdges right
        (-25.0, aMid[0], aMid[1]), # keelArcEdges left
        (25.0, aMid[0], aMid[1]),  # keelArcEdges right
        (0.0, armEndY, 10.0),      # rearTopEdge
    ]
    armFilletEdges = [edgeAtPoint(armBuilder.part, pt) for pt in armTargetPts]
    fillet(armFilletEdges, radius=5.0)
armBody = armBuilder.part


# 2B. Central Dorsal Tension Spine on Part 1A (Y = 20mm to seatFrontY, Z = 11mm to topZ)
spineBotZ = 10.0 + (clampGap / 2.0)  # +11.00mm
with BuildPart() as spineBuilder:
    with Locations((-10.0, 20.0, spineBotZ)):
        Box(20.0, seatFrontY - 20.0, topZ - spineBotZ, align=(Align.MIN, Align.MIN, Align.MIN))

    spineTargetPts = [
        (-10.0, 100.0, topZ),
        (10.0, 100.0, topZ),
        (-10.0, seatFrontY, topZ),
        (10.0, seatFrontY, topZ),
    ]
    spineFilletEdges = [edgeAtPoint(spineBuilder.part, pt) for pt in spineTargetPts]
    fillet(spineFilletEdges, radius=10.0)
spineBody = spineBuilder.part


# 3. Union Main Clamp Block, Arm, Dorsal Spine, and Upward Riser Peg, then Cut All Features
cornerCoords = [
    (-35.0, -22.5, 0.0),
    (35.0, -22.5, 0.0),
    (-35.0, 22.5, 0.0),
    (35.0, 22.5, 0.0),
]

headSeatZMain = topZ - 9.25              # +14.00mm
nutRoofZMain = -11.0
nutSlotH = 2.8
nutFloorZMain = nutRoofZMain - nutSlotH  # -13.80mm

headSeatZDorsal = topZ - 5.25            # +18.00mm
nutRoofZDorsal = -7.0
nutFloorZDorsal = nutRoofZDorsal - nutSlotH  # -9.80mm

with BuildPart() as lClampBuilder:
    # Union primary bodies
    add(clampBlock)
    add(armBody, mode=Mode.ADD)
    add(spineBody, mode=Mode.ADD)

    # Solid Upward Riser Peg at Rear End of Arm
    with Locations((-pegSize / 2.0, pegCenterY - (pegSize / 2.0), 10.0)):
        Box(pegSize, pegSize, 30.0, align=(Align.MIN, Align.MIN, Align.MIN), mode=Mode.ADD)

    # 4. Cut Rounded Square Tube Profile along X-axis
    # Sketch plane: origin=(-50, 0, 0), normal=(1, 0, 0), x_dir=(0, 1, 0) -> local Y is (0, 0, 1)
    tubePlane = Plane(origin=(-50.0, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(tubePlane) as tubeSketch:
        RectangleRounded(tubeSize, tubeSize, radius=tubeRad)
    extrude(tubeSketch.sketch, amount=100.0, mode=Mode.SUBTRACT)

    # 5A. Horizontal Compression Gap at Z = 0mm
    with Locations((-50.0, -35.0, -clampGap / 2.0)):
        Box(100.0, 70.0, clampGap, align=(Align.MIN, Align.MIN, Align.MIN), mode=Mode.SUBTRACT)

    # 5B. Split Part 1A from Part 1B
    with Locations((-25.0 - (clampGap / 2.0), 20.0 - clampGap, -clampGap / 2.0)):
        Box(
            50.0 + clampGap,
            35.0 - (20.0 - clampGap),
            spineBotZ - (-clampGap / 2.0),
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # 6A. Main 4 Corner Clamp Bolts (Through-Holes, Head Counterbores, Hex Nuts, & Side Slots)
    mainNutPlane = Plane(origin=(0.0, 0.0, nutFloorZMain), x_dir=(1, 0, 0), z_dir=(0, 0, 1))
    with BuildSketch(mainNutPlane) as mainNutSk:
        for pt in cornerCoords:
            bx, by, _ = pt
            with Locations((bx, by)):
                # firstVertex at nutPt + (R, 0) puts vertex on +X axis -> rotation=0.0
                RegularPolygon(
                    radius=nutCircumradius,
                    side_count=6,
                    major_radius=True,
                    rotation=0.0,
                    align=None,
                )
    extrude(mainNutSk.sketch, amount=nutSlotH, mode=Mode.SUBTRACT)

    for pt in cornerCoords:
        bx, by, _ = pt

        # M4 Through-Hole (Z = -26mm to +30mm)
        with Locations((bx, by, -26.0)):
            Cylinder(
                radius=m4Clear / 2.0,
                height=56.0,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.SUBTRACT,
            )

        # M4 Head Counterbore (Z = headSeatZMain to +30mm)
        with Locations((bx, by, headSeatZMain)):
            Cylinder(
                radius=m4Head / 2.0,
                height=30.0 - headSeatZMain,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.SUBTRACT,
            )

        # Side-Insertion Nut Slot
        xOuter = -50.0 if bx < 0.0 else 50.0
        xMin = xOuter if bx < 0.0 else bx
        xMax = bx if bx < 0.0 else xOuter

        with Locations((xMin, by - (m4Nut / 2.0), nutFloorZMain)):
            Box(
                xMax - xMin,
                m4Nut,
                nutSlotH,
                align=(Align.MIN, Align.MIN, Align.MIN),
                mode=Mode.SUBTRACT,
            )

    # 6B. 5th Dorsal Anchor Bolt (+4mm Higher in Z for 7mm Part 1A Head Material)
    with Locations((0.0, spineBoltY, -26.0)):
        Cylinder(
            radius=m4Clear / 2.0,
            height=56.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    with Locations((0.0, spineBoltY, headSeatZDorsal)):
        Cylinder(
            radius=m4Head / 2.0,
            height=30.0 - headSeatZDorsal,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    dorsalNutPlane = Plane(origin=(0.0, 0.0, nutFloorZDorsal), x_dir=(1, 0, 0), z_dir=(0, 0, 1))
    with BuildSketch(dorsalNutPlane) as dorsalNutSk:
        with Locations((0.0, spineBoltY)):
            RegularPolygon(
                radius=nutCircumradius,
                side_count=6,
                major_radius=True,
                rotation=0.0,
                align=None,
            )
    extrude(dorsalNutSk.sketch, amount=nutSlotH, mode=Mode.SUBTRACT)

    with Locations((0.0, spineBoltY - (m4Nut / 2.0), nutFloorZDorsal)):
        Box(
            50.0,
            m4Nut,
            nutSlotH,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # 7. Riser Peg Locking Pin Hole (M4 Through-Hole along Y-axis at X = 0mm, Z = +25mm)
    with Locations(Plane(origin=(0.0, pegCenterY - 15.0, 25.0), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Clear / 2.0,
            height=30.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )


# =========================================================================
# 9. Extract & Name Part 1A (Top Saddle Cap) and Part 1B (Lower Cradle & Arm)
# =========================================================================
# Sort solids by Z bounding box or center so lower is 1B and upper is 1A
solidsByZ = sorted(lClampBuilder.part.solids(), key=lambda s: s.bounding_box().min.Z)

part1b = solidsByZ[0]
part1b.label = "Part 1B - OnStage L Clamp Lower Cradle and Arm"

part1a = solidsByZ[1]
part1a.label = "Part 1A - OnStage L Clamp Top Saddle Cap and Spine"

lClampAssembly = Compound(
    label="OnStage L-Clamp (Part 1A & 1B)",
    children=[part1a, part1b],
)

if __name__ == "__main__":
    export_step(lClampAssembly, "part1a_1b_lClamp.step")
    export_stl(part1a, "part1a_topSaddleCapAndSpine.stl")
    export_stl(part1b, "part1b_lowerCradleAndArm.stl")
