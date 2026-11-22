# Don't use this unless you're in FreeCAD's python panel.
# If intending to debug using vscode with ocp_vscode, instead use vscode_show.py script
# To use this, import it into your freecad 1.0 python panel.
#
# Helpful tip: 
"""
import sys
if "/workspace/ClampingMusicDesk/src/py" not in sys.path:
    sys.path.append("/workspace/ClampingMusicDesk/src/py")

from bd_helpers import show_bd, show_bd_children, load_bd
"""
# Then, assuming your module containing one or more models is "M", do:
# import M
# import importlib
# importlib.reload(M) # do this bit whenever M has been changed elsewhere, eg editing it, to force cache refresh
#
# Then, try it like:
#
# show_bd(l_clamp.lClampAssembly, "lClampAssembly")
#
# or, even better:
#
# show_bd_children(l_clamp.lClampAssembly, "L-Clamp")


import io
import re
import FreeCAD as App
import FreeCADGui as Gui
import Part
import build123d as bd

import importlib
import sys
import types
# use this one like so:
# modA = load_bd('modA')
#...
# show_bd_children(modA.modAAssemblyName) # for example...
def load_bd(module_name: str) -> types.ModuleType:
    """Import a module by name, or reload it if it is already in sys.modules."""
    if module_name in sys.modules and sys.modules[module_name] is not None:
        return importlib.reload(sys.modules[module_name])
    return importlib.import_module(module_name)

def show_bd(shape, name="Shape", clear_all=False):
    buf = io.BytesIO()
    bd.export_brep(shape, buf)

    fc_shape = Part.Shape()
    fc_shape.importBrepFromString(buf.getvalue().decode("ascii"))

    doc = App.ActiveDocument or App.newDocument("CAD")

    if clear_all:
        for o in list(doc.Objects):
            doc.removeObject(o.Name)

    # Sanitize internal Name so getObject() always matches what addObject() creates
    internal_name = re.sub(r"[^A-Za-z0-9_]", "_", name)
    if internal_name[0].isdigit():
        internal_name = "_" + internal_name

    # Remove any numbered duplicates (e.g., lClampAssembly001) from previous runs
    # for o in list(doc.Objects):
        # if o.Name != internal_name and (o.Label.startswith(name) or o.Name.startswith(internal_name)):
            # doc.removeObject(o.Name)

    obj = doc.getObject(internal_name) or doc.addObject("Part::Feature", internal_name)
    obj.Label = name
    obj.Shape = fc_shape
    doc.recompute()

    if App.GuiUp and Gui.activeDocument() and Gui.activeDocument().activeView():
        Gui.activeDocument().activeView().fitAll()


def show_bd_children(shape, shape_name="Shape", clear_all=False):
    for i, child in enumerate(shape.children):
        part_name = getattr(child, "label", None) or f"{shape_name}_{i}"
        if clear_all and i == 0:
            show_bd(child, part_name, True)
        else:
            show_bd(child, part_name, False)
