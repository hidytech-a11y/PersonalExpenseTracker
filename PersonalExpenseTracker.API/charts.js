let categoryChartInstance = null;
let trendChartInstance = null;

function renderCategoryChart(expenses) {
    const ctx = document.getElementById('categoryChart').getContext('2d');

    // Group expenses by category
    const categories = {};
    expenses.forEach(e => {
        categories[e.category] = (categories[e.category] || 0) + e.amount;
    });

    if (categoryChartInstance) categoryChartInstance.destroy();

    categoryChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: Object.keys(categories),
            datasets: [{
                data: Object.values(categories),
                backgroundColor: ['#4F46E5', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6']
            }]
        },
        options: { responsive: true, plugins: { title: { display: true, text: 'Spending by Category' } } }
    });
}

function renderTrendChart(expenses) {
    const ctx = document.getElementById('trendChart').getContext('2d');

    // Sort expenses by date
    const sorted = expenses.sort((a, b) => new Date(a.date) - new Date(b.date));
    const labels = sorted.map(e => new Date(e.date).toLocaleDateString());
    const data = sorted.map(e => e.amount);

    if (trendChartInstance) trendChartInstance.destroy();

    trendChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Daily Spending',
                data: data,
                borderColor: '#4F46E5',
                tension: 0.4
            }]
        },
        options: { responsive: true, plugins: { title: { display: true, text: 'Spending Trend' } } }
    });
}