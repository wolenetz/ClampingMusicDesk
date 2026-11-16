NOTE: I'm pausing to reflect on whether or not I want to continue down the road of cloud-native CAD automation. I'm
considering alternatives to do this local-native in tools I haven't used before, to avoid API usage limits/metering/etc.

# **Clamping Music Desk for OnStage Keyboard Stand**

## Project Overview

A 3D-printed clamping music desk that can be assembled in multiple configurations (single-riser, dual-riser, full/low-profile, extra-tall) to hold sheet music or books. It attaches to an "OnStage" brand keyboard stand and is built from OnShape FeatureScript geometry.

### Key Technologies
- **OnShape** – cloud CAD platform with FeatureScript
- **FeatureScript** – JavaScript-like language for parametric geometry  
- **OrcaSlicer** – open-source slicer (used for layer height, infill, etc.)
- **Prusa CORE One** – the 3D printer used for the clamping music desk

### Project Structure
```
├── src/fs/             # FeatureScript sources
│   ├── l_clamp.fs           # L-Clamp parts
│   ├── riser_stem.fs        # Riser stem parts  
│   ├── music_desk_3a_single.fs  # Single riser lower desk
│   ├── music_desk_3a_dual.fs    # Dual riser lower desk
│   ├── music_desk_3b.fs         # Upper sliding desk
│   ├── page_clamps.fs           # Page clamp parts
│   └── upper_desk_cam_lock.fs   # Clamps upper desk to lower desk
├── PrintingGuide.md    # Print & assembly guide
└── .git/                # Git metadata
```

## Getting Started

### Prerequisites
| Tool | Minimum Version | Notes |
|------|-----------------|-------|
| **OnShape** | - | Project hosting |
| **OrcaSlicer** | Latest release | https://github.com/OrcaSlicer/OrcaSlicer |
| **Prusa CORE One** | – | Ensure firmware is up-to-date |
| **Git** | 2.x | For cloning and version control |

### Installation
1. Clone the repo: `git clone https://github.com/wolenetz/ClampingMusicDesk.git`
2. Open in VS Code with Continue extension enabled (not required)
3. Open OnShape project and link the FeatureScript files
4. Export STL from OnShape for each part (see PrintingGuide.md)
5. Load into OrcaSlicer and apply slicer settings
6. Print on Prusa CORE One

### Development Workflow
1. Create feature branch from `main`
2. Edit files in `src/fs/` 
3. Run FeatureScript unit tests with OnShape CLI (TODO, manual review currently)
4. Export STL from OnShape
5. Slice with OrcaSlicer
6. Print on Prusa CORE One
7. Commit and submit pull request

## Common Tasks
### Adding a New Part
1. Define new function in `src/fs/*.fs`
2. Update the set of OnShape feature studio tabs with the src/fs/*.fs
3. Export STL(s) from OnShape and add to slicer, 3D print updated part(s)
4. Update PrintingGuide.md

## References
- **OnShape FeatureScript Docs** – https://cad.onshape.com/docs/featureScript.html  
- **OrcaSlicer** – https://github.com/OrcaSlicer/OrcaSlicer
- **Prusa CORE One Manual** – https://manual.prusa3d.com/
