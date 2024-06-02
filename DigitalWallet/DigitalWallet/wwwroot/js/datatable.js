$(document).ready(function () {
    $('#table').DataTable({
        ajax: {
            url: 'Transactions?handler=Transactions',
            dataSrc: ''
        },
        columns: [
            {
                data: 'time'
            },
            {
                data: 'amount'
            },
            {
                data: 'status',
                render: function (data, type) {
                    switch (data) {
                        case 0:
                            return getIcon('In Progress', 'bi-arrow-repeat text-warning');
                        case 1:
                            return getIcon('Failed', 'bi-dash-circle text-danger');
                        case 2:
                            return getIcon('Success', 'bi-check2-circle text-success');
                        default:
                            return getIcon('Unknown', 'bi-question-circle text-info');
                    }
                }
            },
            {
                data: 'id',
                sortable: false,
                render: function (data, type) {
                    return `<a href="/TransactionDetails?id=${data}">Details</a>`;
                }
            }
        ]
    });
});

function getIcon(title, iconClass) {
    return `<i title="${title}" class="bi ${iconClass}"></i>`;
}