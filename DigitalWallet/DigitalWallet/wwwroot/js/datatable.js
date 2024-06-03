$(document).ready(function () {
    let table = $('#table').DataTable({
        ajax: {
            url: 'Transactions?handler=Transactions',
            dataSrc: ''
        },
        columnDefs: [
            {
                targets: "_all",
                className: 'dt-body-left dt-head-left'
            }
        ],
        columns: [
            {
                data: 'subject'
            },
            {
                data: 'time'
            },
            {
                data: 'amount',
                render: function (data, type) {
                    return data < 0 ? getAmountSpan('danger', data) : getAmountSpan('success', data);
                }
            },
            {
                data: 'status',
                render: function (data, type) {
                    switch (data) {
                        case 'InProgress':
                            return getIcon('In Progress', 'bi-arrow-repeat text-warning');
                        case 'Failed':
                            return getIcon('Failed', 'bi-dash-circle text-danger');
                        case 'Succeeded':
                            return getIcon('Success', 'bi-check2-circle text-success');
                        default:
                            return getIcon('Unknown', 'bi-question-circle text-info');
                    }
                }
            }
        ],
        order: {
            idx: 1,
            dir: 'desc'
        }
    });

    table.on('click', 'tbody tr', function () {
        let data = table.row(this).data();
        location.href = `/TransactionDetails?id=${data['id']}`;
    });
});

function getAmountSpan(textStyle, amount) {
    return `<span class="text-${textStyle}">${amount}$</span>`;
}

function getIcon(title, iconClass) {
    return `<i title="${title}" class="bi ${iconClass}"></i>`;
}