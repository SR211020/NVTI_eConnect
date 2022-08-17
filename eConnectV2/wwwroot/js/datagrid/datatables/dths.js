$(document).ready(function () {
    $('#dt-hs').dataTable({
        responsive: false,
        "order": [dtorderbycol, dtorderby],
        lengthChange: false,
        "pageLength": dtPageSize,
        dom:
            "<'row'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Excel',
                title: null,
                titleAttr: 'Generate Excel',
                className: 'btn-outline-dark btn-xs mr-1',
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
            {
                extend: 'csvHtml5',
                text: 'CSV',
                titleAttr: 'Generate CSV',
                className: 'btn-outline-dark btn-xs mr-1',
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
            {
                extend: 'pdfHtml5',
                text: 'PDF',
                titleAttr: 'Generate PDF',
                className: 'btn-outline-dark btn-xs mr-1',
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
            //{
            //    extend: 'copyHtml5',
            //    text: 'Copy',
            //    titleAttr: 'Copy to clipboard',
            //    className: 'btn-outline-primary btn-xs mr-1',
            //    exportOptions: {
            //        columns: ':not(.notexport)'
            //    }
            //},
            //{
            //    extend: 'print',
            //    text: 'Print',
            //    titleAttr: 'Print Table',
            //    className: 'btn-outline-primary btn-xs',
            //    exportOptions: {
            //        columns: ':not(.notexport)'
            //    }
            //}
        ]
    });
});