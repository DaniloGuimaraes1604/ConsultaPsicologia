// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const logoutBtn = document.getElementById('logoutBtn');

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
                    window.location.reload(); // Recarregar a página para refletir o logout
                } else {
                    Swal.fire({
                        title: 'Erro!',
                        text: 'Não foi possível fazer logout. Por favor, tente novamente.',
                        icon: 'error',
                        confirmButtonText: 'OK',
                        customClass: {
                            confirmButton: 'btn-swal-error'
                        }
                    });
                }
            } catch (error) {
                console.error('Erro ao tentar fazer logout:', error);
                Swal.fire({
                    title: 'Erro de Comunicação!',
                    text: 'Não foi possível conectar ao servidor para fazer logout. Por favor, tente mais tarde.',
                    icon: 'error',
                    confirmButtonText: 'OK',
                    customClass: {
                        confirmButton: 'btn-swal-error'
                    }
                });
            }
        });
    }
});