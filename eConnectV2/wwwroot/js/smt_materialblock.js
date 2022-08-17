function BlockMaterial(qrcode, partno, mcode, area) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, block it!"
    }).then(function (result) {
        if (result.value) {
            var requestUrl = '/SMT/BlockMaterial';
            $.ajax({
                url: requestUrl,
                type: "POST",
                data: { qrcode: qrcode, partno: partno, mcode: mcode, area: area },
                success: function (response) {
                    if (response[0].flag == _errorStatus) {
                        Swal.fire("Info!", response.msg, "error");
                    } else {
                        Swal.fire("Blocked!", response.msg, "success").then(function () {
                            window.location.reload();
                        })
                    }
                },
                error: function () {
                    Swal.fire("Oops...", "Something went wrong!", "error");
                }
            });
        }
    });
}