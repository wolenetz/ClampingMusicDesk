// Dual-Riser Lower Stationary Music Desk (Part 3A-Dual)
import(path : "onshape/std/common.fs", version : "2411.0");
import(path : "./utils.fs", version : "2411.0");

function onStageMusicDesk3ADual(context is Context, id is Id, definition is map)
{
    // Get variables with defaults
    var deskWidth = getVar(context, "deskWidth", 60 * millimeter);
    var deskHeight = getVar(context, "deskHeight", 20 * millimeter);
    var deskDepth = getVar(context, "deskDepth", 15 * millimeter);
    var railHeight = getVar(context, "railHeight", 14 * millimeter);
    var railWidth = getVar(context, "railWidth", 10 * millimeter);

    // Create base sketch
    var baseSketch = newSketchOnPlane(context, id + "baseSketch", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the dual music desk profile
    skLineSegment(baseSketch, "l1", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(deskWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "l2", { "start" : vector(deskWidth, 0 * millimeter), "end" : vector(deskWidth, deskHeight) });
    skLineSegment(baseSketch, "l3", { "start" : vector(deskWidth, deskHeight), "end" : vector(0 * millimeter, deskHeight) });
    skLineSegment(baseSketch, "l4", { "start" : vector(0 * millimeter, deskHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    // Add cutouts for the rails
    skLineSegment(baseSketch, "cut1", { "start" : vector(deskWidth - railWidth, 0 * millimeter), "end" : vector(deskWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "cut2", { "start" : vector(deskWidth, 0 * millimeter), "end" : vector(deskWidth, railHeight) });
    skLineSegment(baseSketch, "cut3", { "start" : vector(deskWidth, railHeight), "end" : vector(deskWidth - railWidth, railHeight) });
    skLineSegment(baseSketch, "cut4", { "start" : vector(deskWidth - railWidth, railHeight), "end" : vector(deskWidth - railWidth, 0 * millimeter) });

    // Add second cutout for the second rail
    skLineSegment(baseSketch, "cut5", { "start" : vector(0 * millimeter, 0 * millimeter), "end" : vector(railWidth, 0 * millimeter) });
    skLineSegment(baseSketch, "cut6", { "start" : vector(railWidth, 0 * millimeter), "end" : vector(railWidth, railHeight) });
    skLineSegment(baseSketch, "cut7", { "start" : vector(railWidth, railHeight), "end" : vector(0 * millimeter, railHeight) });
    skLineSegment(baseSketch, "cut8", { "start" : vector(0 * millimeter, railHeight), "end" : vector(0 * millimeter, 0 * millimeter) });

    skSolve(baseSketch);

    // Extrude the base
    opExtrude(context, id + "extBase", {
            "entities" : qSketchRegion(id + "baseSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : deskDepth
    });

    // Add a chamfer to the top edge
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "extBase", 1),
            "distance" : 1 * millimeter
    });
}