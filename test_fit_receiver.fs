// Test Fit Dovetail Receiver - Unified Implementation
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

function testFitReceiverImpl(context is Context, id is Id, definition is map)
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

// Export the feature so it appears in OnShape with proper annotations
annotation { "Feature Type Name" : "Test Fit Receiver" }
export const testFitReceiver = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        testFitReceiverImpl(context, id, definition);
    }
);