document.addEventListener('DOMContentLoaded', function () {
    const loginModal = document.getElementById('loginModal');
    const loginBtn = document.getElementById('loginBtn');
    const closeLogin = document.getElementById('loginModalClose'); 
    const loginForm = document.getElementById('loginForm');
    const openCadastroFromLoginLink = document.getElementById('openCadastroFromLogin');
    const logoutBtn = document.getElementById('logoutBtn');

    if (loginBtn) {
        loginBtn.onclick = function (event) {
            event.preventDefault();
            loginModal.style.display = 'block';
        }
    }

    if (closeLogin) {
        closeLogin.onclick = function () {
            loginModal.style.display = 'none';
        }
    }

    window.addEventListener('click', function (event) {
        if (event.target == loginModal) {
            loginModal.style.display = 'none';
        }
    });

    if (openCadastroFromLoginLink) {
        openCadastroFromLoginLink.onclick = function (event) {
            event.preventDefault();
            loginModal.style.display = 'none'; // Close login modal
            document.getElementById('cadastroModal').style.display = 'block'; // Open cadastro modal
        }
    }

    loginForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        const email = document.getElementById('emailLogin').value;
        const senha = document.getElementById('senhaLogin').value;

        if (!email || !senha) {
            Swal.fire({
                title: 'Erro!',
                text: 'Por favor, preencha todos os campos.',
                icon: 'error',
                confirmButtonText: 'OK',
                customClass: {
                    popup: 'swal2-popup',
                    title: 'swal2-title',
                    htmlContainer: 'swal2-html-container',
                    confirmButton: 'swal2-confirm'
                }
            });
            return;
        }

        try {
            const response = await fetch('/Login/Entrar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Email: email, Senha: senha }),
            });

            const result = await response.json();

            if (result.success) {
                Swal.fire({
                    title: 'Sucesso!',
                    text: result.message,
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false,
                    customClass: {
                        popup: 'swal2-popup',
                        title: 'swal2-title',
                        htmlContainer: 'swal2-html-container',
                    },
                    showClass: {
                        popup: 'swal2-show',
                        backdrop: 'swal2-backdrop-show',
                        icon: 'swal2-icon-show'
                    },
                    hideClass: {
                        popup: 'swal2-hide',
                        backdrop: 'swal2-backdrop-hide',
                        icon: 'swal2-icon-hide'
                    }
                }).then(() => {
                    const appointmentModal = document.getElementById('appointmentModal');
                    if (appointmentModal && appointmentModal.style.display === 'block') {
                        localStorage.setItem('openAppointmentModalAfterLogin', 'true');
                    }
                    window.location.reload(); 
                });
            } else {
                Swal.fire({
                    title: 'Erro!',
                    text: result.message,
                    icon: 'error',
                    confirmButtonText: 'Tentar Novamente',
                    customClass: {
                        popup: 'swal2-popup',
                        title: 'swal2-title',
                        htmlContainer: 'swal2-html-container',
                        confirmButton: 'swal2-confirm'
                    }
                });
            }
        } catch (error) {
            console.error('Erro ao enviar o formulário de login:', error);
            Swal.fire({
                title: 'Erro de Comunicação!',
                text: 'Não foi possível conectar ao servidor. Por favor, tente mais tarde.',
                icon: 'error',
                confirmButtonText: 'OK',
                customClass: {
                    popup: 'swal2-popup',
                    title: 'swal2-title',
                    htmlContainer: 'swal2-html-container',
                    confirmButton: 'swal2-confirm'
                }
            });
        }
    });

    if (logoutBtn) {
        logoutBtn.addEventListener('click', async function (event) {
            event.preventDefault();
            try {
                const response = await fetch('/Login/Logout', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                const result = await response.json();

                if (result.success) {
                    window.location.reload();
                } else {
                    Swal.fire({
                        title: 'Erro!',
                        text: 'Não foi possível fazer logout.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                        customClass: {
                            popup: 'swal2-popup',
                            title: 'swal2-title',
                            htmlContainer: 'swal2-html-container',
                            confirmButton: 'swal2-confirm'
                        }
                    });
                }
            } catch (error) {
                console.error('Erro ao fazer logout:', error);
                Swal.fire({
                    title: 'Erro de Comunicação!',
                    text: 'Não foi possível conectar ao servidor para fazer logout.',
                    icon: 'error',
                    confirmButtonText: 'OK',
                    customClass: {
                        popup: 'swal2-popup',
                        title: 'swal2-title',
                        htmlContainer: 'swal2-html-container',
                        confirmButton: 'swal2-confirm'
                    },
                    showClass: {
                        popup: 'swal2-show',
                        backdrop: 'swal2-backdrop-show',
                        icon: 'swal2-icon-show'
                    },
                    hideClass: {
                        popup: 'swal2-hide',
                        backdrop: 'swal2-backdrop-hide',
                        icon: 'swal2-icon-hide'
                    }
                });
            }
        });
    }
});
