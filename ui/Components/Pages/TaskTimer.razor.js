class TimerManager {
    #intervalId;
    INTERVAL_DELAY_SEC = 1
    remainingTimeSeconds = 0;
    isRunning = false;

    // format total seconds into whole minutes and seconds in MM:SS format
    formatTime(seconds) {
        let format = "";
        
        if (seconds >= 0) {
            const formatMM = Math.floor(seconds / 60).toString().padStart(2, '0');
            const formatSS = (seconds % 60).toFixed(0).padStart(2, '0');

            format =`${formatMM}:${formatSS}`
        }
        
        return format;
    }

    updateDisplay() {
        const display = document.getElementById('timerDisplay');
        const formatTime = this.formatTime(this.remainingTimeSeconds);
        if (display && formatTime) {
            display.textContent = `${formatTime} remaining`;
        }
    }

    tick() {
        if (this.isRunning) {
            // always count tick and update timer display
            this.remainingTimeSeconds -= this.INTERVAL_DELAY_SEC;
            this.updateDisplay();
        }

        if (this.remainingTimeSeconds <= 0) {
            this.remainingTimeSeconds = 0;
            clearInterval(this.#intervalId);
        }
    }

    pause() {
        if (this.#intervalId > 0) {
            clearInterval(this.#intervalId);
            this.isRunning = false;
        }
    }

    togglePause() {
        if (this.isRunning) {
            this.pause();
        } else if (this.remainingTimeSeconds > 0) {
            this.run();
        }
    }
}

let timerManager;
export function init() {
    timerManager = new TimerManager();
}
export function tick() {
    timerManager.tick();
}
export function run(durationSeconds) {
    // stop any running timers
    if (timerManager.intervalId > 0) {
        clearInterval(timerManager.intervalId);
    }

    timerManager.isRunning = true;
    timerManager.remainingTimeSeconds = durationSeconds;
    timerManager.intervalId = setInterval(tick, timerManager.INTERVAL_DELAY_SEC * 1000);
}
export function pause() {
    timerManager.pause();
}
