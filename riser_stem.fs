// Modular 2-Piece Riser Stem (Part 2A Extension + Part 2B Upper Head)
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

function onStageRiserStemImpl(context is Context, id is Id, definition is map)
{
    var m4Clear = getVar(context, "m4_clear", 4.4 * millimeter);
    var m4Nut   = getVar(context, "m4_nut", 7.4 * millimeter);
    var m4Head  = getVar(context, "m4_head", 7.6 * millimeter);
    var pegSize = getVar(context, "peg_size", 20.0 * millimeter);
    var tol      = getVar(context, "tol", 0.3 * millimeter);

    var colW   = 40 * millimeter; // 40mm wide
    var colD   = 30 * millimeter; // 30mm deep
    var halfW  = colW / 2;       // 20mm
    var halfD  = colD / 2;       // 15mm
    var pegH   = 30 * millimeter; // 30mm tall mating peg (matches Part 1B)
    var doveH  = 70 * millimeter; // 70mm tall mega-dovetail
    var pinZ   = 15 * millimeter; // Locking bolt height inside sockets/pegs

    // Splitting the 223mm column into two bed-friendly segments:
    // 115mm (2A body) + 108mm (2B body) = 223mm total column height
    var colH2A = 115 * millimeter; // Part 2A total length = 115 + 30 = 145mm
    var colH2B = 108 * millimeter; // Part 2B total length = 108 + 70 = 178mm

    var socketSize = pegSize + tol;
    var halfSoc    = socketSize / 2;
    var halfPeg    = pegSize / 2;

    // =================================================================
    // PART 2A: Lower Modular Extension Column (X = 75mm, Y = 0mm)
    // =================================================================
    var cxA = 75 * millimeter;
    var cyA = 0 * millimeter;

    // 1. Main 40x30x115mm Column Body
    fCuboid(context, id + "col2A", {
            "corner1" : vector(cxA - halfW, cyA - halfD, 0 * millimeter),
            "corner2" : vector(cxA + halfW, cyA + halfD, colH2A)
    });

    opFillet(context, id + "filletCol2A", {
            "entities" : qParallelEdges(qOwnedByBody(qCreatedBy(id + "col2A", EntityType.BODY), EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 3 * millimeter
    });

    // 2. Top 20x20x30mm Male Coupling Peg (Z = 115mm to 145mm)
    fCuboid(context, id + "peg2A", {
            "corner1" : vector(cxA - halfPeg, cyA - halfPeg, colH2A),
            "corner2" : vector(cxA + halfPeg, cyA + halfPeg, colH2A + pegH)
    });

    opBoolean(context, id + "union2A", {
            "tools" : qUnion([
                    qCreatedBy(id + "col2A", EntityType.BODY),
                    qCreatedBy(id + "peg2A", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 3. Bottom Female Socket Cutout (31mm deep) + Lower & Upper M4 Locking Holes
    fCuboid(context, id + "socketCut2A", {
            "corner1" : vector(cxA - halfSoc, cyA - halfSoc, -1 * millimeter),
            "corner2" : vector(cxA + halfSoc, cyA + halfSoc, 31 * millimeter)
    });

    // Lower Socket M4 Through-Hole (Z = 15mm)
    fCylinder(context, id + "pinCyl2A_Bot", {
            "bottomCenter" : vector(cxA, cyA - (20 * millimeter), pinZ),
            "topCenter"    : vector(cxA, cyA + (20 * millimeter), pinZ),
            "radius"       : m4Clear / 2
    });

    // Lower Socket Front M4 Head Counterbore (2.0mm deep)
    fCylinder(context, id + "headCyl2A_Bot", {
            "bottomCenter" : vector(cxA, cyA - (20 * millimeter), pinZ),
            "topCenter"    : vector(cxA, cyA - (13 * millimeter), pinZ),
            "radius"       : m4Head / 2
    });

    // Lower Socket Rear M4 Hex Nut Trap (2.0mm deep)
    var nutSk2A = newSketchOnPlane(context, id + "nutSk2A", {
            "sketchPlane" : plane(vector(cxA, cyA + halfD, pinZ), vector(0, 1, 0), vector(1, 0, 0))
    });
    skRegularPolygon(nutSk2A, "hex2A", {
            "center" : vector(0 * millimeter, 0 * millimeter),
            "firstVertex" : vector(0 * millimeter, m4Nut / (2 * cos(30 * degree))),
            "sides" : 6
    });
    skSolve(nutSk2A);

    opExtrude(context, id + "extNut2A", {
            "entities" : qSketchRegion(id + "nutSk2A"),
            "direction" : vector(0, -1, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 2.0 * millimeter
    });

    // Upper Peg M4 Through-Hole (Z = 115 + 15 = 130mm)
    fCylinder(context, id + "pinCyl2A_Top", {
            "bottomCenter" : vector(cxA, cyA - (15 * millimeter), colH2A + pinZ),
            "topCenter"    : vector(cxA, cyA + (15 * millimeter), colH2A + pinZ),
            "radius"       : m4Clear / 2
    });

    opBoolean(context, id + "finalCut2A", {
            "tools" : qUnion([
                    qCreatedBy(id + "socketCut2A", EntityType.BODY),
                    qCreatedBy(id + "pinCyl2A_Bot", EntityType.BODY),
                    qCreatedBy(id + "headCyl2A_Bot", EntityType.BODY),
                    qCreatedBy(id + "extNut2A", EntityType.BODY),
                    qCreatedBy(id + "pinCyl2A_Top", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "col2A", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });


    // =================================================================
    // PART 2B: Upper Dovetail Riser Head (X = 75mm, Y = 60mm)
    // Positioned directly behind Part 2A in +Y so it stays completely
    // clear of Part 3A (Lower Music Desk at X = 110mm..350mm)
    // =================================================================
    var cxB = 75 * millimeter;
    var cyB = 60 * millimeter;

    // 1. Main 40x30x108mm Column Body
    fCuboid(context, id + "col2B", {
            "corner1" : vector(cxB - halfW, cyB - halfD, 0 * millimeter),
            "corner2" : vector(cxB + halfW, cyB + halfD, colH2B)
    });

    opFillet(context, id + "filletCol2B", {
            "entities" : qParallelEdges(qOwnedByBody(qCreatedBy(id + "col2B", EntityType.BODY), EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 3 * millimeter
    });

    // 2. Mega Male Dovetail Slider (Z = 108mm to 178mm)
    var doveSketch = newSketchOnPlane(context, id + "doveSketch", {
            "sketchPlane" : plane(vector(cxB, cyB, colH2B), vector(0, 0, 1), vector(1, 0, 0))
    });
    var yBack    = halfD;                      // +15mm (flush with back face of column for flat printing!)
    var yFront   = halfD - (18 * millimeter);  // -3mm  (18mm thick mega-dovetail)
    var halfNeck = 13.0 * millimeter;           // 26mm total neck width
    var halfWide = 19.0 * millimeter;           // 38mm total front flare width

    skLineSegment(doveSketch, "backEdge",  { "start" : vector(-halfNeck, yBack),  "end" : vector( halfNeck, yBack) });
    skLineSegment(doveSketch, "rightEdge", { "start" : vector( halfNeck, yBack),  "end" : vector( halfWide, yFront) });
    skLineSegment(doveSketch, "frontEdge", { "start" : vector( halfWide, yFront), "end" : vector(-halfWide, yFront) });
    skLineSegment(doveSketch, "leftEdge",  { "start" : vector(-halfWide, yFront), "end" : vector(-halfNeck, yBack) });
    skSolve(doveSketch);

    opExtrude(context, id + "extDove", {
            "entities" : qSketchRegion(id + "doveSketch"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : doveH
    });

    opBoolean(context, id + "union2B", {
            "tools" : qUnion([
                    qCreatedBy(id + "col2B", EntityType.BODY),
                    qCreatedBy(id + "extDove", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 3. Bottom Female Socket Cutout (31mm deep) + M4 Locking Bolt Bore
    fCuboid(context, id + "socketCut2B", {
            "corner1" : vector(cxB - halfSoc, cyB - halfSoc, -1 * millimeter),
            "corner2" : vector(cxB + halfSoc, cyB + halfSoc, 31 * millimeter)
    });

    fCylinder(context, id + "pinCyl2B", {
            "bottomCenter" : vector(cxB, cyB - (20 * millimeter), pinZ),
            "topCenter"    : vector(cxB, cyB + (20 * millimeter), pinZ),
            "radius"       : m4Clear / 2
    });

    fCylinder(context, id + "headCyl2B", {
            "bottomCenter" : vector(cxB, cyB - (20 * millimeter), pinZ),
            "topCenter"    : vector(cxB, cyB - (13 * millimeter), pinZ),
            "radius"       : m4Head / 2
    });

    var nutSk2B = newSketchOnPlane(context, id + "nutSk2B", {
            "sketchPlane" : plane(vector(cxB, cyB + halfD, pinZ), vector(0, 1, 0), vector(1, 0, 0))
    });
    skRegularPolygon(nutSk2B, "hex2B", {
            "center" : vector(0 * millimeter, 0 * millimeter),
            "firstVertex" : vector(0 * millimeter, m4Nut / (2 * cos(30 * degree))),
            "sides" : 6
    });
    skSolve(nutSk2B);

    opExtrude(context, id + "extNut2B", {
            "entities" : qSketchRegion(id + "nutSk2B"),
            "direction" : vector(0, -1, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 2.0 * millimeter
    });

    opBoolean(context, id + "finalCut2B", {
            "tools" : qUnion([
                    qCreatedBy(id + "socketCut2B", EntityType.BODY),
                    qCreatedBy(id + "pinCyl2B", EntityType.BODY),
                    qCreatedBy(id + "headCyl2B", EntityType.BODY),
                    qCreatedBy(id + "extNut2B", EntityType.BODY)
            ]),
            "targets" : qCreatedBy(id + "col2B", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}

// Export the feature so it appears in OnShape with proper annotations
annotation { "Feature Type Name" : "OnStage Riser Stem" }
export const onStageRiserStem = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageRiserStemImpl(context, id, definition);
    }
);