function onMinus(QtyVal,id)
{
    var QTY = parseInt(QtyVal);
    //alert(QTY);
    //alert(QTY - 1);
    $('#productId').val(id);
    $('#QTY').val(QTY - 1);
    $('#viewCartSubmit').click();
}
function onPluse(QtyVal,id) {
    var QTY = parseInt(QtyVal);
    //alert(QTY);
    //alert(QTY + 1);
    $('#productId').val(id);
    $('#QTY').val(QTY + 1);
    $('#viewCartSubmit').click();
}
function onChange(textBox,id)
{
    var edValue = textBox.value;
    var QTY = parseInt(edValue);
    //alert(edValue);
    //alert(id);
    $('#productId').val(id);
    $('#QTY').val(QTY);
    $('#viewCartSubmit').click();
}
function isNumber(evt) {
    var iKeyCode = (evt.which) ? evt.which : evt.keyCode
    if (iKeyCode != 46 && iKeyCode > 31 && (iKeyCode < 48 || iKeyCode > 57))
        return false;
    return true;
}
function onRemoveItem(id)
{
    $('#removeCartProductId').val(id);
    $('#removeCartSubmit').click();
}