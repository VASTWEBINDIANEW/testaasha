//sendeler number Validation
    $(".pp").on('keypress blur', function (e) {
    
        var k = e.keyCode || e.which;
        var id = $(this)[0].id;
        var str = $(this)[0].value;
        var length = str.length;
      
        if (e.type == 'keypress') {
            if (k != 8 && k != 9) {
                k = String.fromCharCode(k);
                var regex = /[0-9]/;
                if (!regex.test(k)) {
                    return false;
                }

                var regexs = /[6-9]/;
                if (length == 0 && !regexs.test(k)) {
                    return false;
                }
            }

            if (length >= 1) {
                $("#" + id).removeClass("error");
                $("p[id=" + id + "]").html("");
                document.getElementById("mobile1").innerHTML = "";
                document.getElementById("mobile2").innerHTML = "";
            }
            return true;
        } else if (e.type == 'blur') {
            var regex = /^[6789][0-9]+$/;
            if (!regex.test(str) && length > 0) {
                $("#" + id).addClass("error");
                document.getElementById("mobile1").innerHTML = "";
                document.getElementById("mobile2").innerHTML = "";
                  $("p[id=" + id + "]").html("Invalid mobile number");
                $("#" + id).val('');
                 
            }

            else {
            
                $("#" + id).removeClass("error");
                $("p[id=" + id + "]").html("");
                event.preventDefault();
            }
            return true;
        }
    });
   


//continue btn validation
//    function continuebuttonvalidation(mobile)
//{
//    if (mobile == "") {

//        document.getElementById("mobile1").innerHTML = "Enter remitter mobile number";
//        event.preventDefault();
//    }
//    else if (mobile != "") {
//        var mobile = document.getElementById("mobile").value.length;
//        if (mobile != 10) {
//            document.getElementById("mobile1").innerHTML = "Number should be [10] digits";
//            event.preventDefault();
//        }
//        else {

//            event.preventDefault();
//        }

//    }
//}

//enter only digit number script
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
