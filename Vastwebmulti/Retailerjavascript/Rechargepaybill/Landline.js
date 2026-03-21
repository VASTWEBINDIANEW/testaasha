/* get landline dropdown text */
function getlandlineddltext(x) {

    if (x === "BSNL Landline") {

        document.getElementById("Divaccountnumber").style.display = "block";
      /*  document.getElementById("divLLILLC").style.display = "block";*/
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";

    }

    if (x === "BSNL Landline - Corporate") {

        document.getElementById("Divaccountnumber").style.display = "block";
     /*   document.getElementById("divLLILLC").style.display = "block";*/
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";

    }

    if (x === "MTNL Delhi Landline") {

        document.getElementById("Divaccountnumber").style.display = "block";
      /*  document.getElementById("divLLILLC").style.display = "none";*/
        document.getElementById("DivSTDCode").style.display = "none";
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";

    }

    if (x === "Airtel Landline") {
        document.getElementById("Divaccountnumber").style.display = "none";
     /*   document.getElementById("divLLILLC").style.display = "none";*/
        document.getElementById("DivSTDCode").style.display = "none";
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";
    }
    if (x === "Reliance LANDLINE") {
        document.getElementById("Divaccountnumber").style.display = "none";
    /*    document.getElementById("divLLILLC").style.display = "none";*/
        document.getElementById("DivSTDCode").style.display = "block";
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";

    }
    if (x === "Tata Docomo LANDLINE") {
        document.getElementById("Divaccountnumber").style.display = "none";
       /* document.getElementById("divLLILLC").style.display = "none";*/
        document.getElementById("DivSTDCode").style.display = "none";
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";

    }
    if (x === "MTNL - Mumbai") {
        document.getElementById("Divaccountnumber").style.display = "block";
        document.getElementById("DivSTDCode").style.display = "none";
       /* document.getElementById("divLLILLC").style.display = "none";*/
        document.getElementById("txtlandline").value = "";
        document.getElementById("STDCODE").value = "";
        document.getElementById("txtaccount").value = "";
        document.getElementById("landamount").value = "";
        document.getElementById("landlinemessage").innerHTML = "";
        document.getElementById("stdcodemessage").innerHTML = "";
        document.getElementById("accountmessage").innerHTML = "";
        document.getElementById("amountmessage").innerHTML = "";


    }
}


/* get landline continue button value */

function getlandlinecontinuebtn(ddllandline, landlineno, Stdcode, accountno, amount, rdio) {
    
    if (ddllandline === "Select Service Provider") {
        document.getElementById("landlineopt").innerHTML = "Select Any Operator";
        event.preventDefault();
        return false;
    }

    else {
        document.getElementById("landlineopt").innerHTML = "";
        if ((landlineno === "" || Stdcode === "" || accountno === "" || amount === "") && ddllandline === "BSNL Landline") {
            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }
            if (Stdcode === "") {
                document.getElementById("stdcodemessage").innerHTML = "STD Code";
            }
            else {
                document.getElementById("stdcodemessage").innerHTML = "";
            }
            if (accountno === "") {
                document.getElementById("accountmessage").innerHTML = "Enter account number";
            }
            else {
                document.getElementById("accountmessage").innerHTML = "";
            }
            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();
            return false;
        }
        else {
            if (ddllandline === "BSNL Landline") {
                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("accountmessage").innerHTML = "";
                document.getElementById("stdcodemessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;
                accountno = document.getElementById("txtaccount").value.length;

                if ((landlineno < 8 || landlineno > 10) && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [8-10] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";
                }
                if (Stdcode.charAt(0) !== "0") {

                    document.getElementById("stdcodemessage").innerHTML = "start with 0";
                    document.getElementById("stdcodemessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("stdcodemessage").innerHTML = "";

                }
                if (accountno !== 10 && accountno !== null && accountno !== "") {
                    document.getElementById("accountmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("accountmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("accountmessage").innerHTML = "";
                    return true;
                }

            }
        }

        if (ddllandline === "BSNL Landline - Corporate") {
            document.getElementById("amountmessage").innerHTML = "";
            document.getElementById("accountmessage").innerHTML = "";
            document.getElementById("stdcodemessage").innerHTML = "";
            document.getElementById("landlinemessage").innerHTML = "";
            landlineno = document.getElementById("txtlandline").value.length;
            accountno = document.getElementById("txtaccount").value.length;

            if ((landlineno < 8 || landlineno > 10) && landlineno !== null && landlineno !== "") {
                document.getElementById("landlinemessage").innerHTML = "Number should be [8-10] digits";
                document.getElementById("landlinemessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }
            if (Stdcode.charAt(0) !== "0") {

                document.getElementById("stdcodemessage").innerHTML = "start with 0";
                document.getElementById("stdcodemessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("stdcodemessage").innerHTML = "";

            }
            if (accountno !== 10 && accountno !== null && accountno !== "") {
                document.getElementById("accountmessage").innerHTML = "Number should be [10] digits";
                document.getElementById("accountmessage").style.display = "block";
                event.preventDefault();
                return false;
            }
            else {

                document.getElementById("accountmessage").innerHTML = "";
                return true;
            }

        }
    }

        /* MTNL Delhi Landline */

        if ((landlineno === "" || accountno === "" || amount === "") && ddllandline === "MTNL Delhi Landline") {
            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }

            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();
            return false;
        }
        else {
            if (ddllandline === "MTNL Delhi Landline") {
                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                document.getElementById("accountmessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;
                if (landlineno !== 8 && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [8] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";

                }
                if (accountno !== 10 && accountno !== null && accountno !== "") {
                    document.getElementById("accountmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("accountmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("accountmessage").innerHTML = "";
                    return true;
                }

            }
        }

        /* MTNL - Mumbai */
        if ((landlineno === "" || accountno === "" || amount === "") && ddllandline === "MTNL - Mumbai") {

            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }

            if (accountno === "") {
                document.getElementById("accountmessage").innerHTML = "Enter account number";
            }
            else {
                document.getElementById("accountmessage").innerHTML = "";
            }
            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();
            return false;
        }
        else {
            if (ddllandline === "MTNL - Mumbai") {
                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("accountmessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;
                accountno = document.getElementById("txtaccount").value.length;

                if (landlineno !== 8 && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [8] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";
                }

                if (accountno !== 10 && accountno !== null && accountno !== "") {
                    document.getElementById("accountmessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("accountmessage").style.display = "block";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("accountmessage").innerHTML = "";
                    return true;
                }

            }
        }

        /* Airtel Landline */

        if ((landlineno === "" || amount === "") && ddllandline === "Airtel Landline") {
            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number with STD Code";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }
            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();

        }
        else {
            if (ddllandline === "Airtel Landline") {
                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("stdcodemessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;
                if (landlineno !== 11 && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();

                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";
                    return true;
                }
            }
        }

        /* Reliance LANDLINE */

        if ((landlineno === "" || Stdcode === "" || amount === "") && ddllandline === "Reliance LANDLINE") {
            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }
            if (Stdcode === "") {
                document.getElementById("stdcodemessage").innerHTML = "STD Code";
            }
            else {
                document.getElementById("stdcodemessage").innerHTML = "";
            }
            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();
            return false;
        }
        else {
            if (ddllandline === "Reliance LANDLINE") {
                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("stdcodemessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;
                if (landlineno !== 8 && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [8] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();

                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";

                }
                if (Stdcode.charAt(0) !== "0") {

                    document.getElementById("stdcodemessage").innerHTML = "start with 0";
                    document.getElementById("stdcodemessage").style.display = "block";
                    event.preventDefault();

                }
                else {
                    document.getElementById("stdcodemessage").innerHTML = "";
                    return true;

                }
            }
        }

        /* Tata Docomo LANDLINE */


        if ((landlineno === "" || amount === "") && ddllandline === "Tata Docomo LANDLINE") {
            if (landlineno === "") {
                document.getElementById("landlinemessage").innerHTML = "Enter landline number";
            }
            else {
                document.getElementById("landlinemessage").innerHTML = "";
            }
            if (amount === "") {
                document.getElementById("amountmessage").innerHTML = "Amount";
            }
            else {
                document.getElementById("amountmessage").innerHTML = "";
            }
            event.preventDefault();
            return false;
        }
        else {
            if (ddllandline === "Tata Docomo LANDLINE") {

                document.getElementById("amountmessage").innerHTML = "";
                document.getElementById("stdcodemessage").innerHTML = "";
                document.getElementById("landlinemessage").innerHTML = "";
                landlineno = document.getElementById("txtlandline").value.length;

                if (landlineno !== 11 && landlineno !== null && landlineno !== "") {
                    document.getElementById("landlinemessage").innerHTML = "Number should be [12] digits";
                    document.getElementById("landlinemessage").style.display = "block";
                    event.preventDefault();
                }
                else {
                    document.getElementById("landlinemessage").innerHTML = "";
                    return true;
                }
            }
        }
    }
