const INTERVAL_DELAY_MS = 1000;
const elementId = 'timerDisplay';

let intervalId = null;
let remainingTimeSeconds = 0;
let isRunning = false;

function init() {
    intervalId = null;
    remainingTimeSeconds = 0;
    isRunning = false;
    updateDisplay(0);
}

function updateDisplay(totalSeconds) {    
    const display = document.getElementById(elementId);
    if (display) {
        display.textContent = `${formatTime(totalSeconds)} remaining`;
    } else {
        console.error(`Element with id ${elementId} not found.`);
    }
}

// Format total seconds into whole minutes and seconds in MM:SS format
export function formatTime(totalSeconds) {
    const minuteNumber = totalSeconds / 60
    const secondNumber = totalSeconds % 60

    const minutes = String(Math.floor(minuteNumber)).padStart(2, '0');
    const seconds = String(secondNumber).padStart(2, '0');

    return `${minutes}:${seconds}`
}

function tick() {
    if (!isRunning) return;

    // always count tick and update timer display
    remainingTimeSeconds -= 1;
    updateDisplay(remainingTimeSeconds);

    // stop at 1 second left on the clock (next tick updates to 0)
    if (remainingTimeSeconds <= 0) {
        pause();
    }
}

export function pause() {
    if (intervalId){
        clearInterval(intervalId);
    }
}

export function run(durationSeconds) {
    // stop any other timers and run this new timer by itself
    pause()

    // the first timer update is immediate
    updateDisplay(durationSeconds);

    // set interval timer to tick at regular rate
    remainingTimeSeconds = durationSeconds;
    isRunning = true;
    intervalId = setInterval(tick, INTERVAL_DELAY_MS);
}
