
function fundreciveonload(frm ,to)
{
        $('#date').bootstrapMaterialDatePicker
       ({
           clearButton: false
       });
        $('#txt_to_date').bootstrapMaterialDatePicker
        ({
            weekStart: 0, format: 'DD/MM/YYYY'
        });
        $('#txt_frm_date').bootstrapMaterialDatePicker
        ({
            weekStart: 0, format: 'DD/MM/YYYY'
        }).on('change', function (e, date) {
            $('#txt_to_date').bootstrapMaterialDatePicker('setMinDate', date);
        });

        $('#min-date').bootstrapMaterialDatePicker({ format: 'DD/MM/YYYY HH:mm', minDate: new Date() });

        var from = frm;
        var to = to;
        if (from != "" && to != "") {
            $('#txt_frm_date').val(from);
            $('#txt_to_date').val(to);
        }
        else {
            var month = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");
            var d = new Date();
            var curr_date = d.getDate();
            var curr_month = d.getMonth();
            var curr_year = d.getFullYear();

            var tt = curr_date + "-" + month[curr_month]
            + "-" + curr_year;
            document.getElementById("txt_frm_date").value = tt;
            document.getElementById("txt_to_date").value = tt;
        }

        $('[data-toggle="tooltip"]').tooltip();
    
}





             function GenerateInvoice(id,ReciveFrom, OldRemain, Value, Commission, FinalValue, NewRemain, Date) {
                 //alert(ReciveFrom+ ''+OldRemain+ ''+Value +''+Commission+ ''+FinalValue+ ''+NewRemain+''+Date)
                 var url = '@Url.Action("GotoInvoicePDF", "Home")?Id=' + id + '&ReciveFrom=' + ReciveFrom + '&OldRemain=' + OldRemain + '&Value=' + Value + '&Commission=' + Commission + '&FinalValue=' + FinalValue + '&NewRemain=' + NewRemain + '&Date=' + Date + '';
                 //window.location.href = url;
                 window.open(url, '_blank');
             }

