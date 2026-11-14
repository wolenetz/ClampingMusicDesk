// Modular Page Clamps (Unified Side Profile & Tapered Spine)
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function onStagePageClamps(context is Context, id is Id, definition is map)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var m3Clear   = getVar(context, "m3_clear", 3.4 * millimeter);

    var cx = 230 * millimeter;
    var cy = 0 * millimeter;

    var clampW  = 240 * millimeter; // Width of clamps
    var halfW  = clampW / 2;       // 120mm
    var clampH = 6 * millimeter;   // Height of clamp body (6mm thick)
    var clampD = 8 * millimeter;   // Depth of clamp (8mm deep)

    // Positioning the clamps at the front edge of the desk (Y = -6mm to 0mm)
    var yFrontEdge = cy - clampH; // -6mm

    // Create a single large body for all clamps
    fCuboid(context, id + "clampBody", {
            "corner1" : vector(cx - halfW, yFrontEdge, 0 * millimeter),
            "corner2" : vector(cx + halfW, cy,           clampD)
    });

    // Create a sketch for the clamp profile
    var clampSketch = newSketchOnPlane(context, id + "clampSketch", {
            "sketchPlane" : plane(vector(cx - halfW, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the clamp profile
    var c1 = vector(-clampH, 0 * millimeter);         // Bottom-back at backplate
    var c2 = vector(yFrontEdge, 0 * millimeter);      // Bottom-front (flat on print bed)
    var c3 = vector(yFrontEdge, clampH - 1 * millimeter); // Start of top chamfer
    var c4 = vector(yFrontEdge + 1 * millimeter, clampH); // End of top chamfer
    var c5 = vector(-clampH, clampH);                 // Top-back of clamp

    skLineSegment(clampSketch, "cBot", { "start" : c1, "end" : c2 });
    skLineSegment(clampSketch, "cFront", { "start" : c2, "end" : c3 });
    skLineSegment(clampSketch, "cTopChamfer", { "start" : c3, "end" : c4 });
    skLineSegment(clampSketch, "cTop", { "start" : c4, "end" : c5 });
    skLineSegment(clampSketch, "cBack", { "start" : c5, "end" : c1 });
    skSolve(clampSketch);

    // Extrude the clamp profile
    opExtrude(context, id + "extClampProfile", {
            "entities" : qSketchRegion(id + "clampSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : clampW
    });

    // Union the clamp body with the profile
    opBoolean(context, id + "unionClamp", {
            "tools" : qUnion([
                    qCreatedBy(id + "clampBody", EntityType.BODY),
                    qCreatedBy(id + "extClampProfile", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // Create the holes for the M3 screws (2 holes, one on each side)
    var screwHoleSketch = newSketchOnPlane(context, id + "screwHoleSketch", {
            "sketchPlane" : plane(vector(cx - halfW + 10 * millimeter, cy - clampH, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // First hole (left side)
    skCircle(screwHoleSketch, "hole1", {
        "center" : vector(0 * millimeter, 0 * millimeter),
        "radius" : m3Clear / 2
    });

    // Second hole (right side)
    skCircle(screwHoleSketch, "hole2", {
        "center" : vector(clampW - 20 * millimeter, 0 * millimeter),
        "radius" : m3Clear / 2
    });

    skSolve(screwHoleSketch);

    // Extrude the holes
    opExtrude(context, id + "extScrewHoles", {
            "entities" : qSketchRegion(id + "screwHoleSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : clampW
    });

    // Subtract the holes from the clamp body
    opBoolean(context, id + "finalClampCut", {
            "tools" : qUnion([
                    qCreatedBy(id + "extScrewHoles", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "clampBody", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}