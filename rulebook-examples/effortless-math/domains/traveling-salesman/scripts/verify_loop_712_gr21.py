#!/usr/bin/env python3
"""Reproduce loop 712 on the unmodified TSPLIB GR21 matrix."""
from __future__ import annotations

import functools
import heapq
import math

N = 21
PUBLISHED_OPTIMUM = 2707
EXPECTED_STATES = 4352
EXPECTED_HELD_KARP_STATES = (N - 1) * (1 << (N - 2))

LOWER_DIAG = [int(x) for x in """0 510 0 635 355 0 91 415 605 0 385 585 390 350 0 155 475 495 120 240 0 110 480 570 78 320 96 0 130 500 540 97 285 36 29 0 490 605 295 460 120 350 425 390 0 370 320 700 280 590 365 350 370 625 0 155 380 640 63 430 200 160 175 535 240 0 68 440 575 27 320 91 48 67 430 300 90 0 610 360 705 520 835 605 590 610 865 250 480 545 0 655 235 585 555 750 615 625 645 775 285 515 585 190 0 480 81 435 380 575 440 455 465 600 245 345 415 295 170 0 265 480 420 235 125 125 200 165 230 475 310 205 715 650 475 0 255 440 755 235 650 370 320 350 680 150 175 265 400 435 385 485 0 450 270 625 345 660 430 420 440 690 77 310 380 180 215 190 545 225 0 170 445 750 160 495 265 220 240 600 235 125 170 485 525 405 375 87 315 0 240 290 590 140 480 255 205 220 515 150 100 170 390 425 255 395 205 220 155 0 380 140 495 280 480 340 350 370 505 185 240 310 345 280 105 380 280 165 305 150 0""".split()]
assert len(LOWER_DIAG) == N * (N + 1) // 2
DIST = [[0] * N for _ in range(N)]
k = 0
for row in range(N):
    for column in range(row + 1):
        DIST[row][column] = DIST[column][row] = LOWER_DIAG[k]
        k += 1


def tour_cost(tour: list[int]) -> int:
    return sum(DIST[tour[i]][tour[(i + 1) % N]] for i in range(N))


def nearest_neighbor(start: int) -> list[int]:
    remaining = set(range(N))
    remaining.remove(start)
    tour = [start]
    while remaining:
        city = min(remaining, key=lambda candidate: (DIST[tour[-1]][candidate], candidate))
        tour.append(city)
        remaining.remove(city)
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
                    tour[left:right + 1] = reversed(tour[left:right + 1])
                    changed = True
    return tour


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


def envelope(last: int, unvisited: int) -> int:
    if not unvisited:
        return DIST[last][0]
    cities = [city for city in range(1, N) if unvisited & (1 << city)]
    return min(DIST[last][city] for city in cities) + mst_cost(unvisited) + min(DIST[city][0] for city in cities)


def exact_search(incumbent: int) -> dict[str, int | bool]:
    remaining = ((1 << N) - 1) ^ 1
    queue = [(envelope(0, remaining), 0, 0, remaining)]
    best = {(0, remaining): 0}
    pushed = 1
    pruned = 0
    while queue:
        bound, prefix, last, unvisited = heapq.heappop(queue)
        if prefix != best.get((last, unvisited)):
            continue
        if bound >= incumbent:
            pruned += 1
            continue
        bits = unvisited
        while bits:
            bit = bits & -bits
            bits -= bit
            city = bit.bit_length() - 1
            next_prefix = prefix + DIST[last][city]
            next_unvisited = unvisited ^ bit
            next_bound = next_prefix + envelope(city, next_unvisited)
            if next_bound >= incumbent:
                pruned += 1
                continue
            key = (city, next_unvisited)
            if next_prefix < best.get(key, math.inf):
                best[key] = next_prefix
                heapq.heappush(queue, (next_bound, next_prefix, city, next_unvisited))
                pushed += 1
    return {"cheaper_tour_found": False, "states": len(best), "pushed": pushed, "pruned": pruned}


def main() -> None:
    witness = min((two_opt(nearest_neighbor(start)) for start in range(N)), key=tour_cost)
    witness_value = tour_cost(witness)
    proof = exact_search(witness_value)
    assert witness_value == PUBLISHED_OPTIMUM
    assert proof["cheaper_tour_found"] is False
    assert proof["states"] == EXPECTED_STATES
    print({
        "instance": "TSPLIB gr21",
        "witness": witness,
        "witness_value": witness_value,
        "quotient_states": proof["states"],
        "held_karp_states": EXPECTED_HELD_KARP_STATES,
        "percentage": 100 * proof["states"] / EXPECTED_HELD_KARP_STATES,
        "pruned": proof["pruned"],
    })


if __name__ == "__main__":
    main()
