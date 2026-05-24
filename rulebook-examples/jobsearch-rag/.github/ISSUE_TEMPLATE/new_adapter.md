---
name: New Board Adapter
about: Propose adding support for a new job board
title: "Adapter: [Board Name]"
labels: adapter, enhancement
assignees: ""
---

## Who / What / Why

- **WHO:** Who benefits from this adapter?
- **WHAT:** What board should be supported?
- **WHY:** Why is this board valuable? (volume, quality, unique listings, etc.)

## Board

- **Name:** [e.g., Indeed, Glassdoor]
- **URL:** [e.g., https://www.indeed.com]

## Data Access Strategy

Describe how job data is available on this board:

- [ ] Server-rendered HTML (CSS selectors work)
- [ ] Client-rendered SPA (data in JSON/script tags)
- [ ] API available (public or authenticated)
- [ ] Other: ...

## Authentication

- [ ] No authentication needed for search results
- [ ] Login required
- [ ] CAPTCHA / bot detection observed
- [ ] Special considerations: ...

## Sample URLs

```
Search: https://...
Detail: https://...
```

## Notes

Any rate limiting, detection mechanisms, or ToS considerations to be aware of.
