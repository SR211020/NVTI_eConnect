$(document).ready(function () {
    $(function () {
        $('#MaterialCategory').on('select2:select', function (e) {
            $('#PartNo').val('');
            BindSupplier(e.params.data.id);
        });
        $('#VendorName').on('select2:select', function (e) {
            $('#PartNo').val('');
            $('#PartNo').val(e.params.data.id);
            $('#VendorNameHdn').val(e.params.data.text);
        });
    });
});
function BindSupplier(material) {
    var requestUrl = 'SMT/GetMaterialSuppliers';
    $.post(requestUrl,
        {
            material: material
        },
        function (data, status) {
            $("#VendorName").empty();
            $("#VendorName").append($("<option></option>").val('').html('Select'));
            $.each(data, function (index, item) {
                $("#VendorName").append($("<option></option>").val(item.value).html(item.text));
            });
        });
}