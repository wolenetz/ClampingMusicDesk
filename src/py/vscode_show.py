# Use this from vscode to show a build123d object in ocp_vscode extension (assuming you've set that up already)

from ocp_vscode import show, show_object, set_defaults, Camera

# Optional: set default viewing behavior
set_defaults(reset_camera=Camera.CENTER)
# Send the object(s) to the viewer
# show(part)
# Or pass multiple objects with custom names/colors:
# show(part1, part2, names=["Base", "Pin"], colors=["steelblue", "orange"])


def show_multiple(shapes, shape_name="Shape"):
    part_names = []
    for i, shape in enumerate(shapes):
        part_names.append(getattr(shape, "label", None) or f"{shape_name}_{i}")
    show(shapes, part_names)

def show_one(shape, shape_name="Shape"):
    show_multiple([shape], shape_name)

def show_children(shape, shape_name="Shape"):
    show_multiple(shape.children)

def show_all_children(shapes, shape_name="Shape"):
    children = []
    for shape in shapes:
        children.append(shape)
    show_multiple(children, shape_name)

# TODO update these to understand if the part has children or not and enumerate them appropriately if a mix.

# TODO: make my model parts provide a method by which I can easily query what part or parts
#       they contain. For now, hardcoding a subset.
import music_desk_3b
#show_one(music_desk_3b.part3b)
import upper_desk_cam_lock
#show_children(upper_desk_cam_lock.camLockAssembly)
import music_desk_3a_single
#show_one(music_desk_3a_single.part3a)
show_multiple([music_desk_3a_single.part3a, music_desk_3b.part3b, *upper_desk_cam_lock.camLockAssembly.children])
