let intervalId = 0;
const INTERVAL_DELAY_SEC = 1
let remainingTimeSeconds = 0;
let isRunning = false;

// format total seconds into whole minutes and seconds in MM:SS format
export function formatTime(seconds) {
    let format = "";

    if (seconds >= 0) {
        const formatMM = Math.floor(seconds / 60).toString().padStart(2, '0');
        const formatSS = (seconds % 60).toFixed(0).padStart(2, '0');

        format = `${formatMM}:${formatSS}`
    }

    return format;
}

function updateDisplay() {
    const display = document.getElementById('timerDisplay');
    const formattedTime = formatTime(remainingTimeSeconds);
    if (display && formattedTime) {
        display.textContent = `${formattedTime} remaining`;
    }
}

function tick() {
    if (isRunning) {
        // always count tick and update timer display
        remainingTimeSeconds -= INTERVAL_DELAY_SEC;
        updateDisplay();
    }

    if (remainingTimeSeconds <= 0) {
        remainingTimeSeconds = 0;
        clearInterval(intervalId);
    }
}

function pause() {
    if (intervalId > 0) {
        clearInterval(intervalId);
        isRunning = false;
    }
}

export function togglePause() {
    if (isRunning) {
        pause();
    } else if (remainingTimeSeconds > 0) {
        run();
    }
}

export function run(durationSeconds) {
    // stop any running timers that were already running
    if (intervalId > 0) {
        clearInterval(intervalId);
    }

    isRunning = true;
    remainingTimeSeconds = durationSeconds;
    intervalId = setInterval(tick, INTERVAL_DELAY_SEC * 1000);
}
