# Test Results: effortless-postgres

## Summary

| Metric | Value |
|--------|-------|
| Total Fields Tested | 81 |
| Passed | 6 |
| Failed | 75 |
| Score | 7.4% |
| Duration | 5s |

## Score by Field Class

| Class | Passed | Tested | Score |
|-------|--------|--------|-------|
| Scalar (calculated) | 4 | 33 | 12.1% |
| Lookup (INDEX/MATCH) | 0 | 36 | 0.0% |
| Aggregation (COUNTIFS/SUMIFS) | 2 | 12 | 16.7% |

## Error

```
Script failed: 
```

## Results by Entity

### 形状

- Fields: 4/27 (14.8%)
- Computed columns: 边的类型, 边标签, 边长, 边的数量, 边角, 角度总和, 最大边长, 非斜边角度总和, 勾股定理成立

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| 1 | 边的类型 | 三角形 | None |
| 1 | 边标签 | 阿, 比, 西 | None |
| 1 | 边长 | 3, 4, 5 | None |
| 1 | 边的数量 | 3 | None |
| 1 | 边角 | 37, 53, 90 | None |
| 1 | 角度总和 | 180 | None |
| 1 | 最大边长 | 5 | None |
| 1 | 非斜边角度总和 | 25 | None |
| 1 | 勾股定理成立 | True | None |
| 2 | 边的类型 | 正方形 | None |
| 2 | 边标签 | 阿, 比, 西, 迪 | None |
| 2 | 边长 | 5, 5, 5, 5 | None |
| 2 | 边的数量 | 4 | None |
| 2 | 边角 | 90, 90, 90, 90 | None |
| 2 | 角度总和 | 360 | None |
| 2 | 最大边长 | 5 | None |
| 3 | 边的类型 | 线 | None |
| 3 | 边标签 | 阿, 比 | None |
| 3 | 边长 | 14, 14 | None |
| 3 | 边的数量 | 2 | None |
| ... | ... | (3 more) | ... |

### 边

- Fields: 2/54 (3.7%)
- Computed columns: 名称, 形状的最大边长, 边的数量, 是否为斜边, 形状类型, 长度平方

| PK | Field | Expected | Actual |
|-----|-------|----------|--------|
| e1 | 名称 | 形状1-阿 | None |
| e1 | 形状的最大边长 | 5 | None |
| e1 | 边的数量 | 3 | None |
| e1 | 形状类型 | 三角形 | None |
| e1 | 长度平方 | 9 | None |
| e2 | 名称 | 形状1-比 | None |
| e2 | 形状的最大边长 | 5 | None |
| e2 | 边的数量 | 3 | None |
| e2 | 形状类型 | 三角形 | None |
| e2 | 长度平方 | 16 | None |
| e3 | 名称 | 形状1-西 | None |
| e3 | 形状的最大边长 | 5 | None |
| e3 | 边的数量 | 3 | None |
| e3 | 是否为斜边 | True | None |
| e3 | 形状类型 | 三角形 | None |
| e3 | 长度平方 | 25 | None |
| e4 | 名称 | 形状2-阿 | None |
| e4 | 形状的最大边长 | 5 | None |
| e4 | 边的数量 | 4 | None |
| e4 | 是否为斜边 | True | None |
| ... | ... | (32 more) | ... |
