class TimerManager {
    static #INTERVAL_DELAY_SEC = 1
    static #INTERVAL_DELAY_MS  = 1000 * this.#INTERVAL_DELAY_SEC;

    #initialDuration;
    #remainingTimeSeconds;
    #intervalId;
    #isRunning;

    constructor(durationMinutes) {
        this.#initialDuration = durationMinutes * 60; // Store as seconds
        this.#remainingTimeSeconds = this.#initialDuration;
        this.#isRunning = false;
    }

    // format total seconds into whole minutes and seconds in MM:SS format
    formatTime(seconds) {
        const formatMM = Math.floor(seconds / 60).toString().padStart(2, '0');
        const formatSS = (seconds % 60).toFixed(0).padStart(2, '0');

        return `${formatMM}:${formatSS}`;
    }

    updateDisplay() {
        const display = document.getElementById('timerDisplay');
        if (display) {
            display.textContent = `${this.formatTime(this.#remainingTimeSeconds)} remaining`;
        }
    }

    reset() {
        clearInterval(this.#intervalId);
        this.#remainingTimeSeconds = 0;
        this.updateDisplay();
    }

    tick() {
        // reset the clock once remaining time is below delay threshold (skip last update)
        if (this.#remainingTimeSeconds - TimerManager.#INTERVAL_DELAY_SEC <= 0) {
            this.reset();
        }

        // always count tick and update timer display
        this.#remainingTimeSeconds -= TimerManager.#INTERVAL_DELAY_SEC;
        this.updateDisplay();
    }

    start() {
        if (!this.#isRunning && this.#remainingTimeSeconds > 0) {
            this.#intervalId = setInterval(() => this.tick(), TimerManager.#INTERVAL_DELAY_MS);
            this.#isRunning = true;
        }
    }

    pause() {
        clearInterval(this.#intervalId);
        this.#isRunning = false;
    }

    resume() {
        this.#intervalId = setInterval(() => this.tick(), TimerManager.#INTERVAL_DELAY_MS);
        this.#isRunning = true;
    }

    togglePause() {
        if (this.#isRunning) {
            this.pause();
        } else {
            this.resume();
        }
    }

    addHandlers() {
        document.getElementById('startTimerBtn')?.addEventListener('click', () => this.start());
        document.getElementById('togglePauseBtn')?.addEventListener('click', () => this.togglePause());
    }
}

let timerManager;
export function addHandlers(durationMinutes) {
    timerManager = new TimerManager(durationMinutes).addHandlers();
}