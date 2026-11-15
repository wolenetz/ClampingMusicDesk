// =========================================================================
// PART 5: Flat-Bed Printable T-Handle Detent Pin
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

function onStageDetentPinImpl(context is Context, id is Id, definition is map, partName is string)
{
    var m3Clear = getVar(context, "m3_clear", 3.4 * millimeter);
    var tol      = getVar(context, "tol", 0.3 * millimeter);
    // Pin diameter is sized 0.35mm smaller than the 3.4mm hole (approx 3.05mm) for an easy, non-binding slip fit
    var pinRad  = (m3Clear - tol - 0.05 * millimeter) / 2;
    
    // Flattening the top and bottom (2.4mm height) creates a D-profile / double-flat shaft:
    // 1. It prints completely flat on the build plate with zero supports and zero bed-adhesion issues!
    // 2. Continuous perimeter wall loops run the entire 38mm length of the shaft for maximum shear strength!
    var flatH     = 2.4 * millimeter; 
    var shaftLen  = 38 * millimeter; // Passes cleanly through the 32mm wide guide block in double-shear
    var xBase = 825 * millimeter;
    var yOff = 0 * millimeter;
            
    // 1. Round Pin Shaft along X-axis (X = 825mm to 863mm)
    fCylinder(context, id + "shaftCyl", {
            "bottomCenter" : vector(xBase, yOff, 0 * millimeter),
            "topCenter"    : vector(xBase + shaftLen, yOff, 0 * millimeter),
            "radius"       : pinRad
    });
    // 2. Bounding Box to Flatten Top & Bottom of Shaft (Z = -1.2mm to +1.2mm) & Bevel Tip
    var boundSk = newSketchOnPlane(context, id + "boundSk", {
            "sketchPlane" : plane(vector(xBase, yOff - 10 * millimeter, 0 * millimeter), vector(0, -1, 0), vector(1, 0, 0))
    });
    var hZ = flatH / 2; // 1.2mm
    var b1 = vector(0 * millimeter,               -hZ);
    var b2 = vector(shaftLen - 1.5 * millimeter, -hZ);
    var b3 = vector(shaftLen,                    -0.5 * millimeter); // Tapered lead-in tip
    var b4 = vector(shaftLen,                     0.5 * millimeter);
    var b5 = vector(shaftLen - 1.5 * millimeter,  hZ);
    var b6 = vector(0 * millimeter,                hZ);
    skLineSegment(boundSk, "bL1", { "start" : b1, "end" : b2 });
    skLineSegment(boundSk, "bL2", { "start" : b2, "end" : b3 });
    skLineSegment(boundSk, "bL3", { "start" : b3, "end" : b4 });
    skLineSegment(boundSk, "bL4", { "start" : b4, "end" : b5 });
    skLineSegment(boundSk, "bL5", { "start" : b5, "end" : b6 });
    skLineSegment(boundSk, "bL6", { "start" : b6, "end" : b1 });
    skSolve(boundSk);
    opExtrude(context, id + "extBound", {
            "entities" : qSketchRegion(id + "boundSk"),
            "direction" : vector(0, 1, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 20 * millimeter
    });
    // Intersect Cylinder and Flat-Top/Tip-Beveled Box to create the print-flat shaft
    opBoolean(context, id + "flatShaft", {
            "tools" : qUnion([
                    qCreatedBy(id + "shaftCyl", EntityType.BODY),
                    qCreatedBy(id + "extBound", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.INTERSECTION
    });
    // 3. Ergonomic T-Handle Grip (14mm long x 18mm wide x 2.4mm thick, flush with top/bottom of shaft!)
    fCuboid(context, id + "tHandle", {
            "corner1" : vector(xBase - 14 * millimeter, yOff - 9 * millimeter, -hZ),
            "corner2" : vector(xBase,                   yOff + 9 * millimeter,  hZ)
    });
    // 4. Collar Stop Shoulder (6mm wide x 4mm long) so the pin seats cleanly against the guide block wall
    fCuboid(context, id + "collar", {
            "corner1" : vector(xBase,                yOff - 3 * millimeter, -hZ),
            "corner2" : vector(xBase + 3 * millimeter, yOff + 3 * millimeter,  hZ)
    });
    opBoolean(context, id + "unionPin", {
            "tools" : qUnion([
                    qCreatedBy(id + "flatShaft", EntityType.BODY),
                    qCreatedBy(id + "tHandle", EntityType.BODY),
                    qCreatedBy(id + "collar", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });
    // 5. Finger Pull-Hole / Lanyard Loop in the T-Handle (7mm diameter)
    fCylinder(context, id + "pullHole", {
            "bottomCenter" : vector(xBase - 7 * millimeter, yOff, -5 * millimeter),
            "topCenter"    : vector(xBase - 7 * millimeter, yOff,  5 * millimeter),
            "radius"       : 3.5 * millimeter
    });
    opBoolean(context, id + "subPull", {
            "tools" : qCreatedBy(id + "pullHole", EntityType.BODY),
            "targets" : qCreatedBy(id + "flatShaft", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });

    // Name it for easier export.
    setProperty(context, {
        "entities" : qCreatedBy(id + "flatShaft", EntityType.BODY),
        "propertyType" : PropertyType.NAME,
        "value" : partName
    });
}

annotation { "Feature Type Name" : "OnStage Detent Pin" }
export const onStageDetentPins = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageDetentPinImpl(context, id, definition, "Part 5 - Detent Pin");
    }
);
