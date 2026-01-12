const API_BASE = 'http://localhost:5259/api'; // CHANGE PORT IF NEEDED

function getToken() {
    return localStorage.getItem('token');
}

async function apiCall(endpoint, method = 'GET', body = null) {
    const headers = { 'Content-Type': 'application/json' };
    const token = getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const config = { method, headers };
    if (body) config.body = JSON.stringify(body);

    try {
        const response = await fetch(`${API_BASE}${endpoint}`, config);

        if (response.status === 401) {
            logout(); // Token expired
            return null;
        }

        if (response.status === 204) return true; // No content (Delete success)

        const data = await response.json();
        if (!response.ok) {
            throw new Error(data.message || 'API Error');
        }
        return data;
    } catch (error) {
        console.error("API Call Failed:", error);
        alert(error.message);
        return null;
    }
}
