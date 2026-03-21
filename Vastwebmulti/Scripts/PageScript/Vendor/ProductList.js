
function ReplicateProduct(idn)
{
    var url = "ReplicateProduct";

    //alert("sdfff : " + $(this).data('request-url'))
    //alert(url);
    //alert(idn)
    $.ajax({
        type: "POST",
        url: url,
        data: { id: idn },
        success: function (data) {
            //alert(data);
            $("#submitBtnlstProduct").click();
        },
        error: function (xhr, err) {
            //alert("readyState: " + xhr.readyState + "\nstatus: " + xhr.status);
            //alert("responseText: " + xhr.responseText);
            console.log(xhr.responseText);
        },
        dataType: 'json'
    });
}