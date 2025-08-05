document.addEventListener('DOMContentLoaded', function () {
    // Apply date mask
    $('#dataNascimento').mask('00/00/0000', {placeholder: "__/__/____"});

    const cadastroForm = document.getElementById('cadastroForm');
    const nomeInput = document.getElementById('nomePaciente');
    const sobrenomeInput = document.getElementById('sobrenomePaciente');
    const senhaInput = document.getElementById('senha');
    const redigitarSenhaInput = document.getElementById('redigitarSenha');
    const passwordMatchError = document.getElementById('passwordMatchError');
    const cadastrarButton = cadastroForm.querySelector('button[type="submit"]');

    // Toggle password visibility
    document.getElementById('togglePassword1').addEventListener('click', function () {
        const type = senhaInput.getAttribute('type') === 'password' ? 'text' : 'password';
        senhaInput.setAttribute('type', type);
        this.querySelector('i').classList.toggle('fa-eye');
        this.querySelector('i').classList.toggle('fa-eye-slash');
    });

    document.getElementById('togglePassword2').addEventListener('click', function () {
        const type = redigitarSenhaInput.getAttribute('type') === 'password' ? 'text' : 'password';
        redigitarSenhaInput.setAttribute('type', type);
        this.querySelector('i').classList.toggle('fa-eye');
        this.querySelector('i').classList.toggle('fa-eye-slash');
    });

    // Password match validation
    function checkPasswordMatch() {
        if (senhaInput.value !== redigitarSenhaInput.value) {
            redigitarSenhaInput.setCustomValidity('As senhas não coincidem.');
            cadastrarButton.disabled = true;
            return false;
        } else {
            redigitarSenhaInput.setCustomValidity('');
            cadastrarButton.disabled = false;
            return true;
        }
    }

    function displayPasswordError() {
        if (senhaInput.value !== redigitarSenhaInput.value && redigitarSenhaInput.value !== '') {
            passwordMatchError.textContent = 'As senhas não coincidem.';
        } else {
            passwordMatchError.textContent = '';
        }
    }

    senhaInput.addEventListener('input', checkPasswordMatch);
    redigitarSenhaInput.addEventListener('input', checkPasswordMatch);
    redigitarSenhaInput.addEventListener('blur', displayPasswordError);

    // Initial check for button state
    checkPasswordMatch();

    cadastroForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        if (!checkPasswordMatch()) {
            displayPasswordError();
            cadastroForm.reportValidity();
            return;
        }

        if (!cadastroForm.checkValidity()) {
            cadastroForm.reportValidity();
            return;
        }

        const formData = new FormData(cadastroForm);
        const data = Object.fromEntries(formData.entries());

        // Convert date to YYYY-MM-DD format
        if (data.DataNascimento) {
            const parts = data.DataNascimento.split('/');
            data.DataNascimento = `${parts[2]}-${parts[1]}-${parts[0]}`;
        }

        // Concatenate Nome and Sobrenome
        data.Nome = `${data.Nome} ${data.Sobrenome}`.trim();

        // Remove Sobrenome and SenhaDigitadaNovamente as they are not mapped to DB
        delete data.Sobrenome;
        delete data.SenhaDigitadaNovamente;

        try {
            const response = await fetch('/Cadastro/Salvar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            });

            const result = await response.json();

            if (result.success) {
                // Fechar a modal de cadastro e limpar o formulário imediatamente
                document.getElementById('cadastroModal').style.display = 'none';
                document.getElementById('main-container').classList.remove('content-blur');
                cadastroForm.reset();
                checkPasswordMatch();
                passwordMatchError.textContent = '';

                Swal.fire({
                    title: 'Sucesso!',
                    text: result.message,
                    icon: 'success',
                    confirmButtonText: 'OK',
                    customClass: {
                        confirmButton: 'btn-swal-confirm'
                    }
                });
            } else {
                Swal.fire({
                    title: 'Erro!',
                    text: result.message,
                    icon: 'error',
                    showCancelButton: result.message.includes('e-mail já está cadastrado'), // Mostrar botão Cancelar apenas se for erro de e-mail duplicado
                    confirmButtonText: 'Tentar Novamente',
                    cancelButtonText: 'Entrar',
                    customClass: {
                        confirmButton: 'btn-swal-error',
                        cancelButton: 'btn-swal-cancel'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Usuário clicou em 'Tentar Novamente', nada a fazer, modal de erro permanece
                    } else if (result.isDismissed && result.dismiss === Swal.DismissReason.cancel) {
                        // Usuário clicou em 'Entrar'
                        document.getElementById('cadastroModal').style.display = 'none';
                        document.getElementById('main-container').classList.remove('content-blur');
                        // Abrir modal de login
                        document.getElementById('loginModal').style.display = 'block';
                        document.getElementById('main-container').classList.add('content-blur');
                    }
                });
            }
        } catch (error) {
            console.error('Erro ao enviar o formulário de cadastro:', error);
            Swal.fire({
                title: 'Erro de Comunicação!',
                text: 'Não foi possível conectar ao servidor. Por favor, tente mais tarde.',
                icon: 'error',
                confirmButtonText: 'OK',
                customClass: {
                    confirmButton: 'btn-swal-error'
                }
            });
        }
    });
});
