function toggleAuthMode() {
    const login = document.getElementById('login-form');
    const register = document.getElementById('register-form');
    if (login && register) {
        login.classList.toggle('hidden');
        register.classList.toggle('hidden');
    }
}

async function handleLogin() {
    const email = document.getElementById('login-email')?.value;
    const password = document.getElementById('login-password')?.value;

    const result = await apiCall('/auth/login', 'POST', { email, password });

    if (result && result.token) {
        localStorage.setItem('token', result.token);
        localStorage.setItem('user', JSON.stringify(result.user));
        location.reload(); // Refresh to enter app
    }
}

async function handleRegister() {
    const name = document.getElementById('reg-name')?.value;
    const email = document.getElementById('reg-email')?.value;
    const password = document.getElementById('reg-password')?.value;

    const result = await apiCall('/auth/register', 'POST', { name, email, password });

    if (result && result.token) {
        localStorage.setItem('token', result.token);
        localStorage.setItem('user', JSON.stringify(result.user));
        location.reload();
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    location.reload();
}
