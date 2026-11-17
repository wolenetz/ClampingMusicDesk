from build123d import *

# =========================================================================
# PART 3B: Upper Sliding Music Desk (Snug Runners + Rear Cam Micro-Grooves)
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

# 2A Parameters: Dual Male 4-Point Trapezoid Dovetail Runners
mFitTol = 0.05                    # Snug anti-rattle fit inside Part 3A's female slot
mYBack = guideDepth - mFitTol     # +13.95mm
mYFront = 5.0 + mFitTol           # +5.05mm
mHNeck = 6.0 - mFitTol            # 5.95mm half-width (11.90mm total neck width)
mHWide = 9.0 - mFitTol            # 8.95mm half-width (17.90mm total flare width)

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

# 4 Parameters: Rear 45-Degree Micro-Groove Indexing Rack
gDepth = 0.65
yOuter = guideDepth + 0.5         # 14.50mm
yTip = mYBack - gDepth            # 13.30mm

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

    # 2A. Dual Male 4-Point Trapezoid Dovetail Runners (At X = +/-75mm, Z = 0mm to 143mm)
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
    railPlane = Plane(origin=(cx - halfW + railInset, cy, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(railPlane) as railSketch:
        Polygon(*railPts, align=None)
    extrude(railSketch.sketch, amount=deskW - (2.0 * railInset), mode=Mode.ADD)

    # 4. Rear 45-Degree Micro-Groove Indexing Rack Along Both Runners (Z = 10mm to 120mm, every 2.5mm)
    groovePlane = Plane(origin=(cx - halfW, cy, 0.0), x_dir=(0, 1, 0), z_dir=(1, 0, 0))
    with BuildSketch(groovePlane) as grooveSk:
        for g in range(45):
            gz = runnerBotZ + 10.0 + (g * 2.5)
            groovePts = [
                (yOuter, gz - gDepth - 0.5),  # gp1
                (yTip, gz),                   # gp2
                (yOuter, gz + gDepth + 0.5),  # gp3
            ]
            Polygon(*groovePts, align=None)
    extrude(grooveSk.sketch, amount=deskW, mode=Mode.SUBTRACT)

part3b = part3bBuilder.part
part3b.label = "Part 3B - Upper Sliding Music Desk"

if __name__ == "__main__":
    export_step(part3b, "part3b_upperSlidingMusicDesk.step")
    export_stl(part3b, "part3b_upperSlidingMusicDesk.stl")
