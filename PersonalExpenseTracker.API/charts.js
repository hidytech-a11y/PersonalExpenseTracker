let categoryChartInstance = null;
let trendChartInstance = null;

function renderCategoryChart(dataPoints) {
    const ctx = document.getElementById('categoryChart').getContext('2d');
    const labels = dataPoints.map(d => d.label);
    const data = dataPoints.map(d => d.value);

    if (categoryChartInstance) categoryChartInstance.destroy();

    categoryChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: ['#4F46E5', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: 'Spending by Category (This Month)' } }
        }
    });
}

function renderTrendChart(dataPoints) {
    const ctx = document.getElementById('trendChart').getContext('2d');
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
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: '30-Day Trend' } }
        }
    });
}