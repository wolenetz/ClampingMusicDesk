// =========================================================================
// PART 3B: Upper Sliding Music Desk (Fixed 4-Point Runners + Loosened Accessory Rail)
// =========================================================================
FeatureScript 2411;
import(path : "onshape/std/common.fs", version : "2411.0");

// Helper function to read variables from your Part Studio Variable Table
function getVar(context is Context, name is string, defaultVal is ValueWithUnits) returns ValueWithUnits
{
    try silent
    {
        return getVariable(context, name);
    }
    catch
    {
        return defaultVal;
    }
}

function onStageMusicDesk3BImpl(context is Context, id is Id, definition is map)
{
    // 1. Pull variables from your Variable Table
    var m3Clear = getVar(context, "m3_clear", 3.4 * millimeter);

    // Placed side-by-side at X = 500mm (and resting on Z = 0mm!) so it is 100% separate from 3A!
    var cx = 500 * millimeter;
    var cy = 0 * millimeter;

    var deskW      = 240 * millimeter;
    var halfW      = deskW / 2;       // 120mm
    var plateT     = 6 * millimeter;
    var guideDepth = 14 * millimeter;

    var runnerBotZ = 0 * millimeter;   // Runners start right on the Z = 0 floor for easy viewing & slicing
    var headBotZ   = 130 * millimeter; // 130mm of exposed runner below the 60mm tall upper faceplate
    var headH      = 60 * millimeter;
    var headTopZ   = headBotZ + headH; // Total Part 3B height = 190mm
    var runnerTopZ = headTopZ - (47 * millimeter); // 143mm

    var railXOffsets = [-75 * millimeter, 75 * millimeter];

    // 2. Upper Music Desk Plate (240mm wide x 6mm thick x 60mm tall, Z = 130mm to 190mm)
    fCuboid(context, id + "upperPlate", {
            "corner1" : vector(cx - halfW, cy - plateT, headBotZ),
            "corner2" : vector(cx + halfW, cy,          headTopZ)
    });

    // Round the 4 vertical (Z-parallel) side edges of the 6mm upper plate while pristine!
    // Leaves the Z = 190mm top face 100% flat for upside-down bed adhesion, and does not touch the accessory rail!
    try silent
    {
        opFillet(context, id + "filletUpperPlateSides", {
            "entities" : qParallelEdges(qOwnedByBody(qCreatedBy(id + "upperPlate", EntityType.BODY), EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 2.8 * millimeter
        });
    }

    // 3A. Dual Male 4-Point Trapezoid Dovetail Runners (At X = +/-75mm, Z = 0mm to 143mm)
    // Matches Part 3A's exact 4-point trapezoid slot (which sits between Y = 4.55mm and Y = 14mm)
    // with 0.25mm extra male clearance so it slides smoothly
    var maleSlideSk = newSketchOnPlane(context, id + "maleSlideSk", {
            "sketchPlane" : plane(vector(cx, cy, runnerBotZ), vector(0, 0, 1), vector(1, 0, 0))
    });

    var mFitTol = 0.25 * millimeter;            // Extra male clearance for smooth sliding in printed 3A
    var mYBack  = guideDepth - mFitTol;         // +13.75mm (stays slightly inside the Y = 14mm rear face)
    var mYFront = (5 * millimeter) + mFitTol;   // +5.25mm  (clears Part 3A's solid Y = 0..4.55mm front wall!)
    var mHNeck  = (6 * millimeter) - mFitTol;   // 11.50mm total neck width at Y = +13.75mm (3A slot is ~12.37mm wide)
    var mHWide  = (9 * millimeter) - mFitTol;   // 17.50mm total flare width at Y = +5.25mm (3A slot is ~18.42mm wide)

    var mIdx = 0;
    for (var rx in railXOffsets)
    {
        mIdx += 1;
        var mp1 = vector(rx - mHWide, mYFront);
        var mp2 = vector(rx + mHWide, mYFront);
        var mp3 = vector(rx + mHNeck, mYBack);
        var mp4 = vector(rx - mHNeck, mYBack);

        skLineSegment(maleSlideSk, "msFront_" ~ mIdx, { "start" : mp1, "end" : mp2 });
        skLineSegment(maleSlideSk, "msRight_" ~ mIdx, { "start" : mp2, "end" : mp3 });
        skLineSegment(maleSlideSk, "msBack_"  ~ mIdx, { "start" : mp3, "end" : mp4 });
        skLineSegment(maleSlideSk, "msLeft_"  ~ mIdx, { "start" : mp4, "end" : mp1 });
    }
    skSolve(maleSlideSk);

    opExtrude(context, id + "extMaleRunners", {
            "entities" : qSketchRegion(id + "maleSlideSk"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : runnerTopZ - runnerBotZ
    });

    // 3B. Upper Stand-Off Webs (Bridges Y = 0mm to Y = +6mm ONLY behind the Upper Plate from Z = 130mm to 143mm!)
    // Connects the trapezoid runners solidly to the back of upperPlate while keeping Z = 0..130mm 100% clear!
    var webBodies = [];
    var wIdx = 0;
    for (var rx in railXOffsets)
    {
        wIdx += 1;
        fCuboid(context, id + ("runnerWeb" ~ wIdx), {
                "corner1" : vector(cx + rx - mHWide, cy,                  headBotZ),
                "corner2" : vector(cx + rx + mHWide, cy + 6 * millimeter, runnerTopZ)
        });
        webBodies = append(webBodies, qCreatedBy(id + ("runnerWeb" ~ wIdx), EntityType.BODY));
    }

    // 4. Part 4 Horizontal Clamp Accessory Rail on Top Back of Part 3B (Z = 150mm to 178mm)
    // Inset by railClearTol (0.25mm) on all mating faces so Part 4 page clamps slide smoothly
    var railInset    = 3 * millimeter;
    var railBaseZ    = headTopZ - (40 * millimeter);
    var railClearTol = 0.25 * millimeter; // Loosens fit against existing Part 4 clamps without modifying Part 4

    var railSketch = newSketchOnPlane(context, id + "railSketch", {
            "sketchPlane" : plane(vector(cx - halfW + railInset, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });
    var u1 = vector(0 * millimeter,                railBaseZ + 4 * millimeter + railClearTol);
    var u2 = vector(6 * millimeter + railClearTol, railBaseZ + 4 * millimeter + railClearTol);
    var u3 = vector(6 * millimeter + railClearTol, railBaseZ + railClearTol);
    var u4 = vector(14 * millimeter - railClearTol, railBaseZ + 4 * millimeter + railClearTol);
    var u5 = vector(14 * millimeter - railClearTol, railBaseZ + 24 * millimeter - railClearTol);
    var u6 = vector(6 * millimeter + railClearTol, railBaseZ + 28 * millimeter - railClearTol);
    var u7 = vector(6 * millimeter + railClearTol, railBaseZ + 24 * millimeter - railClearTol);
    var u8 = vector(0 * millimeter,                railBaseZ + 24 * millimeter - railClearTol);

    skLineSegment(railSketch, "uL1", { "start" : u1, "end" : u2 });
    skLineSegment(railSketch, "uL2", { "start" : u2, "end" : u3 });
    skLineSegment(railSketch, "uL3", { "start" : u3, "end" : u4 });
    skLineSegment(railSketch, "uL4", { "start" : u4, "end" : u5 });
    skLineSegment(railSketch, "uL5", { "start" : u5, "end" : u6 });
    skLineSegment(railSketch, "uL6", { "start" : u6, "end" : u7 });
    skLineSegment(railSketch, "uL7", { "start" : u7, "end" : u8 });
    skLineSegment(railSketch, "uL8", { "start" : u8, "end" : u1 });
    skSolve(railSketch);

    opExtrude(context, id + "extUpperRail", {
            "entities" : qSketchRegion(id + "railSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : deskW - (2 * railInset)
    });

    // Union all Part 3B Upper Extension components into a single solid body
    opBoolean(context, id + "unionUpperDesk", {
            "tools" : qUnion(concatenateArrays([
                    [
                        qCreatedBy(id + "upperPlate", EntityType.BODY),
                        qCreatedBy(id + "extMaleRunners", EntityType.BODY),
                        qCreatedBy(id + "extUpperRail", EntityType.BODY)
                    ],
                    webBodies
            ])),
            "operationType" : BooleanOperationType.UNION
    });

    // 5. Series of 9 Vertical Detent Pin Holes Spaced Every 15mm Along Both Male Dovetail Runners!
    var runnerHoleTools = [];
    var hIdx = 0;
    for (var rx in railXOffsets)
    {
        for (var k = 0; k < 9; k += 1)
        {
            hIdx += 1;
            var holeZ = runnerBotZ + (110 * millimeter) - (k * 15 * millimeter);

            fCylinder(context, id + ("runnerDetent" ~ hIdx), {
                    "bottomCenter" : vector(cx + rx - 12 * millimeter, cy + 9.5 * millimeter, holeZ),
                    "topCenter"    : vector(cx + rx + 12 * millimeter, cy + 9.5 * millimeter, holeZ),
                    "radius"       : m3Clear / 2
            });
            runnerHoleTools = append(runnerHoleTools, qCreatedBy(id + ("runnerDetent" ~ hIdx), EntityType.BODY));
        }
    }

    // Subtract all 18 detent pin holes from Part 3B
    opBoolean(context, id + "finalUpperCut", {
            "tools" : qUnion(runnerHoleTools),
            "targets" : qCreatedBy(id + "upperPlate", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}

annotation { "Feature Type Name" : "OnStage Music Desk 3B (Upper)" }
export const onStageMusicDesk3B = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3BImpl(context, id, definition);
    }
);