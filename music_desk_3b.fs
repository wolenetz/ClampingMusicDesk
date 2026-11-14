// =========================================================================
// PART 3B: Upper Sliding Music Desk (Rounded Plate Sides, Crisp Accessory Rail)
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
    var headH       = 60 * millimeter;
    var headTopZ   = headBotZ + headH; // Total Part 3B height = 190mm

    var railXOffsets = [-75 * millimeter, 75 * millimeter];

    // 2. Upper Music Desk Plate (240mm wide x 6mm thick x 60mm tall, Z = 130mm to 190mm)
    fCuboid(context, id + "upperPlate", {
            "corner1" : vector(cx - halfW, cy - plateT, headBotZ),
            "corner2" : vector(cx + halfW, cy,           headTopZ)
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

    // 3. Dual Male Dovetail Runners (At X = +/-75mm, extending from Z = 0mm to Z = 143mm)
    var maleSlideSk = newSketchOnPlane(context, id + "maleSlideSk", {
            "sketchPlane" : plane(vector(cx, cy, runnerBotZ), vector(0, 0, 1), vector(1, 0, 0))
    });

    var mYBack  = guideDepth;     // +14mm
    var mYNeck  = 5 * millimeter;  // +5mm
    var mYFront = 0 * millimeter;  // 0mm (connects directly to the back of upperPlate from Z = 130 to 143mm!)
    var mHNeck  = 6 * millimeter;  // 12mm total neck width at Y = +14mm
    var mHWide  = 9 * millimeter;  // 18mm total flare width at Y = +5mm

    var mIdx = 0;
    for (var rx in railXOffsets)
    {
        mIdx += 1;
        var mp1 = vector(rx - mHWide, mYFront);
        var mp2 = vector(rx + mHWide, mYFront);
        var mp3 = vector(rx + mHWide, mYNeck);
        var mp4 = vector(rx + mHNeck, mYBack);
        var mp5 = vector(rx - mHNeck, mYBack);
        var mp6 = vector(rx - mHWide, mYNeck);

        skLineSegment(maleSlideSk, "ms1_" ~ mIdx, { "start" : mp1, "end" : mp2 });
        skLineSegment(maleSlideSk, "ms2_" ~ mIdx, { "start" : mp2, "end" : mp3 });
        skLineSegment(maleSlideSk, "ms3_" ~ mIdx, { "start" : mp3, "end" : mp4 });
        skLineSegment(maleSlideSk, "ms4_" ~ mIdx, { "start" : mp4, "end" : mp5 });
        skLineSegment(maleSlideSk, "ms5_" ~ mIdx, { "start" : mp5, "end" : mp6 });
        skLineSegment(maleSlideSk, "ms6_" ~ mIdx, { "start" : mp6, "end" : mp1 });
    }
    skSolve(maleSlideSk);

    opExtrude(context, id + "extMaleRunners", {
            "entities" : qSketchRegion(id + "maleSlideSk"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : (headTopZ - 47 * millimeter) - runnerBotZ
    });

    // 4. Part 4 Horizontal Clamp Accessory Rail on Top Back of Part 3B (Z = 150mm to 178mm)
    // Inset by 3mm on Left & Right ends (X = cx - 117mm to cx + 117mm) so the rail ends sit cleanly
    // inside the 2.8mm rounded vertical back edge of upperPlate without overhanging in mid-air!
    var railInset  = 3 * millimeter;
    var railBaseZ = headTopZ - (40 * millimeter);
    var railSketch  = newSketchOnPlane(context, id + "railSketch", {
            "sketchPlane" : plane(vector(cx - halfW + railInset, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });
    var u1 = vector(0 * millimeter,  railBaseZ + 4 * millimeter);
    var u2 = vector(6 * millimeter,  railBaseZ + 4 * millimeter);
    var u3 = vector(6 * millimeter,  railBaseZ);
    var u4 = vector(14 * millimeter, railBaseZ + 4 * millimeter);
    var u5 = vector(14 * millimeter, railBaseZ + 24 * millimeter);
    var u6 = vector(6 * millimeter,  railBaseZ + 28 * millimeter);
    var u7 = vector(6 * millimeter,  railBaseZ + 24 * millimeter);
    var u8 = vector(0 * millimeter,  railBaseZ + 24 * millimeter);

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
            "tools" : qUnion([
                    qCreatedBy(id + "upperPlate", EntityType.BODY),
                    qCreatedBy(id + "extMaleRunners", EntityType.BODY),
                    qCreatedBy(id + "extUpperRail", EntityType.BODY)
            ]),
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
