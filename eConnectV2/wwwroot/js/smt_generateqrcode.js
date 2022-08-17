function PrintQRcode(qrcode, weekno) {
    var requestUrl = '/SMT/GenerateQRCode';
    $.post(requestUrl,
        {
            qrcode: qrcode
        },
        function (data, status) {
            if (data == '') {
                Swal.fire({
                    title: "Oops...",
                    text: "Something went wrong!",
                    width: 500,
                    padding: "3em",
                    background: "#fff url(/images/trees.png)",
                    backdrop: '\n\t\t\t    rgba(0,0,123,0.4)\n\t\t\t    url("/images/nyan-cat.gif")\n\t\t\t    center left\n\t\t\t    no-repeat\n\t\t\t  '
                });
            }
            else {
                var divToPrint = '<table width="100%" height="100%"><tr><td align="center" valign="middle"><div><img src="' + data + '" alt="" /><br />';
                divToPrint += '<span class="font-weight-bold">' + qrcode + '</span><br /><strong><span>Week No. ' + weekno + '</span></strong></div></td></tr></table>';
                var newWin = window.open('', 'Print-Window');
                newWin.document.open();
                newWin.document.write('<html><body onload="window.print()">' + divToPrint + '</body></html>');
                newWin.document.close();
                setTimeout(function () { newWin.close(); }, 500);
            }
        });
}