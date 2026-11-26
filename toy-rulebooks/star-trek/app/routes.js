// The app's addresses, one place for the browser and the tests.
//   /                                   home
//   /show/<code>                        a series
//   /show/<code>/season-<n>             a season
//   /show/<code>/season-<n>/episode-<n> an episode
//   /movie/<movie-id>                   a movie
(function (root) {
  function routeFor(path) {
    const p = (path || '/').replace(/\/+$/, '') || '/';
    let m;
    if (p === '/') return { page: 'home' };
    if ((m = p.match(/^\/show\/([^/]+)\/season-(\d+)\/episode-(\d+)$/))) {
      return { page: 'episode', show: m[1], season: Number(m[2]), episode: Number(m[3]) };
    }
    if ((m = p.match(/^\/show\/([^/]+)\/season-(\d+)$/))) {
      return { page: 'season', show: m[1], season: Number(m[2]) };
    }
    if ((m = p.match(/^\/show\/([^/]+)$/))) return { page: 'show', show: m[1] };
    if ((m = p.match(/^\/movie\/([^/]+)$/))) return { page: 'movie', movie: m[1] };
    return { page: 'not-found' };
  }

  if (typeof module !== 'undefined' && module.exports) module.exports = { routeFor };
  else root.routeFor = routeFor;
})(this);
