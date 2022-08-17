function PrintQrCodeDiv(id) {
    var divToPrint = "";
    if (id == '' || id == null || id == undefined) {
        divToPrint = document.getElementById('qrcodeprint').innerHTML;
    }
    else {
        divToPrint = document.getElementById('qrcodeprint' + id).innerHTML;
    }
    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    newWin.document.write('<html><body onload="window.print()">' + divToPrint + '</body></html>');
    newWin.document.close();
    setTimeout(function () { newWin.close(); }, 500);
}