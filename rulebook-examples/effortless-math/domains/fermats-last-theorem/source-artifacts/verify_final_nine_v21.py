#!/usr/bin/env python3
import json,sqlite3,sys
from pathlib import Path
root=Path(__file__).resolve().parent
con=sqlite3.connect(root/'fermat_witness_v21.sqlite')
con.row_factory=sqlite3.Row
expected={
 'analytic_chebotarev_density_available':'derived-from-prime-distribution-kernel',
 'hilbert_irreducibility_weak_approximation_available':'derived-from-hilbert-specialization-kernel',
 'mazur_formal_immersion_eisenstein_kernel_available':'derived-from-mazur-formal-kernel',
 'deformation_problem_represented':'derived-from-deformation-duality-kernel',
 'poitou_tate_balance_available':'derived-from-deformation-duality-kernel',
 'modular_curve_geometric_comparison_available':'derived-from-modular-curve-comparison-kernel',
 'semistable_nearby_cycles_comparison_available':'derived-from-modular-curve-comparison-kernel',
 'level_2_weight_2_newform_exists':'derived-from-universal-level-lowering-kernel',
 'solvable_base_change_converse_available':'derived-from-solvable-automorphy-kernel',
}
actual={r['derived_fact']:r['derivation_kind'] for r in con.execute(
 "select derived_fact,derivation_kind from proof_trace where derived_fact in ("
 "'analytic_chebotarev_density_available','hilbert_irreducibility_weak_approximation_available',"
 "'mazur_formal_immersion_eisenstein_kernel_available','deformation_problem_represented',"
 "'poitou_tate_balance_available','modular_curve_geometric_comparison_available',"
 "'semistable_nearby_cycles_comparison_available','level_2_weight_2_newform_exists',"
 "'solvable_base_change_converse_available')")}
parents=con.execute("select count(*) from nine_parent_audit_v21 where parent_removed=1").fetchone()[0]
active=con.execute("select count(*) from import_surface where load_bearing=1").fetchone()[0]
kernels=con.execute("select count(*) from foundation_kernels_v21 where theorem_content_internalized=0").fetchone()[0]
shared=con.execute("select count(*) from foundation_kernels_v21 where kernel_id in ('kernel-global-deformation-duality-v21','kernel-modular-curve-comparison-v21')").fetchone()[0]
contr=con.execute("select count(*) from proof_trace where contradiction=1").fetchone()[0]
ok=(actual==expected and parents==9 and active==7 and kernels==7 and shared==2 and contr==1)
print('trace:',json.dumps(actual,sort_keys=True))
print('parents removed:',parents)
print('active foundation kernels:',active)
print('not fully internalized kernels:',kernels)
print('shared kernels:',shared)
print('contradictions:',contr)
print('pass:',ok)
sys.exit(0 if ok else 1)
