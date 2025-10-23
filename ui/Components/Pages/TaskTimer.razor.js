/**
 * Timer display update interval in milliseconds - 1 second
 * @const {number} INTERVAL_DELAY_MS
 */
const INTERVAL_DELAY_MS = 1000;

/**
 * DOM element ID where timer will be displayed
 * @const {string} elementId
 */
const elementId = 'timerDisplay';

/**
 * Current timer interval ID, used to manage the timer state
 * @type {number|null}
 */
let intervalId = null;

/**
 * Remaining time in seconds for current timer session
 * @type {number}
 */
let remainingTimeSeconds = 0;

/**
 * Current running state of the timer
 * @type {boolean}
 */
let isRunning = false;

/**
 * Updates the timer display with the current remaining time.
 * 
 * @param {number} totalSeconds The total number of seconds remaining
 */
function updateDisplay(totalSeconds) {
    const display = document.getElementById(elementId);
    if (display) {
        display.textContent = `${formatTime(totalSeconds)} remaining`;
    } else {
        console.error(`Element with id ${elementId} not found.`);
    }
}

/**
 * Formats total seconds into MM:SS time string
 * 
 * @export
 * @param {number} totalSeconds The total number of seconds to format
 * @returns {string} Formatted time string in MM:SS format
 */
export function formatTime(totalSeconds) {
    const seconds = totalSeconds % 60;
    const minutes = (totalSeconds - seconds) / 60;

    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`
}

/**
 * Timer tick function - decreases remaining time and updates display
 * 
 * @private
 */
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

/**
 * Pauses the running timer
 * 
 * @export
 */
export function pause() {
    if (intervalId) {
        clearInterval(intervalId);
    }
}

/**
 * Starts or restarts the timer with specified duration in seconds
 * 
 * @export
 * @param {number} durationSeconds The duration to count down from in seconds
 */
export function run(durationSeconds) {
    // stop any other timers and run this new timer by itself
    pause();

    // the first timer update is immediate
    updateDisplay(durationSeconds);

    // set interval timer to tick at regular rate
    remainingTimeSeconds = durationSeconds;
    isRunning = true;
    intervalId = setInterval(tick, INTERVAL_DELAY_MS);
}
