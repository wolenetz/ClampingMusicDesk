# Use this from vscode to show a build123d object in ocp_vscode extension (assuming you've set that up already)

from ocp_vscode import show, show_object, set_defaults, Camera

# Optional: set default viewing behavior
set_defaults(reset_camera=Camera.CENTER)
# Send the object(s) to the viewer
# show(part)
# Or pass multiple objects with custom names/colors:
# show(part1, part2, names=["Base", "Pin"], colors=["steelblue", "orange"])

def show_one(shape, shape_name="Shape"):
    part_name = getattr(shape, "label", None) or f"{shape_name}"
    show(shape, [part_name], colors=["steelblue"])

def show_children(shape, shape_name="Shape"):
    for i, child in enumerate(shape.children):
        show_one(child, f"{shape_name}_{i}")

# TODO: make my model parts provide a method by which I can easily query what part or parts
#       they contain. For now, hardcoding a subset.
import music_desk_3b

show_one(music_desk_3b.part3b)
