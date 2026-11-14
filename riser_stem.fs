// Modular 2-Piece Riser Stem
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function onStageRiserStem(context is Context, id is Id, definition is map)
{
    // Get variables with defaults
    var stemWidth = getVar(context, "stemWidth", 20 * millimeter);
    var stemHeight = getVar(context, "stemHeight", 30 * millimeter);
    var stemDepth = getVar(context, "stemDepth", 15 * millimeter);
    var railHeight = getVar(context, "railHeight", 14 * millimeter);
    var railWidth = getVar(context, "railWidth", 10 * millimeter);

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the riser stem profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(stemWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(stemWidth, 0 * millimeter), "end" : vector(stemWidth, stemHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(stemWidth, stemHeight), "end" : vector(0 * millimeter, stemHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, stemHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    // Add a cutout for the rail
    skLineSegment(baseSketch, "cut1", { "start" : vector(stemWidth - railWidth, 0 * millimeter), "end" : vector(stemWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "cut2", { "start" : vector(stemWidth, 0 * millimeter), "end" : vector(stemWidth, railHeight) });
    skLineSegment(baseSketch, "cut3", { "start" : vector(stemWidth, railHeight), "end" : vector(stemWidth - railWidth, railHeight) });
    skLineSegment(baseSketch, "cut4", { "start" : vector(stemWidth - railWidth, railHeight), "end" : vector(stemWidth - railWidth, 0 * millimeter) });

    skSolve(baseSketch);

    // Extrude the base
    opExtrude(context, id + "extBase", {
            "entities" : qSketchRegion(id + "baseSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : stemDepth
    });

    // Add a chamfer to the top edge
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "extBase", 1),
            "distance" : 1 * millimeter
    });
}