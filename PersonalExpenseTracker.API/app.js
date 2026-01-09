// HELPER: Strict Formatting for Nigerian Naira (NGN)
function formatMoney(amount) {
    return new Intl.NumberFormat('en-NG', {
        style: 'currency',
        currency: 'NGN'
    }).format(amount);
}

// --- INIT ---
document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (token) {
        document.getElementById('auth-container').classList.add('hidden');
        document.getElementById('app-container').classList.remove('hidden');
        const user = JSON.parse(localStorage.getItem('user'));
        if (user) document.getElementById('display-username').innerText = user.name;
        navigate('dashboard');
    }
});

// --- NAVIGATION ---
function navigate(viewName) {
    document.querySelectorAll('.view-section').forEach(el => el.classList.add('hidden'));
    document.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(`view-${viewName}`).classList.remove('hidden');
    document.querySelector(`button[onclick="navigate('${viewName}')"]`).classList.add('active');

    const titles = { dashboard: 'Dashboard', expenses: 'My Expenses', budgets: 'Budget Goals', recurring: 'Recurring Bills' };
    document.getElementById('page-title').innerText = titles[viewName];

    if (viewName === 'dashboard') loadDashboard();
    if (viewName === 'expenses') loadExpenses(); // Will now load with default filters
    if (viewName === 'budgets') loadBudgets();
    if (viewName === 'recurring') loadRecurring();
}

// --- DASHBOARD ---
async function loadDashboard() {
    const stats = await apiCall('/analytics/dashboard');
    if (!stats) return;

    // Apply NGN Formatting
    document.getElementById('total-spent').innerText = formatMoney(stats.totalSpent);
    document.getElementById('tx-count').innerText = stats.transactionCount;

    if (window.renderCategoryChart && stats.categoryBreakdown) renderCategoryChart(stats.categoryBreakdown);
    if (window.renderTrendChart && stats.monthlyTrend) renderTrendChart(stats.monthlyTrend);
}

// --- EXPENSES (With Search & Filter Logic) ---
async function loadExpenses() {
    // 1. Get values from the Filter inputs
    const search = document.getElementById('filter-search')?.value || '';
    const category = document.getElementById('filter-category')?.value || 'All';
    const start = document.getElementById('filter-start')?.value || '';
    const end = document.getElementById('filter-end')?.value || '';

    // 2. Build Query String
    let query = '/expenses?';
    if (search) query += `search=${encodeURIComponent(search)}&`;
    if (category && category !== 'All') query += `category=${encodeURIComponent(category)}&`;
    if (start) query += `startDate=${start}&`;
    if (end) query += `endDate=${end}&`;

    const expenses = await apiCall(query);
    const tbody = document.getElementById('expense-table-body');
    tbody.innerHTML = '';

    if (expenses) {
        expenses.forEach(exp => {
            const row = `<tr>
                <td>${new Date(exp.date).toLocaleDateString()}</td>
                <td>${exp.category}</td>
                <td>${exp.description}</td>
                <td style="font-weight:bold; color: #10B981">${formatMoney(exp.amount)}</td>
                <td>
                    <button onclick="deleteExpense('${exp.id}')" style="color:#EF4444;border:none;background:none;cursor:pointer;">
                        <i class="fas fa-trash"></i> 🗑️
                    </button>
                </td>
            </tr>`;
            tbody.innerHTML += row;
        });
    }
}

async function saveExpense() {
    const date = document.getElementById('exp-date').value;
    const category = document.getElementById('exp-category').value;
    const description = document.getElementById('exp-desc').value;
    const amount = parseFloat(document.getElementById('exp-amount').value);

    if (await apiCall('/expenses', 'POST', { date, category, description, amount })) {
        closeModal('expense-modal');
        loadExpenses();
    }
}

async function deleteExpense(id) {
    if (confirm("Delete this expense?")) {
        await apiCall(`/expenses/${id}`, 'DELETE');
        loadExpenses();
    }
}

// --- BUDGETS ---
async function loadBudgets() {
    const budgets = await apiCall('/budgets');
    const container = document.getElementById('budget-container');
    container.innerHTML = '';

    if (budgets) {
        budgets.forEach(b => {
            let color = '#10B981'; // Green
            if (b.percentage > 80) color = '#F59E0B'; // Orange
            if (b.percentage >= 100) color = '#EF4444'; // Red

            container.innerHTML += `
            <div class="card budget-card">
                <div style="display:flex;justify-content:space-between;">
                    <h3>${b.category}</h3>
                    <button onclick="deleteBudget('${b.id}')" style="border:none;background:none;cursor:pointer;">✕</button>
                </div>
                <p><strong>${formatMoney(b.spent)}</strong> / ${formatMoney(b.monthlyLimit)}</p>
                <div class="progress-bar">
                    <div class="progress-fill" style="width:${Math.min(b.percentage, 100)}%;background:${color}"></div>
                </div>
                <p style="color:${color}">${b.percentage.toFixed(1)}% Used</p>
            </div>`;
        });
    }
}

async function saveBudget() {
    const category = document.getElementById('bud-category').value;
    const limit = parseFloat(document.getElementById('bud-limit').value);

    if (await apiCall('/budgets', 'POST', { category, monthlyLimit: limit })) {
        closeModal('budget-modal');
        loadBudgets();
    }
}

async function deleteBudget(id) {
    if (confirm("Delete budget?")) {
        await apiCall(`/budgets/${id}`, 'DELETE');
        loadBudgets();
    }
}

// --- RECURRING ---
async function loadRecurring() {
    const items = await apiCall('/recurringexpenses');
    const tbody = document.getElementById('recurring-table-body');
    tbody.innerHTML = '';
    const today = new Date();

    if (items) {
        items.forEach(item => {
            let nextDate = new Date(today.getFullYear(), today.getMonth(), item.dayOfMonth);
            if (today.getDate() > item.dayOfMonth) nextDate.setMonth(nextDate.getMonth() + 1);

            tbody.innerHTML += `<tr>
                <td>${item.description}</td>
                <td>${item.category}</td>
                <td>${formatMoney(item.amount)}</td>
                <td>Monthly (Day ${item.dayOfMonth})</td>
                <td>${nextDate.toLocaleDateString()}</td>
                <td>
                    <button onclick="deleteRecurring('${item.id}')" style="color:red;border:none;background:none;cursor:pointer;">
                        <i class="fas fa-trash"></i> 🗑️
                    </button>
                </td>
            </tr>`;
        });
    }
}

async function saveRecurring() {
    const description = document.getElementById('rec-desc').value;
    const category = document.getElementById('rec-category').value;
    const amount = parseFloat(document.getElementById('rec-amount').value);
    const dayOfMonth = parseInt(document.getElementById('rec-day').value);

    const payload = { category, description, amount, dayOfMonth, frequency: "Monthly" };
    if (await apiCall('/recurringexpenses', 'POST', payload)) {
        closeModal('recurring-modal');
        loadRecurring();
    }
}

async function deleteRecurring(id) {
    if (confirm("Stop recurring?")) {
        await apiCall(`/recurringexpenses/${id}`, 'DELETE');
        loadRecurring();
    }
}

// --- MODALS ---
function openModal(id) { document.getElementById(id).classList.remove('hidden'); }
function closeModal(id) { document.getElementById(id).classList.add('hidden'); }