
$("#Quantity").on('keypress blur keyup', function (e) {
   
    var k = e.keyCode || e.which;
    var id = $(this)[0].id;
    var str = $(this)[0].value;
    var length = str.length;
    if (length == 0 && e.type == "keyup") {
        e.preventDefault();
        if (id == 'Quantity') {
            document.getElementById("quantity_validation").innerHTML = "";
            return true;
        }

    }
});

$("#Amount").on('keypress blur keyup', function (e) {
    var k = e.keyCode || e.which;
    var id = $(this)[0].id;
    var str = $(this)[0].value;
    var length = str.length;
    if (length == 0 && e.type == "keyup") {
        e.preventDefault();
        if (id == 'Amount') {
            document.getElementById("amount_validation").innerHTML = "";
            return true;
        }

    }
});

$("#name").on('keypress blur keyup', function (e) {
    var k = e.keyCode || e.which;
    var id = $(this)[0].id;
    var str = $(this)[0].value;
    var length = str.length;
    if (length == 0 && e.type == "keyup") {
        e.preventDefault();
        if (id == 'name') {
            document.getElementById("name_validation").innerHTML = "";
            return true;
        }

    }
});
$("#email").on('keypress blur keyup', function (e) {
    var k = e.keyCode || e.which;
    var id = $(this)[0].id;
    var str = $(this)[0].value;
    var length = str.length;
    if (length == 0 && e.type == "keyup") {
        e.preventDefault();
        if (id == 'email') {
            document.getElementById("email_validation").innerHTML = "";
            return true;
        }

    }
});
$("#message").on('keypress blur keyup', function (e) {
    var k = e.keyCode || e.which;
    var id = $(this)[0].id;
    var str = $(this)[0].value;
    var length = str.length;
    if (length == 0 && e.type == "keyup") {
        e.preventDefault();
        if (id == 'message') {
            document.getElementById("message_validation").innerHTML = "";
            return true;
        }

    }
});