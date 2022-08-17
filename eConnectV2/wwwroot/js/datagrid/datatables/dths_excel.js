$(document).ready(function () {
    $('#dt-hs').dataTable({
        responsive: false,
        order: [[1, 'desc']],
        "columnDefs": [
            { "orderable": false, "targets": [0] },
        ],
        lengthChange: false,
        "pageLength": 20,
        dom:
            "<'row'<'col-sm-12 col-md-3 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-3 d-flex align-items-center justify-content-center'i><'col-sm-12 col-md-3 d-flex align-items-center justify-content-end'lB><'col-sm-12 col-md-3 d-flex align-items-center justify-content-end'p>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Download',
                title: null,
                titleAttr: 'Download',
                className: 'btn-outline-dark btn-xs mr-1',
                exportOptions: {
                    columns: ':not(.notexport)'
                }
            },
        ]
    });
    //$('#dt-hs').dataTable({
    //    responsive: false,
    //    lengthChange: false,
    //    "pageLength": 20,
    //    dom:
    //        "<'row'<'col-sm-12 col-md-4 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-4 d-flex align-items-center justify-content-center'i><'col-sm-12 col-md-4 d-flex align-items-center justify-content-end'lB>>" +
    //        "<'row'<'col-sm-12'tr>>" +
    //        "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
    //    //dom:
    //    //    //"<'row'<'col-sm-12 col-md-6 d-flex align-items-center justify-content-start'f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'lB>>" +
    //    //    //"<'row'<'col-sm-12'tr>>" +
    //    //    //"<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
    //    buttons: [
    //        {
    //            extend: 'excelHtml5',
    //            text: 'Download',
    //            title: null,
    //            titleAttr: 'Generate Excel',
    //            className: 'btn-outline-dark btn-xs mr-1',
    //            exportOptions: {
    //                columns: ':not(.notexport)'
    //            }
    //        },
    //    ]
    //});
});