import math
from build123d import *

# =========================================================================
# PART 2: Modular 2-Piece Riser Stem (Part 2A Extension + Part 2B Upper Head)
# =========================================================================

# Parameters (matching FeatureScript defaults in mm)
m4Clear = 4.4
m4Nut = 7.4
m4Head = 7.6
pegSize = 20.0
tol = 0.3

colW = 40.0
colD = 30.0
halfW = colW / 2.0        # 20.0mm
halfD = colD / 2.0        # 15.0mm
pegH = 30.0               # 30.0mm tall mating peg (matches Part 1B)
doveH = 70.0              # 70.0mm tall mega-dovetail
pinZ = 15.0               # Locking bolt height inside sockets/pegs

# Splitting the 223mm column into two bed-friendly segments:
# 115mm (2A body) + 108mm (2B body) = 223mm total column height
colH2A = 115.0            # Part 2A total length = 115 + 30 = 145mm
colH2B = 108.0            # Part 2B total length = 108 + 70 = 178mm

socketSize = pegSize + tol
halfSoc = socketSize / 2.0
halfPeg = pegSize / 2.0

nutCircumradius = m4Nut / (2.0 * math.cos(math.radians(30.0)))

# =========================================================================
# PART 2A: Lower Modular Extension Column (X = 75mm, Y = 0mm)
# =========================================================================
cxA = 75.0
cyA = 0.0

with BuildPart() as part2aBuilder:
    # 1. Main 40x30x115mm Column Body + 3mm vertical edge fillets
    with Locations((cxA - halfW, cyA - halfD, 0.0)):
        Box(
            colW,
            colD,
            colH2A,
            align=(Align.MIN, Align.MIN, Align.MIN),
        )
    fillet(part2aBuilder.edges().filter_by(Axis.Z), radius=3.0)

    # 2. Top 20x20x30mm Male Coupling Peg (Z = 115mm to 145mm)
    with Locations((cxA - halfPeg, cyA - halfPeg, colH2A)):
        Box(
            pegSize,
            pegSize,
            pegH,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.ADD,
        )

    # 3. Bottom Female Socket Cutout (Z = -1mm to 31mm)
    with Locations((cxA - halfSoc, cyA - halfSoc, -1.0)):
        Box(
            socketSize,
            socketSize,
            32.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket M4 Through-Hole (Y = cyA - 20mm to cyA + 20mm, Z = 15mm)
    with Locations(Plane(origin=(cxA, cyA - 20.0, pinZ), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Clear / 2.0,
            height=40.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket Front M4 Head Counterbore (Y = cyA - 20mm to cyA - 13mm, Z = 15mm)
    with Locations(Plane(origin=(cxA, cyA - 20.0, pinZ), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Head / 2.0,
            height=7.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket Rear M4 Hex Nut Trap (2.0mm deep into +Y face in -Y direction)
    # In FeatureScript: plane normal=(0,1,0), x_dir=(1,0,0) -> local Y is (0,0,-1).
    # First vertex at (0, R) aligns vertices along global Z and vertical flats along global X.
    nutPlane2A = Plane(origin=(cxA, cyA + halfD, pinZ), x_dir=(1, 0, 0), z_dir=(0, 1, 0))
    with BuildSketch(nutPlane2A) as nutSk2A:
        RegularPolygon(
            radius=nutCircumradius,
            side_count=6,
            major_radius=True,
            rotation=90.0,
            align=None,
        )
    extrude(nutSk2A.sketch, amount=-2.0, mode=Mode.SUBTRACT)

    # Upper Peg M4 Through-Hole (Y = cyA - 15mm to cyA + 15mm, Z = 130mm)
    with Locations(Plane(origin=(cxA, cyA - 15.0, colH2A + pinZ), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Clear / 2.0,
            height=30.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

part2a = part2aBuilder.part
part2a.label = "Part 2A - Lower Riser Stem Modular Extension"


# =========================================================================
# PART 2B: Upper Dovetail Riser Head (X = 75mm, Y = 60mm)
# =========================================================================
cxB = 75.0
cyB = 60.0

yBack = halfD                 # +15.0mm (flush with back face of column)
yFront = halfD - 18.0         # -3.0mm  (18mm thick mega-dovetail)
halfNeck = 13.0               # 26.0mm total neck width
halfWide = 19.0               # 38.0mm total front flare width

dovePts = [
    (-halfNeck, yBack),
    (halfNeck, yBack),
    (halfWide, yFront),
    (-halfWide, yFront),
]

with BuildPart() as part2bBuilder:
    # 1. Main 40x30x108mm Column Body + 3mm vertical edge fillets
    with Locations((cxB - halfW, cyB - halfD, 0.0)):
        Box(
            colW,
            colD,
            colH2B,
            align=(Align.MIN, Align.MIN, Align.MIN),
        )
    fillet(part2bBuilder.edges().filter_by(Axis.Z), radius=3.0)

    # 2. Mega Male Dovetail Slider (Z = 108mm to 178mm)
    with BuildSketch(Plane(origin=(cxB, cyB, colH2B), x_dir=(1, 0, 0), z_dir=(0, 0, 1))) as doveSketch:
        Polygon(*dovePts, align=None)
    extrude(doveSketch.sketch, amount=doveH, mode=Mode.ADD)

    # 3. Bottom Female Socket Cutout (Z = -1mm to 31mm)
    with Locations((cxB - halfSoc, cyB - halfSoc, -1.0)):
        Box(
            socketSize,
            socketSize,
            32.0,
            align=(Align.MIN, Align.MIN, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket M4 Through-Hole (Y = cyB - 20mm to cyB + 20mm, Z = 15mm)
    with Locations(Plane(origin=(cxB, cyB - 20.0, pinZ), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Clear / 2.0,
            height=40.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket Front M4 Head Counterbore (Y = cyB - 20mm to cyB - 13mm, Z = 15mm)
    with Locations(Plane(origin=(cxB, cyB - 20.0, pinZ), z_dir=(0, 1, 0))):
        Cylinder(
            radius=m4Head / 2.0,
            height=7.0,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.SUBTRACT,
        )

    # Lower Socket Rear M4 Hex Nut Trap (2.0mm deep into +Y face in -Y direction)
    nutPlane2B = Plane(origin=(cxB, cyB + halfD, pinZ), x_dir=(1, 0, 0), z_dir=(0, 1, 0))
    with BuildSketch(nutPlane2B) as nutSk2B:
        RegularPolygon(
            radius=nutCircumradius,
            side_count=6,
            major_radius=True,
            rotation=90.0,
            align=None,
        )
    extrude(nutSk2B.sketch, amount=-2.0, mode=Mode.SUBTRACT)

part2b = part2bBuilder.part
part2b.label = "Part 2B - Upper Dovetail Riser Head"


# =========================================================================
# Assembly & STL/STEP Export
# =========================================================================
riserStemAssembly = Compound(
    label="OnStage Riser Stem (Part 2A & 2B)",
    children=[part2a, part2b],
)

if __name__ == "__main__":
    export_step(riserStemAssembly, "renders/part2a_2b_riserStem.step")
    export_stl(part2a, "renders/part2a_lowerRiserExtension.stl")
    export_stl(part2b, "renders/part2b_upperDovetailHead.stl")
