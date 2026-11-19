NOTE: I'm pausing to reflect on whether or not I want to continue down the road of cloud-native CAD automation. I'm
considering alternatives to do this local-native in tools I haven't used before, to avoid API usage limits/metering/etc.

# **Clamping Music Desk for OnStage Keyboard Stand**

## Project Overview

A 3D-printed clamping music desk that can be assembled in multiple configurations (single-riser, dual-riser, full/low-profile, extra-tall) to hold sheet music or books. It attaches to an "OnStage" brand keyboard stand and is built from cloud-native CAD platforms.

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
├── .github/workflows/  # GitHub Actions workflows
│   └── convert-printing-guide.yml  # Converts PrintingGuide.md to Printables HTML
├── PrintingGuide.md    # Print & assembly guide (automatically updates PrintingGuide-printables.html for Printables.com)
```

- V1 uses OnShape FeatureScript; vNext uses build123d Python. New iterations develop in `src/py/`.

## Getting Started

### Prerequisites
| Tool | Minimum Version | Notes |
|------|-----------------|-------|
| **OnShape** | - | Project hosting |
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
3. Run FeatureScript unit tests with OnShape CLI (TODO, manual review currently)
4. Export STL from OnShape
5. Slice with OrcaSlicer
6. Print on Prusa CORE One
7. Commit and submit pull request

### Development Workflow (vNext / build123d - WIP)
1. Create feature branch from `main`
2. Edit Python files in `src/py/`
3. Run scripts to generate STEP/STL outputs
4. Inspect generated files for correctness
5. Slice with OrcaSlicer and print on Prusa CORE One
6. Commit and submit pull request
   - Note: Changes to `PrintingGuide.md` will automatically update `PrintingGuide-printables.html` for use in the model description on Printables.com.
   - _Note: Any such asynchronous changes may require a later pull to get the updated result from GitHub._

## Common Tasks
### Adding a New Part (v1 / OnShape)
1. Define new function in `src/fs/*.fs`
2. Update the set of OnShape feature studio tabs with the src/fs/*.fs
3. Export STL(s) from OnShape and add to slicer, 3D print updated part(s)
4. Update PrintingGuide.md

### Adding a New Part (vNext / build123d - WIP)
- Define new function in `src/py/*.py`
- Run script to generate STEP/STL outputs
- Inspect and slice for printing, then update PrintingGuide.md

## References
- **OnShape FeatureScript Docs** – https://cad.onshape.com/docs/featureScript.html  
- **OrcaSlicer** – https://github.com/OrcaSlicer/OrcaSlicer
- **Prusa CORE One Manual** – https://manual.prusa3d.com/
