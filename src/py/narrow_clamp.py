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

# Arm width parameter - narrower than standard l_clamp.py (which uses arm_halfW=25mm, total 50mm)
# This narrow version uses arm_halfW=15mm for a total of 30mm wide arm
arm_halfW = 15.0                                 # Half-width of extension arm (narrow version: 15mm vs standard 25mm)

# Clamp block X-half width parameter - narrower than original l_clamp.py (which uses clampHalfW=45mm, total 90mm)
# This narrow version uses clampHalfW=30mm for a total of 60mm wide clamp block
clampHalfW = 30.0                                # Half-width of vertical clamp block (narrow: 30mm vs standard 45mm)

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

# 1. Solid Clamp Block (Filleted on Z-parallel edges while pristine)
with BuildPart() as clampBlockBuilder:
    with Locations((-clampHalfW, -30.0, botZ)):
        Box(2.0 * clampHalfW, 60.0, topZ - botZ, align=(Align.MIN, Align.MIN, Align.MIN))
    fillet(clampBlockBuilder.edges().filter_by(Axis.Z), radius=4.0)
clampBlock = clampBlockBuilder.part


# 2. Profiled Extension Arm (Part 1B) with Swept Curved Bottom Keel
curveStartY = armEndY - 45.0
a1 = (arm_halfW, botZ)
a2 = (curveStartY, botZ)
a3 = (armEndY, 10.0)
a4 = (arm_halfW, 10.0)
aMid = (curveStartY + 31.0, botZ + 13.0)

with BuildPart() as armBuilder:
    armPlane = Plane(origin=(-arm_halfW, 0.0, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(armPlane) as armProfileSk:
        with BuildLine():
            Line(a1, a2)
            ThreePointArc(a2, aMid, a3)
            Line(a3, a4)
            Line(a4, a1)
        make_face()
    extrude(armProfileSk.sketch, amount=2.0 * arm_halfW)

    # 2A. Fillet Arm Longitudinal Edges, Curved Keel, and Rear Top Edge (radius = 5mm)
    armTargetPts = [
        (-arm_halfW, 50.0, 10.0),       # topLongEdges left
        (arm_halfW, 50.0, 10.0),        # topLongEdges right
        (-arm_halfW, 50.0, botZ),       # botFlatEdges left
        (arm_halfW, 50.0, botZ),        # botFlatEdges right
        (-arm_halfW, aMid[0], aMid[1]), # keelArcEdges left
        (arm_halfW, aMid[0], aMid[1]),  # keelArcEdges right
        (0.0, armEndY, 10.0),           # rearTopEdge
    ]
    armFilletEdges = [edgeAtPoint(armBuilder.part, pt) for pt in armTargetPts]
    fillet(armFilletEdges, radius=5.0)
armBody = armBuilder.part

# 2B. Central Dorsal Tension Spine on Part 1A (Y = 20mm to seatFrontY, Z = 11mm to topZ)
spineBotZ = 10.0 + (clampGap / 2.0)  # +11.00mm
with BuildPart() as spineBuilder:
    with Locations((-10.0, 20.0, spineBotZ)):
        Box(20.0, seatFrontY - 20.0, topZ - spineBotZ, align=(Align.MIN, Align.MIN, Align.MIN))

    # Select the 2 top longitudinal edges (and top rear edge if desired)
    spineTargetPts = [
        (-10.0, 100.0, topZ),     # Left top longitudinal edge
        (10.0, 100.0, topZ),      # Right top longitudinal edge
        (0.0, seatFrontY, topZ),  # Rear top transverse edge (midpoint avoids 3-edge corner ambiguity)
    ]
    spineFilletEdges = [edgeAtPoint(spineBuilder.part, pt) for pt in spineTargetPts]
    # Subtract 1e-4 from 10.0mm so the two 10mm fillets on the 20mm wide spine don't collide at X=0 in OCCT
    fillet(spineFilletEdges, radius=10.0 - 1e-4)
spineBody = spineBuilder.part

# 3. Union Main Clamp Block, Arm, Dorsal Spine, and Upward Riser Peg, then Cut All Features
cornerCoords = [
    (-clampHalfW + 15.0, -22.5, 0.0),   # M4 bolt positions inset from block edge (30-15=15mm offset)
    (clampHalfW - 15.0, -22.5, 0.0),
    (-clampHalfW + 15.0, 22.5, 0.0),
    (clampHalfW - 15.0, 22.5, 0.0),
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

    # 5A. Horizontal Compression Gap at Z = 0mm (split clamp TOP from BOTTOM)
    with Locations((-50.0, -35.0, -clampGap / 2.0)):
        Box(100.0, 70.0, clampGap, align=(Align.MIN, Align.MIN, Align.MIN), mode=Mode.SUBTRACT)

    # 5B. Small Y-notch into front-top of lower arm (Part 1B) where it meets upper clamp backside (Part 1A)
    # Position: just past clamp block front edge at Y=30 (+clampGap/2 for gap), small X-width to match arm,
    # Z-height from Z=0 up toward top of lower arm section (~+11mm below spine bottom)
    # This creates separation without cutting into main clamp block area (-30 to +30 in Y)
    with Locations((-arm_halfW - (clampGap / 2.0), 35.0, 0.0)):
        Box(
            2 * arm_halfW + clampGap,          # X-width: matches arm width (~32mm from -16 to +16)
            clampGap,                            # Small Y-depth (only ~2mm notch into front-top of lower arm)
            spineBotZ - 0.0,                     # Z-height from Z=0 up toward top of lower arm section (~+11mm)
            align=(Align.MIN, Align.CENTER, Align.MIN),
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

        # Side-Insertion Nut Slot (extends from bolt to clamp block edge)
        slotXLen = 15.0  # Distance from bolt center to side of block

        with Locations((bx - slotXLen if bx < 0.0 else bx, by - (m4Nut / 2.0), nutFloorZMain)):
            Box(
                2 * slotXLen,
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
            clampHalfW + 5.0,   # Extended slot for narrow block
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
    export_step(lClampAssembly, "part1a_1b_narrow_lClamp_v4.step")
    export_stl(part1a, "part1a_topSaddleCapSpine.stl")
    export_stl(part1b, "part1b_lowerCradleArmV4.stl")
