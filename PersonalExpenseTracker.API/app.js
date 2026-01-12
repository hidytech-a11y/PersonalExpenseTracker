// HELPER: Strict Formatting for Nigerian Naira (NGN)
function formatMoney(amount) {
    return new Intl.NumberFormat('en-NG', {
        style: 'currency',
        currency: 'NGN'
    }).format(amount);
}

// --- INIT ---
document.addEventListener('DOMContentLoaded', () => {
    // 1. Check for Saved Theme
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.setAttribute('data-theme', 'dark');
        updateThemeIcon(true);
    }
    // 2. Check for Auth Token
    const token = localStorage.getItem('token');
    if (token) {
        const authContainer = document.getElementById('auth-container');
        const appContainer = document.getElementById('app-container');
        if (authContainer) authContainer.classList.add('hidden');
        if (appContainer) appContainer.classList.remove('hidden');

        const user = JSON.parse(localStorage.getItem('user'));
        const displayEl = document.getElementById('display-username');
        if (user && displayEl) displayEl.innerText = user.name;

        navigate('dashboard');
    }
});

// --- DARK MODE LOGIC ---
function toggleDarkMode() {
    const isDark = document.body.getAttribute('data-theme') === 'dark';

    if (isDark) {
        document.body.removeAttribute('data-theme');
        localStorage.setItem('theme', 'light');
        updateThemeIcon(false);
    } else {
        document.body.setAttribute('data-theme', 'dark');
        localStorage.setItem('theme', 'dark');
        updateThemeIcon(true);
    }
}

function updateThemeIcon(isDark) {
    const icon = document.getElementById('theme-icon');
    const text = document.getElementById('theme-text');

    if (!icon || !text) return;

    if (isDark) {
        icon.className = 'fas fa-sun'; // Change moon to sun
        text.innerText = 'Light Mode';
    } else {
        icon.className = 'fas fa-moon';
        text.innerText = 'Dark Mode';
    }
}

// --- NAVIGATION ---
function navigate(viewName) {
    document.querySelectorAll('.view-section').forEach(el => el.classList.add('hidden'));
    document.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active'));

    const view = document.getElementById(`view-${viewName}`);
    if (view) view.classList.remove('hidden');

    const navBtn = document.querySelector(`button[onclick="navigate('${viewName}')"]`);
    if (navBtn) navBtn.classList.add('active');

    const titles = { dashboard: 'Dashboard', expenses: 'My Expenses', budgets: 'Budget Goals', recurring: 'Recurring Bills' };
    const titleEl = document.getElementById('page-title');
    if (titleEl) titleEl.innerText = titles[viewName] || '';

    if (viewName === 'dashboard') loadDashboard();
    if (viewName === 'expenses') loadExpenses(); // Will now load with default filters
    if (viewName === 'budgets') loadBudgets();
    if (viewName === 'recurring') loadRecurring();
}

// --- UPDATED DASHBOARD LOGIC ---
async function loadDashboard() {
    const stats = await apiCall('/analytics/dashboard');
    if (!stats) return;

    // 1. Basic Stats
    const totalSpentEl = document.getElementById('total-spent');
    const txCountEl = document.getElementById('tx-count');
    const topCatEl = document.getElementById('top-category');

    if (totalSpentEl && typeof stats.totalSpent !== 'undefined') totalSpentEl.innerText = formatMoney(stats.totalSpent);
    if (txCountEl && typeof stats.transactionCount !== 'undefined') txCountEl.innerText = stats.transactionCount;
    if (topCatEl) topCatEl.innerText = stats.topCategory || "None";

    // 2. Comparison Logic (The Insight)
    const badge = document.getElementById('spend-badge');
    const icon = document.getElementById('spend-icon');
    const text = document.getElementById('spend-percent');

    if (badge) {
        badge.classList.remove('hidden', 'badge-success', 'badge-danger', 'badge-neutral');

        if (typeof stats.percentageChange !== 'undefined') {
            if (stats.percentageChange > 0) {
                // Spending INCREASED (Bad)
                badge.classList.add('badge-danger');
                if (icon) icon.className = 'fas fa-arrow-up';
                if (text) text.innerText = `+${stats.percentageChange}%`;
            } else if (stats.percentageChange < 0) {
                // Spending DECREASED (Good)
                badge.classList.add('badge-success');
                if (icon) icon.className = 'fas fa-arrow-down';
                if (text) text.innerText = `${stats.percentageChange}%`;
            } else {
                // No Change
                badge.classList.add('badge-neutral');
                if (icon) icon.className = 'fas fa-minus';
                if (text) text.innerText = "0%";
            }
        }
    }

    // 3. Render Charts (guards inside chart functions handle missing canvases)
    if (window.renderCategoryChart && stats.categoryBreakdown) renderCategoryChart(stats.categoryBreakdown);
    if (window.renderTrendChart && stats.monthlyTrend) renderTrendChart(stats.monthlyTrend);
    // NEW: Render 6-Month Chart
    if (window.renderSixMonthChart && stats.sixMonthStats) renderSixMonthChart(stats.sixMonthStats);
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
    if (!tbody) return;
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
                        <i class="fas fa-trash"></i> Delete </button>
                </td>
            </tr>`;
            tbody.innerHTML += row;
        });
    }
}

async function saveExpense() {
    const date = document.getElementById('exp-date')?.value;
    const category = document.getElementById('exp-category')?.value;
    const description = document.getElementById('exp-desc')?.value;
    const amount = parseFloat(document.getElementById('exp-amount')?.value || '0');

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

// --- UPDATED BUDGETS LOGIC ---
async function loadBudgets() {
    const budgets = await apiCall('/budgets');
    const container = document.getElementById('budget-container');
    if (!container) return;
    container.innerHTML = '';

    if (budgets) {
        budgets.forEach(b => {
            // 1. Determine Color & Alert Status
            let color = '#10B981'; // Green (Safe)
            let alertIcon = '';

            if (b.percentage >= 100) {
                color = '#EF4444'; // Red (Over Budget)
                alertIcon = '<i class="fas fa-exclamation-circle" style="color:#EF4444" title="Over Budget!"></i>';
            } else if (b.percentage >= 80) {
                color = '#F59E0B'; // Orange (Warning)
                alertIcon = '<i class="fas fa-exclamation-triangle" style="color:#F59E0B" title="Nearing Limit"></i>';
            }

            // 2. Rollover Badge Text
            let rolloverHtml = '';
            if (b.enableRollover && b.rolloverAmount > 0) {
                rolloverHtml = `<span style="font-size:0.8rem; color:#10B981; background:rgba(16,185,129,0.1); padding:2px 6px; border-radius:4px;">
                    +${formatMoney(b.rolloverAmount)} Rollover
                </span>`;
            }

            // 3. Render Card
            container.innerHTML += `
            <div class="card budget-card" style="border-left: 5px solid ${color}">
                <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:10px;">
                    <h3 style="margin:0; font-size:1.1rem;">${b.category} ${alertIcon}</h3>
                    <button onclick="deleteBudget('${b.id}')" style="border:none; background:none; cursor:pointer; opacity:0.5;">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
                
                <div style="display:flex; justify-content:space-between; font-size:0.9rem; color:var(--text-secondary);">
                    <span>Spent: <strong>${formatMoney(b.spent)}</strong></span>
                    <span>Limit: <strong>${formatMoney(b.effectiveLimit)}</strong></span>
                </div>

                <div class="progress-bar">
                    <div class="progress-fill" style="width:${Math.min(b.percentage, 100)}%; background:${color}"></div>
                </div>
                
                <div style="display:flex; justify-content:space-between; margin-top:5px; align-items:center;">
                    <span style="color:${color}; font-weight:bold; font-size:0.85rem;">${b.percentage.toFixed(1)}% Used</span>
                    ${rolloverHtml}
                </div>
            </div>`;
        });
    }
}

async function saveBudget() {
    const category = document.getElementById('bud-category')?.value;
    const limit = parseFloat(document.getElementById('bud-limit')?.value || '0');
    const rollover = document.getElementById('bud-rollover')?.checked || false; // Capture checkbox

    if (await apiCall('/budgets', 'POST', { category, monthlyLimit: limit, enableRollover: rollover })) {
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
    if (!tbody) return;
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
                        <i class="fas fa-trash"></i> Delete </button>
                </td>
            </tr>`;
        });
    }
}

async function saveRecurring() {
    const description = document.getElementById('rec-desc')?.value;
    const category = document.getElementById('rec-category')?.value;
    const amount = parseFloat(document.getElementById('rec-amount')?.value || '0');
    const dayOfMonth = parseInt(document.getElementById('rec-day')?.value || '0');

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

// --- EXPORT DATA ---
async function exportExpenses() {
    try {
        const token = localStorage.getItem('token');
        if (!token) return;

        // 1. Fetch the file blob (Raw Data)
        const response = await fetch(`${API_BASE}/expenses/export`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) throw new Error("Export failed");

        // 2. Create a temporary download link
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');

        a.href = url;
        a.download = `MyExpenses_${new Date().toISOString().slice(0, 10)}.csv`; // Filename: MyExpenses_2023-10-25.csv
        document.body.appendChild(a);

        // 3. Trigger click and cleanup
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);

    } catch (error) {
        console.error("Export Error:", error);
        alert("Failed to download expenses. Please try again.");
    }
}

// --- MODALS ---
function openModal(id) { const el = document.getElementById(id); if (el) el.classList.remove('hidden'); }
function closeModal(id) { const el = document.getElementById(id); if (el) el.classList.add('hidden'); }