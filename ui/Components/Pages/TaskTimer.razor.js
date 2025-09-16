let timerInterval;
let remainingTime;
let isRunning = false;

function startTimer(minutes) {
    remainingTime = minutes * 60;
    updateDisplay();
    
    if (remainingTime > 0) {
        timerInterval = setInterval(updateTimer, 1000);
        isRunning = true;
    }
}

function togglePause() {
    if (isRunning) {
        // Pause the timer
        clearInterval(timerInterval);
        isRunning = false;
    } else if (!isRunning && remainingTime > 0) {
        // Resume the timer
        timerInterval = setInterval(updateTimer, 1000);
        isRunning = true;
    }
}

function updateTimer() {
    if (remainingTime <= 1) {
        clearInterval(timerInterval);
        remainingTime = 0;
        updateDisplay();
        return;
    }
    
    remainingTime--;
    updateDisplay();
}

function updateDisplay() {
    const display = document.getElementById('timerDisplay');
    display.textContent = `${formatMinutes(remainingTime)} remaining`;
}

function formatMinutes(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${secs.toFixed(0).padStart(2, '0')}`;
}

export function addHandlers([workDuration, breakDuration]) {
    const startTimerBtn = document.getElementById('startTimerBtn');
    const togglePauseBtn = document.getElementById('togglePauseBtn');

    startTimerBtn.addEventListener('click', function () {
        startTimer(workDuration);
    });
    
    togglePauseBtn.addEventListener('click', function () {
        togglePause();
    });
}
