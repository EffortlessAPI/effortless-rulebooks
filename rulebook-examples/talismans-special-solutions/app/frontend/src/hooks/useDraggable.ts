import { useState, useCallback, useEffect, useRef } from "react";

// A tiny drag hook (extracted verbatim from App.jsx): grab the handle, move the
// card, drop it. Position is kept in viewport (fixed) coords and persisted to
// localStorage so it survives reloads. Returns pos=null until the user drags, so
// until then the card sits at its CSS default. Double-clicking the handle calls
// onReset -> back to null.
export interface DragPos {
  top: number;
  left: number;
}

export interface Draggable {
  pos: DragPos | null;
  dragging: boolean;
  onGrabHandle: (e: React.MouseEvent) => void;
  onReset: () => void;
}

export function useDraggable(storageKey: string): Draggable {
  const [pos, setPos] = useState<DragPos | null>(() => {
    try {
      const raw = localStorage.getItem(storageKey);
      return raw ? (JSON.parse(raw) as DragPos) : null;
    } catch {
      return null;
    }
  });
  const [dragging, setDragging] = useState(false);
  const drag = useRef<{ dx: number; dy: number } | null>(null); // pointer-offset within the card

  const onGrabHandle = useCallback((e: React.MouseEvent) => {
    // ignore right-clicks; only start on the handle itself
    if (e.button !== 0) return;
    const card = (e.currentTarget as HTMLElement).closest(".verdict-header") as HTMLElement | null;
    if (!card) return;
    const rect = card.getBoundingClientRect();
    drag.current = { dx: e.clientX - rect.left, dy: e.clientY - rect.top };
    setDragging(true);
    e.preventDefault();
  }, []);

  useEffect(() => {
    if (!dragging) return;
    const onMove = (e: MouseEvent) => {
      const { dx, dy } = drag.current || { dx: 0, dy: 0 };
      // clamp so the card can't be dragged fully off-screen (keep 40px in view)
      const maxLeft = window.innerWidth - 60;
      const maxTop = window.innerHeight - 40;
      const left = Math.max(0, Math.min(e.clientX - dx, maxLeft));
      const top = Math.max(0, Math.min(e.clientY - dy, maxTop));
      setPos({ top, left });
    };
    const onUp = () => {
      setDragging(false);
      setPos((p) => {
        try {
          if (p) localStorage.setItem(storageKey, JSON.stringify(p));
        } catch {
          /* persistence is best-effort */
        }
        return p;
      });
    };
    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
    return () => {
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
    };
  }, [dragging, storageKey]);

  const onReset = useCallback(() => {
    setPos(null);
    try {
      localStorage.removeItem(storageKey);
    } catch {
      /* best-effort */
    }
  }, [storageKey]);

  return { pos, dragging, onGrabHandle, onReset };
}
