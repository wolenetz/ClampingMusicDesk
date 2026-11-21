# Plan: Update Build123d Desks with Wider Central Dovetail for Single Cam Lock Height Adjustment

## Context

**Problem:** The current design uses two cam locks (one on each side) to lock the upper desk height. It's very difficult to get both locks to engage at precisely the same vertical position, causing misalignment and binding.

**Solution:** Add a single, wider central male dovetail that can be locked with just one cam lock positioned centrally behind it. This provides stable, precise height adjustment with a single actuation point. The existing two male dovetails should have their grip notches (cam indexing grooves) removed to maintain smooth sliding when unclamped.

## Scope of Changes

### Files to Modify (build123d Python only - src/py/)

**All design work will be done in build123d Python files:**
- `src/py/music_desk_3b.py` - Upper Sliding Desk (add central wide male dovetail, remove grip notches from existing runners)
- `src/py/upper_desk_cam_lock.py` - Cam Lock Yoke (Part 5A & 5B)
- `src/py/music_desk_3a_single.py` - Lower Single Stationary Desk (add central female dovetail receiver)
- `src/py/music_desk_3a_dual.py` - Lower Dual Stationary Desk (add central female dovetail receiver)

**Note:** FeatureScript files (`src/fs/`) are legacy-only and will NOT be modified. They may become incompatible with vNext geometry but that's acceptable for now.

## Detailed Implementation Plan

### 1. Upper Desk Male Dovetails (Part 3B - music_desk_3b.py)

**Current State:**
- Two male runners at X = ±75mm, each ~18mm wide total (`mHWide = 9.0 - mFitTol`)
- Rear micro-groove indexing rack provides cam lock engagement points (lines 96-104)
- Grip notches are the V-shaped grooves cut every 2.5mm along both runners

**Changes:**
a) **Remove grip notches from existing X=±75mm runners** (Z = 0mm to 143mm):
   - Delete sections that create micro-groove indexing rack (lines 96-104 in current file)
   - This leaves smooth sliding surfaces for the existing cam locks

b) **Add central wide male dovetail at X = 0mm:**
   - Position: Centered between existing runners (X=0, Y from ~5.05mm to ~13.95mm)
   - Height: Z = 0mm to 143mm (same as existing runners)
   - Width: ~70mm total (significantly wider than current ~18mm runners)
     - `halfNeck_wide = 35.0 + mFitTol` (neck width at rear Y position)
     - `halfWide_wide = 42.0 + mFitTol` (flare width at front Y position)
   - Profile: Same 4-point trapezoid geometry as existing but wider
     - Front face (Y=mYFront): ±42mm wide  
     - Back face (Y=mYBack): ±35mm wide

c) **Add grip notches/cam indexing grooves along central dovetail ONLY:**
   - Same 45-degree V-groove pattern but centered only on the new wide dovetail
   - Depth: `gDepth = 0.65mm`, pitch: every 2.5mm from Z=10mm to ~122.5mm
   - This single central rack will be engaged by one cam lock

**Files:** `music_desk_3b.py`

### 2. Cam Lock Yoke Update (Part 5A - upper_desk_cam_lock.py)

**Current State:**
- Part 5A: C-yoke bracket spanning X = ±9.0mm relative to cam center
- `inHalfW = 16.25`, `outHalfW = 20.75` - sized for the narrow ~32mm wide female guides

**Changes:**
a) **Redesign Part 5A C-yoke for central dovetail engagement:**
   - New inner span to clear the wide central dovetail but grip its rear face
   - Recommended dimensions:
     - `inHalfW = 36.0` (inner clearance, just wider than neck width of new dovetail)
     - `outHalfW = 42.0` (outer arm thickness for structural rigidity)
     - `earOutX = 15.0` (rear cam-ear span to accommodate M3x20mm axle + nut)
   - Yoke height remains Z=0-14mm, depth aligned with existing runner profile

b) **Part 5B cam lever:** No changes needed - the eccentric profile already has teeth that will engage the new central dovetail's indexing grooves. The single cam lock positioned centrally behind the wide dovetail provides sufficient clamping force.

**Files:** `upper_desk_cam_lock.py`

### 3. Lower Single Stationary Desk (Part 3A-Single - music_desk_3a_single.py)

**Current State:**
- Two vertical female dovetail housings at X = ±75mm for riser and slider engagement
- Center angled riser receiver block at X=0 (64mm wide, 95mm tall)

**Changes:**
a) **Add central female dovetail receiver alongside existing riser block:**
   - Position: Centered at X=0, Z = 50mm to ~182mm (same height as vertical sliders)
   - Width: Designed to accept the new wide male dovetail from Part 3B
     - `halfNeck_receiver = 36.0 + slideTol` (to fit neck of wide dovetail with tolerance)
     - `halfWide_receiver = 43.0 + slideTol` (to flare with wide dovetail)
   - Depth: `guideDepth = 14mm` (same as existing vertical slide housings)
   - This receiver will be positioned forward of the angled riser block to avoid interference

**Files:** `music_desk_3a_single.py`

### 4. Lower Dual Stationary Desk (Part 3A-Dual - music_desk_3a_dual.py)

**Current State:**
- Two vertical female dovetail housings at X = ±75mm for each riser station
- Left and right angled riser receiver blocks at X = ±75mm

**Changes:**
a) **Add central female dovetail receiver between the two riser stations:**
   - Position: Centered at X=0, Z = 50mm to ~182mm
   - Same dimensions as single version above
   - Must clear both existing riser receiver blocks and maintain structural integrity

**Files:** `music_desk_3a_dual.py`

## Verification Steps

### Build123d Python Files:
```bash
# Test each modified file compiles without errors
python3 /workspace/ClampingMusicDesk/src/py/music_desk_3b.py
python3 /workspace/ClampingMusicDesk/src/py/upper_desk_cam_lock.py
python3 /workspace/ClampingMusicDesk/src/py/music_desk_3a_single.py
python3 /workspace/ClampingMusicDesk/src/py/music_desk_3a_dual.py

# Verify geometry with measure() calls after each change
```

### Assembly Verification:
1. Export STEP files for all modified parts
2. Import into CAD viewer and visually confirm fit between:
   - Part 3B central male dovetail ↔ Part 3A female receiver (single & dual)
   - Part 5A yoke engaging Part 3B central dovetail indexing grooves
3. Verify clearance: existing X=±75mm runners should slide smoothly without cam lock interference

## Design Considerations

1. **Tolerance Stack-up:** The new wide dovetail needs appropriate tolerances for smooth sliding while maintaining rigidity when locked. Using `mFitTol = 0.05mm` (same as current design).

2. **Structural Integrity:** Adding a central female receiver to Part 3A must not compromise the riser receiver blocks or vertical slide housings. May need internal ribbing or adjusted wall thicknesses.

3. **Cam Lock Reach:** The single central cam lock must reach far enough back to engage the wide dovetail's rear face when unlocked, and wedge tightly against it when locked. Verify eccentric profile geometry accommodates this.

4. **Printability:** Ensure all new features are printable without supports - central male dovetail should print flat on Z=0 bed; female receivers in Part 3A may require orientation adjustments.

5. **Backward Compatibility:** The existing X=±75mm runners remain functional for users who don't use the single-cam configuration, providing flexibility for different setups.

## Implementation Order

1. Start with `music_desk_3b.py` - add central wide male dovetail and remove grip notches from side runners
2. Test Part 3B geometry exports correctly
3. Update `upper_desk_cam_lock.py` - redesign Part 5A yoke for the new configuration
4. Test cam lock geometry
5. Update lower desk files (`music_desk_3a_single.py`, `music_desk_3a_dual.py`) to add female receivers
6. Final verification of all parts fitting together

## Commit Strategy

- Each functional change will be committed separately for easy review
- Include descriptive commit messages explaining the changes and rationale
- Push commits locally only (no remote push as per instructions)
