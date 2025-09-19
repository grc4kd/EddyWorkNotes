class TimerManager {
    static #INTERVAL_DELAY = 1000;

    #timerInterval;
    #remainingTime;
    #isRunning;

    constructor(durationMinutes) {
        this.#timerInterval = durationMinutes;
        this.#remainingTime = durationMinutes * 60;
        this.#isRunning = false;
    }

    // format total seconds into whole minutes and seconds in MM:SS format
    formatMinutes(seconds) {
        const formatMM = Math.floor(seconds / 60).toString().padStart(2, '0');
        const formatSS = (seconds % 60).toFixed(0).padStart(2, '0');

        return `${formatMM}:${formatSS}`;
    }

    updateDisplay() {
        const display = document.getElementById('timerDisplay');
        if (display) {
            display.textContent = `${this.formatMinutes(this.#remainingTime)} remaining`;
        }
    }

    resetTimer() {
        clearInterval(this.#timerInterval);
        this.#remainingTime = 0;
        this.updateDisplay();
    }

    updateTimer() {
        if (this.#remainingTime <= 1) {
            this.resetTimer();
            return;
        }

        this.#remainingTime--;
        this.updateDisplay();
    }

    startTimer() {
        if (this.#remainingTime > 0) {
            this.#timerInterval = setInterval(() => this.updateTimer(), TimerManager.#INTERVAL_DELAY);
            this.#isRunning = true;
            this.updateDisplay();
        }
    }

    pause() {
        clearInterval(this.#timerInterval);
        this.#isRunning = false;
    }

    resume() {
        this.#timerInterval = setInterval(() => this.updateTimer(), TimerManager.#INTERVAL_DELAY);
        this.#isRunning = true;
    }

    togglePause() {
        if (this.#isRunning) {
            this.pause();
        } else {
            this.resume();
        }
    }

    initEventListeners() {
        document.getElementById('startTimerBtn')?.addEventListener('click', () => this.startTimer(this.#timerInterval));
        document.getElementById('togglePauseBtn')?.addEventListener('click', () => this.togglePause());
    }
}

export function createTimerManager(durationMinutes) {
    const timerManager = new TimerManager(durationMinutes);
    timerManager.initEventListeners();
}