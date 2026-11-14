// =========================================================================
// TEST FIT: Mega-Dovetail Receiver Block
// =========================================================================
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


function testFitReceiverImpl(context is Context, id is Id, definition is map)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var deskAngle = getVar(context, "desk_angle", 15 * degree);
    
    var cx = -150 * millimeter;
    var cy = 0 * millimeter;

    var recHalfW = 32 * millimeter; // 64mm total width
    var recHeight = 95 * millimeter; // 95mm tall housing
    var baseDepth = 26 * millimeter + (recHeight * sin(deskAngle));

    // 1. Build the Solid Angled Housing Block
    var wedgeSketch = newSketchOnPlane(context, id + "wedgeSketch", {
            "sketchPlane" : plane(vector(cx - recHalfW, cy, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });
    var w1 = vector(0 * millimeter, 0 * millimeter);
    var w2 = vector(baseDepth,     0 * millimeter);
    var w3 = vector(baseDepth - (recHeight * sin(deskAngle)), recHeight * cos(deskAngle));
    var w4 = vector(0 * millimeter, recHeight * cos(deskAngle));

    skLineSegment(wedgeSketch, "wBot",   { "start" : w1, "end" : w2 });
    skLineSegment(wedgeSketch, "wBack",  { "start" : w2, "end" : w3 });
    skLineSegment(wedgeSketch, "wTop",   { "start" : w3, "end" : w4 });
    skLineSegment(wedgeSketch, "wFront", { "start" : w4, "end" : w1 });
    skSolve(wedgeSketch);

    opExtrude(context, id + "extReceiver", {
            "entities" : qSketchRegion(id + "wedgeSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : recHalfW * 2
    });

    // 2. Cut the Female Mega-Dovetail Slot + Tolerance
    var uSlide = vector(0, -sin(deskAngle), cos(deskAngle));
    var cornerOrigin = vector(cx, cy + baseDepth, 0 * millimeter) - (8 * millimeter) * uSlide;

    var doveCutSketch = newSketchOnPlane(context, id + "doveCutSketch", {
            "sketchPlane" : plane(cornerOrigin, uSlide, vector(1, 0, 0))
    });

    var dOuter   = 2 * millimeter;
    var dInner   = -(18 * millimeter) - tol;
    var halfNeck = (13.0 * millimeter) + tol;
    var halfWide = (19.0 * millimeter) + tol;

    skLineSegment(doveCutSketch, "dBack",  { "start" : vector(-halfNeck, dOuter), "end" : vector( halfNeck, dOuter) });
    skLineSegment(doveCutSketch, "dRight", { "start" : vector( halfNeck, dOuter), "end" : vector( halfWide, dInner) });
    skLineSegment(doveCutSketch, "dFront", { "start" : vector( halfWide, dInner), "end" : vector(-halfWide, dInner) });
    skLineSegment(doveCutSketch, "dLeft",  { "start" : vector(-halfWide, dInner), "end" : vector(-halfNeck, dOuter) });
    skSolve(doveCutSketch);

    opExtrude(context, id + "extDoveCut", {
            "entities" : qSketchRegion(id + "doveCutSketch"),
            "direction" : uSlide,
            "endBound" : BoundingType.BLIND,
            "endDepth" : 79 * millimeter
    });

    opBoolean(context, id + "finalTestCut", {
            "tools" : qCreatedBy(id + "extDoveCut", EntityType.BODY),
            "targets" : qCreatedBy(id + "extReceiver", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}

annotation { "Feature Type Name" : "Test Fit Dovetail Receiver" }
export const testFitReceiver = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        testFitReceiverImpl(context, id, definition);
    }
);
