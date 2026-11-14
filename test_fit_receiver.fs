// Test Fit Dovetail Receiver - Unified Implementation
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function testFitReceiver(context is Context, id is Id, definition is map)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var m3Clear   = getVar(context, "m3_clear", 3.4 * millimeter);

    // Test fit receiver dimensions
    var receiverWidth = 20 * millimeter;
    var receiverHeight = 14 * millimeter;
    var receiverDepth = 15 * millimeter;

    // Positioning the test fit receiver at the center of the desk (X = 230mm)
    var cx = 230 * millimeter;
    var cy = 0 * millimeter;

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(cx - (receiverWidth / 2), cy - (receiverHeight / 2), 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the receiver profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(receiverWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(receiverWidth, 0 * millimeter), "end" : vector(receiverWidth, receiverHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(receiverWidth, receiverHeight), "end" : vector(0 * millimeter, receiverHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, receiverHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    skSsolve(baseSketch);

    // Extrude the base
    opExtrude(context, id + "extBase", {
            "entities" : qSketchRegion(id + "baseSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : receiverDepth
    });

    // Add a chamfer to the top edge
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "extBase", 1),
            "distance" : 1 * millimeter
    });
}