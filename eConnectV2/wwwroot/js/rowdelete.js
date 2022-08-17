function RowDeleteWithIsInUsed(id, url1, url2) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!"
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                url: url1,
                type: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        $.ajax({
                            url: url2,
                            type: "POST",
                            data: { id: id },
                            success: function () {
                                Swal.fire("Deleted!", response.responseMsg, "success").then(function () {
                                    window.location.reload();
                                })
                            }
                        });
                    } else {
                        Swal.fire("Already in use!", response.responseMsg, "error");
                    }
                },
                error: function () {
                    Swal.fire("Oops...", "Something went wrong!", "error");
                }
            });
        }
    });
}

function RowDelete(id, url1) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!"
    }).then(function (result) {
        if (result.value) {
            $.ajax({
                url: url1,
                type: "POST",
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        Swal.fire("Deleted!", response.responseMsg, "success").then(function () {
                            window.location.reload();
                        })
                    } else {
                        Swal.fire("Oops...", "Something went wrong!", "error");
                    }
                },
                error: function () {
                    Swal.fire("Oops...", "Something went wrong!", "error");
                }
            });
        }
    });
}