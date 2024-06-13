$(document).ready(function () {
    let table = $('#companies-table').DataTable({
        ajax: {
            url: 'Companies?handler=AllCompanies',
            dataSrc: ''
        },
        columnDefs: [
            {
                targets: [0,1,2],
                className: 'dt-body-left dt-head-left'
            },
            {
                targets: [3, 4],
                className: 'dt-body-center'
            }
        ],
        columns: [
            {
                data: 'name'
            },
            {
                data: 'walletsCount'
            },
            {
                data: 'transactionsCount'
            },
            {
                data: 'id',
                sortable: false,
                render: function (data, type) {
                    return `<a class="text-warning" href="EditCompany?id=${data}"><i class="bi bi-pencil"></i></a>`;
                }
            },
            {
                data: 'id',
                sortable: false,
                render: function (data, type) {
                    return `<a class="text-danger" href="DeleteCompany?id=${data}"><i class="bi bi-trash3"></i></a>`;
                }
            }
        ],
        order: {
            idx: 0,
            dir: 'asc'
        }
    });

    table.columns.adjust();
});