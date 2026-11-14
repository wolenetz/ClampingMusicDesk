// Modular Page Clamps (Part 4)
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function onStagePageClamps(context is Context, id is Id, definition is map)
{
    // Get variables with defaults
    var clampWidth = getVar(context, "clampWidth", 30 * millimeter);
    var clampHeight = getVar(context, "clampHeight", 15 * millimeter);
    var clampDepth = getVar(context, "clampDepth", 20 * millimeter);
    var railHeight = getVar(context, "railHeight", 14 * millimeter);
    var railWidth = getVar(context, "railWidth", 10 * millimeter);

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the page clamp profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(clampWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(clampWidth, 0 * millimeter), "end" : vector(clampWidth, clampHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(clampWidth, clampHeight), "end" : vector(0 * millimeter, clampHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, clampHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    // Add a cutout for the rail
    skLineSegment(baseSketch, "cut1", { "start" : vector(clampWidth - railWidth, 0 * millimeter), "end" : vector(clampWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "cut2", { "start" : vector(clampWidth, 0 * millimeter), "end" : vector(clampWidth, railHeight) });
    skLineSegment(baseSketch, "cut3", { "start" : vector(clampWidth, railHeight), "end" : vector(clampWidth - railWidth, railHeight) });
    skLineSegment(baseSketch, "cut4", { "start" : vector(clampWidth - railWidth, railHeight), "end" : vector(clampWidth - railWidth, 0 * millimeter) });

    skSolve(baseSketch);

    // Extrude the base
    opExtrude(context, id + "extBase", {
            "entities" : qSketchRegion(id + "baseSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : clampDepth
    });

    // Add a chamfer to the top edge
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "extBase", 1),
            "distance" : 1 * millimeter
    });
}