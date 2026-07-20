#!/usr/bin/env python3
"""Reproduce loop 711 on the unmodified TSPLIB GR17 distance matrix."""
from __future__ import annotations

import functools
import heapq
import json
import math
from pathlib import Path

N = 17
PUBLISHED_OPTIMUM = 2085
EXPECTED_HELD_KARP_STATES = 524288
EXPECTED_ENVELOPE_STATES = 21353

_VALUES = """0 633 257 91 412 150 80 134 259 505 353 324 70 211 268 246 121
633 0 390 661 227 488 572 530 555 289 282 638 567 466 420 745 518
257 390 0 228 169 112 196 154 372 262 110 437 191 74 53 472 142
91 661 228 0 383 120 77 105 175 476 324 240 27 182 239 237 84
412 227 169 383 0 267 351 309 338 196 61 421 346 243 199 528 297
150 488 112 120 267 0 63 34 264 360 208 329 83 105 123 364 35
80 572 196 77 351 63 0 29 232 444 292 297 47 150 207 332 29
134 530 154 105 309 34 29 0 249 402 250 314 68 108 165 349 36
259 555 372 175 338 264 232 249 0 495 352 95 189 326 383 202 236
505 289 262 476 196 360 444 402 495 0 154 578 439 336 240 685 390
353 282 110 324 61 208 292 250 352 154 0 435 287 184 140 542 238
324 638 437 240 421 329 297 314 95 578 435 0 254 391 448 157 301
70 567 191 27 346 83 47 68 189 439 287 254 0 145 202 289 55
211 466 74 182 243 105 150 108 326 336 184 391 145 0 57 426 96
268 420 53 239 199 123 207 165 383 240 140 448 202 57 0 483 153
246 745 472 237 528 364 332 349 202 685 542 157 289 426 483 0 336
121 518 142 84 297 35 29 36 236 390 238 301 55 96 153 336 0"""

_flat = [int(value) for value in _VALUES.split()]
assert len(_flat) == N * N
DIST = [_flat[index * N : (index + 1) * N] for index in range(N)]


def tour_cost(tour: list[int]) -> int:
    return sum(DIST[tour[index]][tour[(index + 1) % N]] for index in range(N))


def nearest_neighbor(start: int) -> list[int]:
    unvisited = set(range(N))
    unvisited.remove(start)
    tour = [start]
    while unvisited:
        next_city = min(unvisited, key=lambda city: (DIST[tour[-1]][city], city))
        tour.append(next_city)
        unvisited.remove(next_city)
    return tour


def two_opt(tour: list[int]) -> list[int]:
    tour = tour[:]
    changed = True
    while changed:
        changed = False
        for left in range(1, N - 1):
            for right in range(left + 1, N):
                a, b = tour[left - 1], tour[left]
                c, d = tour[right], tour[(right + 1) % N]
                if DIST[a][c] + DIST[b][d] < DIST[a][b] + DIST[c][d]:
                    tour[left : right + 1] = reversed(tour[left : right + 1])
                    changed = True
    return tour


def constructive_witness() -> list[int]:
    return min((two_opt(nearest_neighbor(start)) for start in range(N)), key=tour_cost)


def held_karp() -> tuple[int, int]:
    calls = 0

    @functools.lru_cache(maxsize=None)
    def solve(mask: int, last: int) -> int:
        nonlocal calls
        calls += 1
        bit = 1 << (last - 1)
        if mask == bit:
            return DIST[0][last]
        previous = mask ^ bit
        return min(
            solve(previous, city) + DIST[city][last]
            for city in range(1, N)
            if previous & (1 << (city - 1))
        )

    full = (1 << (N - 1)) - 1
    optimum = min(solve(full, city) + DIST[city][0] for city in range(1, N))
    return optimum, calls


@functools.lru_cache(maxsize=None)
def mst_cost(mask: int) -> int:
    nodes = [city for city in range(N) if mask & (1 << city)]
    if len(nodes) <= 1:
        return 0
    used = {nodes[0]}
    cheapest = {city: DIST[nodes[0]][city] for city in nodes[1:]}
    total = 0
    while len(used) < len(nodes):
        city = min((candidate for candidate in nodes if candidate not in used), key=lambda candidate: cheapest[candidate])
        total += cheapest[city]
        used.add(city)
        for candidate in nodes:
            if candidate not in used:
                cheapest[candidate] = min(cheapest[candidate], DIST[city][candidate])
    return total


def completion_envelope(last: int, unvisited: int) -> int:
    if not unvisited:
        return DIST[last][0]
    cities = [city for city in range(1, N) if unvisited & (1 << city)]
    return min(DIST[last][city] for city in cities) + mst_cost(unvisited) + min(DIST[city][0] for city in cities)


def envelope_search(incumbent: int) -> dict[str, int | bool]:
    unvisited = ((1 << N) - 1) ^ 1
    queue = [(completion_envelope(0, unvisited), 0, 0, unvisited)]
    best = {(0, unvisited): 0}
    expanded = pushed = 1
    pruned = 0
    while queue:
        bound, prefix, last, remaining = heapq.heappop(queue)
        if prefix != best.get((last, remaining)):
            continue
        if bound >= incumbent:
            pruned += 1
            continue
        expanded += 1
        if not remaining:
            return {"cheaper_tour_found": True, "value": prefix + DIST[last][0], "expanded": expanded, "pushed": pushed, "pruned": pruned}
        bits = remaining
        while bits:
            bit = bits & -bits
            bits -= bit
            city = bit.bit_length() - 1
            next_prefix = prefix + DIST[last][city]
            next_remaining = remaining ^ bit
            next_bound = next_prefix + completion_envelope(city, next_remaining)
            if next_bound >= incumbent:
                pruned += 1
                continue
            key = (city, next_remaining)
            if next_prefix < best.get(key, math.inf):
                best[key] = next_prefix
                heapq.heappush(queue, (next_bound, next_prefix, city, next_remaining))
                pushed += 1
    return {"cheaper_tour_found": False, "expanded": len(best), "pushed": pushed, "pruned": pruned}


def main() -> None:
    witness = constructive_witness()
    witness_value = tour_cost(witness)
    dynamic_value, dynamic_states = held_karp()
    proof = envelope_search(witness_value)

    assert witness_value == PUBLISHED_OPTIMUM
    assert dynamic_value == PUBLISHED_OPTIMUM
    assert dynamic_states == EXPECTED_HELD_KARP_STATES
    assert proof["cheaper_tour_found"] is False
    assert proof["expanded"] == EXPECTED_ENVELOPE_STATES

    result = {
        "instance": "TSPLIB gr17",
        "dimension": N,
        "witness": witness,
        "witness_value": witness_value,
        "held_karp_states": dynamic_states,
        "envelope_search": proof,
        "expanded_fraction_of_held_karp": proof["expanded"] / dynamic_states,
    }
    print(json.dumps(result, indent=2))


if __name__ == "__main__":
    main()
