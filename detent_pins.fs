// Flat-Bed Printable T-Handle Detent Pin (Part 5) - Unified Implementation
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

function onStageDetentPinsImpl(context is Context, id is Id, definition is map)
{
    var tol        = getVar(context, "tol", 0.3 * millimeter);
    var m3Clear   = getVar(context, "m3_clear", 3.4 * millimeter);

    // Detent pin dimensions
    var pinWidth = 8 * millimeter;
    var pinHeight = 12 * millimeter;
    var pinDepth = 20 * millimeter;

    // T-handle dimensions
    var handleWidth = 15 * millimeter;
    var handleHeight = 8 * millimeter;

    // Positioning the detent pins at the center of the desk (X = 230mm)
    var cx = 230 * millimeter;
    var cy = 0 * millimeter;

    // Create the base pin body
    fCuboid(context, id + "pinBody", {
            "corner1" : vector(cx - (pinWidth / 2), cy - (pinHeight / 2), 0 * millimeter),
            "corner2" : vector(cx + (pinWidth / 2), cy + (pinHeight / 2), pinDepth)
    });

    // Create a sketch for the T-handle cutout
    var handleSketch = newSketchOnPlane(context, id + "handleSketch", {
            "sketchPlane" : plane(vector(cx - (pinWidth / 2), cy - (pinHeight / 2), 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    // Sketch the T-handle cutout
    skLineSegment(handleSketch, "cut1", { "start" : vector(0 * millimeter, pinHeight), "end" : vector(handleWidth, pinHeight) });
    skLineSegment(handleSketch, "cut2", { "start" : vector(handleWidth, pinHeight), "end" : vector(handleWidth, pinHeight + handleHeight) });
    skLineSegment(handleSketch, "cut3", { "start" : vector(handleWidth, pinHeight + handleHeight), "end" : vector(0 * millimeter, pinHeight + handleHeight) });
    skLineSegment(handleSketch, "cut4", { "start" : vector(0 * millimeter, pinHeight + handleHeight), "end" : vector(0 * millimeter, pinHeight) });

    skSolve(handleSketch);

    // Extrude the T-handle cutout
    opExtrude(context, id + "extHandleCut", {
            "entities" : qSketchRegion(id + "handleSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : pinWidth
    });

    // Subtract the T-handle cutout from the pin body
    opBoolean(context, id + "finalPinCut", {
            "tools" : qUnion([
                    qCreatedBy(id + "extHandleCut", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "pinBody", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });

    // Add a chamfer to the top edge of the pin body
    opChamfer(context, id + "chamferTop", {
            "entities" : qEdge(id + "pinBody", 1),
            "distance" : 1 * millimeter
    });
}

// Export the feature so it appears in OnShape with proper annotations
annotation { "Feature Type Name" : "OnStage Detent Pins" }
export const onStageDetentPins = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageDetentPinsImpl(context, id, definition);
    }
);