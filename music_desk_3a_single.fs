// =========================================================================
// PART 3A: Lower Stationary Music Desk (Unified Side Profile & Tapered Spine)
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

function onStageMusicDesk3AImpl(context is Context, id is Id, definition is map, partName is string)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var deskAngle = getVar(context, "desk_angle", 15 * degree);
    var m3Clear   = getVar(context, "m3_clear", 3.4 * millimeter);

    var slideTol  = tol + (0.15 * millimeter);

    var cx = 230 * millimeter;
    var cy = 0 * millimeter;

    var deskW  = 240 * millimeter;
    var halfW  = deskW / 2;       // 120mm
    var lowerH = 180 * millimeter; // Height of Part 3A lower plate
    var plateT = 6 * millimeter;   // 6mm rigid main backplate
    var shelfT = 8 * millimeter;   // 8mm bottom shelf
    var shelfD = 48 * millimeter;  // 48mm inside book-resting depth
    var lipT   = 5 * millimeter;   // 5mm front retention lip thickness
    var lipH   = 8 * millimeter;   // 8mm upward front retention lip

    var yFrontEdge   = cy - plateT - shelfD - lipT; // -59mm
    var railXOffsets = [-75 * millimeter, 75 * millimeter];

    // 1. Lower Vertical Backing Plate (Y = -6mm to 0mm, Z = 0 to 180mm)
    fCuboid(context, id + "backPlate", {
            "corner1" : vector(cx - halfW, cy - plateT, 0 * millimeter),
            "corner2" : vector(cx + halfW, cy,           lowerH)
    });

    // 2. Continuous Profiled Book Shelf + Front Lip + 4mm Inside Root Chamfer in ONE Sketch!
    // Note: Sketch plane X-axis is vector(0, 1, 0) [Global Y] and Y-axis is vector(0, 0, 1) [Global Z]
    var shelfSketch = newSketchOnPlane(context, id + "shelfSketch", {
            "sketchPlane" : plane(vector(cx - halfW, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    var lipTopZ = shelfT + lipH;         // 16mm
    var rLip     = 2.4 * millimeter;        // Smooth 2.4mm front-top bullnose radius on the 5mm lip
    var arcOff   = rLip * (1 - cos(45 * degree)); // Exact 45-deg point on circle (~0.703mm from corner)

    var s1 = vector(-plateT,                  0 * millimeter);         // Bottom-back at backplate
    var s2 = vector(yFrontEdge,              0 * millimeter);         // Bottom-front (flat on print bed)
    var s3 = vector(yFrontEdge,              lipTopZ - rLip);      // Start of front-top lip curve
    var sM = vector(yFrontEdge + arcOff,    lipTopZ - arcOff);    // Exact tangent arc midpoint
    var s4 = vector(yFrontEdge + rLip,      lipTopZ);              // End of front-top lip curve
    var s5 = vector(yFrontEdge + lipT,      lipTopZ);              // Top-back of lip
    var s6 = vector(yFrontEdge + lipT,      shelfT);                // Inside bottom of lip
    var s7 = vector(-plateT - 4 * millimeter, shelfT);                // Start of 4mm inside root chamfer
    var s8 = vector(-plateT,                  shelfT + 4 * millimeter); // Top of 4mm inside root chamfer

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

    // 3. Full-Width Lower Rear Torsion Spine (Y = 0 to +6mm, Z = 0 to 50mm)
    // Making it full-width (cx - halfW to cx + halfW) makes its left/right faces 100% coplanar with
    // backPlate and extShelfLip, so when unioned below, they merge into ONE flat side face with zero steps!
    fCuboid(context, id + "rearSpine", {
            "corner1" : vector(cx - halfW, cy,                  0 * millimeter),
            "corner2" : vector(cx + halfW, cy + 6 * millimeter, 50 * millimeter)
    });

    // 4. Union the Main Desk Shell First (Merges Backplate + Shelf/Lip + Rear Spine into a single continuous body)
    opBoolean(context, id + "unionDeskShell", {
            "tools" : qUnion([
                    qCreatedBy(id + "backPlate", EntityType.BODY),
                    qCreatedBy(id + "extShelfLip", EntityType.BODY),
                    qCreatedBy(id + "rearSpine", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 5. Round the Outer Vertical Edges of the Unified Shell!
    // Because the 3 parts are already unioned, Onshape rounds the TRUE outer perimeter corners (at Y = -59mm, Y = 0mm, Y = +6mm)
    // so the rear spine tapers smoothly into the backplate edge with zero internal pinches!
    var shellBody = qCreatedBy(id + "backPlate", EntityType.BODY);
    try silent
    {
        opFillet(context, id + "filletOuterVerticals", {
            "entities" : qParallelEdges(qOwnedByBody(shellBody, EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 4.5 * millimeter
        });
    }

    // 6. Two Vertical Female Dovetail Guide Housings (X = +/-75mm, Z = 50mm to 180mm)
    var guideHalfW = 16 * millimeter;
    var guideDepth  = 14 * millimeter;
    var guideBodies  = [];
    var gIdx = 0;

    for (var rx in railXOffsets)
    {
        gIdx += 1;
        fCuboid(context, id + ("guideBlock" ~ gIdx), {
                "corner1" : vector(cx + rx - guideHalfW, cy,               50 * millimeter),
                "corner2" : vector(cx + rx + guideHalfW, cy + guideDepth, lowerH)
        });
        guideBodies = append(guideBodies, qCreatedBy(id + ("guideBlock" ~ gIdx), EntityType.BODY));
    }

    // 7. Center Angled Heavy-Duty Riser Receiver Block (64mm wide x 95mm tall housing)
    var recHalfW = 32 * millimeter;
    var recHeight = 95 * millimeter;
    var baseDepth = 26 * millimeter + (recHeight * sin(deskAngle));

    var wedgeSketch = newSketchOnPlane(context, id + "wedgeSketch", {
            "sketchPlane" : plane(vector(cx - recHalfW, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });
    var w1 = vector(0 * millimeter, 0 * millimeter);
    var w2 = vector(baseDepth,     0 * millimeter);
    var w3 = vector(baseDepth - (recHeight * sin(deskAngle)), recHeight * cos(deskAngle));
    var w4 = vector(0 * millimeter, recHeight * cos(deskAngle));

    skLineSegment(wedgeSketch, "wBot",   { "start" : w1, "end" : w2 });
    skLineSegment(wedgeSketch, "wBack",  { "start" : w2, "end" : w3 });
    skLineSegment(wedgeSketch, "wTop",   { "start" : w3, "end" : w4 });
    skLineSegment(wedgeSketch, "wFront", { "start" : w4, "end" : w1 });
    skSolve(wedgeSketch);

    opExtrude(context, id + "extReceiver", {
            "entities" : qSketchRegion(id + "wedgeSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : recHalfW * 2
    });

    // Union Guide Blocks and Center Receiver onto the Filleted Desk Shell
    opBoolean(context, id + "unionLowerDesk", {
            "tools" : qUnion(concatenateArrays([
                    [
                        shellBody,
                        qCreatedBy(id + "extReceiver", EntityType.BODY)
                    ],
                    guideBodies
            ])),
            "operationType" : BooleanOperationType.UNION
    });

    // 8. Cut Center Angled Riser Female Dovetail + Dual Vertical Slider Female Dovetails + Detent Pin Holes
    var uSlide = vector(0, -sin(deskAngle), cos(deskAngle));
    var cornerOrigin = vector(cx, baseDepth, 0 * millimeter) - (8 * millimeter) * uSlide;

    var doveCutSketch = newSketchOnPlane(context, id + "doveCutSketch", {
            "sketchPlane" : plane(cornerOrigin, uSlide, vector(1, 0, 0))
    });

    var dOuter   = 2 * millimeter;
    var dInner   = -(18 * millimeter) - tol;
    var halfNeck = (13.0 * millimeter) + tol;
    var halfWide = (19.0 * millimeter) + tol;

    skLineSegment(doveCutSketch, "dBack",  { "start" : vector(-halfNeck, dOuter), "end" : vector( halfNeck, dOuter) });
    skLineSegment(doveCutSketch, "dRight", { "start" : vector( halfNeck, dOuter), "end" : vector( halfWide, dInner) });
    skLineSegment(doveCutSketch, "dFront", { "start" : vector( halfWide, dInner), "end" : vector(-halfWide, dInner) });
    skLineSegment(doveCutSketch, "dLeft",  { "start" : vector(-halfWide, dInner), "end" : vector(-halfNeck, dOuter) });
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

    // Name it for easier export.
    setProperty(context, {
        "entities" : shellBody,
        "propertyType" : PropertyType.NAME,
        "value" : partName
    });
}

annotation { "Feature Type Name" : "OnStage Music Desk 3A (Single Riser)" }
export const onStageMusicDesk3A = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3AImpl(context, id, definition, "Part 3A-Single - Single Riser Lower Music Desk");
    }
);
