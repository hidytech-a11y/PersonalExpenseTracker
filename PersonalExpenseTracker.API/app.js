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

    const titles = {
        dashboard: 'Dashboard',
        expenses: 'My Expenses',
        budgets: 'Budget Goals',
        recurring: 'Recurring Bills' // New Title
    };
    document.getElementById('page-title').innerText = titles[viewName];

    if (viewName === 'dashboard') loadDashboard();
    if (viewName === 'expenses') loadExpenses();
    if (viewName === 'budgets') loadBudgets();
    if (viewName === 'recurring') loadRecurring(); // New Load Function
}

// --- DASHBOARD ---
async function loadDashboard() {
    const expenses = await apiCall('/expenses');
    if (!expenses) return;
    const total = expenses.reduce((sum, item) => sum + item.amount, 0);
    document.getElementById('total-spent').innerText = `$${total.toFixed(2)}`;
    document.getElementById('tx-count').innerText = expenses.length;
    renderCategoryChart(expenses);
    renderTrendChart(expenses);
}

// --- EXPENSES ---
async function loadExpenses() {
    const expenses = await apiCall('/expenses');
    const tbody = document.getElementById('expense-table-body');
    tbody.innerHTML = '';
    expenses.forEach(exp => {
        const row = `<tr>
            <td>${new Date(exp.date).toLocaleDateString()}</td>
            <td>${exp.category}</td>
            <td>${exp.description}</td>
            <td>$${exp.amount.toFixed(2)}</td>
            <td><button onclick="deleteExpense('${exp.id}')" style="color:red;border:none;background:none;cursor:pointer;">🗑️</button></td>
        </tr>`;
        tbody.innerHTML += row;
    });
}

async function saveExpense() {
    const date = document.getElementById('exp-date').value;
    const category = document.getElementById('exp-category').value;
    const description = document.getElementById('exp-desc').value;
    const amount = parseFloat(document.getElementById('exp-amount').value);
    const result = await apiCall('/expenses', 'POST', { date, category, description, amount });
    if (result) { closeModal('expense-modal'); loadExpenses(); }
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
    budgets.forEach(b => {
        let color = '#10B981';
        if (b.percentage > 80) color = '#F59E0B';
        if (b.percentage >= 100) color = '#EF4444';
        const card = `
        <div class="card budget-card">
            <div style="display:flex;justify-content:space-between;">
                <h3>${b.category}</h3>
                <button onclick="deleteBudget('${b.id}')" style="color:#9CA3AF;border:none;background:none;cursor:pointer;">✕</button>
            </div>
            <p style="margin:10px 0; font-size:1.1rem;"><strong>$${b.spent.toFixed(2)}</strong> / $${b.monthlyLimit.toFixed(2)}</p>
            <div class="progress-bar"><div class="progress-fill" style="width: ${Math.min(b.percentage, 100)}%; background: ${color}"></div></div>
            <p style="margin-top:5px; font-size:0.9rem; color:${color}">${b.percentage.toFixed(1)}% Used</p>
        </div>`;
        container.innerHTML += card;
    });
}

async function saveBudget() {
    const category = document.getElementById('bud-category').value;
    const limit = parseFloat(document.getElementById('bud-limit').value);
    const result = await apiCall('/budgets', 'POST', { category, monthlyLimit: limit });
    if (result) { closeModal('budget-modal'); loadBudgets(); }
}

async function deleteBudget(id) {
    if (confirm("Delete this budget?")) {
        await apiCall(`/budgets/${id}`, 'DELETE');
        loadBudgets();
    }
}

// --- NEW: RECURRING EXPENSES LOGIC ---
async function loadRecurring() {
    const items = await apiCall('/recurringexpenses'); // Ensure endpoint matches backend
    const tbody = document.getElementById('recurring-table-body');
    tbody.innerHTML = '';

    const today = new Date();
    const currentDay = today.getDate();

    items.forEach(item => {
        // Calculate "Next Bill Date"
        let nextDate = new Date(today.getFullYear(), today.getMonth(), item.dayOfMonth);
        if (item.dayOfMonth < currentDay) {
            // If bill day passed this month, show next month
            nextDate.setMonth(nextDate.getMonth() + 1);
        }

        const row = `<tr>
            <td>${item.description}</td>
            <td>${item.category}</td>
            <td>$${item.amount.toFixed(2)}</td>
            <td>Monthly (Day ${item.dayOfMonth})</td>
            <td>${nextDate.toLocaleDateString()}</td>
            <td>
                <button onclick="deleteRecurring('${item.id}')" style="color:red;border:none;background:none;cursor:pointer;">🗑️</button>
            </td>
        </tr>`;
        tbody.innerHTML += row;
    });
}

async function saveRecurring() {
    const description = document.getElementById('rec-desc').value;
    const category = document.getElementById('rec-category').value;
    const amount = parseFloat(document.getElementById('rec-amount').value);
    const dayOfMonth = parseInt(document.getElementById('rec-day').value);

    // Basic Validation
    if (!description || !category || isNaN(amount) || isNaN(dayOfMonth) || dayOfMonth < 1 || dayOfMonth > 31) {
        alert("Please fill all fields correctly. Day must be 1-31.");
        return;
    }

    const payload = {
        category,
        description,
        amount,
        dayOfMonth,
        frequency: "Monthly" // Defaulting to Monthly for MVP
    };

    const result = await apiCall('/recurringexpenses', 'POST', payload);
    if (result) {
        closeModal('recurring-modal');
        loadRecurring();
    }
}

async function deleteRecurring(id) {
    if (confirm("Stop this recurring expense?")) {
        await apiCall(`/recurringexpenses/${id}`, 'DELETE');
        loadRecurring();
    }
}

// --- MODAL UTILS ---
function openModal(id) { document.getElementById(id).classList.remove('hidden'); }
function closeModal(id) { document.getElementById(id).classList.add('hidden'); }