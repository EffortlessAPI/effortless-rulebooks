# Visualizer - Interactive Power Laws and Fractals Explorer

An interactive web-based visualization tool for exploring fractal systems and power-law distributions.

## Status: Planned

## Overview

This module will provide rich, interactive visualizations for understanding the behavior of fractal and power-law systems defined in the rulebook.

## Planned Features

### Core Visualizations

1. **Log-Log Plots**: Interactive scatter plots with zoom and pan
2. **Fractal Rendering**: Visual generation of fractal patterns
3. **System Comparison**: Side-by-side multi-system analysis
4. **Animation**: Step-by-step fractal construction
5. **3D Plots**: Three-dimensional fractal exploration

### Interactive Controls

- Parameter sliders for real-time updates
- System selection dropdown
- Iteration/scale range selector
- Export controls (PNG, SVG, PDF)
- Data table view toggle

### Data Integration

- Load systems from JSON/CSV
- Connect to REST API backend
- Real-time data updates
- Sample systems library

## Planned Technology Stack

### Primary Options

**Option 1: D3.js + React**
- D3.js for powerful data visualization
- React for component architecture
- TypeScript for type safety
- Tailwind CSS for styling

**Option 2: Observable**
- Observable Framework for reactive notebooks
- Built-in D3.js integration
- Easy sharing and embedding

**Option 3: Canvas + WebGL**
- HTML5 Canvas for 2D fractals
- Three.js for 3D visualization
- Maximum performance for complex fractals

## Planned UI Layout

```
┌─────────────────────────────────────────────────────┐
│  Power Laws and Fractals Visualizer                 │
├─────────────────────────────────────────────────────┤
│  [System Selector: ▼]  [Load Data]  [Export ▼]     │
├──────────────────┬──────────────────────────────────┤
│                  │                                   │
│  Controls:       │  Log-Log Plot                    │
│                  │  ┌────────────────────────────┐  │
│  Base Scale:     │  │                            │  │
│  [====○====] 1.0 │  │    •                       │  │
│                  │  │      •                     │  │
│  Scale Factor:   │  │        •                   │  │
│  [==○======] 0.5 │  │          •                 │  │
│                  │  │                            │  │
│  Iterations:     │  └────────────────────────────┘  │
│  [0] [1] [2] [3] │                                   │
│  [4] [5] [6] [7] │  Empirical Slope: -1.5847       │
│                  │  Theoretical Slope: -1.5850      │
│  [▶ Animate]     │  Error: 0.0003                   │
│                  │                                   │
├──────────────────┴──────────────────────────────────┤
│  Fractal Visualization                               │
│  ┌──────────────────────────────────────────────┐   │
│  │                                               │   │
│  │         [Sierpinski Triangle Rendering]      │   │
│  │                                               │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

## Example Features

### 1. Log-Log Plot

Interactive scatter plot showing the relationship between log(scale) and log(measure):

```javascript
// D3.js implementation sketch
const logLogPlot = {
    data: scales.map(s => ({
        x: Math.log10(s.scale),
        y: Math.log10(s.measure)
    })),

    render: function(svg) {
        const xScale = d3.scaleLinear()
            .domain(d3.extent(this.data, d => d.x))
            .range([0, width]);

        const yScale = d3.scaleLinear()
            .domain(d3.extent(this.data, d => d.y))
            .range([height, 0]);

        svg.selectAll("circle")
            .data(this.data)
            .join("circle")
            .attr("cx", d => xScale(d.x))
            .attr("cy", d => yScale(d.y))
            .attr("r", 5);

        // Add trend line
        const slope = calculateSlope(this.data);
        // ... render line
    }
};
```

### 2. Fractal Animation

Step-by-step construction of fractals:

```javascript
// Canvas-based Sierpinski triangle animation
function drawSierpinski(ctx, iteration, x, y, size) {
    if (iteration === 0) {
        // Draw base triangle
        drawTriangle(ctx, x, y, size);
    } else {
        const newSize = size / 2;
        drawSierpinski(ctx, iteration - 1, x, y, newSize);
        drawSierpinski(ctx, iteration - 1, x + newSize, y, newSize);
        drawSierpinski(ctx, iteration - 1, x + newSize/2, y - newSize*Math.sqrt(3)/2, newSize);
    }
}

// Animate through iterations
let currentIteration = 0;
function animate() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    drawSierpinski(ctx, currentIteration, 100, 400, 300);
    currentIteration = (currentIteration + 1) % 7;
    setTimeout(animate, 1000);
}
```

### 3. System Comparison

Side-by-side comparison of multiple systems:

```html
<div class="comparison-view">
    <div class="system-panel">
        <h3>Sierpinski Triangle</h3>
        <canvas id="sierpinski"></canvas>
        <div class="stats">
            <p>Slope: -1.585</p>
            <p>Error: 0.0003</p>
        </div>
    </div>
    <div class="system-panel">
        <h3>Koch Snowflake</h3>
        <canvas id="koch"></canvas>
        <div class="stats">
            <p>Slope: -0.262</p>
            <p>Error: 0.0001</p>
        </div>
    </div>
</div>
```

## Planned Directory Structure

```
visualizer/
├── README.md                    # This file
├── index.html                   # Main entry point
├── package.json                 # Dependencies
├── src/
│   ├── components/             # React components
│   │   ├── LogLogPlot.tsx
│   │   ├── FractalCanvas.tsx
│   │   ├── ControlPanel.tsx
│   │   └── SystemSelector.tsx
│   ├── lib/                    # Core libraries
│   │   ├── calculations.ts     # Calculation utilities
│   │   ├── rendering.ts        # Fractal rendering
│   │   └── data.ts             # Data loading
│   ├── styles/                 # CSS styles
│   │   └── main.css
│   └── data/                   # Sample data
│       └── systems.json
├── public/                      # Static assets
│   └── images/
└── dist/                        # Built files
```

## Implementation Plan

1. Basic HTML/CSS structure
2. D3.js log-log plot
3. Parameter controls
4. Data loading from JSON
5. Canvas-based fractal rendering
6. Animation controls
7. Export functionality
8. System comparison view
9. 3D visualization (stretch goal)
10. Responsive design

## Export Formats

- PNG: Raster images for presentations
- SVG: Vector graphics for publications
- PDF: Print-ready documents
- JSON: Data export
- CSV: Spreadsheet-compatible data

## Future Enhancements

- Real-time collaboration
- Annotation tools
- Custom system creation UI
- Integration with Jupyter notebooks
- Mobile-responsive design
- Progressive Web App (PWA)
- VR/AR fractal exploration
