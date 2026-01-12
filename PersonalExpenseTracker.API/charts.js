let categoryChartInstance = null;
let trendChartInstance = null;
let sixMonthChartInstance = null;

// --- 1. Category Chart (Donut) ---
function renderCategoryChart(dataPoints) {
    const canvas = document.getElementById('categoryChart');
    if (!canvas) return; // 🛑 Safety Check

    const ctx = canvas.getContext('2d');
    const labels = dataPoints.map(d => d.label);
    const data = dataPoints.map(d => d.value);

    if (categoryChartInstance) categoryChartInstance.destroy();

    categoryChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: ['#4F46E5', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899', '#6366F1'],
                borderWidth: 2,
                borderColor: getComputedStyle(document.body).getPropertyValue('--bg-card').trim() // Dynamic border color
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: 'Spending by Category', color: getTextColor() },
                legend: { labels: { color: getTextColor() } }
            }
        }
    });
}

// --- 2. Trend Chart (Line) ---
function renderTrendChart(dataPoints) {
    const canvas = document.getElementById('trendChart');
    if (!canvas) return; // 🛑 Safety Check

    const ctx = canvas.getContext('2d');
    const labels = dataPoints.map(d => d.label);
    const data = dataPoints.map(d => d.value);

    if (trendChartInstance) trendChartInstance.destroy();

    trendChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Daily Spending',
                data: data,
                borderColor: '#4F46E5',
                backgroundColor: 'rgba(79, 70, 229, 0.1)',
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: '30-Day Trend', color: getTextColor() },
                legend: { labels: { color: getTextColor() } }
            },
            scales: {
                x: { ticks: { color: getTextColor() }, grid: { color: getGridColor() } },
                y: { beginAtZero: true, ticks: { color: getTextColor() }, grid: { color: getGridColor() } }
            }
        }
    });
}

// --- 3. Six-Month History (Bar) ---
function renderSixMonthChart(dataPoints) {
    const canvas = document.getElementById('sixMonthChart');
    if (!canvas) return; // 🛑 Safety Check

    const ctx = canvas.getContext('2d');
    const labels = dataPoints.map(d => d.label);
    const data = dataPoints.map(d => d.value);

    // Create Gradient for Bars
    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, '#4F46E5'); // Top (Primary Color)
    gradient.addColorStop(1, 'rgba(79, 70, 229, 0.2)'); // Bottom (Faded)

    if (sixMonthChartInstance) sixMonthChartInstance.destroy();

    sixMonthChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Monthly Spending',
                data: data,
                backgroundColor: gradient,
                borderRadius: 5,
                barPercentage: 0.6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: { display: true, text: '6-Month History', color: getTextColor() },
                legend: { display: false }
            },
            scales: {
                x: { ticks: { color: getTextColor() }, grid: { display: false } },
                y: { beginAtZero: true, ticks: { color: getTextColor() }, grid: { color: getGridColor() } }
            }
        }
    });
}

// --- Helper Functions for Dark Mode Support ---
function getTextColor() {
    return getComputedStyle(document.body).getPropertyValue('--text-main').trim();
}

function getGridColor() {
    return getComputedStyle(document.body).getPropertyValue('--border-color').trim();
}