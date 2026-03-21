/* get gas dropdown text */

function getgasdropdowntext(x) {
    //if (x === "Mahanagar Gas") {
    //    $("#divgasbilldiv").addClass("col-md-3");
    //    $("#divgasbilldiv").removeClass("col-md-4");
    //}
    //else {
    //    $("#divgasbilldiv").addClass("col-md-4");
    //    $("#divgasbilldiv").removeClass("col-md-3");
    //}
    if (x === "Mahanagar Gas") {
        document.getElementById("divbillgroup").style.display = "block";
        document.getElementById("txtgasno").placeholder = "Enter CA Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Adani Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Customer Id";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Gujarat Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Customer Id";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Indraprastha Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter BP Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Haryana City Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter CRN Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }

    if (x === "Sabarmati Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Customer Id";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }

    if (x === "Siti Energy - Uttar Pradesh") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter ARN Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }

    if (x === "Unique Central Piped Gases Pvt Ltd (UCPGPL)") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Consumer Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Vadodara Gas Limited") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Consumer Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }
    if (x === "Tripura Natural Gas") {
        document.getElementById("divbillgroup").style.display = "none";
        document.getElementById("txtgasno").placeholder = "Enter Consumer Number";
        document.getElementById("txtgasno").value = "";
        document.getElementById("gasmessage").innerHTML = "";
        document.getElementById("gasamountmessage").innerHTML = "";
    }

    /*LPG*/

    if (x === "HP Gas") {
        document.getElementById("txtlpggasno").placeholder = "Enter Consumer Number";
        document.getElementById("lpgbillgroup").placeholder = "Enter Distributor ID";
        document.getElementById("txtlpggasno").value = "";
        document.getElementById("lpggasmessage").innerHTML = "";
        document.getElementById("lpggasamountmessage").innerHTML = "";
    }
    if (x === "Bharat Gas") {
        document.getElementById("txtlpggasno").placeholder = "Enter LPG ID";
        document.getElementById("lpgbillgroup").placeholder = "Enter Contact Number";
        document.getElementById("txtlpggasno").value = "";
        document.getElementById("lpggasmessage").innerHTML = "";
        document.getElementById("lpggasamountmessage").innerHTML = "";
    }
    if (x === "Indane Gas") {
        document.getElementById("txtlpggasno").placeholder = "Enter Consumer Number";
        document.getElementById("lpgbillgroup").placeholder = "Enter Distributor Code";
        document.getElementById("txtlpggasno").value = "";
        document.getElementById("lpggasmessage").innerHTML = "";
        document.getElementById("lpggasamountmessage").innerHTML = "";
    }
}

/*gas Mahanagar Gas continue button function*/

function getgascontiunebtn(gasddl, txtgasno, gasamount, billgroup) {
    if ((txtgasno == "" || billgroup == "" || gasamount == "") && gasddl == "Mahanagar Gas") {
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter CA number";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (billgroup == "") {
            document.getElementById("billgroupmessage").innerHTML = "Please Enter Bill Group No";
        }
        else {
            document.getElementById("billgroupmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Mahanagar Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            document.getElementById("billgroupmessage").innerHTML = "";
            document.getElementById("gasamountmessage").innerHTML = "";
            billgroupunit = document.getElementById("billgroup").value.length;
            if ((billgroupunit < 1 || billgroupunit > 8) && billgroupunit !== "" && billgroupunit !== null) {
                document.getElementById("billgroupmessage").innerHTML = "Bill Group no should be [1-8] digits";
                document.getElementById("billgroupmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 12 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [12] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }

    /*Adani Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Adani Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter customer Id";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Adani Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 10 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [10] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }

    /*Unique Central Piped Gases Pvt Ltd (UCPGPL) Gas*/
    if ((txtgasno == "" || gasamount == "") && gasddl == "Unique Central Piped Gases Pvt Ltd (UCPGPL)") {
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter customer Id";
        } else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Unique Central Piped Gases Pvt Ltd (UCPGPL)") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 8 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [8] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }

    /*Vadodara Gas Limited*/
    if ((txtgasno == "" || gasamount == "") && gasddl == "Vadodara Gas Limited") {
        alert(txtgasno);
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter customer Id";
        } else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Vadodara Gas Limited") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 7 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [7] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }

    /*Gujarat Gas*/

    /*Gujarat Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Gujarat Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter customer Id";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Gujarat Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 12 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [12] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }
        }
    }


    /*Indraprastha Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Indraprastha Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter BP number";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Indraprastha Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 10 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [10] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }
        }
    }



    /*Haryana City Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Haryana City Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter CRN number";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Haryana City Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if ((gasno < 8 || gasno > 12) && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [8-12] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                // $('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }

    /*Sabarmati Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Sabarmati Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter Customer Id";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Sabarmati Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if (gasno != 12 && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [12] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }


    /*Siti Energy - Uttar Pradesh*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Siti Energy - Uttar Pradesh") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("gasamountmessage").innerHTML = "";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter Customer Id";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Siti Energy - Uttar Pradesh") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if ((gasno < 7 || gasno > 9) && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [7-9] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }


    /*Tripura Natural Gas*/

    if ((txtgasno == "" || gasamount == "") && gasddl == "Tripura Natural Gas") {
        if (gasamount == "") {
            document.getElementById("gasamountmessage").innerHTML = "Please Enter Amount";
        }
        if (txtgasno == "") {
            document.getElementById("gasmessage").innerHTML = "Please enter Customer Id";
        }
        else {
            document.getElementById("gasmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        if (gasddl == "Tripura Natural Gas") {
            document.getElementById("gasmessage").innerHTML = "";
            gasno = document.getElementById("txtgasno").value.length;
            if ((gasno < 1 || gasno > 20) && gasno != "" && gasno != null) {
                document.getElementById("gasmessage").innerHTML = "Number should be [1-20] digits";
                document.getElementById("gasmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("gasamountmessage").innerHTML = "";
                document.getElementById("billgroupmessage").innerHTML = "";
                document.getElementById("gasmessage").innerHTML = "";
                //$('#mygasviewbillmodel').modal('show');
                event.preventDefault();
                return true;
            }

        }
    }
}


function getlpggascontiunebtn(gasddl, txtgasno, gasamount, billgroup, registermobile) {
    debugger;
    /*-------------------------------------------------------------*/
    if ((txtgasno == "" || billgroup == "" || gasamount == "" || registermobile == "")) {
        if (txtgasno == "") {
            if (gasddl == "HP Gas") {
                document.getElementById("lpggasmessage").innerHTML = "Please enter Consumer Number";
            }
            if (gasddl == "Bharat Gas") {
                document.getElementById("lpggasmessage").innerHTML = "Please enter Registered Contact Number";
            }
            if (gasddl == "Indane Gas") {
                document.getElementById("lpggasmessage").innerHTML = "Please enter Consumer Number";
            }
        }
        else {
            document.getElementById("lpggasmessage").innerHTML = "";
        }
        if (gasamount == "") {
            document.getElementById("lpggasamountmessage").innerHTML = "Please Enter Amount";
        }
        else {
            document.getElementById("lpggasamountmessage").innerHTML = "";
        }
        if (registermobile == "") {
            document.getElementById("registermomessage").innerHTML = "Please Enter mobile no.";
        }
        else {
            document.getElementById("registermomessage").innerHTML = "";
        }
        if (billgroup == "") {
            if (gasddl == "HP Gas") {
                document.getElementById("lpgbillgroupmessage").innerHTML = "Please enter Distributor ID";
            }
            if (gasddl == "Bharat Gas") {
                document.getElementById("lpgbillgroupmessage").innerHTML = "Please enter LPG ID";
            }
            if (gasddl == "Indane Gas") {
                document.getElementById("lpgbillgroupmessage").innerHTML = "Please enter Distributor Code";
            }
        }
        else {
            document.getElementById("lpgbillgroupmessage").innerHTML = "";
        }
        event.preventDefault();
        return false;
    }
    else {
        document.getElementById("lpggasamountmessage").innerHTML = "";
        document.getElementById("registermomessage").innerHTML = "";
        document.getElementById("lpgbillgroupmessage").innerHTML = "";
        document.getElementById("lpggasmessage").innerHTML = "";
        //$('#mygasviewbillmodel').modal('show');
        event.preventDefault();
        return true;
    }

}