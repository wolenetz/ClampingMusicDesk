// Flat-Bed Printable T-Handle Detent Pin (Part 5)
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function onStageDetentPins(context is Context, id is Id, definition is map)
{
    // Get variables with defaults
    var pinWidth = getVar(context, "pinWidth", 8 * millimeter);
    var pinHeight = getVar(context, "pinHeight", 12 * millimeter);
    var pinDepth = getVar(context, "pinDepth", 20 * millimeter);
    var handleWidth = getVar(context, "handleWidth", 15 * millimeter);
    var handleHeight = getVar(context, "handleHeight", 8 * millimeter);

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the detent pin profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(pinWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(pinWidth, 0 * millimeter), "end" : vector(pinWidth, pinHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(pinWidth, pinHeight), "end" : vector(0 * millimeter, pinHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, pinHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    // Add a T-handle cutout
    skLineSegment(baseSketch, "cut1", { "start" : vector(0 * millimeter, pinHeight), "end" : vector(handleWidth, pinHeight) });
    skLineSegment(baseSketch, "cut2", { "start" : vector(handleWidth, pinHeight), "end" : vector(handleWidth, pinHeight + handleHeight) });
    skLineSegment(baseSketch, "cut3", { "start" : vector(handleWidth, pinHeight + handleHeight), "end" : vector(0 * millimeter, pinHeight + handleHeight) });
    skLineSegment(baseSketch, "cut4", { "start" : vector(0 * millimeter, pinHeight + handleHeight), "end" : vector(0 * millimeter, pinHeight) });

    skSolve(baseSketch);

    // Extrude the base
    opExtrude(context, id + "extBase", {
            "entities" : qSketchRegion(id + "baseSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : pinDepth
    });

    // Add a chamfer to the top edge
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "extBase", 1),
            "distance" : 1 * millimeter
    });
}