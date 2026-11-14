// Main FeatureScript for Clamping Music Desk
FeatureScript 2411;
import(path : "onshape/std/common.fs", version : "2411.0");

// Import utility functions
import(path : "./utils.fs", version : "2411.0");

// Import feature modules
import(path : "./l_clamp.fs", version : "2411.0");
import(path : "./riser_stem.fs", version : "2411.0");
import(path : "./music_desk_3a.fs", version : "2411.0");
import(path : "./music_desk_3a_dual.fs", version : "2411.0");
import(path : "./music_desk_3b.fs", version : "2411.0");
import(path : "./page_clamps.fs", version : "2411.0");
import(path : "./detent_pins.fs", version : "2411.0");
import(path : "./test_fit_receiver.fs", version : "2411.0");

// Export all feature definitions so they appear in OnShape with proper annotations
annotation { "Feature Type Name" : "OnStage L-Clamp" }
export const onStageLClamp = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageLClamp(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Riser Stem" }
export const onStageRiserStem = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageRiserStem(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Music Desk 3A" }
export const onStageMusicDesk3A = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3A(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Music Desk 3A Dual" }
export const onStageMusicDesk3ADual = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3ADual(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Music Desk 3B" }
export const onStageMusicDesk3B = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageMusicDesk3B(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Page Clamps" }
export const onStagePageClamps = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStagePageClamps(context, id, definition);
    }
);

annotation { "Feature Type Name" : "OnStage Detent Pins" }
export const onStageDetentPins = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        onStageDetentPins(context, id, definition);
    }
);

annotation { "Feature Type Name" : "Test Fit Receiver" }
export const testFitReceiver = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        testFitReceiver(context, id, definition);
    }
);