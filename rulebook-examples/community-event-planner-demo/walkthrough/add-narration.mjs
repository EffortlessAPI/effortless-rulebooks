// Generate voiceover narration with macOS `say`, then mux it onto
// walkthrough-silent.webm via ffmpeg to produce walkthrough.mp4.
//
// The narration is timecoded — each segment starts at a specific second of
// the video, matching the step boundaries in record-walkthrough.mjs. We pad
// individual TTS clips with silence so each line lands on its start cue.
import { execFileSync, execSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import url from 'node:url';

const __dirname = path.dirname(url.fileURLToPath(import.meta.url));
const SILENT_VIDEO = path.join(__dirname, 'walkthrough-silent.webm');
const FINAL_VIDEO  = path.join(__dirname, 'walkthrough.mp4');
const AUDIO_DIR    = path.join(__dirname, '_audio');
const FULL_AUDIO   = path.join(AUDIO_DIR, 'narration.m4a');
const SRT_FILE     = path.join(__dirname, 'narration.srt');
const TXT_FILE     = path.join(__dirname, 'narration.txt');

if (!fs.existsSync(SILENT_VIDEO)) {
  console.error(`✘ ${SILENT_VIDEO} not found — run record-walkthrough.mjs first.`);
  process.exit(1);
}
if (!fs.existsSync(AUDIO_DIR)) fs.mkdirSync(AUDIO_DIR, { recursive: true });
for (const f of fs.readdirSync(AUDIO_DIR)) fs.unlinkSync(path.join(AUDIO_DIR, f));

// Each line: start time in seconds, text. Lines must not overlap; if a step's
// narration is too long, shorten it rather than letting it bleed into the next.
const NARRATION = [
  { t:   0, text: "Welcome to the Community Event Planner. Every number on screen comes from a Postgres view generated from one rulebook." },
  { t:   6, text: "Header-based login with four dev roles. We'll pick Admin so we can show the edit flow later." },
  { t:  12, text: "The dashboard. Four stat boxes — each an aggregation over the Events table: total, ready to publish, attendees, and events with issues." },
  { t:  22, text: "Below, four upcoming events. Each row shows speakers, attendees, seats left, and a calculated status. Yellow flags mark venue conflicts." },
  { t:  32, text: "Now the interesting part. I turn on provenance mode. Every derived value gets a function glyph showing it was computed, not stored." },
  { t:  40, text: "Clicking 'Ready to Publish' opens the explainer for Events dot EventStatus. The formula is an IF over HasSpeakers, AtCapacity, and HasVenueConflict — three sub-calculations with their own DAGs." },
  { t:  52, text: "Back, then 'Total Attendees'. This opens BookedCapacity — a COUNTIFS aggregation over RSVPs filtered to confirmed status." },
  { t:  62, text: "Back to the dashboard. Provenance off, and into tech-talk-1." },
  { t:  70, text: "The Event Status block breaks the status into a checklist: speakers, capacity, venue conflicts. Every check or X is itself a calculated field." },
  { t:  82, text: "Further down, Registration. Deadline and Open or Closed are both calculated from the close-days setting and the current time." },
  { t:  92, text: "As admin I get a pencil to edit. The form changes raw inputs — name, venue, date, duration, close window, category." },
  { t: 104, text: "Watch this: I change Registration Closes from one day to thirty days before. One raw field, RegistrationCloseDaysBeforeEvent." },
  { t: 116, text: "Save fires a PATCH. The view recomputes. UI updates — no frontend math." },
  { t: 122, text: "Now the Speakers tab — a calculated view over the Speakers table." },
  { t: 130, text: "Each speaker shows an AssignmentCount aggregation. If anyone went over three, the page would surface an Overbooked warning at the top." },
  { t: 140, text: "Last hop: clicking AssignmentCount opens its DAG — an aggregation over Assignments. Every number you saw came from the rulebook." },
];

// Total duration we want the audio to cover, in seconds. We probe the video
// for its actual length and pad to that.
function probeVideoSeconds(file) {
  const out = execSync(
    `ffprobe -v error -show_entries format=duration -of csv=p=0 "${file}"`
  ).toString().trim();
  return parseFloat(out);
}
const VIDEO_SECONDS = probeVideoSeconds(SILENT_VIDEO);
console.log(`📹 Video length: ${VIDEO_SECONDS.toFixed(1)}s`);

// ── Generate per-segment audio with macOS `say` ────────────────────────────────
// `say` writes AIFF natively; we then convert each to WAV at 44.1k mono.
console.log(`🗣  Synthesizing ${NARRATION.length} narration segments via macOS \`say\`…`);
NARRATION.forEach((seg, i) => {
  const aiff = path.join(AUDIO_DIR, `seg-${String(i).padStart(2, '0')}.aiff`);
  const wav  = path.join(AUDIO_DIR, `seg-${String(i).padStart(2, '0')}.wav`);
  execFileSync('say', ['-r', '180', '-v', 'Samantha', '-o', aiff, seg.text]);
  execSync(`ffmpeg -y -loglevel error -i "${aiff}" -ar 44100 -ac 1 "${wav}"`);
  fs.unlinkSync(aiff);
});

// Probe each segment's duration so we know when its end lands; if it would
// overrun the next cue, we warn (but keep going — the next cue just gets pushed).
const durations = NARRATION.map((_, i) => {
  const wav = path.join(AUDIO_DIR, `seg-${String(i).padStart(2, '0')}.wav`);
  return probeVideoSeconds(wav);
});
NARRATION.forEach((seg, i) => {
  const next = NARRATION[i + 1]?.t ?? VIDEO_SECONDS;
  const end = seg.t + durations[i];
  const overrun = end - next;
  if (overrun > 0.1) {
    console.warn(`  ⚠ segment ${i} ends ${overrun.toFixed(1)}s past next cue (${next}s)`);
  }
});

// ── Assemble the full narration WAV by padding each segment with silence ────
// We build an ffmpeg filter that delays each segment to its cue and mixes them.
console.log('🎚  Mixing segments into a single audio track…');
const inputs = NARRATION.map((_, i) =>
  `-i "${path.join(AUDIO_DIR, `seg-${String(i).padStart(2, '0')}.wav`)}"`,
).join(' ');

// adelay takes ms; we map each input to a delayed stream, then amix them.
const delays = NARRATION.map((seg, i) => `[${i}:a]adelay=${Math.round(seg.t * 1000)}|${Math.round(seg.t * 1000)}[d${i}]`).join(';');
const mixIns = NARRATION.map((_, i) => `[d${i}]`).join('');
const filter = `${delays};${mixIns}amix=inputs=${NARRATION.length}:duration=longest:normalize=0[mix];[mix]apad=whole_dur=${VIDEO_SECONDS.toFixed(2)}[out]`;

execSync(
  `ffmpeg -y -loglevel error ${inputs} -filter_complex "${filter}" -map "[out]" -c:a aac -b:a 128k "${FULL_AUDIO}"`,
);
console.log(`🎧 Audio: ${FULL_AUDIO}`);

// ── Mux narration over the silent video ────────────────────────────────────
console.log('🎬 Muxing audio onto video → walkthrough.mp4');
execSync(
  `ffmpeg -y -loglevel error -i "${SILENT_VIDEO}" -i "${FULL_AUDIO}" ` +
  `-c:v libx264 -preset medium -crf 23 -pix_fmt yuv420p ` +
  `-c:a aac -b:a 128k -map 0:v:0 -map 1:a:0 -shortest "${FINAL_VIDEO}"`,
);

const stat = fs.statSync(FINAL_VIDEO);
console.log(`✅ Final video: ${FINAL_VIDEO}  (${(stat.size / 1024 / 1024).toFixed(2)} MB)`);

// ── Side-products: narration.txt + narration.srt ───────────────────────────
fs.writeFileSync(
  TXT_FILE,
  NARRATION.map((s) => `[${fmt(s.t)}] ${s.text}`).join('\n\n') + '\n',
);
fs.writeFileSync(SRT_FILE, NARRATION.map((s, i) => {
  const start = srtTime(s.t);
  const end = srtTime(NARRATION[i + 1]?.t ?? VIDEO_SECONDS);
  return `${i + 1}\n${start} --> ${end}\n${s.text}\n`;
}).join('\n'));
console.log(`📝 ${TXT_FILE}`);
console.log(`📝 ${SRT_FILE}`);

function fmt(sec) {
  const m = Math.floor(sec / 60), s = Math.floor(sec % 60);
  return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}
function srtTime(sec) {
  const h = Math.floor(sec / 3600);
  const m = Math.floor((sec % 3600) / 60);
  const s = Math.floor(sec % 60);
  const ms = Math.round((sec - Math.floor(sec)) * 1000);
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')},${String(ms).padStart(3, '0')}`;
}
