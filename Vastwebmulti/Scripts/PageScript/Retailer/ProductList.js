function getProductBySubCataID(subcatid)
{
    var id = subcatid;
    //alert(id);
    $('#SubCatId').val(id);
    $('#lstProductsSubmit').click();
}
$('#Catagories_li').on('click', function () {
    //window.location.replace("Index");
    $('#ProductName').val('');
    $('#SubCatId').val('');
    $('#Price').val('');
    $('#SortBy').val('');
    $('#FilterBy').val('');
    $('#lstProductsSubmit').click();
})
//$('#btnSearchSubmit').on('click', function () {
//    alert("txt Search is showing");
//    var term = $('#txtSearch').val();
//    if (term != null && term != '')
//    {
//        alert(term);
//        $("#ProductName").val(term);
//        $("#lstProductsSubmit").click();
//    }
//})
function OnComplete()
{
    //alert("chalja");
    //$("#txtSearch").autocomplete({
    //    source: '/FindProductByName',
    //    minLength: 1
    //});
}

