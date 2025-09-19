class TimerManager {
    static #INTERVAL_DELAY = 1000;

    #durationMinutes;
    #remainingTimeMinutes;
    #isRunning;

    constructor(durationMinutes) {
        this.#durationMinutes = durationMinutes;
        this.#remainingTimeMinutes = durationMinutes * 60;
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
            display.textContent = `${this.formatTime(this.#remainingTimeMinutes)} remaining`;
        }
    }

    reset() {
        clearInterval(this.#durationMinutes);
        this.#remainingTimeMinutes = 0;
        this.updateDisplay();
    }

    tick() {
        if (this.#remainingTimeMinutes <= 1) {
            this.reset();
            return;
        }

        this.#remainingTimeMinutes--;
        this.updateDisplay();
        this.setInterval(TimerManager.#INTERVAL_DELAY);
    }

    start() {
        if (this.#remainingTimeMinutes > 0) {
            this.#durationMinutes = setInterval(() => this.tick(), TimerManager.#INTERVAL_DELAY);
            this.#isRunning = true;
        }
    }

    pause() {
        clearInterval(this.#durationMinutes);
        this.#isRunning = false;
    }

    resume() {
        this.#durationMinutes = setInterval(() => this.tick(), TimerManager.#INTERVAL_DELAY);
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

export function startTimer(durationMinutes) {
    remainingTimeMinutes = durationMinutes;
    if (timerManager) {
        timerManager.start();
    }
}