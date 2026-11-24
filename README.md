# **Clamping Music Desk for OnStage Keyboard Stand**

# ** WIP on version 2.0 right now -- this documentation has not yet been updated **

## Project Overview

A 3D-printed clamping music desk that can be assembled in multiple configurations (single-riser, dual-riser, full/low-profile, extra-tall) to hold sheet music or books. It attaches to an "OnStage" brand keyboard stand.
CAD model was built interactively with various local and cloud AI tools, targeting cloud OnShape (v1), and currently targeting local native build123d+FreeCad (vNext iterations).

### Key Technologies
- **OnShape** – cloud CAD platform with FeatureScript (V1, legacy)
- **build123d** – Python-based parametric geometry for vNext iterations
- **OrcaSlicer** – open-source slicer (used for layer height, infill, etc.)
- **Prusa CORE One** – the 3D printer used for the clamping music desk

### Project Structure
```
├── src/fs/             # V1 (legacy) OnShape FeatureScript sources
│   ├── l_clamp.fs           # L-Clamp parts
│   ├── riser_stem.fs        # Riser stem parts  
│   ├── music_desk_3a_single.fs  # Single riser lower desk
│   ├── music_desk_3a_dual.fs    # Dual riser lower desk
│   ├── music_desk_3b.fs         # Upper sliding desk
│   ├── page_clamps.fs           # Page clamp parts
│   └── upper_desk_cam_lock.fs   # Clamps upper desk to lower desk
├── src/py/             # vNext (current) build123d Python sources
│   ├── l_clamp.py
│   ├── narrow_clamp.py
│   ├── music_desk_3a_single.py
│   └── ...
├── src/py/ai_scratchpad/  # Build123d verification artifacts only (.png, .step exports)
├── .github/workflows/  # GitHub Actions workflows
│   └── convert-printing-guide.yml  # Converts PrintingGuide.md to Printables HTML
├── PrintingGuide.md    # Print & assembly guide (automatically updates PrintingGuide-printables.html for Printables.com)
```

- V1 uses OnShape FeatureScript; vNext uses build123d Python. New iterations develop in `src/py/`.

## Getting Started

### Prerequisites
| Tool | Minimum Version | Notes |
|------|-----------------|-------|
| **OnShape** | - | V1 rendering and stl exports |
| **build123d** python | 0.10.0 | vNext step/stl generation |
| **FreeCAD** | 1.0 | vNext manual visual inspection |
| **OrcaSlicer** | Latest release | https://github.com/OrcaSlicer/OrcaSlicer |
| **Prusa CORE One** | – | Ensure firmware is up-to-date |
| **Git** | 2.x | For cloning and version control |

### Installation (v1 / OnShape)
1. Clone the repo: `git clone https://github.com/wolenetz/ClampingMusicDesk.git`
2. Open in VS Code with Continue extension enabled (not required)
3. Open OnShape project and link the FeatureScript files from `src/fs/`
4. Export STL from OnShape for each part (see PrintingGuide.md)
5. Load into OrcaSlicer and apply slicer settings
6. Print on Prusa CORE One

### Installation (vNext / build123d - WIP)
Run Python scripts in `src/py/` to generate STEP/STL files, then inspect outputs for slicing/printing.

### Development Workflow (v1 / OnShape)
1. Create feature branch from `main`
2. Edit files in `src/fs/` 
3. Run FeatureScript unit tests with OnShape CLI (None created. Manual review.)
4. Export STL from OnShape
5. Slice with OrcaSlicer
6. Print on Prusa CORE One
7. Update PrintingGuide.md
8. Commit and submit pull request

### Development Workflow (vNext / build123d - WIP)
1. Create feature branch from `main`
2. Edit Python files in `src/py/`
3. Run scripts to generate STEP/STL outputs → placed in `ai_scratchpad/`
4. Inspect generated files for correctness (rendered PNGs and STEP exports)
5. Slice with OrcaSlicer and print on Prusa CORE One
6. Update PrintingGuide.md

7. Commit and submit pull request

**Note:** Build123d verification artifacts (*.png, *.step) should be placed in `src/py/ai_scratchpad/`. Only Python source files belong directly in `src/py/`.

### Printing Guide Note
- Changes to `PrintingGuide.md` will automatically update `PrintingGuide-printables.html` for use in the model description on Printables.com.
- Any such asynchronous changes may require a later pull to get the updated result from GitHub.

## References
- **build123d 0.10.0 docs** - https://build123d.readthedocs.io/en/v0.10.0/
- **OnShape FeatureScript Docs** – https://cad.onshape.com/docs/featureScript.html  
- **OrcaSlicer** – https://github.com/OrcaSlicer/OrcaSlicer
- **Prusa CORE One Manual** – https://manual.prusa3d.com/
