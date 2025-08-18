document.addEventListener('DOMContentLoaded', function () {
    const appointmentModal = document.getElementById('appointmentModal');
    const scheduleBtn = document.getElementById('scheduleBtn');
    const closeAppointment = document.getElementById('appointmentModalClose'); // Use the specific ID
    const dateInput = document.getElementById('appointmentDate');
    const timeSlotsContainer = document.getElementById('timeSlots');
    const appointmentForm = document.getElementById('appointmentForm');
    const confirmAppointmentBtn = appointmentForm.querySelector('button[type="submit"]');

    const onlineAppointmentRadio = document.getElementById('onlineAppointment');
    const presentialAppointmentRadio = document.getElementById('presentialAppointment');
    const appointmentValueGroup = document.getElementById('appointmentValueGroup');
    const appointmentValueInput = document.getElementById('appointmentValue');

    function updateAppointmentValue() {
        if (onlineAppointmentRadio.checked) {
            appointmentValueInput.value = 'R$ 100,00';
            appointmentValueGroup.style.display = 'block';
        } else if (presentialAppointmentRadio.checked) {
            appointmentValueInput.value = 'R$ 180,00';
            appointmentValueGroup.style.display = 'block';
        } else {
            appointmentValueInput.value = '';
            appointmentValueGroup.style.display = 'none';
        }
    }

    onlineAppointmentRadio.addEventListener('change', updateAppointmentValue);
    presentialAppointmentRadio.addEventListener('change', updateAppointmentValue);

    let fp; // flatpickr instance

    // Inicialização do flatpickr
    fp = flatpickr(dateInput, {
        minDate: 'today',
        dateFormat: 'd/m/Y',
        locale: 'pt',
        onChange: function(selectedDates) {
            if (selectedDates.length > 0) {
                generateTimeSlots(selectedDates[0]);
            }
        },
        onOpen: function() {
            timeSlotsContainer.innerHTML = '';
            timeSlotsContainer.style.display = 'none';
        }
    });

    // Função para habilitar/desabilitar campos do modal de agendamento
    window.setAppointmentModalEnabled = function(enabled) {
        if (fp) {
            fp.set('clickOpens', enabled); // Controla se o flatpickr pode ser aberto
        }
        dateInput.disabled = !enabled;
        confirmAppointmentBtn.disabled = !enabled;
        // Adicionar/remover classe para feedback visual de desabilitado
        appointmentModal.classList.toggle('modal-disabled-overlay', !enabled);
        // Desabilitar/habilitar botões de horário se existirem
        const timeSlotButtons = timeSlotsContainer.querySelectorAll('.time-slot-button');
        timeSlotButtons.forEach(button => {
            button.disabled = !enabled;
        });
    };

    // Verificar se o modal de agendamento deve ser reaberto após o login
    const openModalFlag = localStorage.getItem('openAppointmentModalAfterLogin');
    if (openModalFlag === 'true') {
        appointmentModal.style.display = 'block';
        window.setAppointmentModalEnabled(true);
        localStorage.removeItem('openAppointmentModalAfterLogin'); // Limpar a flag
    }

    function generateTimeSlots(selectedDate) {
        timeSlotsContainer.innerHTML = '';
        const day = selectedDate.getDay();

        if (day > 0 && day < 6) { // Monday to Friday
            for (let hour = 9; hour <= 17; hour++) {
                const button = document.createElement('button');
                button.type = 'button';
                button.className = 'btn time-slot-button';
                button.textContent = `${hour.toString().padStart(2, '0')}:00`;
                
                button.onclick = function() {
                    const finalDate = new Date(selectedDate);
                    finalDate.setHours(hour, 0, 0, 0);
                    
                    // Use hidden inputs to store the ISO string for date and time separately
                    document.getElementById('hiddenAppointmentDate').value = finalDate.toLocaleDateString('pt-BR'); // dd/MM/yyyy
                    document.getElementById('hiddenAppointmentTime').value = finalDate.toTimeString().split(' ')[0].substring(0, 5); // HH:MM
                    
                    dateInput.value = finalDate.toLocaleString('pt-BR', {
                        year: 'numeric', month: '2-digit', day: '2-digit',
                        hour: '2-digit', minute: '2-digit'
                    }).replace(',', '');

                    timeSlotsContainer.innerHTML = '';
                    timeSlotsContainer.style.display = 'none';
                };
                timeSlotsContainer.appendChild(button);
            }
        } else {
            timeSlotsContainer.innerHTML = '<p class="text-center text-muted">Não há horários disponíveis para este dia.</p>';
        }
        timeSlotsContainer.style.display = 'block';
    }

    if (scheduleBtn) {
        scheduleBtn.onclick = function (event) {
            event.preventDefault();
            const isAuthenticated = document.getElementById('isAuthenticated').value === 'true';

            appointmentModal.style.display = 'block';
            dateInput.value = '';
            timeSlotsContainer.innerHTML = '';
            timeSlotsContainer.style.display = 'none';

            if (!isAuthenticated) {
                window.setAppointmentModalEnabled(false);
                Swal.fire({
                    title: 'Atenção!',
                    text: 'Você precisa estar logado para agendar uma consulta. Por favor, faça login.',
                    icon: 'info',
                    confirmButtonText: 'OK',
                    customClass: {
                        popup: 'swal2-popup',
                        title: 'swal2-title',
                        htmlContainer: 'swal2-html-container',
                        confirmButton: 'swal2-confirm'
                    }
                }).then(() => {
                    document.getElementById('loginModal').style.display = 'block'; // Show login modal
                });
            } else {
                window.setAppointmentModalEnabled(true);
            }
        }
    }

    function closeModal() {
        appointmentModal.style.display = 'none';
        if (fp) fp.clear();
    }

    if (closeAppointment) {
        closeAppointment.onclick = closeModal;
    }

    window.addEventListener('click', function (event) {
        if (event.target == appointmentModal) {
            closeModal();
        }
    });

    appointmentForm.onsubmit = async function(event) {
        event.preventDefault();
        const hiddenDateValue = document.getElementById('hiddenAppointmentDate').value; // Get date from hidden input
        const hiddenTimeValue = document.getElementById('hiddenAppointmentTime').value; // Get time from hidden input
        const selectedAppointmentType = document.querySelector('input[name="appointmentType"]:checked');
        const pacienteId = document.getElementById('pacienteId')?.value; // Get pacienteId from hidden input

        if (!hiddenDateValue || !hiddenTimeValue || !selectedAppointmentType || !pacienteId) {
            Swal.fire({
                title: 'Erro!',
                text: 'Por favor, selecione uma data, hora, o tipo de consulta e certifique-se de estar logado.',
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

        const data = {
            data: hiddenDateValue, // Send date separately
            hora: hiddenTimeValue, // Send time separately
            tipoConsulta: selectedAppointmentType.value,
            pacienteId: pacienteId,
            valorConsulta: parseFloat(appointmentValueInput.value.replace('R$', '').replace(',', '.').trim()),
            statusConsulta: 1 // Assuming 1 means 'Agendado'
        };

        try {
            const response = await fetch('/Agendamento/Salvar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    Swal.fire({
                        title: 'Sucesso!',
                        text: result.message,
                        icon: 'success',
                        confirmButtonText: 'OK',
                        customClass: {
                            popup: 'swal2-popup',
                            title: 'swal2-title',
                            htmlContainer: 'swal2-html-container',
                            confirmButton: 'swal2-confirm'
                        }
                    });
                    closeModal();
                } else {
                    Swal.fire({
                        title: 'Erro!',
                        text: result.message,
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
            } else if (response.status === 401) { // Unauthorized
                Swal.fire({
                    title: 'Não Autenticado!',
                    text: 'Você precisa estar logado para agendar uma consulta.',
                    icon: 'warning',
                    confirmButtonText: 'OK',
                    customClass: {
                        popup: 'swal2-popup',
                        title: 'swal2-title',
                        htmlContainer: 'swal2-html-container',
                        confirmButton: 'swal2-confirm'
                    }
                }).then(() => {
                    document.getElementById('loginModal').style.display = 'block'; // Show login modal
                });
            } else {
                Swal.fire({
                    title: 'Erro!',
                    text: 'Ocorreu um erro ao salvar a consulta. Por favor, tente novamente.',
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
            console.error('Erro ao enviar o formulário:', error);
            Swal.fire({
                title: 'Erro de Comunicação!',
                text: 'Não foi possível conectar ao servidor. Por favor, tente novamente.',
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
    };
});

// Adiciona a localização em português para o flatpickr
if (typeof flatpickr !== 'undefined') {
    flatpickr.localize(flatpickr.l10ns.pt);
}
