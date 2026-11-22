from build123d import *

# =========================================================================
# PART 3B: Upper Sliding Music Desk (Snug Runners + Central Cam Indexing)
# =========================================================================

cx = 500.0
cy = 0.0

deskW = 240.0
halfW = deskW / 2.0               # 120.0mm
plateT = 6.0
guideDepth = 14.0

runnerBotZ = 0.0                  # Runners start on Z = 0 floor
headBotZ = 130.0                  # 130.0mm of exposed runner below 60mm faceplate
headH = 60.0
headTopZ = headBotZ + headH       # Total Part 3B height = 190.0mm
runnerTopZ = headTopZ - 47.0      # 143.0mm

railXOffsets = [-75.0, 75.0]

# 2A Parameters: Side Male 4-Point Trapezoid Dovetail Runners (unchanged)
mFitTol = 0.05                    # Snug anti-rattle fit inside Part 3A's female slot
mYBack = guideDepth - mFitTol     # +13.95mm
mYFront = 5.0 + mFitTol           # +5.05mm
mHNeck = 6.0 - mFitTol            # 5.95mm half-width (11.90mm total neck width)
mHWide = 9.0 - mFitTol            # 8.95mm half-width (17.90mm total flare width)

# 2A Parameters: Central Wide Male Dovetail for Single Cam Lock Engagement
centralHalfNeck = 35.0 + mFitTol   # ~35.05mm neck half-width (70.1mm total at rear)
centralHalfWide = 42.0 + mFitTol   # ~42.05mm wide half-width (84.1mm total at front)

# 3 Parameters: Horizontal Clamp Accessory Rail on Top Back
railInset = 3.0
railBaseZ = headTopZ - 40.0       # 150.0mm
railClearTol = 0.25

railPts = [
    (0.0, railBaseZ + 4.0 + railClearTol),                 # u1
    (6.0 + railClearTol, railBaseZ + 4.0 + railClearTol),  # u2
    (6.0 + railClearTol, railBaseZ + railClearTol),        # u3
    (14.0 - railClearTol, railBaseZ + 4.0 + railClearTol), # u4
    (14.0 - railClearTol, railBaseZ + 24.0 - railClearTol),# u5
    (6.0 + railClearTol, railBaseZ + 28.0 - railClearTol), # u6
    (6.0 + railClearTol, railBaseZ + 24.0 - railClearTol), # u7
    (0.0, railBaseZ + 24.0 - railClearTol),                # u8
]

# 4 Parameters: Rear 45-Degree Micro-Groove Indexing Rack for Central Dovetail ONLY
gDepth = 0.65
yOuter = guideDepth + 0.5         # 14.50mm (groove outer edge)
centralYTip = centralHalfNeck - gDepth  # Groove tip at central dovetail rear face

with BuildPart() as part3bBuilder:
    # 1. Upper Music Desk Plate (240mm wide x 6mm thick x 60mm tall, Z = 130mm to 190mm)
    with Locations((cx - halfW, cy - plateT, headBotZ)):
        Box(
            deskW,
            plateT,
            headH,
            align=(Align.MIN, Align.MIN, Align.MIN),
        )
    # Round the 4 vertical (Z-parallel) side edges of the 6mm upper plate while pristine
    fillet(part3bBuilder.edges().filter_by(Axis.Z), radius=2.8)

    # 2A. Side Male 4-Point Trapezoid Dovetail Runners (At X = +/-75mm, Z = 0mm to 143mm)
    # These remain smooth - NO indexing grooves for backward compatibility
    runnerPlane = Plane(origin=(cx, cy, runnerBotZ), x_dir=(1, 0, 0), z_dir=(0, 0, 1))
    with BuildSketch(runnerPlane) as maleSlideSk:
        for rx in railXOffsets:
            runnerPts = [
                (rx - mHWide, mYFront),  # mp1
                (rx + mHWide, mYFront),  # mp2
                (rx + mHNeck, mYBack),   # mp3
                (rx - mHNeck, mYBack),   # mp4
            ]
            Polygon(*runnerPts, align=None)
    extrude(maleSlideSk.sketch, amount=runnerTopZ - runnerBotZ, mode=Mode.ADD)

    # 2A-Central. Wide Central Male Dovetail for Single Cam Lock (At X = 0mm, Z = 0mm to 143mm)
    with BuildSketch(runnerPlane) as centralDoveSk:
        centralPts = [
            (-centralHalfWide, mYFront),   # cp1: front wide face left
            (+centralHalfWide, mYFront),   # cp2: front wide face right
            (+centralHalfNeck, mYBack),    # cp3: rear neck right
            (-centralHalfNeck, mYBack),    # cp4: rear neck left
        ]
        Polygon(*centralPts, align=None)
    extrude(centralDoveSk.sketch, amount=runnerTopZ - runnerBotZ, mode=Mode.ADD)

    # 2B. Upper Stand-Off Webs (Bridges Y = 0mm to Y = +6mm behind Upper Plate from Z = 130mm to 143mm)
    for rx in railXOffsets:
        with Locations((cx + rx - mHWide, cy, headBotZ)):
            Box(
                2.0 * mHWide,
                6.0,
                runnerTopZ - headBotZ,
                align=(Align.MIN, Align.MIN, Align.MIN),
                mode=Mode.ADD,
            )

    # 3. Part 4 Horizontal Clamp Accessory Rail on Top Back of Part 3B (Z = 150mm to 178mm)
    railPlaneSketch = Plane(origin=(cx - halfW + railInset, cy, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(railPlaneSketch) as railSketch:
        Polygon(*railPts, align=None)
    extrude(railSketch.sketch, amount=deskW - (2.0 * railInset), mode=Mode.ADD)

    # 4. Rear 45-Degree Micro-Groove Indexing Rack for Central Dovetail ONLY
    # V-grooves cut into rear face of central wide doveetail at X=cx (global X=500mm)
    # Side runners at X = +/-75mm remain smooth for backward compatibility
    gDepth = 0.65          # Groove depth in mm
    gPitch = 2.5           # Pitch between groove centers in mm
    zStart = 10.0          # Start Z height from bottom of runners
    zEnd = 122.5           # End Z height

    with BuildPart() as grooveCuts:
        for iz in range(int((zEnd - zStart) / gPitch) + 1):
            zPos = zStart + iz * gPitch

            # Position at rear face of doveetails (Y=mYBack), centered on X=cx (central doveetail only!)
            with Locations((cx, mYBack, zPos)):
                vGrooveWidth = gDepth * 2.0  # ~1.3mm total width for V-groove

                # Box cut: extend NEGATIVE Y from rear face INTO doveetail material
                # Align MAX in Y means box extends negative direction from location (toward front)
                Box(
                    vGrooveWidth,      # X width of groove opening
                    gDepth * 3.0,      # Y length extending into doveetail
                    gDepth * 1.5,      # Z height of groove feature
                    align=(Align.CENTER, Align.MAX, Align.CENTER),
                )

    # Subtract groove cuts from base part to create indexing rack on central dovetail only
    part3bBuilder.part = (part3bBuilder.part - grooveCuts.part)

part3b = part3bBuilder.part.fuse()  # Fuse all solids into single compound at end
part3b.label = "Part 3B - Upper Sliding Music Desk with Central Cam Lock"

if __name__ == "__main__":
    export_step(part3b, "part3b_upperSlidingMusicDesk.step")
    export_stl(part3b, "part3b_upperSlidingMusicDesk.stl")
