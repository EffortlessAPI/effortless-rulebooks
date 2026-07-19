#!/usr/bin/env python3
from pathlib import Path
import subprocess
import sys

if len(sys.argv) != 2:
    raise SystemExit('usage: patch_calibration_647_710.py <executor>')

executor = Path(sys.argv[1])
text = executor.read_text()

old_finish = '''def finish_loop(rb: dict[str, Any], contract: dict[str, Any], order: int, after: str, witness: str) -> None:\n    base.finish_loop(rb, contract, order, after, witness)\n'''
new_finish = '''def finish_loop(rb: dict[str, Any], contract: dict[str, Any], order: int, after: str, witness: str) -> None:\n    loop = next(row for row in rows(rb, "TSPLoops") if int(row["LoopOrder"]) == order)\n    loop.update(\n        {\n            "Status": "CLOSED",\n            "AfterState": after,\n            "WitnessSummary": witness,\n            "CompletionDisposition": "CLOSED",\n            "NextFrontier": LOOPS[order]["next"],\n        }\n    )\n    contract_loop = next(row for row in contract["Loops"] if int(row["LoopOrder"]) == order)\n    contract_loop.update(\n        {\n            "Status": "CLOSED",\n            "AfterState": after,\n            "Result": witness,\n            "CompletionDisposition": "CLOSED",\n        }\n    )\n    set_meta(rb, "last_loop", "integer", integer=order)\n    set_meta(rb, "highest_completed_loop", "integer", integer=order)\n'''
if old_finish in text:
    text = text.replace(old_finish, new_finish)
elif '"NextFrontier": LOOPS[order]["next"]' not in text:
    raise AssertionError('calibration finish_loop patch target missing')

sparse_anchor = '''    register_derived(rb, f"coined-{spec['key']}-calibration-witness", f"{spec['family']} Calibration Witness", "SEMANTIC_ARC(instance,EXACT_VALUE,optimum)+SEMANTIC_ARC(instance,RESIDUAL_KERNEL,kernel)+WARRANTED_REWRITE(graph,deterministic_closure,MIXED,residue,structural_rules)", order, "TSPInstances,LocalDegreeBounds,InstanceLowerBounds,CandidateTours,TSPSearchCertificates,TSPDefectProfiles")\n'''
sparse_extra = sparse_anchor + '''    if spec["key"] == "sparse8":\n        register_derived(\n            rb,\n            "coined-witness-feasibility-independence",\n            "Witness Feasibility Independence",\n            "SEMANTIC_ARC(graph,COMPLETENESS,status)+SEMANTIC_ARC(witness,EDGE_ADMISSIBILITY,all_legs_available)+WARRANTED_REWRITE(witness,validate_without_global_completeness,CONTRACTIVE,HAMILTONIAN_CYCLE_WITNESS,coverage_and_leg_checks)",\n            order,\n            "TSPInstances,TravelEdges,CandidateTours,TourStops,TourLegs,reference_model.py",\n        )\n'''
if 'coined-witness-feasibility-independence' not in text:
    if sparse_anchor not in text:
        raise AssertionError('sparse feasibility concept patch target missing')
    text = text.replace(sparse_anchor, sparse_extra)

executor.write_text(text)

reference_model = executor.parent / 'reference_model.py'
reference_text = reference_model.read_text()
old_validity = '''        valid = (\n            graph[iid]["is_complete_undirected_graph"]\n            and len(ordered) == len(required_stop_ids)\n'''
new_validity = '''        valid = (\n            len(ordered) == len(required_stop_ids)\n'''
if old_validity in reference_text:
    reference_text = reference_text.replace(old_validity, new_validity)
elif 'valid = (\n            len(ordered) == len(required_stop_ids)' not in reference_text:
    raise AssertionError('reference-model sparse witness patch target missing')
reference_model.write_text(reference_text)
subprocess.run(['git', 'add', '--', str(reference_model)], check=True)

print('calibration finish_loop hotfix: PASS')
print('sparse witness feasibility independence hotfix: PASS')
