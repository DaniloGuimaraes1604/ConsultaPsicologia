$(document).ready(function () {
    const pageSize = 15; // Number of records per page
    let currentPage = 1;

    function loadPacientes(page) {
        const nomeCompleto = $('#filterNomeCompleto').val();
        const nomeCompletoType = $('#filterNomeCompletoType').val();
        const dataNascimento = $('#filterDataNascimento').val();
        const dataNascimentoType = $('#filterDataNascimentoType').val();
        const email = $('#filterEmail').val();
        const emailType = $('#filterEmailType').val();

        $.ajax({
            url: '/Admin/GetPacientes',
            type: 'GET',
            data: {
                page: page,
                pageSize: pageSize,
                nomeCompleto: nomeCompleto,
                nomeCompletoType: nomeCompletoType,
                dataNascimento: dataNascimento,
                dataNascimentoType: dataNascimentoType,
                email: email,
                emailType: emailType
            },
            success: function (response) {
                $('#pacientesTableBody').html(response.html);
                updatePagination(response.totalPages, page);
                currentPage = page;
            },
            error: function (xhr, status, error) {
                console.error("Erro ao carregar pacientes: ", error, xhr); // Log full xhr for more info
                let errorMessage = 'Não foi possível carregar os pacientes.';
                if (xhr.responseText) {
                    errorMessage += ` Detalhes: ${xhr.responseText}`;
                } else if (error) {
                    errorMessage += ` Detalhes: ${error}`;
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Erro',
                    text: errorMessage
                });
            }
        });
    }

    function updatePagination(totalPages, currentPage) {
        const paginationControls = $('#paginationControls');
        paginationControls.empty();

        // Previous button
        paginationControls.append(`
            <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Anterior</a>
            </li>
        `);

        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            paginationControls.append(`
                <li class="page-item ${currentPage === i ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `);
        }

        // Next button
        paginationControls.append(`
            <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Próximo</a>
            </li>
        `);
    }

    // Filter icon click handler
    $('.filter-icon').on('click', function (e) {
        e.stopPropagation(); // Prevent click from propagating to document
        const filterFor = $(this).data('filter-for');
        $('.filter-dropdown').hide(); // Hide all other filter dropdowns
        $('#filterDropdown' + filterFor).toggle(); // Toggle current filter dropdown
    });

    // Apply filter button click handler
    $('.apply-filter-btn').on('click', function () {
        loadPacientes(1);
        $(this).closest('.filter-dropdown').hide(); // Hide dropdown after applying filter
    });

    // Clear filter button click handler
    $('.clear-filter-btn').on('click', function () {
        const filterId = $(this).data('filter-id');
        $('#filter' + filterId).val('');
        $('#filter' + filterId + 'Type').val(filterId === 'DataNascimento' ? 'equals' : 'contains');
        loadPacientes(1);
        $(this).closest('.filter-dropdown').hide(); // Hide dropdown after clearing filter
    });

    // Click outside to close dropdowns
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.filter-dropdown').length && !$(e.target).hasClass('filter-icon')) {
            $('.filter-dropdown').hide();
        }
    });

    $('#paginationControls').on('click', '.page-link', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        // Ensure page is within valid bounds
        if (page >= 1 && page <= totalPages) { 
            loadPacientes(page);
        }
    });

    // Initial load
    loadPacientes(1);
});