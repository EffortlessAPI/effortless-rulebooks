# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 2084 |
| Passed | 2084 |
| Failed | 0 |
| Score | 100.0% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 1228 | 1228 | 100.0% |
| Lookup (INDEX/MATCH) | 764 | 764 | 100.0% |
| Aggregation (COUNTIFS/SUMIFS) | 92 | 92 | 100.0% |

## Results by Entity

### systems

- Fields: 56/56 (100.0%)
- Computed columns: empirical_fit_quality, empirical_slope_deviation, measurement_noise_level, data_quality_score, relative_slope_error, is_high_quality_fit, scale_range_span, measure_range_span

### scales

- Fields: 728/728 (100.0%)
- Computed columns: base_scale, scale_factor, scale_factor_power, scale, log_scale, log_measure, theoretical_log_log_slope, empirical_log_log_slope, system_min_log_scale, system_max_log_scale, system_delta_log_scale, scale_ratio, log_scale_normalized

### system_stats

- Fields: 161/161 (100.0%)
- Computed columns: system_display_name, theoretical_log_log_slope, point_count, min_log_scale, max_log_scale, min_log_measure, max_log_measure, delta_log_measure, delta_log_scale, empirical_log_log_slope, slope_error, fitted_slope, fitted_vs_empirical_delta, r2, quality_weighted_slope, residual_rms, noise_sigma, slope_to_noise_ratio, deviation_score, abs_delta_log_measure, log_log_area, data_density, relative_slope_error

### measurement_models

- Fields: 63/63 (100.0%)
- Computed columns: mean_absolute_residual, outlier_count, total_point_count, outlier_rate, effective_point_count, residual_rms_from_inference, cutoff_log_min_scale, cutoff_log_max_scale, cutoff_range_span

### observed_scales

- Fields: 896/896 (100.0%)
- Computed columns: base_scale, scale_factor, scale_factor_power, scale, log_scale, log_measure, fitted_slope, fitted_intercept, residual_rms, predicted_log_measure, residual, residual_squared, standardized_residual, is_outlier, scale_ratio, abs_residual

### inference_runs

- Fields: 140/140 (100.0%)
- Computed columns: slope_confidence_interval, min_log_scale, max_log_scale, min_log_measure, max_log_measure, log_measure_range, abs_slope_delta, slope_is_significant, one_plus_residual_rms, fit_efficiency, normalized_rmse, slope_to_theoretical_ratio, one_minus_r2, point_count_minus_one, point_count_minus_two, adjusted_r2, residual_rms_squared, log_residual_rms_squared, log_point_count, bic

### scale_regimes

- Fields: 40/40 (100.0%)
- Computed columns: regime_span, regime_center, theoretical_log_log_slope, slope_deviation_from_global, points_in_regime
