const INTERVAL_DELAY_MS = 1000

let intervalId = null;
let remainingTimeSeconds = 0;
let isRunning = false;

// Format total seconds into whole minutes and seconds in MM:SS format
export function formatTime(totalSeconds) {
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = Math.floor(totalSeconds % 60);

  // Pad single-digit numbers with a leading zero for consistent formatting
  return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
}

function updateDisplay() {
    const elementId = 'timerDisplay';
    const display = document.getElementById(elementId);
    if (display) {
        display.textContent = `${formatTime(remainingTimeSeconds)} remaining`;
    } else {
        console.error(`Element with id ${elementId} not found.`);
    }
}

function tick() {
    if (!isRunning) return;

    // always count tick and update timer display
    remainingTimeSeconds -= 1;
    updateDisplay();

    if (remainingTimeSeconds <= 0) {
        remainingTimeSeconds = 0;
        stop();
    }
}

export function pause() {
    if (intervalId && isRunning) {
        clearInterval(intervalId);
        intervalId = null;
        isRunning = false;
    }

export function run(durationSeconds) {
    // stop any running timers that were already running
    stop();

    // the first timer update is immediate
    updateDisplay(durationSeconds);

    // set interval timer to tick at regular rate
    remainingTimeSeconds = durationSeconds;
    intervalId = setInterval(tick, INTERVAL_DELAY_MS);
}

function stop() {
    if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
    }
    isRunning = false;
}
