// Small wrapper to ensure previous app.js changes live in wwwroot copy
// (main app logic already in root app.js for dev preview)

// Importing existing logic might be better, but we keep a minimal loader here.

document.addEventListener('DOMContentLoaded', () => {
    // No-op loader to ensure login flows find functions
});
