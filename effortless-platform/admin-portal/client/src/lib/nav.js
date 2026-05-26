// Area metadata — colours, labels, icons. The rulebook carries NavArea per row;
// this file provides the display properties for each area value.
export const AREAS = {
  main:      { label: "Overview",  color: "#6ea8fe", icon: "Home" },
  developer: { label: "Developer", color: "#b48cff", icon: "Wrench" },
  admin:     { label: "Admin",     color: "#f3b03e", icon: "Shield" },
  docs:      { label: "Docs",      color: "#4ade80", icon: "BookOpen" },
};

export function areaOf(path) {
  if (path.startsWith("/developer")) return "developer";
  if (path.startsWith("/admin"))     return "admin";
  if (path.startsWith("/docs"))      return "docs";
  return "main";
}

export function roleMeetsMin(myRole, minRoleId, roles) {
  const min = roles.find((r) => r.RoleId === minRoleId);
  if (!min) return true;
  const tier = { read: 0, write: 1, "full-admin": 2 };
  const mine = tier[myRole?.AccessLevel] ?? 0;
  const req  = tier[min?.AccessLevel]    ?? 0;
  return mine >= req;
}
