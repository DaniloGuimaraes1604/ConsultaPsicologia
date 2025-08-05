document.addEventListener('DOMContentLoaded', function () {
    var cadastroModal = document.getElementById('cadastroModal');
    var cadastroBtn = document.getElementById('cadastroBtn');
    var cadastroSpan = document.getElementById('cadastroModalClose');
    var mainContainer = document.getElementById('main-container');

    var loginModal = document.getElementById('loginModal');
    var loginBtn = document.getElementById('loginBtn');
    var loginSpan = document.getElementById('loginModalClose');

    var appointmentModal = document.getElementById('appointmentModal');
        var scheduleBtn = document.getElementById('scheduleBtn');
    var appointmentSpan = document.getElementById('appointmentModalClose');

    // Function to update appointment modal title
    function updateAppointmentModalTitle(userName) {
        const titleElement = document.getElementById('appointmentModalTitle');
        if (titleElement) {
            titleElement.innerHTML = `Olá, ${userName}, selecione data e hora para sua consulta`;
        }
    }

    // Open Cadastro Modal
    if (cadastroBtn) {
        cadastroBtn.onclick = function (event) {
            event.preventDefault();
            cadastroModal.style.display = 'block';
            mainContainer.classList.add('content-blur');
        }
    }

    // Close Cadastro Modal
    if (cadastroSpan) {
        cadastroSpan.onclick = function () {
            cadastroModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        }
    }

    // Open Login Modal
    if (loginBtn) {
        loginBtn.onclick = function (event) {
            event.preventDefault();
            loginModal.style.display = 'block';
            mainContainer.classList.add('content-blur');
        }
    }

    // Close Login Modal
    if (loginSpan) {
        loginSpan.onclick = function () {
            loginModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        }
    }

    // Open Appointment Modal
    if (scheduleBtn) {
        scheduleBtn.onclick = function (event) {
            event.preventDefault();
            const logoutButton = document.getElementById('logoutBtn'); // Check if user is logged in
            console.log('Botão Agende sua Consulta clicado.');
            console.log('logoutButton existe (usuário logado)?', logoutButton);
            console.log('logoutButton é null ou undefined?', logoutButton === null || typeof logoutButton === 'undefined');

            if (logoutButton) { // User IS logged in
                appointmentModal.style.display = 'block';
                mainContainer.classList.add('content-blur');
                loginModal.style.display = 'none'; // Garante que a modal de login esteja fechada
                
                // Get user name from the displayed element
                const userNameElement = document.querySelector('.navbar-nav .nav-item span');
                if (userNameElement) {
                    const userNameText = userNameElement.textContent.replace('Olá, ', '').trim();
                    updateAppointmentModalTitle(userNameText);
                }
            } else { // User IS NOT logged in
                loginModal.style.display = 'block';
                mainContainer.classList.add('content-blur');
                console.log('Usuário não logado. Abrindo modal de login.');
            }
        }
    }

    // Close Appointment Modal
    if (appointmentSpan) {
        appointmentSpan.onclick = function () {
            appointmentModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        }
    }

    // Close modals when clicking outside
    window.onclick = function (event) {
        if (event.target == cadastroModal) {
            cadastroModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        } else if (event.target == loginModal) {
            loginModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        } else if (event.target == appointmentModal) {
            appointmentModal.style.display = 'none';
            mainContainer.classList.remove('content-blur');
        }
    }

    // Expose updateAppointmentModalTitle to other scripts
    window.updateAppointmentModalTitle = updateAppointmentModalTitle;
});