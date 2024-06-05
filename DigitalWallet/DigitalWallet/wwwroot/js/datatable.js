﻿$(document).ready(function () {
    let table = $('#table').DataTable({
        ajax: {
            url: 'Transactions?handler=AllTransactions',
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
                    return getAmountSpan(data < 0 ? 'danger' : 'success', data);
                }
            },
            {
                data: 'status',
                render: function (data, type) {
                    switch (data) {
                        case 'InProgress':
                            return getIcon('In Progress', 'warning', 'arrow-repeat');
                        case 'Failed':
                            return getIcon('Failed', 'danger', 'dash-circle');
                        case 'Succeeded':
                            return getIcon('Success', 'success', 'check2-circle');
                        default:
                            return getIcon('Unknown', 'info', 'question-circle');
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

function getIcon(statusText, textStyle, iconStyle) {
    return `<span class="badge text-bg-${textStyle}">${statusText} <i class="bi bi-${iconStyle}"></i></span>`;
}