const INTERVAL_DELAY_MS = 1000;

let intervalId = null;
let remainingTimeSeconds = 0;
let isRunning = false;

// Format total seconds into whole minutes and seconds in MM:SS format
export function formatTime(totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = Math.floor(totalSeconds % 60);
    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
}

function updateDisplay(totalSeconds) {
    const elementId = 'timerDisplay';
    const display = document.getElementById(elementId);
    if (display) {
        display.textContent = `${formatTime(totalSeconds)} remaining`;
    } else {
        console.error(`Element with id ${elementId} not found.`);
    }
}

function tick() {
    if (!isRunning) return;

    // always count tick and update timer display
    remainingTimeSeconds -= 1;
    updateDisplay(remainingTimeSeconds);

    // stop at 1 second left on the clock (next tick updates to 0)
    if (remainingTimeSeconds <= 1) {
        remainingTimeSeconds = 1;
        stop();
    }
}

export function pause() {
    if (intervalId && isRunning) {
        clearInterval(intervalId);
        intervalId = null;
        isRunning = false;
    }
}

export function run(durationSeconds) {
    // stop any other timers and run this new timer by itself
    stop();

    // the first timer update is immediate
    updateDisplay(durationSeconds);

    // set interval timer to tick at regular rate
    remainingTimeSeconds = durationSeconds;
    isRunning = true;
    intervalId = setInterval(tick, INTERVAL_DELAY_MS);
}

function stop() {
    if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
    }
    isRunning = false;
}
