//
// Data Models
//
// Auto-generated from rulebook
// Model: FractalsAndPowerLaws_CMCC
//
// Represents the core data structures for fractal and power-law systems
//

package rulebook

import (
	"math"
)

// System represents a fractal or power-law system
type System struct {
	SystemID               string   `json:"SystemID"`
	DisplayName            string   `json:"DisplayName"`
	Class                  string   `json:"Class"`
	BaseScale              float64  `json:"BaseScale"`
	ScaleFactor            float64  `json:"ScaleFactor"`
	MeasureName            string   `json:"MeasureName"`
	FractalDimension       *float64 `json:"FractalDimension"`
	TheoreticalLogLogSlope float64  `json:"TheoreticalLogLogSlope"`
}

// Scale represents a scale measurement with computed values
type Scale struct {
	ScaleID     string  `json:"ScaleID"`
	System      string  `json:"System"`
	Iteration   int     `json:"Iteration"`
	Measure     float64 `json:"Measure"`
	IsProjected bool    `json:"IsProjected"`

	// Computed values (nil until calculated)
	baseScale        *float64
	scaleFactor      *float64
	scaleFactorPower *float64
	scale            *float64
	logScale         *float64
	logMeasure       *float64
}

// SystemsMap is a lookup dictionary for systems by ID
type SystemsMap map[string]*System

// GetBaseScale returns the cached BaseScale or computes it
func (s *Scale) GetBaseScale() float64 {
	if s.baseScale != nil {
		return *s.baseScale
	}
	return 0
}

// GetScaleFactor returns the cached ScaleFactor or computes it
func (s *Scale) GetScaleFactor() float64 {
	if s.scaleFactor != nil {
		return *s.scaleFactor
	}
	return 0
}

// GetScaleFactorPower returns the cached ScaleFactorPower or computes it
func (s *Scale) GetScaleFactorPower() float64 {
	if s.scaleFactorPower != nil {
		return *s.scaleFactorPower
	}
	return 0
}

// GetScale returns the cached Scale or computes it
func (s *Scale) GetScale() float64 {
	if s.scale != nil {
		return *s.scale
	}
	return 0
}

// GetLogScale returns the cached LogScale or computes it
func (s *Scale) GetLogScale() float64 {
	if s.logScale != nil {
		return *s.logScale
	}
	return 0
}

// GetLogMeasure returns the cached LogMeasure or computes it
func (s *Scale) GetLogMeasure() float64 {
	if s.logMeasure != nil {
		return *s.logMeasure
	}
	return 0
}

// CalculateBaseScale looks up BaseScale from parent system
func (s *Scale) CalculateBaseScale(systems SystemsMap) float64 {
	if s.baseScale == nil {
		if system, ok := systems[s.System]; ok {
			s.baseScale = &system.BaseScale
		} else {
			zero := 0.0
			s.baseScale = &zero
		}
	}
	return *s.baseScale
}

// CalculateScaleFactor looks up ScaleFactor from parent system
func (s *Scale) CalculateScaleFactor(systems SystemsMap) float64 {
	if s.scaleFactor == nil {
		if system, ok := systems[s.System]; ok {
			s.scaleFactor = &system.ScaleFactor
		} else {
			zero := 0.0
			s.scaleFactor = &zero
		}
	}
	return *s.scaleFactor
}

// CalculateScaleFactorPower computes ScaleFactor ^ Iteration
func (s *Scale) CalculateScaleFactorPower() float64 {
	if s.scaleFactorPower == nil {
		result := math.Pow(s.GetScaleFactor(), float64(s.Iteration))
		s.scaleFactorPower = &result
	}
	return *s.scaleFactorPower
}

// CalculateScale computes BaseScale * ScaleFactorPower
func (s *Scale) CalculateScale() float64 {
	if s.scale == nil {
		result := s.GetBaseScale() * s.GetScaleFactorPower()
		s.scale = &result
	}
	return *s.scale
}

// CalculateLogScale computes log10(Scale)
func (s *Scale) CalculateLogScale() float64 {
	if s.logScale == nil {
		scale := s.GetScale()
		var result float64
		if scale > 0 {
			result = math.Log10(scale)
		} else {
			result = 0
		}
		s.logScale = &result
	}
	return *s.logScale
}

// CalculateLogMeasure computes log10(Measure)
func (s *Scale) CalculateLogMeasure() float64 {
	if s.logMeasure == nil {
		var result float64
		if s.Measure > 0 {
			result = math.Log10(s.Measure)
		} else {
			result = 0
		}
		s.logMeasure = &result
	}
	return *s.logMeasure
}

// CalculateAllFields computes all derived values in dependency order
func (s *Scale) CalculateAllFields(systems SystemsMap) {
	s.CalculateBaseScale(systems)
	s.CalculateScaleFactor(systems)
	s.CalculateScaleFactorPower()
	s.CalculateScale()
	s.CalculateLogScale()
	s.CalculateLogMeasure()
}

// ToOutputMap converts Scale to a map for JSON output (rounded to 6 decimal places)
func (s *Scale) ToOutputMap() map[string]interface{} {
	return map[string]interface{}{
		"ScaleID":          s.ScaleID,
		"System":           s.System,
		"Iteration":        s.Iteration,
		"Measure":          roundTo(s.Measure, 6),
		"BaseScale":        roundTo(s.GetBaseScale(), 6),
		"ScaleFactor":      roundTo(s.GetScaleFactor(), 6),
		"ScaleFactorPower": roundTo(s.GetScaleFactorPower(), 6),
		"Scale":            roundTo(s.GetScale(), 6),
		"LogScale":         roundTo(s.GetLogScale(), 6),
		"LogMeasure":       roundTo(s.GetLogMeasure(), 6),
		"IsProjected":      s.IsProjected,
	}
}

// roundTo rounds a float to a specified number of decimal places
func roundTo(val float64, places int) float64 {
	factor := math.Pow(10, float64(places))
	return math.Round(val*factor) / factor
}
