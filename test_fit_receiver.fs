// Test Fit Dovetail Receiver
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function testFitReceiver(context is Context, id is Id, definition is map)
{
    // Get variables with defaults
    var receiverWidth = getVar(context, "receiverWidth", 20 * millimeter);
    var receiverHeight = getVar(context, "receiverHeight", 14 * millimeter);
    var receiverDepth = getVar(context, "receiverDepth", 15 * millimeter);

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the receiver profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(receiverWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(receiverWidth, 0 * millimeter), "end" : vector(receiverWidth, receiverHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(receiverWidth, receiverHeight), "end" : vector(0 * millimeter, receiverHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, receiverHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    skSolve(baseSketch);

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