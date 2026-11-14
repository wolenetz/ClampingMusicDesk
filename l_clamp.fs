// Horizontal-Split L-Clamp (Dorsal Bolt Raised +4mm for Max Part 1A Strength)
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

function onStageLClampImpl(context is Context, id is Id, definition is map)
{
    var tubeSize  = getVar(context, "tube_size", 30.5 * millimeter);
    var tubeRad   = getVar(context, "tube_rad", 3.5 * millimeter);
    var clampGap  = getVar(context, "clamp_gap", 2.0 * millimeter);
    var m4Clear   = getVar(context, "m4_clear", 4.4 * millimeter);
    var m4Nut     = getVar(context, "m4_nut", 7.4 * millimeter);
    var m4Head    = getVar(context, "m4_head", 7.6 * millimeter);
    var pegSize   = getVar(context, "peg_size", 20.0 * millimeter);
    var backReach = getVar(context, "back_reach", 155.0 * millimeter);

    var halfTube = tubeSize / 2;                // 15.25mm
    var topZ     = halfTube + (8 * millimeter); // +23.25mm
    var botZ     = -35 * millimeter;             // -35.00mm

    // Peg center is at Y = 15.25 + 155 + 12.5 = 182.75mm
    var pegCenterY  = halfTube + backReach + (12.5 * millimeter);
    var armExtension = 10.0 * millimeter;
    var armEndY     = halfTube + backReach + (25.0 * millimeter) + armExtension; // 205.25mm
    var seatFrontY  = pegCenterY - (15.0 * millimeter); // 167.75mm (Front edge of 40x30mm riser footprint)
    var spineBoltY  = seatFrontY - (20.0 * millimeter); // 147.75mm (20mm in front of riser footprint)

    // 1. Solid 90mm Wide Clamp Block
    fCuboid(context, id + "clampBlock", {
            "corner1" : vector(-45 * millimeter, -30 * millimeter, botZ),
            "corner2" : vector( 45 * millimeter,  30 * millimeter, topZ)
    });

    opFillet(context, id + "filletClampOuter", {
            "entities" : qParallelEdges(qOwnedByBody(qCreatedBy(id + "clampBlock", EntityType.BODY), EntityType.EDGE), vector(0, 0, 1)),
            "radius" : 4 * millimeter
    });

    // 2. Profiled Extension Arm (Part 1B) with Swept Curved Bottom Keel
    var armProfileSk = newSketchOnPlane(context, id + "armProfileSk", {
            "sketchPlane" : plane(vector(-25 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });

    var curveStartY = armEndY - (45 * millimeter);
    var a1 = vector(20 * millimeter, botZ);
    var a2 = vector(curveStartY,   botZ);
    var a3 = vector(armEndY,       10 * millimeter);
    var a4 = vector(20 * millimeter, 10 * millimeter);

    var aMid = vector(curveStartY + (31 * millimeter), botZ + (13 * millimeter));

    skLineSegment(armProfileSk, "aBottomFlat",  { "start" : a1, "end" : a2 });
    skArc(armProfileSk,         "aBottomSweep", { "start" : a2, "mid" : aMid, "end" : a3 });
    skLineSegment(armProfileSk, "aTopFlat",     { "start" : a3, "end" : a4 });
    skLineSegment(armProfileSk, "aFrontClose",  { "start" : a4, "end" : a1 });
    skSolve(armProfileSk);

    opExtrude(context, id + "extArm", {
            "entities" : qSketchRegion(id + "armProfileSk"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 50 * millimeter
    });

    var armBody  = qCreatedBy(id + "extArm", EntityType.BODY);
    var armEdges = qOwnedByBody(armBody, EntityType.EDGE);

    // 2A. Fillet Arm Longitudinal Edges, Curved Keel, and Rear Top Edge
    var topLongEdges = qUnion([
            qContainsPoint(armEdges, vector(-25 * millimeter, 50 * millimeter, 10 * millimeter)),
            qContainsPoint(armEdges, vector( 25 * millimeter, 50 * millimeter, 10 * millimeter))
    ]);
    var botFlatEdges = qUnion([
            qContainsPoint(armEdges, vector(-25 * millimeter, 50 * millimeter, botZ)),
            qContainsPoint(armEdges, vector( 25 * millimeter, 50 * millimeter, botZ))
    ]);
    var keelArcEdges = qUnion([
            qContainsPoint(armEdges, vector(-25 * millimeter, aMid[0], aMid[1])),
            qContainsPoint(armEdges, vector( 25 * millimeter, aMid[0], aMid[1]))
    ]);
    var rearTopEdge = qContainsPoint(armEdges, vector(0 * millimeter, armEndY, 10 * millimeter));

    opFillet(context, id + "filletArmContours", {
            "entities" : qUnion([
                    topLongEdges,
                    botFlatEdges,
                    keelArcEdges,
                    rearTopEdge
            ]),
            "radius" : 5 * millimeter
    });

    // 2B. Central Dorsal Tension Spine on Part 1A (Y = 20mm to seatFrontY, Z = 11mm to topZ)
    var spineBotZ = 10 * millimeter + (clampGap / 2); // +11.00mm
    fCuboid(context, id + "armSpine", {
            "corner1" : vector(-10 * millimeter, 20 * millimeter, spineBotZ),
            "corner2" : vector( 10 * millimeter, seatFrontY,    topZ)
    });
    var spineBody  = qCreatedBy(id + "armSpine", EntityType.BODY);
    var spineEdges = qOwnedByBody(spineBody, EntityType.EDGE);

    opFillet(context, id + "filletSpine", {
            "entities" : qUnion([
                    qContainsPoint(spineEdges, vector(-10 * millimeter, 100 * millimeter, topZ)), // moved to top of spine
                    qContainsPoint(spineEdges, vector( 10 * millimeter, 100 * millimeter, topZ)), // moved to top of spine
                    qContainsPoint(spineEdges, vector(-10 * millimeter, seatFrontY,     topZ)),
                    qContainsPoint(spineEdges, vector( 10 * millimeter, seatFrontY,     topZ))
            ]),
            "radius" : 10 * millimeter
    });

    // 3. Solid Upward Riser Peg at the Rear End of the Arm
    fCuboid(context, id + "riserPeg", {
            "corner1" : vector(-pegSize / 2, pegCenterY - pegSize / 2, 10 * millimeter),
            "corner2" : vector( pegSize / 2, pegCenterY + pegSize / 2, 40 * millimeter)
    });

    opBoolean(context, id + "unionBody", {
            "tools" : qUnion([
                    qCreatedBy(id + "clampBlock", EntityType.BODY),
                    armBody,
                    spineBody,
                    qCreatedBy(id + "riserPeg", EntityType.BODY)
            ]),
            "operationType" : BooleanOperationType.UNION
    });

    // 4. Cut Rounded Square Tube Profile along X-axis
    var tubeSketch = newSketchOnPlane(context, id + "tubeSketch", {
            "sketchPlane" : plane(vector(-50 * millimeter, 0 * millimeter, 0 * millimeter), vector(1, 0, 0), vector(0, 1, 0))
    });
    var h = halfTube;
    var r = tubeRad;
    var c = h - r;
    var k = r * cos(45 * degree);

    skLineSegment(tubeSketch, "top",    { "start" : vector(-c,  h), "end" : vector( c,  h) });
    skLineSegment(tubeSketch, "bottom", { "start" : vector(-c, -h), "end" : vector( c, -h) });
    skLineSegment(tubeSketch, "left",   { "start" : vector(-h, -c), "end" : vector(-h,  c) });
    skLineSegment(tubeSketch, "right",  { "start" : vector( h, -c), "end" : vector( h,  c) });

    skArc(tubeSketch, "arcTR", { "start" : vector( c,  h), "mid" : vector( c + k,  c + k), "end" : vector( h,  c) });
    skArc(tubeSketch, "arcTL", { "start" : vector(-h,  c), "mid" : vector(-c - k,  c + k), "end" : vector(-c,  h) });
    skArc(tubeSketch, "arcBL", { "start" : vector(-c, -h), "mid" : vector(-c - k, -c - k), "end" : vector(-h, -c) });
    skArc(tubeSketch, "arcBR", { "start" : vector( h, -c), "mid" : vector( c + k, -c - k), "end" : vector( c, -h) });
    skSolve(tubeSketch);

    opExtrude(context, id + "extTube", {
            "entities" : qSketchRegion(id + "tubeSketch"),
            "direction" : vector(1, 0, 0),
            "endBound" : BoundingType.BLIND,
            "endDepth" : 100 * millimeter
    });

    // 5A. Horizontal Compression Gap at Z = 0mm
    fCuboid(context, id + "gapToolHoriz", {
            "corner1" : vector(-50 * millimeter, -35 * millimeter, -clampGap / 2),
            "corner2" : vector( 50 * millimeter,  35 * millimeter,  clampGap / 2)
    });

    // 5B. Split Part 1A from Part 1B
    fCuboid(context, id + "armClearanceCut", {
            "corner1" : vector(-25 * millimeter - (clampGap / 2), 20 * millimeter - clampGap, -clampGap / 2),
            "corner2" : vector( 25 * millimeter + (clampGap / 2), 35 * millimeter,             spineBotZ)
    });

    // 6. Four Corner Clamping Bolts + 5th Raised Dorsal Spine Anchor Bolt
    var boltTools = [];

    // --- 6A. Main 4 Corner Clamp Bolts ---
    var cornerCoords = [
        vector(-35 * millimeter, -22.5 * millimeter, 0 * millimeter),
        vector( 35 * millimeter, -22.5 * millimeter, 0 * millimeter),
        vector(-35 * millimeter,  22.5 * millimeter, 0 * millimeter),
        vector( 35 * millimeter,  22.5 * millimeter, 0 * millimeter)
    ];

    var headSeatZMain = topZ - (9.25 * millimeter); // +14.00mm
    var nutRoofZMain  = -11 * millimeter;
    var nutSlotH       = 2.8 * millimeter;
    var nutFloorZMain = nutRoofZMain - nutSlotH; // -13.80mm

    var mainNutSk = newSketchOnPlane(context, id + "mainNutSk", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, nutFloorZMain), vector(0, 0, 1), vector(1, 0, 0))
    });

    var idx = 0;
    for (var pt in cornerCoords)
    {
        idx += 1;
        var bx = pt[0];
        var by = pt[1];

        fCylinder(context, id + ("holeCyl" ~ idx), {
                "bottomCenter" : vector(bx, by, -26 * millimeter),
                "topCenter"    : vector(bx, by,  30 * millimeter),
                "radius"       : m4Clear / 2
        });
        boltTools = append(boltTools, qCreatedBy(id + ("holeCyl" ~ idx), EntityType.BODY));

        fCylinder(context, id + ("headCyl" ~ idx), {
                "bottomCenter" : vector(bx, by, headSeatZMain),
                "topCenter"    : vector(bx, by, 30 * millimeter),
                "radius"       : m4Head / 2
        });
        boltTools = append(boltTools, qCreatedBy(id + ("headCyl" ~ idx), EntityType.BODY));

        var nutPt = vector(bx, by);
        skRegularPolygon(mainNutSk, "nut" ~ idx, {
                "center" : nutPt,
                "firstVertex" : nutPt + vector(m4Nut / (2 * cos(30 * degree)), 0 * millimeter),
                "sides" : 6
        });

        var xOuter = (bx < 0 * millimeter) ? (-50 * millimeter) : (50 * millimeter);
        var xMin   = (bx < 0 * millimeter) ? xOuter : bx;
        var xMax   = (bx < 0 * millimeter) ? bx : xOuter;

        fCuboid(context, id + ("sideSlot" ~ idx), {
                "corner1" : vector(xMin, by - (m4Nut / 2), nutFloorZMain),
                "corner2" : vector(xMax, by + (m4Nut / 2), nutRoofZMain)
        });
        boltTools = append(boltTools, qCreatedBy(id + ("sideSlot" ~ idx), EntityType.BODY));
    }
    skSolve(mainNutSk);

    opExtrude(context, id + "extMainNuts", {
            "entities" : qSketchRegion(id + "mainNutSk"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : nutSlotH
    });
    boltTools = append(boltTools, qCreatedBy(id + "extMainNuts", EntityType.BODY));

    // --- 6B. 5th Dorsal Anchor Bolt (+4mm Higher in Z for 7mm Part 1A Head Material) ---
    var headSeatZDorsal = topZ - (5.25 * millimeter); // +18.00mm (7mm solid plastic under head!)
    var nutRoofZDorsal  = -7 * millimeter;             // Raised 4mm
    var nutFloorZDorsal = nutRoofZDorsal - nutSlotH; // -9.80mm

    idx += 1;
    fCylinder(context, id + ("holeCyl" ~ idx), {
            "bottomCenter" : vector(0 * millimeter, spineBoltY, -26 * millimeter),
            "topCenter"    : vector(0 * millimeter, spineBoltY,  30 * millimeter),
            "radius"       : m4Clear / 2
    });
    boltTools = append(boltTools, qCreatedBy(id + ("holeCyl" ~ idx), EntityType.BODY));

    fCylinder(context, id + ("headCyl" ~ idx), {
            "bottomCenter" : vector(0 * millimeter, spineBoltY, headSeatZDorsal),
            "topCenter"    : vector(0 * millimeter, spineBoltY, 30 * millimeter),
            "radius"       : m4Head / 2
    });
    boltTools = append(boltTools, qCreatedBy(id + ("headCyl" ~ idx), EntityType.BODY));

    var dorsalNutSk = newSketchOnPlane(context, id + "dorsalNutSk", {
            "sketchPlane" : plane(vector(0 * millimeter, 0 * millimeter, nutFloorZDorsal), vector(0, 0, 1), vector(1, 0, 0))
    });
    skRegularPolygon(dorsalNutSk, "dorsalNut", {
            "center" : vector(0 * millimeter, spineBoltY),
            "firstVertex" : vector(0 * millimeter, spineBoltY) + vector(m4Nut / (2 * cos(30 * degree)), 0 * millimeter),
            "sides" : 6
    });
    skSolve(dorsalNutSk);

    opExtrude(context, id + "extDorsalNut", {
            "entities" : qSketchRegion(id + "dorsalNutSk"),
            "direction" : vector(0, 0, 1),
            "endBound" : BoundingType.BLIND,
            "endDepth" : nutSlotH
    });
    boltTools = append(boltTools, qCreatedBy(id + "extDorsalNut", EntityType.BODY));

    fCuboid(context, id + "sideSlotDorsal", {
            "corner1" : vector(0 * millimeter, spineBoltY - (m4Nut / 2), nutFloorZDorsal),
            "corner2" : vector(50 * millimeter, spineBoltY + (m4Nut / 2), nutRoofZDorsal)
    });
    boltTools = append(boltTools, qCreatedBy(id + "sideSlotDorsal", EntityType.BODY));


    // 7. Riser Peg Locking Pin Hole (M4 Through-Hole along Y-axis at X = 0mm, Z = +25mm)
    fCylinder(context, id + "pinCyl", {
            "bottomCenter" : vector(0 * millimeter, pegCenterY - 15 * millimeter, 25 * millimeter),
            "topCenter"    : vector(0 * millimeter, pegCenterY + 15 * millimeter, 25 * millimeter),
            "radius"       : m4Clear / 2
    });
    boltTools = append(boltTools, qCreatedBy(id + "pinCyl", EntityType.BODY));

    // 8. Subtract All Cutouts
    opBoolean(context, id + "finalCut", {
            "tools" : qUnion( concatenateArrays([
                    [
                        qCreatedBy(id + "extTube", EntityType.BODY),
                        qCreatedBy(id + "gapToolHoriz", EntityType.BODY),
                        qCreatedBy(id + "armClearanceCut", EntityType.BODY)
                    ],
                    boltTools
            ]) ),
            "targets" : qCreatedBy(id + "clampBlock", EntityType.BODY),
            "operationType" : BooleanOperationType.SUBTRACTION
    });
}

// Export the feature so it appears in OnShape with proper annotations
annotation { "Feature Type Name" : "OnStage L-Clamp" }
export const onStageLClamp = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageLClampImpl(context, id, definition);
    }
);