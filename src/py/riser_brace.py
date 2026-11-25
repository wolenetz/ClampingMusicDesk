import math
from build123d import *

# =========================================================================
# Single-Piece Riser-to-Riser Brace (40x30mm Risers) - Support-Free
# Sized to fit diagonally on a Prusa CORE One bed (250 x 220 x 270 mm)
# =========================================================================

# Riser & spacing parameters (in mm)
riser_w = 40.0               # Riser X-width (outer dimension)
riser_d = 30.0               # Riser Y-depth (front-to-back)
riser_clearance = 0.3        # Slip-fit clearance so collar slides onto riser cleanly
riser_corner_r = 3.0         # Fillet radius on riser vertical Z-edges
riser_center_dist_x = 240.0  # Center-to-center X distance between left & right risers

# Collar & brace dimensions (in mm)
collar_wall = 4.0            # 4mm wall -> 288mm total length (fits diagonally on 250x220 bed)
collar_h = 20.0              # Height of each riser clamp collar
crossbar_h = 16.0            # Vertical height of cross-tie beam (flush on bed at Z = 0)
crossbar_d = 14.0            # Front-to-back depth of cross-tie beam
gusset_len = 28.0            # Gusset length inward from each collar's inner face

# M4 pinch-bolt boss on the front (+Y) face of each collar (0mm penalty to diagonal fit)
boss_w = 14.0
boss_d = 5.0                 # Adds 5mm to collar_wall -> 9mm total thickness for M4 nut/insert
m4_clear = 4.4
m4_nut_af = 7.4              # Across-flats for captive M4 hex nut
m4_nut_h = 3.2

# Print bed orientation (40.5 deg fits ~239 x 211 mm on 250 x 220 mm CORE One bed)
print_diagonal_on_bed = True
bed_diag_angle = 40.5

pocket_w = riser_w + riser_clearance
pocket_d = riser_d + riser_clearance
outer_w = pocket_w + 2.0 * collar_wall
outer_d = pocket_d + 2.0 * collar_wall
pocket_r = max(0.5, min(riser_corner_r, 0.4 * min(pocket_w, pocket_d)))

half_span = 0.5 * riser_center_dist_x     # 120.0mm
inner_face_x = half_span - 0.5 * outer_w
span_len = 2.0 * inner_face_x
z_bot = 0.0                               # Flush on print bed -> zero supports needed
nut_rad = m4_nut_af / (2.0 * math.cos(math.radians(30.0)))

with BuildPart() as brace_builder:
    # 1. Two end collars at X = -120mm and X = +120mm
    for side in (-1.0, 1.0):
        cx = side * half_span

        # Outer collar body
        with BuildSketch(Plane(origin=(cx, 0.0, 0.0))) as collar_sk:
            RectangleRounded(outer_w, outer_d, radius=pocket_r + collar_wall)
        extrude(collar_sk.sketch, amount=collar_h, mode=Mode.ADD)

        # Local front (+Y) pinch-bolt boss centered on the collar
        boss_cy = 0.5 * outer_d + 0.5 * boss_d - 1.0
        with Locations((cx, boss_cy, 0.0)):
            Box(
                boss_w,
                boss_d + 2.0,
                collar_h,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.ADD,
            )

        # Riser pocket (40x30mm + clearance, with matching filleted Z corners)
        with BuildSketch(Plane(origin=(cx, 0.0, -1.0))) as pocket_sk:
            RectangleRounded(pocket_w, pocket_d, radius=pocket_r)
        extrude(pocket_sk.sketch, amount=collar_h + 2.0, mode=Mode.SUBTRACT)

        # M4 pinch-bolt through-hole along Y into the riser pocket
        bolt_start_y = 0.5 * pocket_d - 1.0
        with Locations(Plane(origin=(cx, bolt_start_y, 0.5 * collar_h), z_dir=(0, 1, 0))):
            Cylinder(
                radius=m4_clear / 2.0,
                height=collar_wall + boss_d + 4.0,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.SUBTRACT,
            )

        # Captive M4 hex nut pocket inserted from the top of the boss
        nut_y = 0.5 * pocket_d + 0.5 * (collar_wall + boss_d)
        with BuildSketch(Plane(origin=(cx, nut_y, 0.5 * collar_h), x_dir=(1, 0, 0), z_dir=(0, 1, 0))) as nut_sk:
            RegularPolygon(radius=nut_rad, side_count=6, major_radius=True, rotation=30.0)
        extrude(nut_sk.sketch, amount=m4_nut_h, both=True, mode=Mode.SUBTRACT)

        # Top drop-in slot for the M4 hex nut
        with Locations((cx, nut_y, 0.5 * collar_h)):
            Box(
                m4_nut_af,
                m4_nut_h,
                0.5 * collar_h + 2.0,
                align=(Align.CENTER, Align.CENTER, Align.MIN),
                mode=Mode.SUBTRACT,
            )

    # 2. Crossbar flush on the build plate (Z = 0..crossbar_h)
    with Locations((0.0, 0.0, z_bot)):
        Box(
            span_len,
            crossbar_d,
            crossbar_h,
            align=(Align.CENTER, Align.CENTER, Align.MIN),
            mode=Mode.ADD,
        )

    # 3. Gussets flush on the build plate (Z = 0..crossbar_h)
    clamped_gusset_len = min(gusset_len, 0.45 * span_len)
    for side in (-1.0, 1.0):
        base_x = side * inner_face_x
        tip_x = base_x - side * clamped_gusset_len
        y_half = 0.5 * crossbar_d
        with BuildSketch(Plane(origin=(0.0, 0.0, z_bot), x_dir=(1, 0, 0), z_dir=(0, 0, 1))) as g_sk:
            with BuildLine():
                Polyline(
                    [
                        (base_x, -y_half),
                        (base_x, y_half),
                        (tip_x, 0.0),
                        (base_x, -y_half),
                    ],
                    close=True,
                )
            make_face()
        extrude(g_sk.sketch, amount=crossbar_h, mode=Mode.ADD)

retrofit_brace = brace_builder.part

if print_diagonal_on_bed:
    retrofit_brace = retrofit_brace.rotate(Axis.Z, bed_diag_angle)

# Relocate to the left so it doesn't interfere with other assembly parts
retrofit_brace.position += (-170,0,0)

retrofit_brace.label = "Riser-to-Riser Cross Brace (40x30mm, CORE One 1-Piece)"

if __name__ == "__main__":
    bb = retrofit_brace.bounding_box()
    print(f"Print footprint: {bb.size.X:.1f} x {bb.size.Y:.1f} x {bb.size.Z:.1f} mm (CORE One bed: 250 x 220 x 270 mm)")
    export_step(retrofit_brace, "renders/riser_to_riser_brace_24cm.step")
    export_stl(retrofit_brace, "renders/riser_to_riser_brace_24cm.stl")