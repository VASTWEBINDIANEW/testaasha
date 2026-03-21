/*get electricty dropdown text function */
function electricitydropdown(x) {
    if (x === "MSEDC - MAHARASHTRA") {

        $("#divpayamount").addClass("col-md-1");
        $("#divpayamount").removeClass("col-md-2");
    }
    else {
        $("#divpayamount").addClass("col-md-2");
        $("#divpayamount").removeClass("col-md-1");
    }
    if (x === "BSES Rajdhani Power-DELHI" || x === " BSES Yamuna Power-DELHI") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "MSEDC - MAHARASHTRA") {

        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";
        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "block";
        document.getElementById("divbillunit").style.display = "block";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "Tata Power-Delhi") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";

    }
    else if (x === "JBVNL-JHARKHAND") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "block";
        document.getElementById("billunit").placeholder = "Subdivison Code";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";

    }
    else if (x === "MP Poorv Kshetra Vidyut Vitaran") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "North Bihar Electricity" || x === "South Bihar Electricity") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "BEST Electricity" || x === "CSEB - CHHATTISGARH" || x === "Madhya Kshetra Vitaran - MADHYA PRADESH" || x === "Noida Power - NOIDA" || x === "Paschim Kshetra Vitaran - MADHYA PRADESH" || x === "Southern Power - TELANGANA" || x === "CSPDCL - CHHATTISGARH" || x === "JUSCO - JAMSHEDPUR") {
        if (x === "CSPDCL - CHHATTISGARH" || x === "JUSCO - JAMSHEDPUR") {
            document.getElementById("txtelectricity").placeholder = "Business Partner Number";
            document.getElementById("electricitymessage").style.display = "none";

            document.getElementById("btncontinue").style.display = "block";
            document.getElementById("divprocessingcycle").style.display = "none";
            document.getElementById("divbillunit").style.display = "none";
            document.getElementById("divcity").style.display = "none";
            document.getElementById("txtelectricity").value = "";
        }
        else {
            document.getElementById("txtelectricity").placeholder = "Consumer Number";
            document.getElementById("electricitymessage").style.display = "none";

            document.getElementById("btncontinue").style.display = "block";
            document.getElementById("divprocessingcycle").style.display = "none";
            document.getElementById("divbillunit").style.display = "none";
            document.getElementById("divcity").style.display = "none";
            document.getElementById("txtelectricity").value = "";
        }

    }
    else if (x === "Ajmer Vidyut Vitran Nigam - RAJASTHAN" || x === "Jaipur Vidyut Vitran Nigam - RAJASTHAN" || x === "Jodhpur Vidyut Vitran Nigam - RAJASTHAN" || x === "APDCL - ASSAM" || x === "BESL - BHARATPUR" || x === "BESL - BHARATPUR" ||
        x === "BkESL - BIKANER" || x === "India Power - WEST BENGAL" || x === "KEDL - KOTA" || x === "Muzaffarpur Vidyut Vitran" || x === "NESCO - ODISHA" || x === "SNDL Power - NAGPUR" || x === "SOUTHCO - ODISHA" || x === "Tata Power - MUMBAI" || x === "TPADL - AJMER" || x === "TSECL - TRIPURA" || x === "UPPCL - UTTAR PRADESH" || x === "UPPCL(RURAL) - UTTAR PRADESH") {
        if (x === "TSECL - TRIPURA") {
            document.getElementById("txtelectricity").placeholder = "Customer ID";
            document.getElementById("electricitymessage").style.display = "none";
            document.getElementById("btncontinue").style.display = "block";
            document.getElementById("divprocessingcycle").style.display = "none";
            document.getElementById("divbillunit").style.display = "none";
            document.getElementById("divcity").style.display = "none";
            document.getElementById("txtelectricity").value = "";
        }
        else if (x === "Ajmer Vidyut Vitran Nigam - RAJASTHAN" || x === "Jaipur Vidyut Vitran Nigam - RAJASTHAN" || x === "Jodhpur Vidyut Vitran Nigam - RAJASTHAN") {

            document.getElementById("txtelectricity").placeholder = "K Number";
            document.getElementById("electricitymessage").style.display = "none";

            document.getElementById("btncontinue").style.display = "block";
            document.getElementById("divprocessingcycle").style.display = "none";
            document.getElementById("divbillunit").style.display = "none";
            document.getElementById("divcity").style.display = "none";
            document.getElementById("txtelectricity").value = "";

        }
        else {
            document.getElementById("txtelectricity").placeholder = "Consumer Number";
            document.getElementById("electricitymessage").style.display = "none";

            document.getElementById("btncontinue").style.display = "block";
            document.getElementById("divprocessingcycle").style.display = "none";
            document.getElementById("divbillunit").style.display = "none";
            document.getElementById("divcity").style.display = "none";
            document.getElementById("txtelectricity").value = "";
        }


    }
    else if (x === "BESCOM - BENGALURU") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "DGVCL - GUJARAT" || x === "MGVCL - GUJARAT" || x === "PGVCL - GUJARAT" || x === "UGVCL - GUJARAT") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
        document.getElementById("btngetviewbill").style.display = "block";


    }
    else if (x === "CESC - WEST BENGAL") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "Southern Power - ANDHRA PRADESH") {
        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "APEPDCL - ANDHRA PRADESH" || x === "APSPDCL - ANDHRA PRADESH") {
        document.getElementById("txtelectricity").placeholder = "Service Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "Daman and Diu Electricity") {
        document.getElementById("txtelectricity").placeholder = "Account Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";
    }
    else if (x === "Reliance Energy-MUMBAI") {

        document.getElementById("txtelectricity").placeholder = "Account number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "block";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("txtelectricity").value = "";


    }

    else if (x === "Torrent Power") {

        document.getElementById("txtelectricity").placeholder = "Service Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "block";
        document.getElementById("txtelectricity").value = "";
        document.getElementById("divcity").className = "col-md-2";
        document.getElementById("btngetviewbill").style.marginLeft = "-11px";
        document.getElementById("ddlcitymessage").style.marginTop = "-27px";
        document.getElementById("divcitytamilnaduoperator").style.display = "none";
    }
    else if (x === "TNEB-Tamil Nadu") {

        document.getElementById("txtelectricity").placeholder = "Consumer Number";
        document.getElementById("electricitymessage").style.display = "none";

        document.getElementById("btncontinue").style.display = "block";
        document.getElementById("divprocessingcycle").style.display = "none";
        document.getElementById("divbillunit").style.display = "none";
        document.getElementById("divcity").style.display = "none";
        document.getElementById("divcitytamilnaduoperator").style.display = "block";
        document.getElementById("txtelectricity").value = "";
        document.getElementById("divcitytamilnaduoperator").className = "col-md-2";
        document.getElementById("btngetviewbill").style.marginLeft = "-11px";
        document.getElementById("ddlcitymessage").style.marginTop = "-27px";

    }
}

/* continue button validation */
function continuebtnvalidation(eleddl, txtval, cycleno, billunit, amount, ddlcity) {

    if (eleddl === "Select Operators") {
        document.getElementById("electricityoperator").innerHTML = "Please Select Any Operator";
        return false;
    }
    else if (amount === "") {
        if (eleddl !== "Select Operators") {
            document.getElementById("electricityoperator").innerHTML = "";
            document.getElementById("ddlcitymessage").innerHTML = "";
        }
        document.getElementById("amountelectricityMessage").innerHTML = "Enter Amount";
        return false;
    }
    else if (ddlcity === "" && eleddl === "Torrent Power") {
        document.getElementById("ddlcitymessage").style.marginTop = "10px";
        document.getElementById("ddlcty").style.marginTop = "-1px";
        document.getElementById("ddlcitymessage").innerHTML = "Please Select Any city";
        document.getElementById("ddlcitymessage").style.display = "block";

        return false;
    }
    else if (ddlcity === "" && eleddl === "TNEB-Tamil Nadu") {
        document.getElementById("ddlcitymessage").style.marginTop = "10px";
        document.getElementById("ddlctytamil").style.marginTop = "-1px";
        document.getElementById("ddlcitymessage").innerHTML = "Please Select Any city";
        document.getElementById("ddlcitymessage").style.display = "block";

        return false;
    }
    else {
        document.getElementById("electricityoperator").innerHTML = "";
        if ((txtval === "") && (eleddl === "BSES Rajdhani Power-DELHI" || eleddl === "BSES Yamuna Power-DELHI")) {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "BSES Rajdhani Power-DELHI" || eleddl === "BSES Yamuna Power-DELHI") {
                var electricity = document.getElementById("txtelectricity").value.length;

                if ((electricity < 9 || electricity > 10) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [9-10] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "Tata Power-Delhi") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "Tata Power-Delhi") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 11 || electricity > 12) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [11-12] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    // $('#myViewpaybillmodel').modal('show');
                    // event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "MP Poorv Kshetra Vidyut Vitaran") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "MP Poorv Kshetra Vidyut Vitaran") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 2 || electricity > 16) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [10] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    // $('#myViewpaybillmodel').modal('show');
                    // event.preventDefault();
                    return true;
                }
            }
        }
        if ((txtval === "") && (eleddl === "DGVCL - GUJARAT" || eleddl === "MGVCL - GUJARAT" || eleddl === "PGVCL - GUJARAT" || eleddl === "UGVCL - GUJARAT")) {

            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "DGVCL - GUJARAT" || eleddl === "MGVCL - GUJARAT" || eleddl === "PGVCL - GUJARAT" || eleddl === "UGVCL - GUJARAT") {

                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 5 || electricity > 11) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [5-11] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    // $('#myViewpaybillmodel').modal('show');
                    // event.preventDefault();
                    return true;
                }
            }
        }

        if (txtval === "" && eleddl === "North Bihar Electricity") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();

            return false;
        }
        else {
            if (eleddl === "North Bihar Electricity" || eleddl === "South Bihar Electricity") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 9 || electricity > 12) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [9-11] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if ((txtval === "") && (eleddl === "BEST Electricity" || eleddl === "CSEB - CHHATTISGARH" || eleddl === "Madhya Kshetra Vitaran - MADHYA PRADESH" || eleddl === "Noida Power - NOIDA" || eleddl === "Paschim Kshetra Vitaran - MADHYA PRADESH" || eleddl === "Southern Power - TELANGANA" || eleddl === "CSPDCL - CHHATTISGARH" || eleddl === "JUSCO - JAMSHEDPUR")) {
            if ((txtval === "") && (eleddl === "CSPDCL - CHHATTISGARH" || eleddl === "JUSCO - JAMSHEDPUR")) {
                document.getElementById("electricitymessage").innerHTML = "Please Business partner number";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();
                return false;
            }
            else {
                document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();
                return false;
            }

        }
        //else {
        //    if (eleddl === "BEST Electricity" || eleddl === "CSEB - CHHATTISGARH" || eleddl === "Madhya Kshetra Vitaran - MADHYA PRADESH" || eleddl === "Noida Power - NOIDA" || eleddl === "Paschim Kshetra Vitaran - MADHYA PRADESH" || eleddl === "Southern Power - TELANGANA" || eleddl === "CSPDCL - CHHATTISGARH" || eleddl === "JUSCO - JAMSHEDPUR") {
        //        electricity = document.getElementById("txtelectricity").value.length;
        //        if ((electricity < 9 || electricity > 10) && electricity !== null && electricity !== "") {
        //            document.getElementById("electricitymessage").innerHTML = "Number should be [10] digits";
        //            document.getElementById("electricitymessage").style.display = "block";
        //            document.getElementById("elecity").style.marginTop = "8px";
        //            event.preventDefault();
        //            return false;
        //        }
        //        else {
        //            document.getElementById("electricitymessage").style.display = "none";
        //            document.getElementById("elecity").style.marginTop = "8px";
        //            //$('#myViewpaybillmodel').modal('show');
        //            //   event.preventDefault();
        //            return true;
        //        }
        //    }
        //}
        if ((txtval === "") && (eleddl === "Ajmer Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jaipur Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jodhpur Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "APDCL - ASSAM" || eleddl === "BESL - BHARATPUR" || eleddl === "BkESL - BIKANER" || eleddl === "India Power - WEST BENGAL" || eleddl === "KEDL - KOTA" || eleddl === "Muzaffarpur Vidyut Vitran" || eleddl === "NESCO - ODISHA" || eleddl === "SNDL Power - NAGPUR" || eleddl === "SOUTHCO - ODISHA" || eleddl === "Tata Power - MUMBAI" || eleddl === "TPADL - AJMER" || eleddl === "TSECL - TRIPURA" || eleddl === "UPPCL - UTTAR PRADESH" || eleddl === "UPPCL(RURAL) - UTTAR PRADESH")) {


            if (txtval === "" && eleddl === "TSECL - TRIPURA") {
                document.getElementById("electricitymessage").innerHTML = "Please enter customer ID";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();
                return false;

            }
            else if ((txtval === "") && (eleddl === "Ajmer Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jaipur Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jodhpur Vidyut Vitran Nigam - RAJASTHAN")) {
                document.getElementById("electricitymessage").innerHTML = "Please enter K number";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();
                return false;
            }
            else {

                document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();

                return false;
            }


        }
        else {
            if (eleddl === "Ajmer Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jaipur Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "Jodhpur Vidyut Vitran Nigam - RAJASTHAN" || eleddl === "APDCL - ASSAM" || eleddl === "BESL - BHARATPUR" || eleddl === "BkESL - BIKANER" || eleddl === "India Power - WEST BENGAL" || eleddl === "KEDL - KOTA" || eleddl === "Muzaffarpur Vidyut Vitran" || eleddl === "NESCO - ODISHA" || eleddl === "SNDL Power - NAGPUR" || eleddl === "SOUTHCO - ODISHA" || eleddl === "Tata Power - MUMBAI" || eleddl === "TPADL - AJMER" || eleddl === "TSECL - TRIPURA" || eleddl === "UPPCL - UTTAR PRADESH" || eleddl === "UPPCL(RURAL) - UTTAR PRADESH") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 7 && electricity > 20) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be between [8-20] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "BESCOM - BENGALURU") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "BESCOM - BENGALURU") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 7 || electricity > 10) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [7-10] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";

                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;

                }
            }
        }

        if (txtval === "" && eleddl === "CESC - WEST BENGAL") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "CESC - WEST BENGAL") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 11) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [11] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "Southern Power - ANDHRA PRADESH") {
            document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "Southern Power - ANDHRA PRADESH") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 13) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [13] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    // event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "APEPDCL - ANDHRA PRADESH") {

            document.getElementById("electricitymessage").innerHTML = "Please enter service number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "APEPDCL - ANDHRA PRADESH") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity < 15 || electricity > 20) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [20] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //  event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "APSPDCL - ANDHRA PRADESH") {
            document.getElementById("electricitymessage").innerHTML = "Please enter service number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "APSPDCL - ANDHRA PRADESH") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 13) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [13] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {
                    document.getElementById("electricitymessage").style.display = "none";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "Daman and Diu Electricity") {
            document.getElementById("electricitymessage").innerHTML = "Please enter account number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "Daman and Diu Electricity") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 6) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [6] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if ((txtval === "" || cycleno === "") && eleddl === "Reliance Energy-MUMBAI") {
            document.getElementById("processingcycle").innerHTML = "Please enter cycle number";
            document.getElementById("processingcycle").style.display = "block";
            document.getElementById("electricitymessage").innerHTML = "Please enter account number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "Reliance Energy-MUMBAI") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 9) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [9] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("processingcycle").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("processingcycle").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if ((txtval === "" || cycleno === "" || billunit === "") && eleddl === "MSEDC - MAHARASHTRA") {

            document.getElementById("processingcycle").innerHTML = "Please enter cycle number";
            document.getElementById("processingcycle").style.display = "block";
            document.getElementById("electricitymessage").innerHTML = "Please consumer number";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("billunitmessage").innerHTML = "Bill unit";
            document.getElementById("billunitmessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();

            if (cycleno !== "" && eleddl === "MSEDC - MAHARASHTRA") {
                document.getElementById("processingcycle").style.display = "none";
            }
            if (billunit !== "" && eleddl === "MSEDC - MAHARASHTRA") {
                document.getElementById("billunitmessage").style.display = "none";
            }
            return false;
        }
        else {
            if (eleddl === "MSEDC - MAHARASHTRA") {
                electricity = document.getElementById("txtelectricity").value.length;
                if ((electricity !== 12) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [12] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("processingcycle").style.display = "none";
                    document.getElementById("billunitmessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("billunitmessage").style.display = "none";
                    document.getElementById("processingcycle").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }
        if (txtval === "" && eleddl === "Torrent Power") {
            document.getElementById("electricitymessage").innerHTML = "Please enter service no";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("elecity").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "Torrent Power") {
                electricity = document.getElementById("txtelectricity").value.length;
                if (![9, 10, 11, 12].includes(electricity) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [9] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("elecity").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("elecity").style.marginTop = "8px";
                    document.getElementById("ddlcitymessage").style.display = "none";
                    document.getElementById("ddlcty").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
        }

        if (txtval === "" && eleddl === "TNEB-Tamil Nadu") {
            document.getElementById("electricitymessage").innerHTML = "Please enter Consumer no";
            document.getElementById("electricitymessage").style.display = "block";
            document.getElementById("ddlctytamil").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
        else {
            if (eleddl === "TNEB-Tamil Nadu") {
                electricity = document.getElementById("txtelectricity").value.length;
                if (([10, 11, 12].indexOf(electricity) == -1) && electricity !== null && electricity !== "") {
                    document.getElementById("electricitymessage").innerHTML = "Number should be [" + electricity +"] digits";
                    document.getElementById("electricitymessage").style.display = "block";
                    document.getElementById("ddlctytamil").style.marginTop = "8px";
                    event.preventDefault();
                    return false;
                }
                else {

                    document.getElementById("electricitymessage").style.display = "none";
                    document.getElementById("ddlctytamil").style.marginTop = "8px";
                    document.getElementById("ddlcitymessage").style.display = "none";
                    document.getElementById("ddlctytamil").style.marginTop = "8px";
                    //$('#myViewpaybillmodel').modal('show');
                    //event.preventDefault();
                    return true;
                }
            }
            else {
                document.getElementById("electricitymessage").style.display = "none";
                document.getElementById("ddlctytamil").style.marginTop = "8px";
                document.getElementById("ddlcitymessage").style.display = "none";
                document.getElementById("ddlctytamil").style.marginTop = "8px";
                //$('#myViewpaybillmodel').modal('show');
                //event.preventDefault();
                return true;
            }
        }
    }
}

/*Paybillbutton javascript */
function Getpaybillbutton(eleddl, txtval, Amount) {

    if ((txtval === "" || Amount === "") && (eleddl === "DGVCL - GUJARAT" || eleddl === "MGVCL - GUJARAT" || eleddl === "PGVCL - GUJARAT" || eleddl === "UGVCL - GUJARAT")) {
        document.getElementById("electricitymessage").innerHTML = "Please enter consumer number";
        document.getElementById("amount").innerHTML = "Please enter amount";
        document.getElementById("electricitymessage").style.display = "block";
        document.getElementById("amount").style.display = "block";
        document.getElementById("elecity").style.marginTop = "8px";
        event.preventDefault();
    }
    else {
        if (eleddl === "DGVCL - GUJARAT" || eleddl === "MGVCL - GUJARAT" || eleddl === "PGVCL - GUJARAT" || eleddl === "UGVCL - GUJARAT") {
            electricity = document.getElementById("txtelectricity").value.length;
            if ((electricity !== 11) && electricity !== null && electricity !== "") {
                document.getElementById("electricitymessage").innerHTML = "Number should be [11] digits";
                document.getElementById("electricitymessage").style.display = "block";
                document.getElementById("amount").style.display = "none";
                document.getElementById("elecity").style.marginTop = "8px";
                event.preventDefault();
            }
            else {

                return true;
            }
        }
    }
}