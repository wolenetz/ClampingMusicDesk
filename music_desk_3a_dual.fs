// =========================================================================
// PART 3A-DUAL: Dual-Riser Lower Stationary Music Desk (Left & Right Risers)
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

function onStageMusicDesk3ADualImpl(context is Context, id is Id, definition is map)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var deskAngle = getVar(context, "desk_angle", 15 * degree);
    var m3Clear   = getVar(context, "m3_clear", 3.4 * millimeter);

    var slideTol  = tol + (0.15 * millimeter);

    // Positioned directly behind standard Part 3A at Y = 220mm for clean Part Studio viewing
    var cx = 230 * millimeter;
    var cy = 220 * millimeter;

    var deskW  = 240 * millimeter;
    var halfW  = deskW / 2;       // 120mm
    var lowerH = 180 * millimeter; // Height of Part 3A lower plate
    var plateT = 6 * millimeter;   // 6mm rigid main backplate
    var shelfT = 8 * millimeter;   // 8mm bottom shelf
    var shelfD = 48 * millimeter;  // 48mm inside book-resting depth
    var lipT   = 5 * millimeter;   // 5mm front retention lip thickness
    var lipH   = 8 * millimeter;   // 8mm upward front retention lip

    // Sketch-local Y coordinates (relative to cy) so profile math is identical to 3A
    var yFrontLocal  = -plateT - shelfD - lipT; // -59mm
    var railXOffsets = [-75 * millimeter, 75 * millimeter];
    var riserXOffsets = [-75 * millimeter, 75 * millimeter]; // 150mm center-to-center dual riser spacing

    // 1. Lower Vertical Backing Plate (Y = cy - 6mm to cy, Z = 0 to 180mm)
    fCuboid(context, id + "backPlate", {
            "corner1" : vector(cx - halfW, cy - plateT, 0 * millimeter),
            "corner2" : vector(cx + halfW, cy,           lowerH)
    });

    // 2. Continuous Profiled Book Shelf + Front Lip + 4mm Inside Root Chamfer
    var shelfSketch = newSketchOnPlane(context, id + "shelfSketch", {
            "sketchPlane" : plane(vector(cx - halfW, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    var lipTopZ = shelfT + lipH;                // 16mm
    var rLip     = 2.4 * millimeter;               // Smooth 2.4mm front-top bullnose radius
    var arcOff   = rLip * (1 - cos(45 * degree)); // Exact 45-deg tangent point

    var s1 = vector(-plateT,                  0 * millimeter);
    var s2 = vector(yFrontLocal,             0 * millimeter);
    var s3 = vector(yFrontLocal,             lipTopZ - rLip);
    var sM = vector(yFrontLocal + arcOff,   lipTopZ - arcOff);
    var s4 = vector(yFrontLocal + rLip,     lipTopZ);
    var s5 = vector(yFrontLocal + lipT,     lipTopZ);
    var s6 = vector(yFrontLocal + lipT,     shelfT);
    var s7 = vector(-plateT - 4 * millimeter, shelfT);
    var s8 = vector(-plateT,                  shelfT + 4 * millimeter);

    skLineSegment(shelfSketch, "sBot",      { "start" : s1, "end" : s2 });
    skLineSegment(shelfSketch, "sFront",    { "start" : s2, "end" : s3 });
    skArc(shelfSketch,         "sNoseArc",  { "start" : s3, "mid" : sM, "end" : s4 });
    skLineSegment(shelfSketch, "sLipTop",   { "start" : s4, "end" : s5 });
    skLineSegment(shelfSketch, "sLipBack",  { "start" : s5, "end" : s6 });
    skLineSegment(shelfSketch, "sShelfTop", { "start" : s6, "end" : s7 });
    skLineSegment(shelfSketch, "sRootBev",  { "start" : s7, "end" : s8 });
    skLineSegment(shelfSketch, "sClose",    { "start" : s8, "end" : s1 });
    skSolve(shelfSketch);

    opExtrude(context, id + "extShelfLip", {
            "entities" : qSketchRegion(id + "shelfSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : deskW
    });

    // 3. Full-Width Lower Rear Torsion Spine (Y = cy to cy + 6mm, Z = 0 to 50mm)
    fCuboid(context, id + "rearSpine", {
            "corner1" : vector(cx - halfW, cy,                  0 * millimeter),
            "corner2" : vector(cx + halfW, cy + 6 * millimeter, 50 * millimeter)
    });

    // 4. Union the Main Desk Shell First
    opBoolean(context, id + "unionDeskShell", {
            "tools" : qUnion([
                    qCreatedBy(id + "backPlate", EntityType.BODY),
                    qCreatedBy(id + "extShelfLip", EntityType.BODY),
                    qCreatedBy(id + "rearSpine", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 5. Round the Outer Vertical Edges of the Unified Shell
    var shellBody = qCreatedBy(id + "backPlate", EntityType.BODY);
    try silent
    {
        opFillet(context, id + "filletOuterVerticals", {
            "entities" : qParallelEdges(qOwnedByBody(shellBody, EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 4.5 * millimeter
        });
    }

    // 6. Two Vertical Female Dovetail Guide Housings (X = cx +/- 75mm, Z = 50mm to 180mm)
    var guideHalfW = 16 * millimeter;
    var guideDepth  = 14 * millimeter;
    var addBodies    = [shellBody];
    var gIdx = 0;

    for (var rx in railXOffsets)
    {
        gIdx += 1;
        fCuboid(context, id + ("guideBlock" ~ gIdx), {
                "corner1" : vector(cx + rx - guideHalfW, cy,               50 * millimeter),
                "corner2" : vector(cx + rx + guideHalfW, cy + guideDepth, lowerH)
        });
        addBodies = append(addBodies, qCreatedBy(id + ("guideBlock" ~ gIdx), EntityType.BODY));
    }

    // 7. Dual Left & Right Angled Heavy-Duty Riser Receiver Blocks (at X = cx +/- 75mm)
    var recHalfW = 32 * millimeter;
    var recHeight = 95 * millimeter;
    var baseDepth = 26 * millimeter + (recHeight * sin(deskAngle));

    var rIdx = 0;
    for (var roff in riserXOffsets)
    {
        rIdx += 1;
        var wedgeSk = newSketchOnPlane(context, id + ("wedgeSk" ~ rIdx), {
                "sketchPlane" : plane(vector(cx + roff - recHalfW, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
        });
        var w1 = vector(0 * millimeter, 0 * millimeter);
        var w2 = vector(baseDepth,     0 * millimeter);
        var w3 = vector(baseDepth - (recHeight * sin(deskAngle)), recHeight * cos(deskAngle));
        var w4 = vector(0 * millimeter, recHeight * cos(deskAngle));

        skLineSegment(wedgeSk, "wBot",   { "start" : w1, "end" : w2 });
        skLineSegment(wedgeSk, "wBack",  { "start" : w2, "end" : w3 });
        skLineSegment(wedgeSk, "wTop",   { "start" : w3, "end" : w4 });
        skLineSegment(wedgeSk, "wFront", { "start" : w4, "end" : w1 });
        skSolve(wedgeSk);

        opExtrude(context, id + ("extReceiver" ~ rIdx), {
                "entities" : qSketchRegion(id + ("wedgeSk" ~ rIdx)),
                "direction" : vector(1, 0, 0),
                "endBound" : BoundingType.BLIND,
                "endDepth" : recHalfW * 2
        });
        addBodies = append(addBodies, qCreatedBy(id + ("extReceiver" ~ rIdx), EntityType.BODY));
    }

    // Union Guide Blocks and Dual Receivers onto the Filleted Desk Shell
    opBoolean(context, id + "unionLowerDesk", {
            "tools" : qUnion(addBodies),
            "operationType" : BooleanOperationType.UNION
    });

    // 8. Cut Dual Angled Riser Female Dovetails + Dual Vertical Slider Female Dovetails + Detent Pin Holes
    var uSlide = vector(0, -sin(deskAngle), cos(deskAngle));
    var cornerOrigin = vector(cx, cy + baseDepth, 0 * millimeter) - (8 * millimeter) * uSlide;

    var doveCutSketch = newSketchOnPlane(context, id + "doveCutSketch", {
            "sketchPlane" : plane(cornerOrigin, uSlide, vector(1, 0, 0))
    });

    var dOuter   = 2 * millimeter;
    var dInner   = -(18 * millimeter) - tol;
    var halfNeck = (13.0 * millimeter) + tol;
    var halfWide = (19.0 * millimeter) + tol;

    var dcIdx = 0;
    for (var roff in riserXOffsets)
    {
        dcIdx += 1;
        skLineSegment(doveCutSketch, "dBack" ~ dcIdx,  { "start" : vector(roff - halfNeck, dOuter), "end" : vector(roff + halfNeck, dOuter) });
        skLineSegment(doveCutSketch, "dRight" ~ dcIdx, { "start" : vector(roff + halfNeck, dOuter), "end" : vector(roff + halfWide, dInner) });
        skLineSegment(doveCutSketch, "dFront" ~ dcIdx, { "start" : vector(roff + halfWide, dInner), "end" : vector(roff - halfWide, dInner) });
        skLineSegment(doveCutSketch, "dLeft" ~ dcIdx,  { "start" : vector(roff - halfWide, dInner), "end" : vector(roff - halfNeck, dOuter) });
    }
    skSolve(doveCutSketch);

    opExtrude(context, id + "extDoveCut", {
            "entities" : qSketchRegion(id + "doveCutSketch"),
            "direction" : uSlide,
            "endBound" : BoundingType.BLIND,
            "endDepth" : 79 * millimeter
    });

    var femSlideSk = newSketchOnPlane(context, id + "femSlideSk", {
            "sketchPlane" : plane(vector(cx, cy, 50 * millimeter), vector(0, 0, 1), vector(1, 0, 0))
    });

    var sYOut  = guideDepth + (1 * millimeter);
    var sYIn   = (5 * millimeter) - slideTol;
    var sHNeck = (6 * millimeter) + slideTol;
    var sHWide = (9 * millimeter) + slideTol;

    var lockTools = [];
    var sIdx = 0;
    for (var rx in railXOffsets)
    {
        sIdx += 1;
        skLineSegment(femSlideSk, "fsB" ~ sIdx, { "start" : vector(rx - sHNeck, sYOut), "end" : vector(rx + sHNeck, sYOut) });
        skLineSegment(femSlideSk, "fsR" ~ sIdx, { "start" : vector(rx + sHNeck, sYOut), "end" : vector(rx + sHWide, sYIn) });
        skLineSegment(femSlideSk, "fsF" ~ sIdx, { "start" : vector(rx + sHWide, sYIn),  "end" : vector(rx - sHWide, sYIn) });
        skLineSegment(femSlideSk, "fsL" ~ sIdx, { "start" : vector(rx - sHWide, sYIn),  "end" : vector(rx - sHNeck, sYOut) });

        fCylinder(context, id + ("baseDetentHole" ~ sIdx), {
            "bottomCenter" : vector(cx + rx - guideHalfW - (2 * millimeter), cy + 9.5 * millimeter, 160 * millimeter),
            "topCenter"    : vector(cx + rx + guideHalfW + (2 * millimeter), cy + 9.5 * millimeter, 160 * millimeter),
            "radius"       : m3Clear / 2
        });
        lockTools = append(lockTools, qCreatedBy(id + ("baseDetentHole" ~ sIdx), EntityType.BODY));
    }
    skSolve(femSlideSk);

    opExtrude(context, id + "extFemSlide", {
        "entities" : qSketchRegion(id + "femSlideSk"),
        "direction" : vector(0, 0, 1),
        "endBound" : BoundingType.BLIND,
        "endDepth" : 132 * millimeter
    });

    opBoolean(context, id + "finalLowerCut", {
        "tools" : qUnion(concatenateArrays([
            [
                qCreatedBy(id + "extDoveCut", EntityType.BODY),
                qCreatedBy(id + "extFemSlide", EntityType.BODY)
            ],
            lockTools
        ])),
        "targets" : shellBody,
        "operationType" : BooleanOperationType.SUBTRACTION
    });
}

annotation { "Feature Type Name" : "OnStage Music Desk 3A (Dual Riser)" }
export const onStageMusicDesk3ADual = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3ADualImpl(context, id, definition);
    }
);
