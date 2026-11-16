// =========================================================================
// PART 5A & 5B: Rear C-Yoke Bracket & Eccentric Quick-Release Cam Lever
// Hardware per side: 2x M3x10mm (side anchors) + 1x M3x20mm & 1x M3 nut (cam axle)
// =========================================================================
FeatureScript 2411;
import(path : "onshape/std/common.fs", version : "2411.0");

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

annotation { "Feature Type Name" : "OnStage Cam Lock (Part 5A & 5B)" }
export const upperDeskCamLockParts = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        var m3Clear = getVar(context, "m3_clear", 3.4 * millimeter);
        var m3Nut   = getVar(context, "m3_nut",   5.6 * millimeter);

        // Placed at X = 835mm, flat on Z = 0mm for support-free printing
        var cx = 835 * millimeter;
        var cy = 0 * millimeter;
        var yokeH = 14 * millimeter; // 14mm tall collar (Z = 0 to 14mm)

        // =================================================================
        // PART 5A: Rear C-Yoke Bracket (Sized for 32.0mm wide Part 3A guide)
        // =================================================================
        // Local Y corresponds to Part 3A's Y-axis (Y = 9.5mm is side hole center, Y = 14mm is rear of 3A)
        var inHalfW   = 16.25 * millimeter; // 32.5mm inner span (0.25mm per-side clearance over 32.0mm Part 3A guide)
        var outHalfW  = 20.75 * millimeter; // 4.5mm thick side arms (M3x10mm screw reaches 5.25mm into 3A's 8.3mm wall)
        var earOutX   = 9.0 * millimeter;   // 18mm total rear cam-ear span (fits M3x20mm axle screw + M3 nut)
        var slotHalfX = 5.25 * millimeter;  // 10.5mm center clevis slot for the 9.8mm wide cam lever

        var yokeSk = newSketchOnPlane(context, id + "yokeSk", {
                "sketchPlane" : plane(vector(cx, cy, 0 * millimeter), vector(0, 0, 1), vector(1, 0, 0))
        });

        var p1  = vector(-outHalfW,  4.0 * millimeter);
        var p2  = vector(-inHalfW,   4.0 * millimeter);
        var p3  = vector(-inHalfW,  14.2 * millimeter);
        var p4  = vector(-slotHalfX, 14.2 * millimeter);
        var p5  = vector(-slotHalfX, 25.0 * millimeter);
        var p6  = vector(-earOutX,  25.0 * millimeter);
        var p7  = vector(-earOutX,  18.5 * millimeter);
        var p8  = vector(-outHalfW, 14.2 * millimeter);

        var p9  = vector( outHalfW, 14.2 * millimeter);
        var p10 = vector( earOutX,  18.5 * millimeter);
        var p11 = vector( earOutX,  25.0 * millimeter);
        var p12 = vector( slotHalfX, 25.0 * millimeter);
        var p13 = vector( slotHalfX, 14.2 * millimeter);
        var p14 = vector( inHalfW,  14.2 * millimeter);
        var p15 = vector( inHalfW,   4.0 * millimeter);
        var p16 = vector( outHalfW,  4.0 * millimeter);

        // Top tie-arch connects the two rear ears over the cam slot from Z = 11mm to 14mm,
        // so we first extrude the full C-yoke outline and then cut the center cam slot from Z = 0 to 14mm
        skLineSegment(yokeSk, "yL1",  { "start" : p1,  "end" : p2 });
        skLineSegment(yokeSk, "yL2",  { "start" : p2,  "end" : p3 });
        skLineSegment(yokeSk, "yL3",  { "start" : p3,  "end" : p4 });
        skLineSegment(yokeSk, "yL4",  { "start" : p4,  "end" : p5 });
        skLineSegment(yokeSk, "yL5",  { "start" : p5,  "end" : p6 });
        skLineSegment(yokeSk, "yL6",  { "start" : p6,  "end" : p7 });
        skLineSegment(yokeSk, "yL7",  { "start" : p7,  "end" : p8 });
        skLineSegment(yokeSk, "yL8",  { "start" : p8,  "end" : p1 });

        skLineSegment(yokeSk, "yR1",  { "start" : p9,  "end" : p10 });
        skLineSegment(yokeSk, "yR2",  { "start" : p10, "end" : p11 });
        skLineSegment(yokeSk, "yR3",  { "start" : p11, "end" : p12 });
        skLineSegment(yokeSk, "yR4",  { "start" : p12, "end" : p13 });
        skLineSegment(yokeSk, "yR5",  { "start" : p13, "end" : p14 });
        skLineSegment(yokeSk, "yR6",  { "start" : p14, "end" : p15 });
        skLineSegment(yokeSk, "yR7",  { "start" : p15, "end" : p16 });
        skLineSegment(yokeSk, "yR8",  { "start" : p16, "end" : p9 });
        skSolve(yokeSk);

        opExtrude(context, id + "extYokeHalves", {
                "entities" : qSketchRegion(id + "yokeSk"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : yokeH
        });

        // Connect the left and right halves with a solid 3.0mm base bridge (Z = 0 to 3.0mm) at the back
        fCuboid(context, id + "yokeBridge", {
                "corner1" : vector(cx - earOutX, cy + 14.2 * millimeter, 0 * millimeter),
                "corner2" : vector(cx + earOutX, cy + 25.0 * millimeter, 3.0 * millimeter)
        });

        opBoolean(context, id + "unionYoke", {
                "tools" : qUnion([
                        qCreatedBy(id + "extYokeHalves", EntityType.BODY),
                        qCreatedBy(id + "yokeBridge", EntityType.BODY)
                ]),
                "operationType" : BooleanOperationType.UNION
        });

        // Cut the 2x M3x10mm side trunnion holes at Y = 9.5mm, Z = 8.5mm
        // and the 1x M3x20mm rear cam axle hole at Y = 20.0mm, Z = 8.5mm
        var holeZ = 8.5 * millimeter;
        fCylinder(context, id + "sideAnchorBore", {
                "bottomCenter" : vector(cx - 25 * millimeter, cy + 9.5 * millimeter, holeZ),
                "topCenter"    : vector(cx + 25 * millimeter, cy + 9.5 * millimeter, holeZ),
                "radius"       : m3Clear / 2
        });

        fCylinder(context, id + "camAxleBore", {
                "bottomCenter" : vector(cx - 12 * millimeter, cy + 20.0 * millimeter, holeZ),
                "topCenter"    : vector(cx + 12 * millimeter, cy + 20.0 * millimeter, holeZ),
                "radius"       : m3Clear / 2
        });

        // Captured M3 Hex Nut Pocket on the Right Rear Ear (X = +6.5mm to +9.5mm)
        var nutSk = newSketchOnPlane(context, id + "nutSk", {
                "sketchPlane" : plane(vector(cx + 6.5 * millimeter, cy + 20.0 * millimeter, holeZ), vector(1, 0, 0), vector(0, 1, 0))
        });
        skRegularPolygon(nutSk, "m3Hex", {
                "center" : vector(0 * millimeter, 0 * millimeter),
                "firstVertex" : vector(0 * millimeter, m3Nut / (2 * cos(30 * degree))),
                "sides" : 6
        });
        skSolve(nutSk);

        opExtrude(context, id + "extAxleNut", {
                "entities" : qSketchRegion(id + "nutSk"),
                "direction" : vector(1, 0, 0),
                "endBound" : BoundingType.BLIND,
                "endDepth" : 3.5 * millimeter
        });

        opBoolean(context, id + "cutYokeHoles", {
                "tools" : qUnion([
                        qCreatedBy(id + "sideAnchorBore", EntityType.BODY),
                        qCreatedBy(id + "camAxleBore", EntityType.BODY),
                        qCreatedBy(id + "extAxleNut", EntityType.BODY)
                ]),
                "targets" : qCreatedBy(id + "extYokeHalves", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION
        });

        setProperty(context, {
                "entities" : qCreatedBy(id + "extYokeHalves", EntityType.BODY),
                "propertyType" : PropertyType.NAME,
                "value" : "Part 5A - Rear Cam-Lock Yoke"
        });

// =================================================================
        // PART 5B: Eccentric Quick-Release Cam Lever (Prints flat next to 5A)
        // =================================================================
        // Keeps original 9.8mm thickness & m3Clear bore, while fixing:
        // 1. Bridge Clearance: Bottom radius kept <= 4.9mm AND lever handle routed
        //    outside X = +5.5mm (Y > 25.5mm on 5A) so it NEVER hits 5A's Z = 3.0mm bridge!
        // 2. Runner Engagement: Cam nose extended to 6.80mm flat + 7.55mm dual teeth
        //    so it protrudes 1.75mm out the front of 5A to bite & wedge Part 3B's runner!
        var lcx = cx + 45 * millimeter;
        var lcy = cy + 15 * millimeter;
        var camThick = 9.8 * millimeter; // Original snug width inside 5A's 10.5mm slot

        var camSk = newSketchOnPlane(context, id + "camSk", {
                "sketchPlane" : plane(vector(lcx, lcy, 0 * millimeter), vector(0, 0, 1), vector(1, 0, 0))
        });

        // Cam profile around axle origin (0, 0):
        // -X points forward toward Part 3B's runner (axle to 5A front face is 5.8mm; axle to seated runner is 6.55mm).
        // -Y points down toward Part 5A's bridge (axle to bridge top face is 5.5mm, back edge of bridge is at +5.0mm).
        var c1  = vector(-3.50 * millimeter, -3.40 * millimeter); // R = 4.88mm (Clears 5.5mm bridge floor!)
        var c2  = vector(-5.80 * millimeter, -2.00 * millimeter); // Eccentric lead-in ramp
        var c3  = vector(-6.80 * millimeter, -0.75 * millimeter); // Lower wedging flat (6.80mm reach > 6.55mm seated runner!)
        var c4  = vector(-7.55 * millimeter,  0.00 * millimeter); // Tooth #1 tip (7.55mm reach bites full depth into 3B groove!)
        var c5  = vector(-6.80 * millimeter,  0.75 * millimeter); // Inter-tooth flat
        var c6  = vector(-6.80 * millimeter,  1.75 * millimeter); // Tooth #2 base
        var c7  = vector(-7.55 * millimeter,  2.50 * millimeter); // Tooth #2 tip (matches 2.5mm groove pitch on Part 3B)
        var c8  = vector(-6.80 * millimeter,  3.25 * millimeter); // Upper wedging flat
        var c9  = vector(-5.00 * millimeter,  4.80 * millimeter); // Top shoulder
        var c10 = vector( 9.50 * millimeter,  4.80 * millimeter); // Steps out past Part 5A's rear bridge edge (X > 5.0mm)!
        var c11 = vector( 9.50 * millimeter, -26.0 * millimeter); // Outer back edge of thumb handle
        var c12 = vector( 5.50 * millimeter, -26.0 * millimeter); // Inner front edge of thumb handle (clears X = 5.0mm bridge!)
        var c13 = vector( 5.50 * millimeter, -4.80 * millimeter); // Clears the (X = +5.0mm, Y = -5.5mm) rear corner of 5A's bridge!
        var c14 = vector( 0.00 * millimeter, -4.80 * millimeter); // Unlocked bottom face (4.80mm < 5.50mm bridge floor!)

        skLineSegment(camSk, "cL1",  { "start" : c1,  "end" : c2 });
        skLineSegment(camSk, "cL2",  { "start" : c2,  "end" : c3 });
        skLineSegment(camSk, "cL3",  { "start" : c3,  "end" : c4 });
        skLineSegment(camSk, "cL4",  { "start" : c4,  "end" : c5 });
        skLineSegment(camSk, "cL5",  { "start" : c5,  "end" : c6 });
        skLineSegment(camSk, "cL6",  { "start" : c6,  "end" : c7 });
        skLineSegment(camSk, "cL7",  { "start" : c7,  "end" : c8 });
        skLineSegment(camSk, "cL8",  { "start" : c8,  "end" : c9 });
        skLineSegment(camSk, "cL9",  { "start" : c9,  "end" : c10 });
        skLineSegment(camSk, "cL10", { "start" : c10, "end" : c11 });
        skLineSegment(camSk, "cL11", { "start" : c11, "end" : c12 });
        skLineSegment(camSk, "cL12", { "start" : c12, "end" : c13 });
        skLineSegment(camSk, "cL13", { "start" : c13, "end" : c14 });
        skLineSegment(camSk, "cL14", { "start" : c14, "end" : c1 });
        skSolve(camSk);

        opExtrude(context, id + "extCamLever", {
                "entities" : qSketchRegion(id + "camSk"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : camThick
        });

        // Cut the M3 axle pivot hole through the cam center (lcx, lcy)
        fCylinder(context, id + "camPivotHole", {
                "bottomCenter" : vector(lcx, lcy, -1 * millimeter),
                "topCenter"    : vector(lcx, lcy, camThick + 1 * millimeter),
                "radius"       : m3Clear / 2
        });

        opBoolean(context, id + "cutCamPivot", {
                "tools" : qCreatedBy(id + "camPivotHole", EntityType.BODY),
                "targets" : qCreatedBy(id + "extCamLever", EntityType.BODY),
                "operationType" : BooleanOperationType.SUBTRACTION
        });

        setProperty(context, {
                "entities" : qCreatedBy(id + "extCamLever", EntityType.BODY),
                "propertyType" : PropertyType.NAME,
                "value" : "Part 5B - Eccentric Cam Lever"
        });
    });