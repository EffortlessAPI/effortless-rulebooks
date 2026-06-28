#!/usr/bin/env python3
"""
Power Laws & Fractals - Interactive Demo

Just run:  python demo.py
"""

from rulebook import (
    load_sample_data, 
    build_systems_dict, 
    calculate_all_scales, 
    calculate_all_system_stats,
    validate_system
)

def main():
    # Load and calculate everything
    data = load_sample_data()
    systems = data['systems']
    scales = data['scales']
    stats = data['system_stats']
    
    systems_dict = build_systems_dict(systems)
    calculate_all_scales(scales, systems_dict)
    calculate_all_system_stats(stats, systems_dict, scales)
    
    print("\n" + "="*60)
    print("  🔺 POWER LAWS & FRACTALS - Python Demo")
    print("="*60)
    
    print("\n📊 Available Systems:\n")
    for i, s in enumerate(systems, 1):
        icon = "🔺" if s.class_ == "fractal" else "📈"
        print(f"  {i}. {icon} {s.display_name}")
        print(f"      Type: {s.class_}, Slope: {s.theoretical_log_log_slope}")
    
    print("\n" + "-"*60)
    print("✅ Validation Results:\n")
    
    all_valid = True
    for st in stats:
        valid = validate_system(st)
        status = "✓ PASS" if valid else "✗ FAIL"
        color_status = f"\033[92m{status}\033[0m" if valid else f"\033[91m{status}\033[0m"
        print(f"  {st._system_display_name:30} {color_status}")
        print(f"      Empirical: {st._empirical_log_log_slope:+.4f}  Theoretical: {st._theoretical_log_log_slope:+.4f}  Error: {st._slope_error:.6f}")
        if not valid:
            all_valid = False
    
    print("\n" + "-"*60)
    if all_valid:
        print("🎉 All systems validated! Empirical data matches theory.\n")
    else:
        print("⚠️  Some systems failed validation.\n")
    
    # Show how to explore further
    print("💡 To explore interactively, start Python and try:\n")
    print("   from rulebook import *")
    print("   data = load_sample_data()")
    print("   systems = data['systems']")
    print("   scales = data['scales']")
    print()
    print("   # Look at Sierpinski Triangle")
    print("   sierpinski = [s for s in scales if s.system == 'Sierpinski']")
    print("   for s in sierpinski:")
    print("       print(f'Iteration {s.iteration}: measure={s.measure}')")
    print()

if __name__ == '__main__':
    main()



