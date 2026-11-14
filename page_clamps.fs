// Modular Page Clamps (Unified Side Profile & Tapered Spine) - Full Implementation
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

// Helper function to cut the matching Upper Dovetail Rail profile + tolerance
function makeRailCutout(context is Context, id is Id, xStart is ValueWithUnits, width is ValueWithUnits, tol is ValueWithUnits) returns Query
{
    var rSketch = newSketchOnPlane(context, id + "rSketch", {
            "sketchPlane" : plane(vector(xStart - 1 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Matches Part 3's extUpperRail (Y = 0 to 14mm, Z = 140 to 168mm) expanded by tol
    var u1 = vector(-1 * millimeter,        144 * millimeter - tol);
    var u2 = vector( 6 * millimeter - tol,  144 * millimeter - tol);
    var u3 = vector( 6 * millimeter - tol,  140 * millimeter - tol);
    var u4 = vector(14 * millimeter + tol,  144 * millimeter - tol);
    var u5 = vector(14 * millimeter + tol,  164 * millimeter + tol);
    var u6 = vector( 6 * millimeter - tol,  168 * millimeter + tol);
    var u7 = vector( 6 * millimeter - tol,  164 * millimeter + tol);
    var u8 = vector(-1 * millimeter,        164 * millimeter + tol);

    skLineSegment(rSketch, "rL1", { "start" : u1, "end" : u2 });
    skLineSegment(rSketch, "rL2", { "start" : u2, "end" : u3 });
    skLineSegment(rSketch, "rL3", { "start" : u3, "end" : u4 });
    skLineSegment(rSketch, "rL4", { "start" : u4, "end" : u5 });
    skLineSegment(rSketch, "rL5", { "start" : u5, "end" : u6 });
    skLineSegment(rSketch, "rL6", { "start" : u6, "end" : u7 });
    skLineSegment(rSketch, "rL7", { "start" : u7, "end" : u8 });
    skLineSegment(rSketch, "rL8", { "start" : u8, "end" : u1 });
    skSolve(rSketch);

    opExtrude(context, id + "extRailCut", {
            "entities" : qSketchRegion(id + "rSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : width + 2 * millimeter
    });

    return qCreatedBy(id + "extRailCut", EntityType.BODY);
}

function onStagePageClampsImpl(context is Context, id is Id, definition is map)
{
    var tol      = getVar(context, "tol", 0.3 * millimeter);
    var m3Clear = getVar(context, "m3_clear", 3.4 * millimeter);
    var m3Nut   = getVar(context, "m3_nut", 5.6 * millimeter);

    // Placed starting at X = 380mm (cleanly to the right of Part 3's X = 350mm edge)
    // All variants are generated at their true Z-height (around Z = 140..180mm) to show exact desk alignment!

    // =====================================================================
    // VARIANT A: Thin Sheet "Swan-Neck" Leaf Spring (X = 380mm, Width = 16mm)
    // =====================================================================
    // 1-piece compliant spring that kisses the front face of the desk (Y = -6mm)
    // with a ski-tip bottom so single sheets slide underneath effortlessly.
    var xA = 660 * millimeter;
    var wA = 16 * millimeter;

    var skA = newSketchOnPlane(context, id + "skA", {
            "sketchPlane" : plane(vector(xA, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Outer & inner loop tracing the carriage + overhead arch + 2mm thick flexible leaf + ski-tip
    var aPts = [
        vector(  0 * millimeter, 135 * millimeter), // 1: Carriage bottom-front (flush with Y=0 backplate)
        vector( 20 * millimeter, 135 * millimeter), // 2: Carriage bottom-back
        vector( 20 * millimeter, 189 * millimeter), // 3: Carriage top-back
        vector(-12 * millimeter, 189 * millimeter), // 4: Arch forward over the 6mm desk plate
        vector( -5.5 * millimeter, 148 * millimeter), // 5: Leaf spring pre-load contact point (presses against Y=-6mm face!)
        vector(-14 * millimeter, 136 * millimeter), // 6: Ski-tip flare bottom-front
        vector(-12 * millimeter, 135 * millimeter), // 7: Ski-tip bottom edge
        vector( -3.5 * millimeter, 148 * millimeter), // 8: Inside of contact knee (2mm spring thickness)
        vector( -9.5 * millimeter, 185 * millimeter), // 9: Inside of top arch
        vector( -6 * millimeter - tol, 185 * millimeter), // 10: Clears front of desk top edge (Y=-6mm)
        vector( -6 * millimeter - tol, 180 * millimeter + tol), // 11: Rests on top of desk (Z=180mm)
        vector(  0 * millimeter, 180 * millimeter + tol)  // 12: Back of desk top edge (Y=0mm)
    ];

    for (var i = 0; i < size(aPts); i += 1)
    {
        var nextIdx = (i + 1) % size(aPts);
        skLineSegment(skA, "aL" ~ i, { "start" : aPts[i], "end" : aPts[nextIdx] });
    }
    skSolve(skA);

    opExtrude(context, id + "extA", {
            "entities" : qSketchRegion(id + "skA"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : wA
    });

    var cutA = makeRailCutout(context, id + "cutA", xA, wA, tol);
    opBoolean(context, id + "subA", {
            "tools" : cutA,
            "targets" : qCreatedBy(id + "extA", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });

    // =====================================================================
    // VARIANT B: Medium Book Flip-UP Circular Knuckle Hinge (X = 695mm & 735mm)
    // =====================================================================
    // Coaxial circular knuckles on both B1 and B2 eliminate all square-corner
    // interference so the Batwing Finger rotates smoothly through 180+ degrees!
    var xB1  = 695 * millimeter;
    var wB1  = 24 * millimeter;
    var pivY = 7 * millimeter;
    var pivZ = 191 * millimeter; // Raised 1mm so a 7mm radius knuckle is tangent to Z = 198mm!

    // 1. Base Rail Carriage (Z = 135mm to 183mm, leaving 8mm clearance below the Z = 191mm pivot!)
    var skB1 = newSketchOnPlane(context, id + "skB1", {
            "sketchPlane" : plane(vector(xB1, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    var bPts = [
        vector( 0 * millimeter, 135 * millimeter),
        vector(20 * millimeter, 135 * millimeter),
        vector(20 * millimeter, 183 * millimeter),
        vector(-6 * millimeter, 183 * millimeter),
        vector(-6 * millimeter, 180 * millimeter + tol),
        vector( 0 * millimeter, 180 * millimeter + tol)
    ];

    for (var i = 0; i < size(bPts); i += 1)
    {
        var nextIdx = (i + 1) % size(bPts);
        skLineSegment(skB1, "bL" ~ i, { "start" : bPts[i], "end" : bPts[nextIdx] });
    }
    skSolve(skB1);

    opExtrude(context, id + "extB1", {
            "entities" : qSketchRegion(id + "skB1"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : wB1
    });

    // 2. Circular Center Hinge Turret on Top of B1 (14mm wide, 7mm radius drum + lower pedestal)
    fCuboid(context, id + "b1TurretBase", {
            "corner1" : vector(xB1 + 5 * millimeter,  0 * millimeter, 182 * millimeter),
            "corner2" : vector(xB1 + 19 * millimeter, 14 * millimeter, pivZ)
    });
    fCylinder(context, id + "b1TurretCyl", {
            "bottomCenter" : vector(xB1 + 5 * millimeter,  pivY, pivZ),
            "topCenter"    : vector(xB1 + 19 * millimeter, pivY, pivZ),
            "radius"       : 7 * millimeter
    });

    opBoolean(context, id + "unionB1", {
            "tools" : qUnion([
                    qCreatedBy(id + "extB1", EntityType.BODY),
                    qCreatedBy(id + "b1TurretBase", EntityType.BODY),
                    qCreatedBy(id + "b1TurretCyl", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    var cutB1 = makeRailCutout(context, id + "cutB1", xB1, wB1, tol);

    // Horizontal M3 Hinge Hole along X-axis through the Circular Turret
    fCylinder(context, id + "b1Hole", {
            "bottomCenter" : vector(xB1,       pivY, pivZ),
            "topCenter"    : vector(xB1 + wB1, pivY, pivZ),
            "radius"       : m3Clear / 2
    });

    opBoolean(context, id + "subB1", {
            "tools" : qUnion([
                    cutB1,
                    qCreatedBy(id + "b1Hole", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "extB1", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });

    // =====================================================================
    // Part B2: Circular-Knuckle Clevis Batwing Finger (Offset to X = 735mm)
    // =====================================================================
    // Uses 6.5mm radius circular ears + tangent top webs so it rotates with 1.5mm
    // radial clearance above B1's floor AND prints upside-down on a flat Z = 198mm face!
    var xB2        = 735 * millimeter;
    var clevisGap = 14 * millimeter + (2 * tol); // 14.6mm inner gap to straddle the 14mm B1 turret
    var earW      = 4.5 * millimeter;            // 4.5mm thick left and right hinge ears
    var halfGap   = clevisGap / 2;              // 7.3mm
    var earRad    = 6.5 * millimeter;            // 6.5mm radius (leaves 1.5mm clearance above Z = 183mm step!)

    // 1. Left & Right Circular Hinge Drums (Coaxial with pivY = 7mm, pivZ = 191mm)
    fCylinder(context, id + "b2EarCylL", {
            "bottomCenter" : vector(xB2 - halfGap - earW, pivY, pivZ),
            "topCenter"    : vector(xB2 - halfGap,         pivY, pivZ),
            "radius"       : earRad
    });
    fCylinder(context, id + "b2EarCylR", {
            "bottomCenter" : vector(xB2 + halfGap,         pivY, pivZ),
            "topCenter"    : vector(xB2 + halfGap + earW, pivY, pivZ),
            "radius"       : earRad
    });

    // 2. Tangent Upper Ear Webs (Connects the circular drums forward to Y = -1mm, flush at Z = 198mm for flat printing)
    fCuboid(context, id + "b2EarWebL", {
            "corner1" : vector(xB2 - halfGap - earW, -1 * millimeter, pivZ),
            "corner2" : vector(xB2 - halfGap,         pivY,            198 * millimeter)
    });
    fCuboid(context, id + "b2EarWebR", {
            "corner1" : vector(xB2 + halfGap,         -1 * millimeter, pivZ),
            "corner2" : vector(xB2 + halfGap + earW, pivY,            198 * millimeter)
    });

    // 3. Forward Reach Bridge (Starts at Y = -1mm -> 8mm away from pivot so it never touches the 7mm radius B1 turret!)
    fCuboid(context, id + "b2Arm", {
            "corner1" : vector(xB2 - halfGap - earW, -30 * millimeter, 192 * millimeter),
            "corner2" : vector(xB2 + halfGap + earW,  -1 * millimeter, 198 * millimeter)
    });

    // 4. Downward Page-Holding Batwing Paddle at Y = -30mm to -25mm (50mm wide x 36mm tall, Z = 162..198mm)
    fCuboid(context, id + "b2Paddle", {
            "corner1" : vector(xB2 - 25 * millimeter, -30 * millimeter, 162 * millimeter),
            "corner2" : vector(xB2 + 25 * millimeter, -25 * millimeter, 198 * millimeter)
    });

    opBoolean(context, id + "unionB2", {
            "tools" : qUnion([
                    qCreatedBy(id + "b2EarCylL", EntityType.BODY),
                    qCreatedBy(id + "b2EarCylR", EntityType.BODY),
                    qCreatedBy(id + "b2EarWebL", EntityType.BODY),
                    qCreatedBy(id + "b2EarWebR", EntityType.BODY),
                    qCreatedBy(id + "b2Arm", EntityType.BODY),
                    qCreatedBy(id + "b2Paddle", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 5. Horizontal M3 Hinge Hole + Captured Hex Nut Trap in Right Ear (at Y = +7mm, Z = 191mm)
    fCylinder(context, id + "b2PinHole", {
            "bottomCenter" : vector(xB2 - 15 * millimeter, pivY, pivZ),
            "topCenter"    : vector(xB2 + 15 * millimeter, pivY, pivZ),
            "radius"       : m3Clear / 2
    });

    var b2NutSk = newSketchOnPlane(context, id + "b2NutSk", {
            "sketchPlane" : plane(vector(xB2 + halfGap + earW, pivY, pivZ), vector(1, 0, 0), vector(0, 1, 0))
    });
    skRegularPolygon(b2NutSk, "b2Hex", {
            "center" : vector(0 * millimeter, 0 * millimeter),
            "firstVertex" : vector(0 * millimeter, m3Nut / (2 * cos(30 * degree))),
            "sides" : 6
    });
    skSolve(b2NutSk);

    opExtrude(context, id + "extB2Nut", {
            "entities" : qSketchRegion(id + "b2NutSk"),
            "direction" : vector(-1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 2.5 * millimeter
    });

    opBoolean(context, id + "subB2", {
            "tools" : qUnion([
                    qCreatedBy(id + "b2PinHole", EntityType.BODY),
                    qCreatedBy(id + "extB2Nut", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "b2EarCylL", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });


    // =====================================================================
    // VARIANT C: Thick & Short/Tall Book Deep-Reach S-Spring (X = 495mm)
    // =====================================================================
    // 42mm deep throat reach (to Y = -48mm) with a high-flex C-bow and a
    // deep 65mm vertical plunge (down to Z = 115mm) so it catches BOTH short hymnals & tall thick books!
    var xC = 775 * millimeter;
    var wC = 22 * millimeter;

    var skC = newSketchOnPlane(context, id + "skC", {
            "sketchPlane" : plane(vector(xC, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // 2.8mm thick compliant C-bow spring rising to Z = 212mm and plunging to Z = 115mm
    var cPts = [
        vector(  0 * millimeter,   135 * millimeter), // 1: Carriage bottom-front
        vector( 20 * millimeter,   135 * millimeter), // 2: Carriage bottom-back
        vector( 20 * millimeter,   212 * millimeter), // 3: High-arch spring back (rises 32mm above desk for tall books!)
        vector(-50 * millimeter,   212 * millimeter), // 4: Deep 44mm throat reach forward over thick spines
        vector(-44 * millimeter,   125 * millimeter), // 5: Plunges down to Z=125mm (catches short 5-inch books!)
        vector(-53 * millimeter,   115 * millimeter), // 6: Forward easy-entry ski-tip flare
        vector(-50.2 * millimeter, 114 * millimeter), // 7: Ski-tip bottom edge
        vector(-41 * millimeter,   125 * millimeter), // 8: Inside of lower knee (3mm thick spring wall)
        vector(-46.8 * millimeter, 208.8 * millimeter), // 9: Inside of front top corner (3.2mm spring wall)
        vector( 16.8 * millimeter, 208.8 * millimeter), // 10: Inside of rear top corner
        vector( 16.8 * millimeter, 184 * millimeter), // 11: Flex relief step behind carriage
        vector( -6 * millimeter - tol, 184 * millimeter), // 12: Clears front of desk top edge
        vector( -6 * millimeter - tol, 180 * millimeter + tol), // 13: Rests on top of desk
        vector(  0 * millimeter,   180 * millimeter + tol)  // 14: Back of desk top edge
    ];

    for (var i = 0; i < size(cPts); i += 1)
    {
        var nextIdx = (i + 1) % size(cPts);
        skLineSegment(skC, "cL" ~ i, { "start" : cPts[i], "end" : cPts[nextIdx] });
    }
    skSolve(skC);

    opExtrude(context, id + "extC", {
            "entities" : qSketchRegion(id + "skC"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : wC
    });

    var cutC = makeRailCutout(context, id + "cutC", xC, wC, tol);
    opBoolean(context, id + "subC", {
            "tools" : cutC,
            "targets" : qCreatedBy(id + "extC", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}

// Export the feature so it appears in OnShape with proper annotations
annotation { "Feature Type Name" : "OnStage Page Clamps" }
export const onStagePageClamps = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStagePageClampsImpl(context, id, definition);
    }
);